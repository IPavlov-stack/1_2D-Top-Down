namespace _1_2D_Top_Down
{
    /// <summary>
    /// Owns the domain managers and runtime objects for the active mission.
    /// </summary>
    public sealed class GameWorld
    {
        public EnemyManager Enemies { get; } = new();
        public ProjectileManager Projectiles { get; } = new();
        public CollectibleManager Collectibles { get; } = new();

        public void ClearMissionObjects()
        {
            Enemies.Clear();
            Projectiles.Clear();
            Collectibles.Clear();
        }
    }
}
