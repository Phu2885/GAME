using System;
using Heretic.Combat;
using UnityEngine;

namespace Heretic.Player
{
    /// <summary>Tunable health values. Serialized on <see cref="PlayerTestBootstrap"/> so edits persist in the scene.</summary>
    [Serializable]
    public class PlayerHealthSettings
    {
        [Min(1)] public int MaxHealth = 5;
        [Min(0f)] public float HitInvulnerableSeconds = 1f;
    }

    /// <summary>
    /// Player health, invulnerability and death. Implements story 003 (brief MVP feature 1):
    /// at 0 health the player is dead and stays down until <see cref="ResetPlayer"/>.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        private PlayerHealthSettings _s;
        private Rigidbody2D _body;
        private Vector2 _startPosition;
        private float _invulnerableTimer;

        /// <summary>Raised once when health reaches 0.</summary>
        public event Action Died;

        /// <summary>Raised by <see cref="ResetPlayer"/> after health and position are restored.</summary>
        public event Action WasReset;

        /// <summary>Current health.</summary>
        public int Health { get; private set; }

        /// <summary>Maximum health.</summary>
        public int MaxHealth => _s.MaxHealth;

        /// <summary>True once health has reached 0.</summary>
        public bool IsDead => Health <= 0;

        /// <summary>True while damage is ignored (after a hit or during a dash).</summary>
        public bool IsInvulnerable => _invulnerableTimer > 0f;

        /// <summary>Applies the settings and remembers the current position as the start. Call right after AddComponent.</summary>
        /// <param name="settings">Tunable values (owned by the bootstrap).</param>
        public void Initialize(PlayerHealthSettings settings)
        {
            _s = settings;
            _body = GetComponent<Rigidbody2D>();
            _startPosition = _body.position;
            Health = _s.MaxHealth;
        }

        /// <summary>Takes damage unless dead or invulnerable; starts the post-hit invulnerability. Reaching 0 raises <see cref="Died"/>.</summary>
        /// <param name="amount">Health to remove.</param>
        public void TakeDamage(int amount)
        {
            TakeDamage(amount, false);
        }

        /// <summary>Takes damage unless dead; invulnerability is skipped when asked (undodgeable attacks, kill zones).</summary>
        /// <param name="amount">Health to remove.</param>
        /// <param name="ignoreInvulnerability">True to hurt the player through dash or post-hit invulnerability.</param>
        public void TakeDamage(int amount, bool ignoreInvulnerability)
        {
            if (IsDead || (IsInvulnerable && !ignoreInvulnerability)) return;

            Health = Mathf.Max(0, Health - amount);
            _invulnerableTimer = _s.HitInvulnerableSeconds;
            if (Health <= 0) Died?.Invoke();
        }

        /// <summary>Makes the player ignore damage for at least the given time (used by the dash).</summary>
        /// <param name="seconds">Invulnerability duration.</param>
        public void GrantInvulnerability(float seconds)
        {
            _invulnerableTimer = Mathf.Max(_invulnerableTimer, seconds);
        }

        /// <summary>Full health, back to the start position, control returns.</summary>
        public void ResetPlayer()
        {
            Health = _s.MaxHealth;
            _invulnerableTimer = 0f;
            _body.position = _startPosition;
            _body.linearVelocity = Vector2.zero;
            WasReset?.Invoke();
        }

        private void Update()
        {
            _invulnerableTimer -= Time.deltaTime;
        }
    }
}
