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

    /// <summary>
    /// Optional dynamic resistance supplied by behaviors whose committed
    /// movement must not be interrupted by ordinary player hits.
    /// </summary>
    public interface IEnemyHitReactionPolicy
    {
        bool IsKnockbackImmune { get; }
    }
}
