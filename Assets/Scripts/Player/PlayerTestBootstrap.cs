using UnityEngine;
using UnityEngine.InputSystem;

namespace Heretic.Player
{
    /// <summary>
    /// TEST BOOTSTRAP for story 003. Attach to an empty GameObject in an empty scene that has a camera.
    /// Builds at runtime: ground and walls, the player, two training dummies and an OnGUI HUD.
    /// R resets the player (health, position). All player tunables live here so Inspector edits persist in the scene.
    /// </summary>
    public class PlayerTestBootstrap : MonoBehaviour
    {
        [Header("Arena")]
        [SerializeField, Min(4f)] private float _arenaHalfWidth = 12f;
        [SerializeField, Min(1f)] private float _wallHeight = 20f;
        [SerializeField, Min(1f)] private float _cameraSize = 7f;
        [SerializeField] private Vector2 _playerStart = new Vector2(-8f, 0f);
        [SerializeField] private float[] _dummyXPositions = { 2f, 6f };

        [Header("Sprite sheet: 14 sprites in order demon_0 ... demon_13 (0-1 idle, 2-5 run, 6 jump, 7 dash, 8-10 attack, 11-13 death)")]
        [SerializeField] private Sprite[] _frames;

        [Header("Player tuning (persists in the scene)")]
        [SerializeField] private PlayerSettings _playerSettings = new PlayerSettings();
        [SerializeField] private PlayerHealthSettings _healthSettings = new PlayerHealthSettings();
        [SerializeField] private PlayerAnimationSettings _animationSettings = new PlayerAnimationSettings();

        [Header("Training dummy tuning")]
        [SerializeField] private TrainingDummySettings _dummySettings = new TrainingDummySettings();

        private Sprite _squareSprite;
        private PlayerHealth _health;
        private TrainingDummy[] _dummies;
        private GUIStyle _label;
        private GUIStyle _big;

        private void Awake()
        {
            var texture = new Texture2D(32, 32) { filterMode = FilterMode.Point };
            var pixels = new Color[32 * 32];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.white;
            texture.SetPixels(pixels);
            texture.Apply();
            _squareSprite = Sprite.Create(texture, new Rect(0f, 0f, 32f, 32f), new Vector2(0.5f, 0.5f), 32f);

            Camera testCamera = Camera.main;
            if (testCamera == null) testCamera = FindFirstObjectByType<Camera>();
            if (testCamera == null) testCamera = new GameObject("TestCamera").AddComponent<Camera>();
            testCamera.orthographic = true;
            testCamera.orthographicSize = _cameraSize;
            testCamera.transform.position = new Vector3(0f, _cameraSize - 2f, -10f);
            testCamera.clearFlags = CameraClearFlags.SolidColor;
            testCamera.backgroundColor = new Color(0.05f, 0.08f, 0.14f);

            // Ground top surface at y = 0; walls' inner faces at +-arenaHalfWidth.
            var groundColor = new Color(0.2f, 0.25f, 0.35f);
            AddBlock("Ground", new Vector2(0f, -0.5f), new Vector2(_arenaHalfWidth * 2f + 2f, 1f), groundColor);
            AddBlock("WallLeft", new Vector2(-_arenaHalfWidth - 0.5f, _wallHeight * 0.5f), new Vector2(1f, _wallHeight), groundColor);
            AddBlock("WallRight", new Vector2(_arenaHalfWidth + 0.5f, _wallHeight * 0.5f), new Vector2(1f, _wallHeight), groundColor);

            _dummies = new TrainingDummy[_dummyXPositions.Length];
            for (int i = 0; i < _dummies.Length; i++)
            {
                // Dummy sprite is a 1 x 1 square scaled to 0.8 x 1.2, centred 0.6 above the ground.
                GameObject dummyObject = CreateSquare($"TrainingDummy{i}", new Vector2(_dummyXPositions[i], 0.6f), new Vector2(0.8f, 1.2f), Color.white, 1);
                dummyObject.AddComponent<BoxCollider2D>().size = Vector2.one;
                _dummies[i] = dummyObject.AddComponent<TrainingDummy>();
                _dummies[i].Initialize(_dummySettings);
            }

            // Player: pivot at the feet, so the transform sits on the ground.
            PlayerFactory.Create(_playerStart, _playerSettings, _healthSettings, _animationSettings, _frames, out _health);
        }

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null) return;
            if (keyboard.rKey.wasPressedThisFrame) _health.ResetPlayer();
            if (keyboard.kKey.wasPressedThisFrame) _health.TakeDamage(1);
        }

        private GameObject CreateSquare(string objectName, Vector2 center, Vector2 size, Color color, int sortingOrder)
        {
            var square = new GameObject(objectName);
            square.transform.position = center;
            square.transform.localScale = new Vector3(size.x, size.y, 1f);
            var spriteRenderer = square.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = _squareSprite;
            spriteRenderer.color = color;
            spriteRenderer.sortingOrder = sortingOrder;
            return square;
        }

        private void AddBlock(string objectName, Vector2 center, Vector2 size, Color color)
        {
            GameObject block = CreateSquare(objectName, center, size, color, 0);
            block.AddComponent<BoxCollider2D>().size = Vector2.one;
        }

        private void OnGUI()
        {
            if (_health == null) return;
            _label ??= new GUIStyle(GUI.skin.label) { fontSize = 18 };
            _big ??= new GUIStyle(GUI.skin.label) { fontSize = 40, alignment = TextAnchor.MiddleCenter };

            DrawBar(new Rect(20f, 20f, 250f, 24f), _health.Health / (float)_health.MaxHealth, new Color(0.3f, 0.8f, 1f));
            GUI.Label(new Rect(20f, 46f, 300f, 28f), $"Health {_health.Health}/{_health.MaxHealth}", _label);

            string dummies = "Dummies:";
            for (int i = 0; i < _dummies.Length; i++) dummies += $" {_dummies[i].Health}";
            GUI.Label(new Rect(20f, 72f, 400f, 28f), dummies, _label);

            if (_health.IsDead) GUI.Label(new Rect(0f, Screen.height * 0.4f, Screen.width, 100f), "YOU DIED\nPress R to restart", _big);

            GUI.Label(new Rect(20f, Screen.height - 34f, Screen.width, 28f), "A/D run   W/Space jump   Left Shift dash   J attack   K = take 1 damage   R = restart", _label);
        }

        private static void DrawBar(Rect rect, float fraction, Color fill)
        {
            GUI.color = new Color(0f, 0f, 0f, 0.7f);
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            GUI.color = fill;
            GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width * Mathf.Clamp01(fraction), rect.height), Texture2D.whiteTexture);
            GUI.color = Color.white;
        }
    }
}
