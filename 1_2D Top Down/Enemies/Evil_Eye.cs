using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _1_2D_Top_Down
{
    public class Evil_Eye : Enemy
    {
        private const int FlyingRow = 0;

        public Evil_Eye(
            Texture2D texture,
            Vector2 startPosition,
            EnemyDefinition definition,
            IEnemyBehavior behavior)
            : base(
                texture,
                startPosition,
                frameCount: 6,
                frameRows: 3,
                frameDuration: 0.15f,
                scale: 0.5f,
                definition,
                behavior)

        {
            SetAnimation(FlyingRow, 4);
            ChangeState(EnemyState.Idle);
        }
    }
}
