using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Heretic.Boss
{
    /// <summary>Tunable values of the temporary player. Serialized on <see cref="BossArenaBootstrap"/> so edits persist in the scene.</summary>
    [Serializable]
    public class TempPlayerSettings
    {
        public Vector2 Size = new Vector2(0.8f, 1.2f);
        [Min(1)] public int MaxHealth = 5;

        [Header("Movement")]
        [Min(0.1f)] public float MoveSpeed = 8f;
        [Min(0.1f)] public float JumpSpeed = 15f;
        [Min(0.1f)] public float GravityScale = 4f;

        [Header("Dash")]
        [Min(0.1f)] public float DashSpeed = 20f;
        [Min(0.01f)] public float DashSeconds = 0.18f;
        [Min(0f)] public float DashCooldownSeconds = 0.8f;
        [Min(0f)] public float DashInvulnerableSeconds = 0.25f;

        [Header("Damage")]
        [Min(0f)] public float HitInvulnerableSeconds = 1f;
    }

    /// <summary>
    /// TEMPORARY STAND-IN PLAYER, used only in the boss arena so story 002 can be exercised.
    /// Story 003 (the real player) replaces this class; do not build on it.
    /// Controls: A/D run, W or Space jump, Left Shift dash (brief invulnerability, cooldown).
    /// Arrow keys are NOT used: they belong to the QTE (story 001).
    /// Uses scaled time and an interpolated Rigidbody2D so slow motion stays smooth.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(SpriteRenderer))]
    public class TempPlayer : MonoBehaviour
    {
        private const float GroundProbeOffset = 0.08f;
        private const float GroundProbeHeight = 0.1f;

        private TempPlayerSettings _s;
        private Rigidbody2D _body;
        private BoxCollider2D _collider;
        private SpriteRenderer _renderer;
        private Color _baseColor;
        private Vector3 _startPosition;
        private float _facing = 1f;
        private float _dashTimer;
        private float _dashCooldown;
        private float _invulnerableTimer;

        /// <summary>Raised once when health reaches 0.</summary>
        public event Action Died;

        /// <summary>Current health.</summary>
        public int Health { get; private set; }

        /// <summary>Maximum health.</summary>
        public int MaxHealth => _s.MaxHealth;

        /// <summary>World-space bounds of the player's collider.</summary>
        public Bounds Bounds => _collider.bounds;

        /// <summary>True while standing on a collider (probe just below the feet).</summary>
        public bool IsGrounded
        {
            get
            {
                Bounds bounds = _collider.bounds;
                var center = new Vector2(bounds.center.x, bounds.min.y - GroundProbeOffset);
                return Physics2D.OverlapBox(center, new Vector2(bounds.size.x * 0.9f, GroundProbeHeight), 0f) != null;
            }
        }

        /// <summary>Applies the tunable settings and sets up the body. Call right after AddComponent.</summary>
        /// <param name="settings">Tunable values (owned by the bootstrap so they persist in the scene).</param>
        public void Initialize(TempPlayerSettings settings)
        {
            _s = settings;
            _body = GetComponent<Rigidbody2D>();
            _collider = GetComponent<BoxCollider2D>();
            _renderer = GetComponent<SpriteRenderer>();
            _baseColor = _renderer.color;
            _startPosition = transform.position;

            transform.localScale = new Vector3(_s.Size.x, _s.Size.y, 1f);
            _collider.size = Vector2.one;

            _body.gravityScale = _s.GravityScale;
            _body.constraints = RigidbodyConstraints2D.FreezeRotation;
            // Fixed steps stay fixedDeltaTime of game time, so at timeScale 0.2 they run less often in real time;
            // interpolation keeps the motion smooth between them.
            _body.interpolation = RigidbodyInterpolation2D.Interpolate;
            _body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            _body.sharedMaterial = new PhysicsMaterial2D("TempPlayerNoFriction") { friction = 0f, bounciness = 0f };

            Health = _s.MaxHealth;
        }

        /// <summary>Takes damage unless invulnerable. Reaching 0 health raises <see cref="Died"/> and halts the player.</summary>
        /// <param name="amount">Health to remove.</param>
        /// <param name="ignoreInvulnerability">True for attacks that cannot be dodged (the QTE failure).</param>
        public void TakeDamage(int amount, bool ignoreInvulnerability = false)
        {
            if (Health <= 0) return;
            if (!ignoreInvulnerability && _invulnerableTimer > 0f) return;

            Health = Mathf.Max(0, Health - amount);
            _invulnerableTimer = _s.HitInvulnerableSeconds;
            if (Health > 0) return;

            Halt();
            Died?.Invoke();
        }

        /// <summary>Stops input and movement (fight over). <see cref="ResetPlayer"/> re-enables the player.</summary>
        public void Halt()
        {
            enabled = false;
            _dashTimer = 0f;
            _body.gravityScale = _s.GravityScale;
            _body.linearVelocity = Vector2.zero;
        }

        /// <summary>Restores full health, returns to the start position and re-enables control.</summary>
        public void ResetPlayer()
        {
            _body.position = _startPosition;
            _body.linearVelocity = Vector2.zero;
            _body.gravityScale = _s.GravityScale;
            _dashTimer = 0f;
            _dashCooldown = 0f;
            _invulnerableTimer = 0f;
            Health = _s.MaxHealth;
            _renderer.color = _baseColor;
            enabled = true;
        }

        private void Update()
        {
            float dt = Time.deltaTime;
            _dashCooldown -= dt;
            _invulnerableTimer -= dt;

            Keyboard keyboard = Keyboard.current;
            float input = 0f;
            bool jumpPressed = false;
            bool dashPressed = false;
            if (keyboard != null)
            {
                input = (keyboard.dKey.isPressed ? 1f : 0f) - (keyboard.aKey.isPressed ? 1f : 0f);
                jumpPressed = keyboard.wKey.wasPressedThisFrame || keyboard.spaceKey.wasPressedThisFrame;
                dashPressed = keyboard.leftShiftKey.wasPressedThisFrame;
            }
            if (input != 0f) _facing = input;

            if (dashPressed && _dashTimer <= 0f && _dashCooldown <= 0f)
            {
                _dashTimer = _s.DashSeconds;
                _dashCooldown = _s.DashCooldownSeconds;
                _invulnerableTimer = Mathf.Max(_invulnerableTimer, _s.DashInvulnerableSeconds);
                _body.gravityScale = 0f;
            }

            if (_dashTimer > 0f)
            {
                _dashTimer -= dt;
                _body.linearVelocity = new Vector2(_facing * _s.DashSpeed, 0f);
                if (_dashTimer <= 0f) _body.gravityScale = _s.GravityScale;
            }
            else
            {
                float velocityY = _body.linearVelocity.y;
                if (jumpPressed && IsGrounded) velocityY = _s.JumpSpeed;
                _body.linearVelocity = new Vector2(input * _s.MoveSpeed, velocityY);
            }

            // Half-transparent while invulnerable (dash or just hit).
            Color color = _baseColor;
            if (_invulnerableTimer > 0f) color.a = 0.5f;
            _renderer.color = color;
        }
    }
}
