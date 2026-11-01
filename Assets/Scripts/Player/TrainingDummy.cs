using System;
using Heretic.Combat;
using UnityEngine;

namespace Heretic.Player
{
    /// <summary>Tunable dummy values. Serialized on <see cref="PlayerTestBootstrap"/> so edits persist in the scene.</summary>
    [Serializable]
    public class TrainingDummySettings
    {
        [Min(1)] public int MaxHealth = 5;
        [Min(0.01f)] public float FlashSeconds = 0.12f;
        public Color BaseColor = new Color(0.7f, 0.55f, 0.3f);
        public Color FlashColor = Color.white;
    }

    /// <summary>
    /// Test target for the player's slash (story 003). Flashes when hit and refills when its health runs out,
    /// so it can be hit forever. Story 004 replaces it with real enemies.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class TrainingDummy : MonoBehaviour, IDamageable
    {
        private TrainingDummySettings _s;
        private SpriteRenderer _renderer;
        private float _flashTimer;

        /// <summary>Current health.</summary>
        public int Health { get; private set; }

        /// <summary>Applies the settings. Call right after AddComponent.</summary>
        /// <param name="settings">Tunable values (owned by the bootstrap).</param>
        public void Initialize(TrainingDummySettings settings)
        {
            _s = settings;
            _renderer = GetComponent<SpriteRenderer>();
            _renderer.color = _s.BaseColor;
            Health = _s.MaxHealth;
        }

        /// <summary>Removes health and flashes; refills at 0.</summary>
        /// <param name="amount">Health to remove.</param>
        public void TakeDamage(int amount)
        {
            Health -= amount;
            if (Health <= 0) Health = _s.MaxHealth;
            _flashTimer = _s.FlashSeconds;
            _renderer.color = _s.FlashColor;
        }

        private void Update()
        {
            if (_flashTimer <= 0f) return;
            _flashTimer -= Time.deltaTime;
            if (_flashTimer <= 0f) _renderer.color = _s.BaseColor;
        }
    }
}
