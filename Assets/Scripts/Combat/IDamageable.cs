namespace Heretic.Combat
{
    /// <summary>Anything the player's slash can hurt. Looked up with GetComponent on the collider that was hit.</summary>
    public interface IDamageable
    {
        /// <summary>Removes health from this target.</summary>
        /// <param name="amount">Health to remove.</param>
        void TakeDamage(int amount);
    }
}
