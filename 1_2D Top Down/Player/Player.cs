using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace _1_2D_Top_Down
{
    public class Player
    {
        public Texture2D texture;

        private const int FrameCount = 4;
        private const float FrameDuration = 0.15f;

        private int currentFrame;
        private float animationTimer;
        private int FrameWidth => texture.Width / FrameCount;
        private int FrameHeight => texture.Height;

        private const float Scale = 1.3f;
        private const float Speed = 300f;

        public Vector2 Position;
        public Vector2 playerPosition = new Vector2(400, 500);
        public Rectangle Bounds
        {
            get
            {
                int spriteWidth = (int)(FrameWidth * Scale);
                int spriteHeight = (int)(FrameHeight * Scale);

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


        public Player(Texture2D texture, Vector2 startPosition)
        {
            this.texture = texture;
            Position = startPosition;
        }
        public void Update(GameTime gameTime, Rectangle arena)
        {
            KeyboardState keyboard = Keyboard.GetState();

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (keyboard.IsKeyDown(Keys.Left) || keyboard.IsKeyDown(Keys.A))
                Position.X -= Speed * deltaTime;

            if (keyboard.IsKeyDown(Keys.Right) || keyboard.IsKeyDown(Keys.D))
                Position.X += Speed * deltaTime;

            if (keyboard.IsKeyDown(Keys.Up) || keyboard.IsKeyDown(Keys.W))
                Position.Y -= Speed * deltaTime;

            if (keyboard.IsKeyDown(Keys.Down) || keyboard.IsKeyDown(Keys.S))
                Position.Y += Speed * deltaTime;

            float playerWidth = FrameWidth * Scale;
            float playerHeight = FrameHeight * Scale;

            Position.X = Math.Clamp(Position.X, 0, arena.Width - playerWidth);
            Position.Y = Math.Clamp(Position.Y, 0, arena.Height - playerHeight);

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
                Color.White,
                0f,
                Vector2.Zero,
                Scale,
                SpriteEffects.None,
                0f);
        }
    }
}
