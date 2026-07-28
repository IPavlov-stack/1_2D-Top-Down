using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace _1_2D_Top_Down
{
    public class Enemy
    {
        private Texture2D texture;
        private float speed = 100f;

        private const int FrameCount = 4;
        private const float FrameDuration = 0.15f;
        private int currentFrame;
        private float animationTimer;
        private int FrameWidth => texture.Width / FrameCount;
        private int FrameHeight => texture.Height;

        public Vector2 Position;
        public Rectangle Bounds => new Rectangle(
        (int)Position.X,
        (int)Position.Y,
        FrameWidth,
        FrameHeight);
        public Enemy(Texture2D texture, Vector2 startPosition)
        {
            this.texture = texture;
            Position = startPosition;
        }
        public void Update(GameTime gameTime, Vector2 playerPosition)
        {
            Vector2 direction = playerPosition - Position;

            if (direction != Vector2.Zero)
                direction.Normalize();

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position += direction * speed * deltaTime;

            animationTimer += deltaTime;

            if (animationTimer >= FrameDuration)
            {
                currentFrame++;
                animationTimer = 0f;

                if (currentFrame >= FrameCount)
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
                Position,
                sourceRectangle,
                Color.White);
        }


    }
}
