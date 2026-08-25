using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework;

namespace _1_2D_Top_Down
{
    public sealed class EnemyUpdateContext
    {
        private readonly List<EnemyActionRequest> requests;

        public Player Target { get; }
        public Rectangle WorldBounds { get; }
        public Vector2 WorldTileSize { get; }
        public Func<Rectangle, bool> IntersectsMapCollision { get; }

        internal EnemyUpdateContext(
            Player target,
            List<EnemyActionRequest> requests,
            Rectangle worldBounds,
            Vector2 worldTileSize,
            Func<Rectangle, bool> intersectsMapCollision)
        {
            Target = target;
            this.requests = requests;
            WorldBounds = worldBounds;
            WorldTileSize = worldTileSize;
            IntersectsMapCollision = intersectsMapCollision;
        }

        public void RequestPlayerDamage(CombatHit hit)
        {
            requests.Add(new DamagePlayerRequest(hit));
        }

        public void RequestProjectile(
            ProjectileSpec spec,
            string sourceId,
            Vector2 position,
            Vector2 direction)
        {
            requests.Add(
                new ProjectileSpawnRequest(
                    spec,
                    sourceId,
                    position,
                    direction));
        }

        public void RequestEnemySpawn(string enemyId, Vector2 position)
        {
            requests.Add(new EnemySpawnRequest(enemyId, position));
        }

        public void RequestAreaEffect(
            AreaEffectDefinition definition,
            Vector2 center,
            string sourceId,
            CombatFaction sourceFaction = CombatFaction.Enemy)
        {
            requests.Add(new AreaEffectSpawnRequest(
                definition,
                center,
                WorldTileSize,
                sourceFaction,
                sourceId));
        }
    }
}
