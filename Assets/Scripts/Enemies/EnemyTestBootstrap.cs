using Heretic.Player;
using UnityEngine;

namespace Heretic.Enemies
{
    /// <summary>
    /// TEST BOOTSTRAP for story 004. Put it in a scene that already has a PlayerTestBootstrap.
    /// Spawns ground and flying enemies at the serialized positions (prefab if assigned, else built from scratch).
    /// </summary>
    public class EnemyTestBootstrap : MonoBehaviour
    {
        [SerializeField] private GameObject _groundEnemyPrefab;
        [SerializeField] private GameObject _flyingEnemyPrefab;
        [SerializeField] private Vector2[] _groundEnemyPositions = { new Vector2(4f, 0.5f), new Vector2(9f, 0.5f) };
        [SerializeField] private Vector2[] _flyingEnemyPositions = { new Vector2(3f, 3f), new Vector2(8f, 3.5f) };

        private void Start()
        {
            if (FindFirstObjectByType<PlayerController>() == null)
            {
                Debug.LogWarning("EnemyTestBootstrap: no PlayerController in the scene; add a PlayerTestBootstrap.");
                return;
            }

            foreach (Vector2 position in _groundEnemyPositions) Spawn<GroundEnemy>(_groundEnemyPrefab, "GroundEnemy", position);
            foreach (Vector2 position in _flyingEnemyPositions) Spawn<FlyingEnemy>(_flyingEnemyPrefab, "FlyingEnemy", position);
        }

        private static void Spawn<T>(GameObject prefab, string objectName, Vector2 position) where T : EnemyBase
        {
            if (prefab != null)
            {
                Instantiate(prefab, position, Quaternion.identity);
                return;
            }

            var enemy = new GameObject(objectName);
            enemy.transform.position = position;
            enemy.AddComponent<T>();
        }
    }
}
