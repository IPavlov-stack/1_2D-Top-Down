using System.Collections.Generic;

namespace _1_2D_Top_Down
{
    /// <summary>
    /// Owns enemy runtime collections for the active mission.
    /// Enemy behaviour remains in Game1 during this transitional step.
    /// </summary>
    public sealed class EnemyManager
    {
        public List<Demon> Demons { get; } = new();
        public List<Evil_Eye> EvilEyes { get; } = new();
        public List<DeathAnimation> DemonDeathAnimations { get; } = new();

        public void Clear()
        {
            Demons.Clear();
            EvilEyes.Clear();
            DemonDeathAnimations.Clear();
        }
    }
}