using UnityEngine;
using UnityEngine.InputSystem;

namespace Heretic.Combat
{
    /// <summary>
    /// TEST HARNESS ONLY (story-001 criterion 7). Attach to an empty GameObject in any scene.
    /// Space = fake boss heavy attack (parry, short sequence); Enter = heavy attack (counter, long).
    /// Shows prompts, a fake player's health, and the last result via OnGUI.
    /// Replaced by the real boss (story 002) and player (story 003).
    /// </summary>
    public class QteTestRoom : MonoBehaviour
    {
        [SerializeField, Min(1)] private int _maxHealth = 5;
        [SerializeField, Min(1)] private int _damageOnFail = 1;

        private SlowMotionController _controller;
        private int _health;
        private string _lastResult = "Press Space (parry) or Enter (counter)";

        private void Awake()
        {
            _controller = gameObject.AddComponent<SlowMotionController>();
            _health = _maxHealth;
        }

        private void OnEnable()
        {
            _controller.ParrySucceeded += HandleParry;
            _controller.CounterSucceeded += HandleCounter;
            _controller.QteFailed += HandleFail;
        }

        private void OnDisable()
        {
            _controller.ParrySucceeded -= HandleParry;
            _controller.CounterSucceeded -= HandleCounter;
            _controller.QteFailed -= HandleFail;
        }

        private void HandleParry() => _lastResult = "PARRY!";
        private void HandleCounter() => _lastResult = "COUNTER!";

        private void HandleFail()
        {
            _health = Mathf.Max(0, _health - _damageOnFail);
            _lastResult = "FAILED";
        }

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null) return;
            if (keyboard.spaceKey.wasPressedThisFrame) _controller.TriggerHeavyAttack(QteKind.Parry);
            if (keyboard.enterKey.wasPressedThisFrame) _controller.TriggerHeavyAttack(QteKind.Counter);
        }

        private void OnGUI()
        {
            var big = new GUIStyle(GUI.skin.label) { fontSize = 32 };
            GUILayout.BeginArea(new Rect(20, 20, 900, 300));
            GUILayout.Label($"Fake player health: {_health}/{_maxHealth}", big);
            GUILayout.Label(_lastResult, big);

            QteSequence sequence = _controller.Sequence;
            if (sequence != null)
            {
                string prompt = "";
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
                GUILayout.Label(prompt, big);
                GUILayout.Label($"Time left: {sequence.Remaining:0.00}s", big);
            }
            GUILayout.EndArea();
        }
    }
}
