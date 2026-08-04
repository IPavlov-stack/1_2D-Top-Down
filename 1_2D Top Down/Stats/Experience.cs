using System;

namespace _1_2D_Top_Down
{
    public sealed class Experience
    {
        private const int StartingExperienceRequirement = 100;
        private const float RequirementGrowth = 1.25f;

        public int Level { get; private set; } = 1;
        public int CurrentExperience { get; private set; }
        public long TotalExperience { get; private set; }

        public int ExperienceToNextLevel =>
            CalculateExperienceRequirement(Level);

        public float LevelProgress =>
            CurrentExperience / (float)ExperienceToNextLevel;

        public event Action<int>? LevelUp;

        public void AddExperience(int amount)
        {
            if (amount <= 0)
                return;

            CurrentExperience += amount;
            TotalExperience += amount;

            // Поддържа и няколко level ups 
            while (CurrentExperience >= ExperienceToNextLevel)
            {
                CurrentExperience -= ExperienceToNextLevel;
                Level++;

                LevelUp?.Invoke(Level);
            }
        }

        public void Reset()
        {
            Level = 1;
            CurrentExperience = 0;
            TotalExperience = 0;
        }

        private static int CalculateExperienceRequirement(int level)
        {
            return (int)MathF.Round(
                StartingExperienceRequirement *
                MathF.Pow(RequirementGrowth, level - 1));
        }
    }
}