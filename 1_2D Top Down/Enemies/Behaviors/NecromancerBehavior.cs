using System;
using Microsoft.Xna.Framework;

namespace _1_2D_Top_Down
{
    public sealed class NecromancerBehavior : IEnemyBehavior
    {
        private readonly IEnemyBehavior rangedBehavior;
        private readonly string summonEnemyId;
        private readonly float summonCooldown;
        private readonly float summonRadius;
        private readonly Random random = new();

        private float summonTimer;

        public NecromancerBehavior(
            IEnemyBehavior rangedBehavior,
            string summonEnemyId,
            float summonCooldown,
            float summonRadius)
        {
            this.rangedBehavior = rangedBehavior;
            this.summonEnemyId = summonEnemyId;
            this.summonCooldown = summonCooldown;
            this.summonRadius = summonRadius;
        }

        public void Update(
            Enemy enemy,
            GameTime gameTime,
            EnemyUpdateContext context)
        {
            rangedBehavior.Update(enemy, gameTime, context);

            summonTimer +=
                (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (summonTimer < summonCooldown)
                return;

            summonTimer = 0f;

            float angle = (float)(random.NextDouble() * Math.PI * 2.0);
            Vector2 offset = new Vector2(
                MathF.Cos(angle),
                MathF.Sin(angle)) * summonRadius;

            context.RequestEnemySpawn(
                summonEnemyId,
                enemy.Bounds.Center.ToVector2() + offset);
        }
    }
}
