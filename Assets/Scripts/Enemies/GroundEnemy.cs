using System;
using UnityEngine;

namespace Heretic.Enemies
{
    /// <summary>Tunables of the walking enemy.</summary>
    [Serializable]
    public class GroundEnemySettings : EnemySettings
    {
        [Min(0f)] public float Speed = 2f;
        [Min(0f)] public float TurnCooldown = 0.4f;
        [Min(0.01f)] public float WallProbeDistance = 0.2f;
        [Min(0.01f)] public float GroundProbeDepth = 0.4f;
        [Min(0.01f)] public float GroundProbeWidth = 0.2f;
        public bool StartFacingRight = false;

        /// <summary>Walker defaults (red 0.8 x 0.8 body).</summary>
        public GroundEnemySettings()
        {
            BodySize = new Vector2(0.8f, 0.8f);
            BaseColor = new Color(0.8f, 0.25f, 0.2f);
        }
    }

    /// <summary>Walks the ground, turns at ledges and walls, damages the player on contact.</summary>
    public class GroundEnemy : EnemyBase
    {
        private const float ProbeMargin = 0.05f;
        private const float FeetRaise = 0.1f;

        [SerializeField] private GroundEnemySettings _settings = new GroundEnemySettings();

        private float _direction;
        private float _turnTimer;

        /// <inheritdoc/>
        protected override EnemySettings Settings => _settings;

        /// <inheritdoc/>
        protected override void Awake()
        {
            base.Awake();
            Body.bodyType = RigidbodyType2D.Dynamic;
            _direction = _settings.StartFacingRight ? 1f : -1f;
        }

        /// <inheritdoc/>
        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            if (IsDead) return;

            _turnTimer -= Time.fixedDeltaTime;
            Bounds bounds = Collider.bounds;
            float edgeX = bounds.center.x + _direction * (bounds.extents.x + ProbeMargin);

            // Wall probe: a box ahead of the body, kept above the feet so the floor is not hit.
            var wallSize = new Vector2(_settings.WallProbeDistance, bounds.size.y * 0.6f);
            var wallCenter = new Vector2(edgeX + _direction * wallSize.x * 0.5f, bounds.center.y);
            bool wallAhead = OverlapsSolid(wallCenter, wallSize);

            // Ground probe: a box ahead of the feet reaching below them.
            var groundSize = new Vector2(_settings.GroundProbeWidth, _settings.GroundProbeDepth + FeetRaise);
            var groundCenter = new Vector2(edgeX + _direction * groundSize.x * 0.5f, bounds.min.y + FeetRaise - groundSize.y * 0.5f);
            bool groundAhead = OverlapsSolid(groundCenter, groundSize);

            if (EnemyPatrolLogic.ShouldTurn(groundAhead, wallAhead, _turnTimer))
            {
                _direction = -_direction;
                _turnTimer = _settings.TurnCooldown;
            }

            Body.linearVelocity = new Vector2(_direction * _settings.Speed, Body.linearVelocity.y);
        }
    }
}
