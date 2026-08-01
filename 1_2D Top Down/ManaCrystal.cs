using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _1_2D_Top_Down
{
    public class ManaCrystal
    {
        private const int FrameCount = 6;
        private const int FrameWidth = 64;
        private const int FrameHeight = 64;
        private const float FrameDuration = 0.12f;
        private const float Scale = 0.75f;

        private readonly Texture2D texture;
        private Vector2 position;

        private int currentFrame;
        private float animationTimer;

        public Rectangle Bounds
        {
            get
            {
                int size = (int)(FrameWidth * Scale);

                return new Rectangle(
                    (int)position.X,
                    (int)position.Y,
                    size,
                    size);
            }
        }

        public ManaCrystal(Texture2D texture, Vector2 worldCenter)
        {
            this.texture = texture;

            float scaledSize = FrameWidth * Scale;

            position = worldCenter - new Vector2(
                scaledSize / 2f,
                scaledSize / 2f);
        }

        public void Update(GameTime gameTime)
        {
            animationTimer +=
                (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (animationTimer < FrameDuration)
                return;

            animationTimer = 0f;
            currentFrame++;

            if (currentFrame >= FrameCount)
            {
                currentFrame = 0;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle sourceRectangle = new Rectangle(
                currentFrame * FrameWidth,
                0,
                FrameWidth,
                FrameHeight);

            spriteBatch.Draw(
                texture,
                position,
                sourceRectangle,
                Color.White,
                0f,
                Vector2.Zero,
                Scale,
                SpriteEffects.None,
                0f);
        }
    }
}