using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _1_2D_Top_Down
{
    public class DeathAnimation
    {
        private Texture2D texture;

        private const int FrameCount = 7;
        private const float FrameDuration = 0.1f;

        private int currentFrame;
        private float animationTimer;

        public Vector2 Position { get; }
        public bool IsFinished { get; private set; }

        private int FrameWidth => texture.Width / FrameCount;
        private int FrameHeight => texture.Height;

        public DeathAnimation(Texture2D texture, Vector2 position)
        {
            this.texture = texture;
            Position = position;
        }

        public void Update(GameTime gameTime)
        {
            animationTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (animationTimer < FrameDuration)
                return;

            animationTimer = 0f;
            currentFrame++;

            if (currentFrame >= FrameCount)
                IsFinished = true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle sourceRectangle = new Rectangle(
                currentFrame * FrameWidth,
                0,
                FrameWidth,
                FrameHeight);

            Vector2 origin = new Vector2(
                FrameWidth / 2f,
                FrameHeight / 2f);

            spriteBatch.Draw(
                texture,
                Position,
                sourceRectangle,
                Color.White,
                0f,
                origin,
                1f,
                SpriteEffects.None,
                0f);
        }
    }
}