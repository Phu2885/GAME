using System;

namespace Heretic.Combat
{
    /// <summary>Arrow keys used by the QTE. Independent of WASD movement.</summary>
    public enum QteArrow { Up, Down, Left, Right }

    /// <summary>Which reward a QTE sequence grants: short = parry, long = counter.</summary>
    public enum QteKind { Parry, Counter }

    /// <summary>Current state of a <see cref="QteSequence"/>.</summary>
    public enum QteStatus { InProgress, Succeeded, Failed }

    /// <summary>
    /// Pure C# arrow-key QTE logic (no MonoBehaviour). Generates a random sequence,
    /// checks each key press, and tracks a real-time window.
    /// Implements: design/game-brief.md, MVP feature 5 (story-001-slowmo-qte).
    /// </summary>
    public class QteSequence
    {
        private readonly QteArrow[] _arrows;
        private int _progress;
        private float _remaining;

        /// <summary>Whether this sequence is a parry (short) or a counter (long).</summary>
        public QteKind Kind { get; }

        /// <summary>The full arrow sequence the player must enter, in order.</summary>
        public ReadOnlySpan<QteArrow> Arrows => _arrows;

        /// <summary>Number of arrows entered correctly in a row so far (resets to 0 on a wrong arrow).</summary>
        public int Progress => _progress;

        /// <summary>Seconds left in the window.</summary>
        public float Remaining => _remaining;

        /// <summary>Current status. Once not InProgress, further input and ticks are ignored.</summary>
        public QteStatus Status { get; private set; } = QteStatus.InProgress;

        /// <summary>Creates a sequence with a random arrow for each of <paramref name="length"/> slots.</summary>
        /// <param name="kind">Parry or counter.</param>
        /// <param name="length">Number of arrows.</param>
        /// <param name="windowSeconds">Real-time seconds allowed to finish.</param>
        /// <param name="random">Random source (inject a seeded one for deterministic tests).</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="length"/> is less than 1.</exception>
        public QteSequence(QteKind kind, int length, float windowSeconds, Random random)
        {
            if (length < 1) throw new ArgumentOutOfRangeException(nameof(length), "A QTE needs at least one arrow.");
            Kind = kind;
            _remaining = windowSeconds;
            _arrows = new QteArrow[length];
            int arrowCount = Enum.GetValues(typeof(QteArrow)).Length;
            for (int i = 0; i < _arrows.Length; i++)
            {
                _arrows[i] = (QteArrow)random.Next(arrowCount);
            }
        }

        /// <summary>Advances the window by real (unscaled) time; fails the sequence on timeout.</summary>
        public void Tick(float unscaledDeltaTime)
        {
            if (Status != QteStatus.InProgress) return;
            _remaining -= unscaledDeltaTime;
            if (_remaining <= 0f)
            {
                _remaining = 0f;
                Status = QteStatus.Failed;
            }
        }

        /// <summary>
        /// Submits a pressed arrow. A wrong arrow sends the player back to the first arrow (the
        /// clock keeps running); the last correct arrow succeeds. Only a timeout fails the sequence.
        /// </summary>
        public void Press(QteArrow arrow)
        {
            if (Status != QteStatus.InProgress) return;
            if (arrow != _arrows[_progress])
            {
                _progress = 0;
                return;
            }
            _progress++;
            if (_progress >= _arrows.Length) Status = QteStatus.Succeeded;
        }
    }
}
