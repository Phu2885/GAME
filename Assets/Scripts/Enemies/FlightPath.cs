using UnityEngine;

namespace Heretic.Enemies
{
    /// <summary>Pure flight curve of the flying enemy: horizontal sine sweep plus vertical sine (testable).</summary>
    public static class FlightPath
    {
        private const float MinSweepHalfWidth = 0.01f;

        /// <summary>Position at a time since spawn.</summary>
        /// <param name="origin">Start point the path oscillates around.</param>
        /// <param name="time">Seconds since the path started.</param>
        /// <param name="settings">Sweep half width, peak speed, vertical amplitude and frequency.</param>
        public static Vector2 Evaluate(Vector2 origin, float time, FlyingEnemySettings settings)
        {
            float halfWidth = Mathf.Max(settings.SweepHalfWidth, MinSweepHalfWidth);
            float x = halfWidth * Mathf.Sin(settings.FlightSpeed / halfWidth * time);
            float y = settings.VerticalAmplitude * Mathf.Sin(2f * Mathf.PI * settings.VerticalFrequency * time);
            return origin + new Vector2(x, y);
        }
    }
}
