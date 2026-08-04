using System;

namespace _1_2D_Top_Down
{
    // Данни които ще принадлежат на постоянния профил на играча
    public sealed class PlayerProfile
    {
        public Experience Experience { get; }

        public PlayerProfile()
            : this(new Experience())
        {
        }

        public PlayerProfile(Experience experience)
        {
            Experience = experience ?? throw new ArgumentNullException(nameof(experience));
        }
    }
}