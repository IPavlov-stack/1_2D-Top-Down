using Microsoft.Xna.Framework;

namespace _1_2D_Top_Down
{
    /// <summary>
    /// Adds a timed poison trail to any existing enemy behavior.
    /// </summary>
    public sealed class PoisonTrailBehavior : IEnemyBehavior
    {
        private readonly IEnemyBehavior innerBehavior;
        private readonly AreaEffectDefinition poisonEffect;
        private readonly float spawnInterval;
        private readonly float offsetInTiles;

        private float spawnTimer;
        private Vector2 lastMovementDirection = Vector2.UnitY;

        public PoisonTrailBehavior(
            IEnemyBehavior innerBehavior,
            AreaEffectDefinition poisonEffect,
            float spawnInterval,
            float offsetInTiles)
        {
            this.innerBehavior = innerBehavior;
            this.poisonEffect = poisonEffect;
            this.spawnInterval = spawnInterval;
            this.offsetInTiles = offsetInTiles;
        }

        public void Update(
            Enemy enemy,
            GameTime gameTime,
            EnemyUpdateContext context)
        {
            innerBehavior.Update(enemy, gameTime, context);

            if (enemy.Motor.DesiredDirection != Vector2.Zero)
                lastMovementDirection = enemy.Motor.DesiredDirection;

            spawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (spawnTimer < spawnInterval)
                return;

            spawnTimer %= spawnInterval;

            Vector2 trailOffset = new(
                lastMovementDirection.X * context.WorldTileSize.X,
                lastMovementDirection.Y * context.WorldTileSize.Y);
            trailOffset *= offsetInTiles;

            context.RequestAreaEffect(
                poisonEffect,
                enemy.MovementBounds.Center.ToVector2() - trailOffset,
                enemy.Definition.Id);
        }
    }
}
