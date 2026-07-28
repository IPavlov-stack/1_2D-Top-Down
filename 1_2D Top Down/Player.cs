using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct2D1.Effects;
using System;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace _1_2D_Top_Down
{
    public class Player
    {
        private Game game;
        private Texture2D texture;

        private const int FrameCount = 4;
        private const float FrameDuration = 0.15f;

        private int currentFrame;
        private float animationTimer;
        private int FrameWidth => texture.Width / FrameCount;
        private int FrameHeight => texture.Height;

        private const float Scale = 1.5f;
        private const float Speed = 300f;

        public Vector2 Position;
        public Vector2 playerPosition = new Vector2(400, 500);
        public Rectangle Bounds => new Rectangle(
        (int)Position.X,
        (int)Position.Y,
        (int)(FrameWidth * Scale),
        (int)(FrameHeight * Scale));



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

            float playerWidth = texture.Width * Scale;
            float playerHeight = texture.Height * Scale;

            Position.X = Math.Clamp(Position.X, 0 , arena.Width - playerWidth);
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
