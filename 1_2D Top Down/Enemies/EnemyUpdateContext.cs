using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace _1_2D_Top_Down
{
    public sealed class EnemyUpdateContext
    {
        private readonly List<EnemyActionRequest> requests;

        public Player Target { get; }

        internal EnemyUpdateContext(
            Player target,
            List<EnemyActionRequest> requests)
        {
            Target = target;
            this.requests = requests;
        }

        public void RequestPlayerDamage(int damage)
        {
            requests.Add(new DamagePlayerRequest(damage));
        }

        public void RequestProjectile(
            string projectileAsset,
            Vector2 position,
            Vector2 direction)
        {
            requests.Add(
                new ProjectileSpawnRequest(
                    projectileAsset,
                    position,
                    direction));
        }

        public void RequestEnemySpawn(string enemyId, Vector2 position)
        {
            requests.Add(new EnemySpawnRequest(enemyId, position));
        }
    }
}
