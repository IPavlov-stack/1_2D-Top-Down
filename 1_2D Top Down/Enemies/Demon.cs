using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _1_2D_Top_Down
{
    public class Demon : Enemy
    {
        private const float Speed = 120f;

        public Demon(Texture2D texture, Vector2 startPosition)
            : base(
                texture,
                startPosition,
                frameCount: 4,
                frameRows: 1,
                frameDuration: 0.15f,
                scale: 1.25f)
        {
        }

        public void Update(GameTime gameTime, Player player)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 direction = player.Bounds.Center.ToVector2() -
                                Bounds.Center.ToVector2();

            if (direction != Vector2.Zero)
            {
                direction.Normalize();
                Position += direction * Speed * deltaTime;
            }

            UpdateAnimation(gameTime);
        }
    }
}