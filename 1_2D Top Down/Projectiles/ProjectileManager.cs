using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace _1_2D_Top_Down
{
    /// <summary>
    /// Owns all projectiles that belong to the current mission.
    /// Gameplay-specific reactions remain with Game1 while the manager owns
    /// projectile lifetime, movement and common cleanup rules.
    /// </summary>
    public sealed class ProjectileManager
    {
        private readonly List<PlayerProjectile> playerProjectiles = new();
        private readonly List<EnemyProjectile> enemyProjectiles = new();

        public IReadOnlyList<PlayerProjectile> PlayerProjectiles => playerProjectiles;
        public IReadOnlyList<EnemyProjectile> EnemyProjectiles => enemyProjectiles;

        public void Clear()
        {
            playerProjectiles.Clear();
            enemyProjectiles.Clear();
        }

        public void AddPlayerProjectile(PlayerProjectile projectile)
        {
            playerProjectiles.Add(projectile);
        }

        public void AddEnemyProjectile(EnemyProjectile projectile)
        {
            enemyProjectiles.Add(projectile);
        }

        public void UpdatePlayerProjectiles(
            GameTime gameTime,
            Rectangle worldBounds,
            Func<Rectangle, bool> intersectsMapCollision,
            Func<PlayerProjectile, bool> tryHitEnemy)
        {
            for (int i = playerProjectiles.Count - 1; i >= 0; i--)
            {
                PlayerProjectile projectile = playerProjectiles[i];
                projectile.Update(gameTime);

                if (!worldBounds.Intersects(projectile.Bounds) ||
                    intersectsMapCollision(projectile.Bounds) ||
                    tryHitEnemy(projectile))
                {
                    playerProjectiles.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Updates enemy projectiles and returns true when the player dies
        /// from one of their hits during this frame.
        /// </summary>
        public bool UpdateEnemyProjectiles(
            GameTime gameTime,
            Rectangle worldBounds,
            Func<Rectangle, bool> intersectsMapCollision,
            Player player)
        {
            bool playerDied = false;

            for (int i = enemyProjectiles.Count - 1; i >= 0; i--)
            {
                EnemyProjectile projectile = enemyProjectiles[i];
                projectile.Update(gameTime);

                if (projectile.HasReachedMaxTravelDistance ||
                    intersectsMapCollision(projectile.Bounds) ||
                    !worldBounds.Intersects(projectile.Bounds))
                {
                    enemyProjectiles.RemoveAt(i);
                    continue;
                }

                if (!projectile.Bounds.Intersects(player.Bounds))
                {
                    continue;
                }

                player.TakeDamage(15);
                enemyProjectiles.RemoveAt(i);
                playerDied |= player.Health.IsDead;
            }

            return playerDied;
        }
    }
}
