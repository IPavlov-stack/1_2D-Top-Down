using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace _1_2D_Top_Down
{
    public partial class Game1
    {
        private void HandleExit(KeyboardState keyboard)
        {
            if (keyboard.IsKeyDown(Keys.Escape))
            {
                Exit();
            }
        }
        private void UpdateGameObjects(GameTime gameTime)
        {
            player.Update(gameTime,new Rectangle(0,0,(int)worldMap.WorldWidth,(int)worldMap.WorldHeight));

            if (isEnemySpawningEnabled)
            {
                UpdateEnemySpawning(gameTime);

            }
            UpdateEvilEyes(gameTime);
            UpdateEnemyProjectiles(gameTime);
            UpdateDemons(gameTime);
            UpdatePlayerProjectiles(gameTime);
            UpdateDemonDeathAnimations(gameTime);
        }
        private void RestartGame()
        {
            isGameOver = false;

            projectiles.Clear();
            demons.Clear();
            evilEyes.Clear();
            enemyProjectiles.Clear();
            demonDeathAnimations.Clear();

            player.Position = playerStartPosition;
            spawnTimer = 0f;

            SpawnEnemy();
        }
        private void UpdateDemons(GameTime gameTime)
        {
            for (int i = demons.Count - 1; i >= 0; i--)
            {
                Demon demon = demons[i];

                demon.Update(gameTime, player);

                if (player.Bounds.Intersects(demon.Bounds))
                {
                    isGameOver = true;
                }
            }
        }
        private void UpdateEvilEyes(GameTime gameTime)
        {
            for (int i = evilEyes.Count - 1; i >= 0; i--)
            {
                Evil_Eye evilEye = evilEyes[i];

                EnemyProjectile? enemyProjectile = evilEye.Update(
                    gameTime,
                    player,
                    evilEyeProjectileTexture);

                if (enemyProjectile != null)
                {
                    enemyProjectiles.Add(enemyProjectile);
                }

                if (evilEye.IsDeathAnimationFinished)
                {
                    evilEyes.RemoveAt(i);
                }
            }
        }
        private void UpdateEnemyProjectiles(GameTime gameTime)
        {
            Rectangle worldBounds = new Rectangle(
                0,
                0,
                (int)worldMap.WorldWidth,
                (int)worldMap.WorldHeight);

            for (int i = enemyProjectiles.Count - 1; i >= 0; i--)
            {
                EnemyProjectile enemyProjectile = enemyProjectiles[i];

                enemyProjectile.Update(gameTime);

                if (enemyProjectile.Bounds.Intersects(player.Bounds))
                {
                    isGameOver = true;
                    enemyProjectiles.RemoveAt(i);
                    continue;
                }

                if (!worldBounds.Intersects(enemyProjectile.Bounds))
                {
                    enemyProjectiles.RemoveAt(i);
                }
            }
        }
        private void UpdatePlayerProjectiles(GameTime gameTime)
        {
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                PlayerProjectile projectile = projectiles[i];

                projectile.Update(gameTime);

                Rectangle worldBounds = new Rectangle(0, 0, WorldWidth, WorldHeight);

                bool isOutsideWorld =
                    !worldBounds.Intersects(projectile.Bounds);
                if (isOutsideWorld)
                {
                    projectiles.RemoveAt(i);
                    continue;
                }
                bool projectileHitEnemy = false;

                for (int j = demons.Count - 1; j >= 0; j--)
                {
                    if (projectile.Bounds.Intersects(demons[j].Bounds))
                    {
                        Vector2 deathPosition = demons[j].Bounds.Center.ToVector2();

                        demonDeathAnimations.Add(
                            new DeathAnimation(demonDeathTexture, deathPosition));

                        demons.RemoveAt(j);

                        projectileHitEnemy = true;
                        break;
                    }
                }
                for (int j = evilEyes.Count - 1; j >= 0; j--)
                {
                    Evil_Eye evilEye = evilEyes[j];

                    if (!evilEye.IsDead &&
                        projectile.Bounds.Intersects(evilEye.Bounds))
                    {
                        evilEye.Die();
                        projectiles.RemoveAt(i);
                        break;
                    }
                    if (!evilEye.IsDead && evilEye.Bounds.Intersects(player.Bounds))
                    {
                        isGameOver = true;
                    }
                }

                if (projectileHitEnemy)
                {
                    projectiles.RemoveAt(i);
                }

            }
        }
        private void UpdateDemonDeathAnimations(GameTime gameTime)
        {
            for (int i = demonDeathAnimations.Count - 1; i >= 0; i--)
            {
                demonDeathAnimations[i].Update(gameTime);

                if (demonDeathAnimations[i].IsFinished)
                {
                    demonDeathAnimations.RemoveAt(i);
                }
            }
        }
        private void UpdateEnemySpawning(GameTime gameTime)
        {
            spawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (spawnTimer >= SpawnInterval)
            {
                SpawnEnemy();
                spawnTimer = 0f;
            }
        }
        private void HandlePlayerShooting(
            MouseState mouse,
            KeyboardState keyboard)
        {
            bool clickedLeftButton =
                mouse.LeftButton == ButtonState.Pressed &&
                previousMouseState.LeftButton == ButtonState.Released;

            bool pressedE =
                keyboard.IsKeyDown(Keys.E) &&
                previousKeyboard.IsKeyUp(Keys.E);

            if (!clickedLeftButton && !pressedE)
            {
                return;
            }

            Vector2 startPosition = player.Bounds.Center.ToVector2();

            Vector2 mouseWorldPosition =
                mouse.Position.ToVector2() + camera.Position;

            Vector2 direction = mouseWorldPosition - startPosition;

            if (direction != Vector2.Zero)
            {
                direction.Normalize();

                projectiles.Add(new PlayerProjectile(
                        playerProjectileTexture,
                        startPosition,
                        direction));
            }
        }
    }
}