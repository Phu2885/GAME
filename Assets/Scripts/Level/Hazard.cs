using Heretic.Player;
using UnityEngine;

namespace Heretic.Level
{
    /// <summary>
    /// Trigger that hurts the player: spikes (damage with a re-hit cooldown) or a kill zone (fatal, ignores invulnerability).
    /// Needs a trigger Collider2D on the same object. Story 005, brief MVP feature 2.
    /// </summary>
    public class Hazard : MonoBehaviour
    {
        private const float CooldownTolerance = 0.0001f;

        private int _damage;
        private float _cooldownSeconds;
        private bool _kills;
        private float _cooldownTimer;

        /// <summary>Sets the hazard values. Call right after AddComponent.</summary>
        /// <param name="damage">Health removed per hit (ignored for a kill zone).</param>
        /// <param name="cooldownSeconds">Minimum time between two hits.</param>
        /// <param name="kills">True for a kill zone: removes all health, through dash invulnerability.</param>
        public void Initialize(int damage, float cooldownSeconds, bool kills)
        {
            _damage = damage;
            _cooldownSeconds = cooldownSeconds;
            _kills = kills;
        }

        private void Update()
        {
            _cooldownTimer -= Time.deltaTime;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (_cooldownTimer > CooldownTolerance) return;
            if (!other.TryGetComponent(out PlayerHealth health) || health.IsDead) return;

            if (_kills)
            {
                health.TakeDamage(health.MaxHealth, true);
                return;
            }

            // The player's own hit invulnerability may swallow the hit; only start the cooldown when it landed.
            if (health.IsInvulnerable) return;
            health.TakeDamage(_damage);
            _cooldownTimer = _cooldownSeconds;
        }
    }
}
