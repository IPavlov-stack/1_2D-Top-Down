using System.Collections.Generic;

namespace _1_2D_Top_Down
{
    /// <summary>
    /// Owns objects that exist only during the current mission.
    /// Game1 coordinates their update and drawing for now; dedicated managers
    /// can be introduced later without moving this state back into Game1.
    /// </summary>
    public sealed class GameWorld
    {
        public List<Demon> Demons { get; } = new();
        public List<Evil_Eye> EvilEyes { get; } = new();
        public List<PlayerProjectile> PlayerProjectiles { get; } = new();
        public List<EnemyProjectile> EnemyProjectiles { get; } = new();
        public List<Coin> Coins { get; } = new();
        public List<ManaCrystal> ManaCrystals { get; } = new();
        public List<DeathAnimation> DemonDeathAnimations { get; } = new();

        public void ClearMissionObjects()
        {
            Demons.Clear();
            EvilEyes.Clear();
            PlayerProjectiles.Clear();
            EnemyProjectiles.Clear();
            Coins.Clear();
            ManaCrystals.Clear();
            DemonDeathAnimations.Clear();
        }
    }
}