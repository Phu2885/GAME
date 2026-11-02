using System.Collections;
using Heretic.Enemies;
using Heretic.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace Heretic.Level
{
    /// <summary>
    /// LEVEL BOOTSTRAP for story 005 (brief MVP feature 2). Attach to an empty GameObject in an empty scene that has a camera.
    /// Builds the level at runtime from the ASCII map: a Tilemap for the ground, spikes, the real player (story 003),
    /// both enemy types (story 004), the door to the boss room, a kill zone below the level, a follow camera and an OnGUI HUD.
    /// Death (health 0, spikes, enemies, a fall) restarts the level after a delay; R restarts at any time.
    ///
    /// Map legend (first line is the top row; the last line is y = 0; one character = one world unit):
    ///   #  solid ground tile     .  empty      ^  spikes
    ///   P  player start (feet on the bottom of the cell)
    ///   G  ground enemy          F  flying enemy      D  door to the boss room
    /// </summary>
    public class LevelOneBootstrap : MonoBehaviour
    {
        private const int SpriteSize = 32;

        [Header("Level map")]
        [SerializeField, TextArea(14, 20)] private string _map = DefaultMap;

        [Header("Look")]
        [SerializeField] private Color _backgroundColor = new Color(0.05f, 0.08f, 0.14f);
        [SerializeField] private Color _groundColor = new Color(0.2f, 0.25f, 0.35f);
        [SerializeField] private Color _spikeColor = new Color(0.65f, 0.7f, 0.8f);
        [SerializeField] private Color _doorColor = new Color(0.45f, 0.25f, 0.15f);
        [SerializeField] private CameraFollowSettings _cameraSettings = new CameraFollowSettings();

        [Header("Hazards and door")]
        [SerializeField, Min(0)] private int _spikeDamage = 1;
        [SerializeField, Min(0f)] private float _spikeCooldownSeconds = 0.5f;
        [SerializeField] private Vector2 _spikeHitboxSize = new Vector2(0.8f, 0.6f);
        [SerializeField, Min(0.5f), Tooltip("How far below the bottom of the map the kill zone sits.")] private float _killZoneDepth = 6f;
        [SerializeField] private Vector2 _doorSize = new Vector2(1f, 2f);
        [SerializeField] private string _bossSceneName = "BossArena";

        [Header("Death")]
        [SerializeField, Min(0f), Tooltip("Seconds between dying and the level restarting (the death animation plays).")] private float _restartDelaySeconds = 1.5f;

        [Header("Enemies (empty prefab = built from scratch with AddComponent)")]
        [SerializeField] private GameObject _groundEnemyPrefab;
        [SerializeField] private GameObject _flyingEnemyPrefab;

        [Header("Sprite sheet: 14 sprites in order demon_0 ... demon_13 (0-1 idle, 2-5 run, 6 jump, 7 dash, 8-10 attack, 11-13 death)")]
        [SerializeField] private Sprite[] _frames;

        [Header("Player tuning (persists in the scene)")]
        [SerializeField] private PlayerSettings _playerSettings = new PlayerSettings();
        [SerializeField] private PlayerHealthSettings _healthSettings = new PlayerHealthSettings();
        [SerializeField] private PlayerAnimationSettings _animationSettings = new PlayerAnimationSettings();

        private PlayerController _player;
        private PlayerHealth _health;
        private Sprite _squareSprite;
        private Sprite _spikeSprite;
        private bool _restarting;
        private GUIStyle _label;
        private GUIStyle _big;

        private const string DefaultMap =
            "#................................................................................#\n" +
            "#................................................................................#\n" +
            "#................................................................................#\n" +
            "#................................................................................#\n" +
            "#................................................................................#\n" +
            "#................................................................................#\n" +
            "#................................................................................#\n" +
            "#.......................................G.........F..............................#\n" +
            "#..................#................########.....................................#\n" +
            "#..P...............#...^^^....G.....########................................D....#\n" +
            "############...#################################....#######...####################\n" +
            "############...#################################....#######...####################\n" +
            "############...#################################....#######...####################";

        private void Awake()
        {
            _squareSprite = MakeSprite(false);
            _spikeSprite = MakeSprite(true);

            string[] rows = _map.Replace("\r", "").Split('\n');
            int height = rows.Length;
            while (height > 0 && rows[height - 1].Length == 0) height--;
            int width = 0;
            for (int i = 0; i < height; i++) width = Mathf.Max(width, rows[i].Length);

            Tilemap tilemap = CreateTilemap();
            var tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = _squareSprite;
            tile.colliderType = Tile.ColliderType.Grid;

            var playerStart = Vector2.zero;
            bool hasStart = false;
            for (int row = 0; row < height; row++)
            {
                int y = height - 1 - row;
                for (int x = 0; x < rows[row].Length; x++)
                {
                    var cellCenter = new Vector2(x + 0.5f, y + 0.5f);
                    switch (rows[row][x])
                    {
                        case '#':
                            tilemap.SetTile(new Vector3Int(x, y, 0), tile);
                            break;
                        case '^':
                            CreateSpikes(cellCenter);
                            break;
                        case 'P':
                            playerStart = new Vector2(x + 0.5f, y);
                            hasStart = true;
                            break;
                        case 'G':
                            Spawn<GroundEnemy>(_groundEnemyPrefab, "GroundEnemy", cellCenter);
                            break;
                        case 'F':
                            Spawn<FlyingEnemy>(_flyingEnemyPrefab, "FlyingEnemy", cellCenter);
                            break;
                        case 'D':
                            CreateDoor(new Vector2(x + 0.5f, y + _doorSize.y * 0.5f));
                            break;
                    }
                }
            }

            if (!hasStart) Debug.LogError("LevelOneBootstrap: the map has no 'P' (player start).");
            var levelBounds = new Rect(0f, 0f, width, height);

            _player = PlayerFactory.Create(playerStart, _playerSettings, _healthSettings, _animationSettings, _frames, out _health);
            _health.Died += HandlePlayerDied;

            CreateKillZone(levelBounds);
            CreateCamera(levelBounds);
        }

        private void OnDestroy()
        {
            if (_health != null) _health.Died -= HandlePlayerDied;
        }

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard != null && keyboard.rKey.wasPressedThisFrame) RestartLevel();
        }

        private void HandlePlayerDied()
        {
            StartCoroutine(RestartAfterDelay());
        }

        private IEnumerator RestartAfterDelay()
        {
            yield return new WaitForSecondsRealtime(_restartDelaySeconds);
            RestartLevel();
        }

        // Reloading the scene also respawns every enemy and the player.
        private void RestartLevel()
        {
            if (_restarting) return;
            _restarting = true;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private Tilemap CreateTilemap()
        {
            var gridObject = new GameObject("Grid");
            gridObject.AddComponent<Grid>();

            var groundObject = new GameObject("Ground");
            groundObject.transform.SetParent(gridObject.transform, false);
            Tilemap tilemap = groundObject.AddComponent<Tilemap>();
            tilemap.color = _groundColor;
            var tilemapRenderer = groundObject.AddComponent<TilemapRenderer>();
            tilemapRenderer.sortingOrder = 0;
            if (tilemapRenderer.sharedMaterial == null)
            {
                // Borrow the default sprite material so the tiles never render as the error colour.
                var materialSource = new GameObject("MaterialSource").AddComponent<SpriteRenderer>();
                tilemapRenderer.sharedMaterial = materialSource.sharedMaterial;
                Destroy(materialSource.gameObject);
            }

            // Static body + composite so the tiles merge into one solid shape (no seams for the player to catch on).
            groundObject.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            groundObject.AddComponent<CompositeCollider2D>().geometryType = CompositeCollider2D.GeometryType.Polygons;
            groundObject.AddComponent<TilemapCollider2D>().compositeOperation = Collider2D.CompositeOperation.Merge;
            return tilemap;
        }

        private void CreateSpikes(Vector2 cellCenter)
        {
            var spikes = new GameObject("Spikes");
            spikes.transform.position = cellCenter;
            var spriteRenderer = spikes.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = _spikeSprite;
            spriteRenderer.color = _spikeColor;
            spriteRenderer.sortingOrder = 1;

            // Hitbox sits on the floor of the cell.
            var box = spikes.AddComponent<BoxCollider2D>();
            box.isTrigger = true;
            box.size = _spikeHitboxSize;
            box.offset = new Vector2(0f, -0.5f + _spikeHitboxSize.y * 0.5f);
            spikes.AddComponent<Hazard>().Initialize(_spikeDamage, _spikeCooldownSeconds, false);
        }

        private void CreateDoor(Vector2 center)
        {
            var door = new GameObject("BossDoor");
            door.transform.position = center;
            door.transform.localScale = new Vector3(_doorSize.x, _doorSize.y, 1f);
            var spriteRenderer = door.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = _squareSprite;
            spriteRenderer.color = _doorColor;
            spriteRenderer.sortingOrder = 1;
            door.AddComponent<BoxCollider2D>().isTrigger = true;
            door.AddComponent<LevelDoor>().Initialize(_bossSceneName);
        }

        private void CreateKillZone(Rect levelBounds)
        {
            var zone = new GameObject("KillZone");
            zone.transform.position = new Vector2(levelBounds.center.x, -_killZoneDepth);
            var box = zone.AddComponent<BoxCollider2D>();
            box.isTrigger = true;
            box.size = new Vector2(levelBounds.width + 40f, 2f);
            zone.AddComponent<Hazard>().Initialize(0, 0f, true);
        }

        private void CreateCamera(Rect levelBounds)
        {
            Camera levelCamera = Camera.main;
            if (levelCamera == null) levelCamera = FindFirstObjectByType<Camera>();
            if (levelCamera == null) levelCamera = new GameObject("LevelCamera").AddComponent<Camera>();
            levelCamera.clearFlags = CameraClearFlags.SolidColor;
            levelCamera.backgroundColor = _backgroundColor;
            if (!levelCamera.TryGetComponent(out CameraFollow follow)) follow = levelCamera.gameObject.AddComponent<CameraFollow>();
            follow.Initialize(_cameraSettings, _player.transform, levelBounds);
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

        // White square, or an upward triangle, from a Point-filtered texture at 32 PPU (one cell = one unit).
        private static Sprite MakeSprite(bool triangle)
        {
            var texture = new Texture2D(SpriteSize, SpriteSize) { filterMode = FilterMode.Point };
            var pixels = new Color[SpriteSize * SpriteSize];
            for (int py = 0; py < SpriteSize; py++)
            {
                for (int px = 0; px < SpriteSize; px++)
                {
                    // Triangle: half-width shrinks from the full cell at the bottom to nothing at the top.
                    float halfWidth = (SpriteSize - py) * 0.5f;
                    bool filled = !triangle || Mathf.Abs(px + 0.5f - SpriteSize * 0.5f) <= halfWidth;
                    pixels[py * SpriteSize + px] = filled ? Color.white : Color.clear;
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();
            return Sprite.Create(texture, new Rect(0f, 0f, SpriteSize, SpriteSize), new Vector2(0.5f, 0.5f), SpriteSize);
        }

        private void OnGUI()
        {
            if (_health == null) return;
            _label ??= new GUIStyle(GUI.skin.label) { fontSize = 18 };
            _big ??= new GUIStyle(GUI.skin.label) { fontSize = 40, alignment = TextAnchor.MiddleCenter };

            GUI.color = new Color(0f, 0f, 0f, 0.7f);
            GUI.DrawTexture(new Rect(20f, 20f, 250f, 24f), Texture2D.whiteTexture);
            GUI.color = new Color(0.3f, 0.8f, 1f);
            GUI.DrawTexture(new Rect(20f, 20f, 250f * Mathf.Clamp01(_health.Health / (float)_health.MaxHealth), 24f), Texture2D.whiteTexture);
            GUI.color = Color.white;
            GUI.Label(new Rect(20f, 46f, 300f, 28f), $"Health {_health.Health}/{_health.MaxHealth}", _label);

            if (_health.IsDead) GUI.Label(new Rect(0f, Screen.height * 0.4f, Screen.width, 100f), "YOU DIED\nRestarting...", _big);

            GUI.Label(new Rect(20f, Screen.height - 34f, Screen.width, 28f), "A/D run   W/Space jump   Left Shift dash   J attack   Reach the door   R = restart", _label);
        }
    }
}
