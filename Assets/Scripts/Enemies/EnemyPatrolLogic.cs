namespace Heretic.Enemies
{
    /// <summary>Pure patrol decisions for the walking enemy (no engine dependency, testable).</summary>
    public static class EnemyPatrolLogic
    {
        private const float CooldownTolerance = 0.0001f;

        /// <summary>True when the walker should reverse direction.</summary>
        /// <param name="groundAhead">A ground probe ahead of the feet found a floor.</param>
        /// <param name="wallAhead">A probe ahead of the body found a solid.</param>
        /// <param name="turnCooldownRemaining">Seconds left before another turn is allowed.</param>
        public static bool ShouldTurn(bool groundAhead, bool wallAhead, float turnCooldownRemaining)
        {
            if (turnCooldownRemaining > CooldownTolerance) return false;
            return !groundAhead || wallAhead;
        }
    }
}
