using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace _1_2D_Top_Down
{
    public class Player
    {
        private Game game;
        private Texture2D texture;

        private const float scale = 1.5f;
        private const float speed = 300f;

        public Vector2 Position;
        public Vector2 playerPosition = new Vector2(400, 500);



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
                Position.X -= speed * deltaTime;

            if (keyboard.IsKeyDown(Keys.Right) || keyboard.IsKeyDown(Keys.D))
                Position.X += speed * deltaTime;

            if (keyboard.IsKeyDown(Keys.Up) || keyboard.IsKeyDown(Keys.W))
                Position.Y -= speed * deltaTime;

            if (keyboard.IsKeyDown(Keys.Down) || keyboard.IsKeyDown(Keys.S))
                Position.Y += speed * deltaTime;

            float playerWidth = texture.Width * scale;
            float playerHeight = texture.Height * scale;

            Position.X = Math.Clamp(Position.X, 0 , arena.Width - playerWidth);
            Position.Y = Math.Clamp(Position.Y, 0, arena.Height - playerHeight);

        }
        public void Draw( SpriteBatch spritebatch)
        {
            spritebatch.Draw(texture,                  // texture
            new Vector2(Position.X,
            Position.Y),
            null,                                       // sourceRectangle
            Color.White,                                // color
            0.0f,                                       // rotation
            Vector2.Zero,                               // origin
            scale,                                      // scale
            SpriteEffects.None,                         // effects
            0.0f);                                      // layerDepth

        }
    }
}
