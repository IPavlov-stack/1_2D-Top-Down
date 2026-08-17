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
        public List<PlayerProjectile> PlayerProjectiles { get; } = new();
        public List<EnemyProjectile> EnemyProjectiles { get; } = new();

        public void Clear()
        {
            PlayerProjectiles.Clear();
            EnemyProjectiles.Clear();
        }
        public void AddEnemyProjectile( EnemyProjectile projectile)
        {
            EnemyProjectiles.Add(projectile);
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

            for (int i = EnemyProjectiles.Count - 1; i >= 0; i--)
            {
                EnemyProjectile projectile = EnemyProjectiles[i];
                projectile.Update(gameTime);

                if (projectile.HasReachedMaxTravelDistance ||
                    intersectsMapCollision(projectile.Bounds) ||
                    !worldBounds.Intersects(projectile.Bounds))
                {
                    EnemyProjectiles.RemoveAt(i);
                    continue;
                }

                if (!projectile.Bounds.Intersects(player.Bounds))
                {
                    continue;
                }

                player.TakeDamage(15);
                EnemyProjectiles.RemoveAt(i);
                playerDied |= player.Health.IsDead;
            }

            return playerDied;
        }
    }
}