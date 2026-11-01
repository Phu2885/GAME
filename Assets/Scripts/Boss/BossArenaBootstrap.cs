using Heretic.Combat;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Heretic.Boss
{
    /// <summary>
    /// TEST/ARENA BOOTSTRAP for story 002. Attach to an empty GameObject in an empty scene that has a camera.
    /// Builds at runtime: ground and walls, the temporary player, the Paladin boss, the QTE controller
    /// (story 001) and an OnGUI HUD (health bars, boss state, QTE prompt, VICTORY / DEFEAT, R to restart).
    /// Placeholder visuals only: coloured squares from a Point-filtered texture at 32 PPU.
    /// </summary>
    public class BossArenaBootstrap : MonoBehaviour
    {
        private enum FightResult { Fighting, Victory, Defeat }

        [Header("Arena")]
        [SerializeField, Min(4f)] private float _arenaHalfWidth = 12f;
        [SerializeField, Min(1f)] private float _wallHeight = 20f;
        [SerializeField, Min(1f)] private float _cameraSize = 7f;

        [Header("Boss tuning (persists in the scene)")]
        [SerializeField] private PaladinBossSettings _bossSettings = new PaladinBossSettings();

        [Header("Temporary player tuning (persists in the scene)")]
        [SerializeField] private TempPlayerSettings _playerSettings = new TempPlayerSettings();

        private Sprite _squareSprite;
        private TempPlayer _player;
        private PaladinBoss _boss;
        private SlowMotionController _qte;
        private FightResult _result = FightResult.Fighting;
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

            Camera arenaCamera = Camera.main;
            if (arenaCamera == null) arenaCamera = FindFirstObjectByType<Camera>();
            if (arenaCamera == null) arenaCamera = new GameObject("ArenaCamera").AddComponent<Camera>();
            arenaCamera.orthographic = true;
            arenaCamera.orthographicSize = _cameraSize;
            arenaCamera.transform.position = new Vector3(0f, _cameraSize - 2f, -10f);
            arenaCamera.clearFlags = CameraClearFlags.SolidColor;
            arenaCamera.backgroundColor = new Color(0.05f, 0.08f, 0.14f);

            // Ground top surface at y = 0; walls' inner faces at +-arenaHalfWidth.
            var groundColor = new Color(0.2f, 0.25f, 0.35f);
            AddBlock("Ground", new Vector2(0f, -0.5f), new Vector2(_arenaHalfWidth * 2f + 2f, 1f), groundColor);
            AddBlock("WallLeft", new Vector2(-_arenaHalfWidth - 0.5f, _wallHeight * 0.5f), new Vector2(1f, _wallHeight), groundColor);
            AddBlock("WallRight", new Vector2(_arenaHalfWidth + 0.5f, _wallHeight * 0.5f), new Vector2(1f, _wallHeight), groundColor);

            _qte = gameObject.AddComponent<SlowMotionController>();

            GameObject playerObject = CreateSquare("TempPlayer", new Vector2(-8f, 1f), Vector2.one, new Color(0.3f, 0.8f, 1f), 3);
            _player = playerObject.AddComponent<TempPlayer>();
            _player.Initialize(_playerSettings);

            GameObject bossObject = CreateSquare("PaladinBoss", Vector2.zero, Vector2.one, new Color(0.85f, 0.85f, 0.9f), 1);
            _boss = bossObject.AddComponent<PaladinBoss>();
            _boss.Initialize(_bossSettings, _player, _qte, -_arenaHalfWidth, _arenaHalfWidth, 0f);

            _player.Died += HandlePlayerDied;
            _boss.BossDefeated += HandleBossDefeated;
        }

        private void OnValidate()
        {
            // The boss needs at least one attack to cycle through.
            if (_bossSettings.Pattern == null || _bossSettings.Pattern.Length == 0)
            {
                _bossSettings.Pattern = new[] { BossAttack.GroundSlam };
            }
        }

        private void OnDestroy()
        {
            if (_player != null) _player.Died -= HandlePlayerDied;
            if (_boss != null) _boss.BossDefeated -= HandleBossDefeated;
        }

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (_result == FightResult.Fighting || keyboard == null || !keyboard.rKey.wasPressedThisFrame) return;

            // Fight lost or won: restart from the beginning with full health for both.
            _boss.ResetFight();
            _player.ResetPlayer();
            _result = FightResult.Fighting;
        }

        private void HandlePlayerDied()
        {
            _result = FightResult.Defeat;
            _boss.StopFight();
        }

        private void HandleBossDefeated()
        {
            _result = FightResult.Victory;
            _boss.StopFight();
            _player.Halt();
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
            if (_boss == null || _player == null) return;
            _label ??= new GUIStyle(GUI.skin.label) { fontSize = 18 };
            _big ??= new GUIStyle(GUI.skin.label) { fontSize = 40, alignment = TextAnchor.MiddleCenter };

            float width = Screen.width;
            float height = Screen.height;

            DrawBar(new Rect(20f, 20f, 250f, 24f), _player.Health / (float)_player.MaxHealth, new Color(0.3f, 0.8f, 1f));
            GUI.Label(new Rect(20f, 46f, 300f, 28f), $"Player {_player.Health}/{_player.MaxHealth}", _label);

            DrawBar(new Rect(width * 0.5f - 250f, 20f, 500f, 24f), _boss.Health / (float)_boss.MaxHealth, new Color(0.9f, 0.2f, 0.2f));
            GUI.Label(new Rect(width * 0.5f - 250f, 46f, 500f, 28f), $"Paladin {_boss.Health}/{_boss.MaxHealth}   State: {_boss.State} ({_boss.CurrentAttack})", _label);

            QteSequence sequence = _qte.Sequence;
            if (sequence != null)
            {
                string prompt = sequence.Kind == QteKind.Parry ? "PARRY\n" : "COUNTER\n";
                for (int i = 0; i < sequence.Arrows.Length; i++)
                {
                    string glyph = sequence.Arrows[i] switch
                    {
                        QteArrow.Up => "^",
                        QteArrow.Down => "v",
                        QteArrow.Left => "<",
                        _ => ">",
                    };
                    prompt += i < sequence.Progress ? $"[{glyph}] " : $" {glyph}  ";
                }
                prompt += $"\n{sequence.Remaining:0.00}s";
                GUI.Label(new Rect(0f, height * 0.2f, width, 200f), prompt, _big);
            }

            if (_result != FightResult.Fighting)
            {
                string message = _result == FightResult.Victory ? "VICTORY" : "DEFEAT";
                GUI.Label(new Rect(0f, height * 0.4f, width, 100f), $"{message}\nPress R to restart the fight", _big);
            }

            GUI.Label(new Rect(20f, height - 34f, width, 28f), "A/D move   W/Space jump   Left Shift dash   Arrow keys = QTE   R restart (after the fight ends)", _label);
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
