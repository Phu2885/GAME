using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Heretic.Combat
{
    /// <summary>
    /// Slows time on a boss heavy attack and runs an arrow-key QTE in real time.
    /// Implements: design/game-brief.md, MVP feature 5 (story-001-slowmo-qte).
    /// All tuning values are serialized fields so designers can edit them in the Inspector.
    /// </summary>
    public class SlowMotionController : MonoBehaviour
    {
        [Header("Slow motion")]
        [SerializeField, Range(0.01f, 1f)] private float _slowTimeScale = 0.2f;

        [Header("Parry (short sequence)")]
        [SerializeField, Min(1)] private int _parryMinKeys = 3;
        [SerializeField, Min(1)] private int _parryMaxKeys = 4;
        [SerializeField, Min(0.1f)] private float _parryWindowSeconds = 2f;

        [Header("Counter (long sequence)")]
        [SerializeField, Min(1)] private int _counterMinKeys = 6;
        [SerializeField, Min(1)] private int _counterMaxKeys = 8;
        [SerializeField, Min(0.1f)] private float _counterWindowSeconds = 3.5f;

        private QteSequence _sequence;
        private float _savedTimeScale = 1f;
        private readonly System.Random _random = new System.Random();

        /// <summary>Raised when a short (parry) sequence is completed in time.</summary>
        public event Action ParrySucceeded;

        /// <summary>Raised when a long (counter) sequence is completed in time.</summary>
        public event Action CounterSucceeded;

        /// <summary>Raised when the window runs out before the sequence is finished. Listeners apply the health loss.</summary>
        public event Action QteFailed;

        /// <summary>True while slow motion and a QTE are running.</summary>
        public bool IsActive => _sequence != null;

        /// <summary>The running sequence, or null when idle (for UI to display).</summary>
        public QteSequence Sequence => _sequence;

        /// <summary>
        /// Boss heavy-attack event: slows time and starts a QTE of the given kind.
        /// Ignored while a QTE is already running.
        /// </summary>
        public void TriggerHeavyAttack(QteKind kind)
        {
            if (IsActive) return;
            bool isParry = kind == QteKind.Parry;
            int min = isParry ? _parryMinKeys : _counterMinKeys;
            int max = isParry ? _parryMaxKeys : _counterMaxKeys;
            float window = isParry ? _parryWindowSeconds : _counterWindowSeconds;

            _sequence = new QteSequence(kind, _random.Next(min, max + 1), window, _random);
            _savedTimeScale = Time.timeScale;
            Time.timeScale = _slowTimeScale;
        }

        private void Update()
        {
            if (_sequence == null) return;

            // Unscaled time and polled keys: slow motion must not slow the player's input.
            // Keys are read before the clock advances, so a correct last key pressed in the
            // frame the window runs out still counts. Press ignores input once the sequence ended.
            Keyboard keyboard = Keyboard.current;
            if (keyboard != null)
            {
                if (keyboard.upArrowKey.wasPressedThisFrame) _sequence.Press(QteArrow.Up);
                if (keyboard.downArrowKey.wasPressedThisFrame) _sequence.Press(QteArrow.Down);
                if (keyboard.leftArrowKey.wasPressedThisFrame) _sequence.Press(QteArrow.Left);
                if (keyboard.rightArrowKey.wasPressedThisFrame) _sequence.Press(QteArrow.Right);
            }

            _sequence.Tick(Time.unscaledDeltaTime);

            if (_sequence.Status == QteStatus.InProgress) return;

            QteSequence finished = _sequence;
            _sequence = null;
            Time.timeScale = _savedTimeScale;

            if (finished.Status == QteStatus.Failed) QteFailed?.Invoke();
            else if (finished.Kind == QteKind.Parry) ParrySucceeded?.Invoke();
            else CounterSucceeded?.Invoke();
        }

        private void OnValidate()
        {
            // Random.Next(min, max + 1) throws when max < min.
            _parryMaxKeys = Mathf.Max(_parryMinKeys, _parryMaxKeys);
            _counterMaxKeys = Mathf.Max(_counterMinKeys, _counterMaxKeys);
        }

        private void OnDisable()
        {
            if (_sequence == null) return;
            _sequence = null;
            Time.timeScale = _savedTimeScale;
        }
    }
}
