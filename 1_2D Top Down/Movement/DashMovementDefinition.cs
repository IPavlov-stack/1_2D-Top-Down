using System;

namespace _1_2D_Top_Down
{
    /// <summary>
    /// Reusable tuning shared by committed dash movement. The powered phase
    /// travels a fixed distance at a constant speed. It is followed by a
    /// time-based ease-out slide that starts at the same speed.
    /// </summary>
    public sealed class DashMovementDefinition
    {
        public float InitialSpeed { get; }
        public float Distance { get; }
        public float Cooldown { get; }
        public float MaximumMovementStep { get; }
        public float SlideDuration { get; }
        public float SlideEasePower { get; }
        public float PoweredDuration => Distance / InitialSpeed;
        public float SlideDistance =>
            InitialSpeed * SlideDuration / (SlideEasePower + 1f);
        public float TotalDistance => Distance + SlideDistance;
        public float TotalDuration => PoweredDuration + SlideDuration;

        public DashMovementDefinition(
            float initialSpeed,
            float distance,
            float cooldown,
            float maximumMovementStep = 8f,
            float slideDuration = 0f,
            float slideEasePower = 2f)
        {
            if (initialSpeed <= 0f)
                throw new ArgumentOutOfRangeException(nameof(initialSpeed));
            if (distance <= 0f)
                throw new ArgumentOutOfRangeException(nameof(distance));
            if (cooldown < 0f)
                throw new ArgumentOutOfRangeException(nameof(cooldown));
            if (maximumMovementStep <= 0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maximumMovementStep));
            }
            if (slideDuration < 0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(slideDuration));
            }
            if (slideEasePower < 1f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(slideEasePower),
                    "Slide easing power must be at least 1.");
            }

            InitialSpeed = initialSpeed;
            Distance = distance;
            Cooldown = cooldown;
            MaximumMovementStep = maximumMovementStep;
            SlideDuration = slideDuration;
            SlideEasePower = slideEasePower;
        }
    }
}
