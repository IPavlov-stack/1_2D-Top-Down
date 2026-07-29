using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace _1_2D_Top_Down
{
    public class Projectile
    {
        protected Texture2D texture;

        protected Vector2 direction;
        protected float speed;
        protected float scale;
        protected float rotation;

        protected int frameCount;
        protected int frameRows;
        protected float frameDuration;

        protected int currentFrame;
        protected float animationTimer;

        public Vector2 Position { get; protected set; }

        protected int FrameWidth => texture.Width / frameCount;
        protected int FrameHeight => texture.Height / frameRows;

        public virtual Rectangle Bounds
        {
            get
            {
                int width = (int)(FrameWidth * scale);
                int height = (int)(FrameHeight * scale);

                return new Rectangle(
                    (int)(Position.X - width / 2f),
                    (int)(Position.Y - height / 2f),
                    width,
                    height);
            }
        }

        public Projectile(
            Texture2D texture,
            Vector2 startPosition,
            Vector2 direction,
            float speed,
            float scale,
            int frameCount,
            int frameRows,
            float frameDuration)
        {
            this.texture = texture;
            Position = startPosition;

            this.direction = direction;
            this.speed = speed;
            this.scale = scale;

            this.frameCount = frameCount;
            this.frameRows = frameRows;
            this.frameDuration = frameDuration;

            rotation = MathF.Atan2(direction.Y, direction.X);
        }

        public virtual void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Position += direction * speed * deltaTime;

            UpdateAnimation(gameTime);
        }

        protected void UpdateAnimation(GameTime gameTime)
        {
            animationTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (animationTimer >= frameDuration)
            {
                currentFrame++;
                animationTimer = 0f;

                if (currentFrame >= frameCount)
                {
                    currentFrame = 0;
                }
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
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
                rotation,
                origin,
                scale,
                SpriteEffects.None,
                0f);
        }
    }
}