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
            Rectangle worldBounds = new Rectangle(
                0,
                0,
                (int)worldMap.WorldWidth,
                (int)worldMap.WorldHeight);

            player.Update(gameTime, worldBounds, solidCollisionRectangles);

            if (isEnemySpawningEnabled)
            {
                UpdateEnemySpawning(gameTime);

            }
            UpdateEvilEyes(gameTime);
            UpdateEnemyProjectiles(gameTime);
            UpdateDemons(gameTime);
            UpdatePlayerProjectiles(gameTime);
            UpdateDemonDeathAnimations(gameTime);
            UpdateCoins(gameTime);
        }
        private void RestartGame()
        {
            isGameOver = false;

            projectiles.Clear();
            demons.Clear();
            evilEyes.Clear();
            enemyProjectiles.Clear();
            demonDeathAnimations.Clear();
            coins.Clear();
            coinsCollected = 0;

            player.Position = playerStartPosition;
            player.Health.Reset();
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
                    player.TakeDamage(20);

                    if (player.Health.IsDead)
                    {
                        isGameOver = true;
                    }
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
                    player.TakeDamage(15);

                    enemyProjectiles.RemoveAt(i);

                    if (player.Health.IsDead)
                    {
                        isGameOver = true;
                    }

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

                Rectangle worldBounds = new Rectangle(
                    0,
                    0,
                    (int)worldMap.WorldWidth,
                    (int)worldMap.WorldHeight);

                bool isOutsideWorld =
                    !worldBounds.Intersects(projectile.Bounds);
                if (isOutsideWorld)
                {
                    projectiles.RemoveAt(i);
                    continue;
                }

                if (IntersectsMapCollision(projectile.Bounds))
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

                        TryDropCoin(demons[j].Bounds.Center.ToVector2());
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
                        TryDropCoin(evilEye.Bounds.Center.ToVector2());
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

        private bool IntersectsMapCollision(Rectangle bounds)
        {
            foreach (Rectangle collisionRectangle in solidCollisionRectangles)
            {
                if (bounds.Intersects(collisionRectangle))
                    return true;
            }

            return false;
        }

        private void TryDropCoin(Vector2 enemyCenter)
        {
            if (random.Next(100) < CoinDropChancePercent)
            {
                coins.Add(new Coin(coinTexture, enemyCenter));
            }
        }

        private void UpdateCoins(GameTime gameTime)
        {
            for (int i = coins.Count - 1; i >= 0; i--)
            {
                Coin coin = coins[i];
                coin.Update(gameTime);

                if (player.Bounds.Intersects(coin.Bounds))
                {
                    coins.RemoveAt(i);
                    coinsCollected++;
                    PlayNextCoinPickupSound();
                }
            }
        }

        private void PlayNextCoinPickupSound()
        {
            if (coinPickupSounds == null || coinPickupSounds.Length == 0)
                return;

            int randomSoundIndex = random.Next(coinPickupSounds.Length);

            coinPickupSounds[randomSoundIndex].Play(
                SoundEffectsVolume, //volume
                0f,                 //pitch
                0f);                //pan
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
        private void HandleDeveloperMode(KeyboardState keyboard)
        {
            if (keyboard.IsKeyDown(Keys.F3) &&
                previousKeyboard.IsKeyUp(Keys.F3))
            {
                isDeveloperMode = !isDeveloperMode;
            }
        }
    }
}