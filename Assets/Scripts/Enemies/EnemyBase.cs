using System;
using System.Collections;
using System.Collections.Generic;
using Heretic.Combat;
using Heretic.Player;
using UnityEngine;

namespace Heretic.Enemies
{
    /// <summary>Tunables shared by every enemy type. Serialized on the enemy component (so on its prefab).</summary>
    [Serializable]
    public class EnemySettings
    {
        [Min(1)] public int MaxHealth = 3;
        [Min(0)] public int ContactDamage = 1;
        [Min(0f)] public float ContactCooldown = 0.8f;
        [Min(0.01f)] public float HitFlashSeconds = 0.1f;
        [Min(0.01f)] public float DeathFadeSeconds = 0.3f;
        public Vector2 BodySize = new Vector2(0.8f, 0.8f);
        public Color BaseColor = Color.red;
        public Color FlashColor = Color.white;
    }

    /// <summary>
    /// Shared enemy behaviour: health, hit flash, contact damage to the player, death and removal.
    /// Placeholder visual is built in Awake when no sprite is assigned.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(BoxCollider2D))]
    public abstract class EnemyBase : MonoBehaviour, IDamageable
    {
        private const float ContactPadding = 0.05f;
        private static Sprite _squareSprite;

        private readonly List<Collider2D> _overlaps = new List<Collider2D>();
        private float _flashTimer;
        private float _contactTimer;

        /// <summary>The enemy's rigidbody.</summary>
        protected Rigidbody2D Body { get; private set; }

        /// <summary>The enemy's collider.</summary>
        protected BoxCollider2D Collider { get; private set; }

        /// <summary>The enemy's renderer.</summary>
        protected SpriteRenderer Renderer { get; private set; }

        /// <summary>Settings of the concrete type.</summary>
        protected abstract EnemySettings Settings { get; }

        /// <summary>Raised once when health reaches 0.</summary>
        public event Action<EnemyBase> Died;

        /// <summary>Current health.</summary>
        public int Health { get; private set; }

        /// <summary>True once health has reached 0.</summary>
        public bool IsDead { get; private set; }

        /// <summary>Gets components, builds the placeholder visual and sets health.</summary>
        protected virtual void Awake()
        {
            Body = GetComponent<Rigidbody2D>();
            Collider = GetComponent<BoxCollider2D>();
            Renderer = GetComponent<SpriteRenderer>();
            Body.interpolation = RigidbodyInterpolation2D.Interpolate;
            Body.freezeRotation = true;

            if (Renderer.sprite == null)
            {
                if (_squareSprite == null)
                {
                    var texture = new Texture2D(32, 32) { filterMode = FilterMode.Point };
                    var pixels = new Color[32 * 32];
                    for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.white;
                    texture.SetPixels(pixels);
                    texture.Apply();
                    _squareSprite = Sprite.Create(texture, new Rect(0f, 0f, 32f, 32f), new Vector2(0.5f, 0.5f), 32f);
                }

                Renderer.sprite = _squareSprite;
                Renderer.sortingOrder = 2;
                transform.localScale = new Vector3(Settings.BodySize.x, Settings.BodySize.y, 1f);
                Collider.size = Vector2.one;
            }

            Renderer.color = Settings.BaseColor;
            Health = Settings.MaxHealth;
        }

        /// <summary>Removes health and flashes; at 0 the enemy dies.</summary>
        /// <param name="amount">Health to remove.</param>
        public void TakeDamage(int amount)
        {
            if (IsDead) return;

            Health -= amount;
            if (Health <= 0)
            {
                Die();
                return;
            }

            _flashTimer = Settings.HitFlashSeconds;
            Renderer.color = Settings.FlashColor;
        }

        /// <summary>Counts the contact cooldown and damages a player overlapping the body.</summary>
        protected virtual void FixedUpdate()
        {
            _contactTimer -= Time.fixedDeltaTime;
            if (_contactTimer > 0f) return;

            Bounds bounds = Collider.bounds;
            Vector2 size = (Vector2)bounds.size + Vector2.one * (ContactPadding * 2f);
            Physics2D.OverlapBox(bounds.center, size, 0f, ContactFilter2D.noFilter, _overlaps);
            foreach (Collider2D other in _overlaps)
            {
                if (other == Collider) continue;
                if (!other.TryGetComponent(out PlayerHealth playerHealth)) continue;

                playerHealth.TakeDamage(Settings.ContactDamage);
                _contactTimer = Settings.ContactCooldown;
                return;
            }
        }

        /// <summary>Overlap test for a probe box that ignores this enemy, triggers and dynamic bodies (the player, etc.).</summary>
        /// <param name="center">Box center.</param>
        /// <param name="size">Box size.</param>
        /// <param name="includeOtherEnemies">True to also count another enemy (anything with an <see cref="EnemyBase"/>) as solid.</param>
        protected bool OverlapsSolid(Vector2 center, Vector2 size, bool includeOtherEnemies = false)
        {
            Physics2D.OverlapBox(center, size, 0f, ContactFilter2D.noFilter, _overlaps);
            foreach (Collider2D other in _overlaps)
            {
                if (other == Collider || other.isTrigger) continue;
                if (includeOtherEnemies && other.TryGetComponent(out EnemyBase _)) return true;
                Rigidbody2D otherBody = other.attachedRigidbody;
                if (otherBody != null && otherBody.bodyType == RigidbodyType2D.Dynamic) continue;
                return true;
            }

            return false;
        }

        private void Update()
        {
            if (_flashTimer <= 0f) return;
            _flashTimer -= Time.deltaTime;
            if (_flashTimer <= 0f) Renderer.color = Settings.BaseColor;
        }

        private void Die()
        {
            IsDead = true;
            Health = 0;
            Body.linearVelocity = Vector2.zero;
            Body.simulated = false;
            Collider.enabled = false;
            Renderer.color = Settings.FlashColor;
            Died?.Invoke(this);
            StartCoroutine(FadeAndRemove());
            enabled = false;
        }

        private IEnumerator FadeAndRemove()
        {
            float duration = Settings.DeathFadeSeconds;
            Color start = Settings.FlashColor;
            for (float t = 0f; t < duration; t += Time.deltaTime)
            {
                Renderer.color = new Color(start.r, start.g, start.b, 1f - t / duration);
                yield return null;
            }

            Destroy(gameObject);
        }
    }
}
