using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _1_2D_Top_Down
{
    public class Projectile
    {
        private Texture2D texture;
        private Vector2 direction;
        private float speed = 600f;

        public Vector2 Position;
        public Rectangle Bounds => new Rectangle(
        (int)Position.X,
        (int)Position.Y,
        texture.Width,
        texture.Height);

        public Projectile(Texture2D texture, Vector2 startPosition, Vector2 direction)
        {
            this.texture = texture;
            Position = startPosition;
            this.direction = direction;
        }

        public void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Position += direction * speed * deltaTime;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, Color.Yellow);
        }
    }
}
