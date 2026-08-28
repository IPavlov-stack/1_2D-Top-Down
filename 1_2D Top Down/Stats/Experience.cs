using System;

#nullable enable

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
        public event Action? Changed;

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

            Changed?.Invoke();
        }

        public void Reset()
        {
            Level = 1;
            CurrentExperience = 0;
            TotalExperience = 0;
            Changed?.Invoke();
        }

        internal void Restore(
            int level,
            int currentExperience,
            long totalExperience)
        {
            Level = Math.Max(1, level);
            CurrentExperience = Math.Max(0, currentExperience);
            TotalExperience = Math.Max(0, totalExperience);

            while (CurrentExperience >= ExperienceToNextLevel)
            {
                CurrentExperience -= ExperienceToNextLevel;
                Level++;
            }
        }

        private static int CalculateExperienceRequirement(int level)
        {
            return (int)MathF.Round(
                StartingExperienceRequirement *
                MathF.Pow(RequirementGrowth, level - 1));
        }
    }
}
