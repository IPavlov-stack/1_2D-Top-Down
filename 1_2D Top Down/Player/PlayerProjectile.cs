using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _1_2D_Top_Down
{
    public class PlayerProjectile : Projectile
    {
        public PlayerProjectile(
            Texture2D texture,
            Vector2 startPosition,
            Vector2 direction)
            : base(
                texture,
                startPosition,
                direction,
                speed: 600f,
                scale: 0.9f,
                frameCount: 3,
                frameRows: 1,
                frameDuration: 0.1f)
        {
        }
    }
}