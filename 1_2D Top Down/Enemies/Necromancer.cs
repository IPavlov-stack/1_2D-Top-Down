using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _1_2D_Top_Down
{
    public sealed class Necromancer : Enemy
    {
        public Necromancer(
            Texture2D texture,
            Vector2 startPosition,
            EnemyDefinition definition,
            IEnemyBehavior behavior)
            : base(
                texture,
                startPosition,
                frameCount: 4,
                frameRows: 3,
                frameDuration: 0.15f,
                scale: 0.55f,
                definition,
                behavior)
        {
            SetAnimation(row: 0, animationFrameCount: 4);
            ChangeState(EnemyState.Idle);
        }
    }
}
