using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _1_2D_Top_Down
{
    public class Enemy
    {
        protected Texture2D texture;

        // Размерът на цялата мрежа в sprite sheet
        protected int frameCount;
        protected int frameRows;
        protected float rotation;
        protected float frameDuration;
        protected float scale;

        protected int currentFrame;
        protected int animationRow;
        protected int framesInCurrentAnimation;
        protected float animationTimer;

        protected int FrameWidth => texture.Width / frameCount;
        protected int FrameHeight => texture.Height / frameRows;

        protected Vector2 SpriteCenter =>
            Position + new Vector2(
                FrameWidth * scale / 2f,
                FrameHeight * scale / 2f);

        public Vector2 Position;
        public Health Health { get; }

        public Rectangle Bounds
        {
            get
            {
                int spriteWidth = (int)(FrameWidth * scale);
                int spriteHeight = (int)(FrameHeight * scale);

                int hitboxWidth = (int)(spriteWidth * 0.7f);
                int hitboxHeight = (int)(spriteHeight * 0.7f);

                int offsetX = (spriteWidth - hitboxWidth) / 2;
                int offsetY = (spriteHeight - hitboxHeight) / 2;

                return new Rectangle(
                    (int)Position.X + offsetX,
                    (int)Position.Y + offsetY,
                    hitboxWidth,
                    hitboxHeight);
            }
        }

        public Enemy(
            Texture2D texture,
            Vector2 startPosition,
            int frameCount,
            int frameRows,
            float frameDuration,
            float scale,
            int maxHealth)
        {
            this.texture = texture;
            Position = startPosition;

            this.frameCount = frameCount;
            this.frameRows = frameRows;
            this.frameDuration = frameDuration;
            this.scale = scale;

            framesInCurrentAnimation = frameCount;
            Health = new Health(maxHealth);
        }

        protected void SetAnimation(int row, int animationFrameCount)
        {
            bool animationChanged =
                animationRow != row ||
                framesInCurrentAnimation != animationFrameCount;

            if (animationChanged)
            {
                animationRow = row;
                framesInCurrentAnimation = animationFrameCount;
                currentFrame = 0;
                animationTimer = 0f;
            }
        }

        // Връща true само ако non-looping анимацията е приключила.
        protected bool UpdateAnimation(GameTime gameTime, bool loop = true)
        {
            animationTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (animationTimer < frameDuration)
            {
                return false;
            }

            animationTimer -= frameDuration;
            currentFrame++;

            if (currentFrame < framesInCurrentAnimation)
            {
                return false;
            }

            if (loop)
            {
                currentFrame = 0;
                return false;
            }

            currentFrame = framesInCurrentAnimation - 1;
            return true;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            Rectangle sourceRectangle = new Rectangle(
                currentFrame * FrameWidth,
                animationRow * FrameHeight,
                FrameWidth,
                FrameHeight);

            spriteBatch.Draw(
                 texture,
                 SpriteCenter,
                 sourceRectangle,
                 Color.White,
                 rotation,
                 new Vector2(FrameWidth / 2f, FrameHeight / 2f),
                 scale,
                 SpriteEffects.None,
                 0f);
        }
    }
}