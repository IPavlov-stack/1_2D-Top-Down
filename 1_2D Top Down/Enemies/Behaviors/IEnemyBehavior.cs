using Microsoft.Xna.Framework;

namespace _1_2D_Top_Down
{
    public interface IEnemyBehavior
    {
        void Update(
            Enemy enemy,
            GameTime gameTime,
            EnemyUpdateContext context);
    }
}
