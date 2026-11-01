using System;
using System.Collections.Generic;
using Heretic.Combat;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Heretic.Player
{
    /// <summary>Tunable movement, dash and attack values. Serialized on <see cref="PlayerTestBootstrap"/> so edits persist in the scene.</summary>
    [Serializable]
    public class PlayerSettings
    {
        [Header("Collider (sprite is 1 x 1.5 units, pivot at the feet)")]
        public Vector2 ColliderSize = new Vector2(0.6f, 1.3f);
        public Vector2 ColliderOffset = new Vector2(0f, 0.65f);

        [Header("Movement")]
        [Min(0.1f)] public float MoveSpeed = 8f;
        [Min(0.1f)] public float JumpSpeed = 15f;
        [Min(0.1f)] public float GravityScale = 4f;
        [Range(0f, 1f), Tooltip("Vertical speed is multiplied by this when the jump key is released while rising.")]
        public float JumpCutMultiplier = 0.4f;
        [Min(0f)] public float CoyoteSeconds = 0.1f;
        [Min(0f)] public float JumpBufferSeconds = 0.1f;

        [Header("Dash")]
        [Min(0.1f)] public float DashSpeed = 20f;
        [Min(0.01f)] public float DashSeconds = 0.18f;
        [Min(0f)] public float DashCooldownSeconds = 0.8f;
        [Min(0f)] public float DashInvulnerableSeconds = 0.25f;

        [Header("Attack")]
        [Min(0)] public int AttackDamage = 1;
        [Min(0.1f)] public float AttackReach = 1.4f;
        [Min(0.1f)] public float AttackHeight = 1.1f;
        [Min(0f)] public float AttackCenterHeight = 0.7f;
        [Min(0f)] public float AttackWindupSeconds = 0.08f;
        [Min(0.01f)] public float AttackActiveSeconds = 0.08f;
        [Min(0f)] public float AttackRecoverySeconds = 0.12f;
        [Min(0f), Tooltip("Measured from the start of the swing; the swing cannot restart while it is still playing.")]
        public float AttackCooldownSeconds = 0.4f;
    }

    /// <summary>
    /// Player movement, dash and melee attack (story 003, brief MVP feature 1).
    /// Controls: A/D run, W or Space jump, Left Shift dash, J attack. Arrow keys belong to the QTE and are never read.
    /// Input is read and applied in Update with scaled time; the body is interpolated so slow motion stays smooth.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
    public class PlayerController : MonoBehaviour
    {
        private const float GroundProbeOffset = 0.08f;
        private const float GroundProbeHeight = 0.1f;
        private const float RisingThreshold = 0.01f;

        private PlayerSettings _s;
        private PlayerHealth _health;
        private Rigidbody2D _body;
        private BoxCollider2D _collider;
        private readonly HashSet<IDamageable> _hitThisSwing = new HashSet<IDamageable>();
        private readonly List<Collider2D> _overlaps = new List<Collider2D>();
        private float _coyoteTimer;
        private float _bufferTimer;
        private float _dashTimer;
        private float _dashCooldown;
        private float _attackElapsed;
        private float _attackCooldown;
        private bool _attacking;
        private bool _jumpCutDone;

        /// <summary>-1 or +1, the last horizontal input (starts facing right).</summary>
        public float Facing { get; private set; } = 1f;

        /// <summary>Horizontal input this frame, -1, 0 or +1.</summary>
        public float MoveInput { get; private set; }

        /// <summary>True while a dash is in progress.</summary>
        public bool IsDashing => _dashTimer > 0f;

        /// <summary>True while a swing is playing (wind-up, strike or recovery).</summary>
        public bool IsAttacking => _attacking;

        /// <summary>Swing phase while <see cref="IsAttacking"/>: 0 wind-up, 1 strike, 2 recovery.</summary>
        public int AttackPhase
        {
            get
            {
                float windup = _s.AttackWindupSeconds;
                if (_attackElapsed < windup) return 0;
                return _attackElapsed < windup + _s.AttackActiveSeconds ? 1 : 2;
            }
        }

        /// <summary>True while standing on a collider (probe a few hundredths below the feet).</summary>
        public bool IsGrounded
        {
            get
            {
                Bounds bounds = _collider.bounds;
                var center = new Vector2(bounds.center.x, bounds.min.y - GroundProbeOffset);
                return Physics2D.OverlapBox(center, new Vector2(bounds.size.x * 0.9f, GroundProbeHeight), 0f) != null;
            }
        }

        /// <summary>Applies the settings and sets up the body. Call right after AddComponent.</summary>
        /// <param name="settings">Tunable values (owned by the bootstrap so they persist in the scene).</param>
        /// <param name="health">Health component on the same object.</param>
        public void Initialize(PlayerSettings settings, PlayerHealth health)
        {
            _s = settings;
            _health = health;
            _body = GetComponent<Rigidbody2D>();
            _collider = GetComponent<BoxCollider2D>();

            _collider.size = _s.ColliderSize;
            _collider.offset = _s.ColliderOffset;

            _body.bodyType = RigidbodyType2D.Dynamic;
            _body.gravityScale = _s.GravityScale;
            _body.constraints = RigidbodyConstraints2D.FreezeRotation;
            // Fixed steps are not scaled by timeScale, so interpolation keeps motion smooth in slow motion.
            _body.interpolation = RigidbodyInterpolation2D.Interpolate;
            _body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            _body.sharedMaterial = new PhysicsMaterial2D("PlayerNoFriction") { friction = 0f, bounciness = 0f };

            _health.WasReset += ClearState;
        }

        private void OnDestroy()
        {
            if (_health != null) _health.WasReset -= ClearState;
        }

        private void ClearState()
        {
            _coyoteTimer = 0f;
            _bufferTimer = 0f;
            _dashTimer = 0f;
            _dashCooldown = 0f;
            _attackCooldown = 0f;
            _attacking = false;
            _body.gravityScale = _s.GravityScale;
        }

        private void Update()
        {
            if (_health.IsDead)
            {
                // Lose control: stop running, restore gravity so the body drops and stays down.
                _dashTimer = 0f;
                _attacking = false;
                MoveInput = 0f;
                _body.gravityScale = _s.GravityScale;
                _body.linearVelocity = new Vector2(0f, _body.linearVelocity.y);
                return;
            }

            float dt = Time.deltaTime;
            _dashCooldown -= dt;
            _attackCooldown -= dt;
            _coyoteTimer -= dt;
            _bufferTimer -= dt;

            Keyboard keyboard = Keyboard.current;
            float input = 0f;
            bool jumpPressed = false;
            bool jumpHeld = false;
            bool dashPressed = false;
            bool attackPressed = false;
            if (keyboard != null)
            {
                input = (keyboard.dKey.isPressed ? 1f : 0f) - (keyboard.aKey.isPressed ? 1f : 0f);
                jumpPressed = keyboard.wKey.wasPressedThisFrame || keyboard.spaceKey.wasPressedThisFrame;
                jumpHeld = keyboard.wKey.isPressed || keyboard.spaceKey.isPressed;
                dashPressed = keyboard.leftShiftKey.wasPressedThisFrame;
                attackPressed = keyboard.jKey.wasPressedThisFrame;
            }

            MoveInput = input;
            if (Mathf.Abs(input) > 0.01f) Facing = Mathf.Sign(input);

            UpdateAttack(attackPressed, dt);

            if (dashPressed && _dashTimer <= 0f && _dashCooldown <= 0f)
            {
                _dashTimer = _s.DashSeconds;
                _dashCooldown = _s.DashCooldownSeconds;
                _health.GrantInvulnerability(_s.DashInvulnerableSeconds);
                _body.gravityScale = 0f;
            }

            if (_dashTimer > 0f)
            {
                _dashTimer -= dt;
                _body.linearVelocity = new Vector2(Facing * _s.DashSpeed, 0f);
                if (_dashTimer <= 0f) _body.gravityScale = _s.GravityScale;
                return;
            }

            Vector2 velocity = _body.linearVelocity;
            bool rising = velocity.y > RisingThreshold;

            // Coyote time refills only while standing, not on the frames right after a jump.
            if (!rising && IsGrounded) _coyoteTimer = _s.CoyoteSeconds;
            if (jumpPressed) _bufferTimer = _s.JumpBufferSeconds;

            if (_bufferTimer > 0f && _coyoteTimer > 0f)
            {
                velocity.y = _s.JumpSpeed;
                _bufferTimer = 0f;
                _coyoteTimer = 0f;
                _jumpCutDone = false;
            }
            else if (rising && !jumpHeld && !_jumpCutDone)
            {
                // Variable jump height: releasing the key cuts the rise, once per jump.
                velocity.y *= _s.JumpCutMultiplier;
                _jumpCutDone = true;
            }

            velocity.x = input * _s.MoveSpeed;
            _body.linearVelocity = velocity;
        }

        private void UpdateAttack(bool attackPressed, float dt)
        {
            if (attackPressed && !_attacking && _attackCooldown <= 0f)
            {
                _attacking = true;
                _attackElapsed = 0f;
                _attackCooldown = _s.AttackCooldownSeconds;
                _hitThisSwing.Clear();
            }

            if (!_attacking) return;

            _attackElapsed += dt;
            float strikeStart = _s.AttackWindupSeconds;
            float strikeEnd = strikeStart + _s.AttackActiveSeconds;

            // A long frame can jump over the active window, so test "window not yet over" rather than "inside it".
            if (_attackElapsed >= strikeStart && _attackElapsed - dt < strikeEnd) StrikeHit();

            if (_attackElapsed >= strikeEnd + _s.AttackRecoverySeconds) _attacking = false;
        }

        private void StrikeHit()
        {
            Vector2 origin = _body.position;
            var center = new Vector2(origin.x + Facing * _s.AttackReach * 0.5f, origin.y + _s.AttackCenterHeight);
            var size = new Vector2(_s.AttackReach, _s.AttackHeight);

            _overlaps.Clear();
            Physics2D.OverlapBox(center, size, 0f, ContactFilter2D.noFilter, _overlaps);
            foreach (Collider2D other in _overlaps)
            {
                if (other.attachedRigidbody == _body) continue;
                if (!other.TryGetComponent(out IDamageable target)) continue;
                if (_hitThisSwing.Add(target)) target.TakeDamage(_s.AttackDamage);
            }
        }
    }
}
