using _1_2D_Top_Down;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _1_2D_Top_Down
{
    public class EnemyProjectile : Projectile
    {
        public EnemyProjectile(
            Texture2D texture,
            Vector2 startPosition,
            Vector2 direction)
            : base(
                texture,
                startPosition,
                direction,
                speed: 325f,
                scale: 2f,
                frameCount: 11,
                frameRows: 1,
                frameDuration: 0.04f)
        {
        }
        public override Rectangle Bounds
        {
            get
            {
                int width = (int)(FrameWidth * scale * 0.75f);
                int height = (int)(FrameHeight * scale * 0.75f);

                return new Rectangle(
                    (int)(Position.X - width / 2f),
                    (int)(Position.Y - height / 2f),
                    width,
                    height);
            }
        }
    }
}