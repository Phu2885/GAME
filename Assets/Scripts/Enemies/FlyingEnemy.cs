using System;
using UnityEngine;

namespace Heretic.Enemies
{
    /// <summary>Tunables of the flying enemy.</summary>
    [Serializable]
    public class FlyingEnemySettings : EnemySettings
    {
        [Min(0.01f)] public float FlightSpeed = 2.5f;
        [Min(0.01f)] public float SweepHalfWidth = 3f;
        [Min(0f)] public float VerticalAmplitude = 0.8f;
        [Min(0f)] public float VerticalFrequency = 0.6f;

        /// <summary>Flyer defaults (purple 0.7 x 0.5 body).</summary>
        public FlyingEnemySettings()
        {
            MaxHealth = 2;
            BodySize = new Vector2(0.7f, 0.5f);
            BaseColor = new Color(0.6f, 0.3f, 0.8f);
        }
    }

    /// <summary>Flies a sine curve around its spawn point (kinematic trigger body), damages the player on contact.</summary>
    public class FlyingEnemy : EnemyBase
    {
        [SerializeField] private FlyingEnemySettings _settings = new FlyingEnemySettings();

        private Vector2 _origin;
        private float _time;

        /// <inheritdoc/>
        protected override EnemySettings Settings => _settings;

        /// <inheritdoc/>
        protected override void Awake()
        {
            base.Awake();
            Body.bodyType = RigidbodyType2D.Kinematic;
            Body.gravityScale = 0f;
            Collider.isTrigger = true;
        }

        private void Start()
        {
            // Read here, not in Awake, so a spawner may position the object after AddComponent.
            _origin = Body.position;
        }

        /// <inheritdoc/>
        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            if (IsDead) return;

            _time += Time.fixedDeltaTime;
            Body.MovePosition(FlightPath.Evaluate(_origin, _time, _settings));
        }
    }
}
