using Heretic.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Heretic.Level
{
    /// <summary>Trigger at the end of the level: touching it loads the boss room (story 005 -> story 002).</summary>
    public class LevelDoor : MonoBehaviour
    {
        private string _sceneName;
        private bool _loading;

        /// <summary>Sets the scene to load. Call right after AddComponent.</summary>
        /// <param name="sceneName">Name of the boss scene (must be in the build settings).</param>
        public void Initialize(string sceneName)
        {
            _sceneName = sceneName;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_loading || !other.TryGetComponent(out PlayerHealth health) || health.IsDead) return;

            _loading = true;
            SceneManager.LoadScene(_sceneName);
        }
    }
}
