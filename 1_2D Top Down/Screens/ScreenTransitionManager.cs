using Microsoft.Xna.Framework;

namespace _1_2D_Top_Down
{
    public sealed class ScreenTransitionManager
    {
        private readonly float duration;
        private float elapsed;
        private bool midpointReached;

        public ScreenTransitionManager(float duration)
        {
            this.duration = duration;
        }

        public bool IsActive { get; private set; }

        public float Opacity
        {
            get
            {
                if (!IsActive)
                    return 0f;

                float progress = elapsed / duration;
                return MathHelper.Clamp(
                    progress <= 0.5f ? progress * 2f : (1f - progress) * 2f,
                    0f,
                    1f);
            }
        }

        public void Start()
        {
            elapsed = 0f;
            midpointReached = false;
            IsActive = true;
        }

        public bool Update(GameTime gameTime)
        {
            if (!IsActive)
                return false;

            elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
            bool reachedMidpointNow = !midpointReached && elapsed >= duration / 2f;
            midpointReached |= reachedMidpointNow;

            if (elapsed >= duration)
                IsActive = false;

            return reachedMidpointNow;
        }
    }
}
