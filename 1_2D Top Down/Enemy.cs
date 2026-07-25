using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace _1_2D_Top_Down
{
    public class Enemy
    {
        private Texture2D texture;
        private const float speed = 70f;

        public Vector2 Position;
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
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, Color.White);
        }
    }
}
