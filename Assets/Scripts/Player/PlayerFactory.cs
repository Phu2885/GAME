using UnityEngine;

namespace Heretic.Player
{
    /// <summary>Builds the real player (story 003) at runtime; shared by the test, boss and level bootstraps.</summary>
    public static class PlayerFactory
    {
        /// <summary>Creates the player object with body, collider, controller, health and (when frames are given) animator.</summary>
        /// <param name="position">Start position; the sprite pivot is at the feet, so this is where the feet stand. Also the respawn point.</param>
        /// <param name="playerSettings">Movement, dash and attack tunables (owned by the caller's bootstrap).</param>
        /// <param name="healthSettings">Health tunables (owned by the caller's bootstrap).</param>
        /// <param name="animationSettings">Animation tunables (owned by the caller's bootstrap).</param>
        /// <param name="frames">The 14 demon_sheet sprites in order (demon_0 ... demon_13).</param>
        /// <param name="health">The health component of the new player.</param>
        /// <returns>The controller of the new player.</returns>
        public static PlayerController Create(Vector2 position, PlayerSettings playerSettings, PlayerHealthSettings healthSettings,
            PlayerAnimationSettings animationSettings, Sprite[] frames, out PlayerHealth health)
        {
            var playerObject = new GameObject("Player");
            playerObject.transform.position = position;
            var playerRenderer = playerObject.AddComponent<SpriteRenderer>();
            playerRenderer.sortingOrder = 3;
            PlayerController controller = playerObject.AddComponent<PlayerController>();
            health = playerObject.AddComponent<PlayerHealth>();
            health.Initialize(healthSettings);
            controller.Initialize(playerSettings, health);

            if (frames != null && frames.Length >= 14)
            {
                PlayerAnimator animator = playerObject.AddComponent<PlayerAnimator>();
                animator.Initialize(animationSettings, frames, controller, health);
            }
            else
            {
                Debug.LogWarning("PlayerFactory: assign the 14 demon_sheet sprites (demon_0 ... demon_13) to _frames; the player is invisible without them.");
            }

            return controller;
        }
    }
}
