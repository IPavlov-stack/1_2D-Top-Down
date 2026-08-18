using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _1_2D_Top_Down
{
    public class Demon : Enemy
    {
        public Demon(
            Texture2D texture,
            Vector2 startPosition,
            EnemyDefinition definition,
            IEnemyBehavior behavior)
            : base(
                texture,
                startPosition,
                frameCount: 4,
                frameRows: 1,
                frameDuration: 0.15f,
                scale: 1.25f,
                definition,
                behavior)
        {
        }
    }
}
