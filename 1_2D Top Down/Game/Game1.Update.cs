using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

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

            player.Update(gameTime, worldBounds, solidCollisionRectangles, !isInventoryOpen);

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
            UpdateManaCrystals(gameTime);
            UpdatePlayerResourceAnimations(gameTime);
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
            manaCrystals.Clear();
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
                    if (demon.TryDamagePlayer(player, 20) &&
                        player.Health.IsDead)
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
                        Demon demon = demons[j];
                        demon.Health.TakeDamage(PlayerProjectileDamage);

                        demon.ApplyKnockback(projectile.Bounds.Center.ToVector2(), Player.BasicAttackKnockbackForce);

                        if (demon.Health.IsDead)
                        {
                            Vector2 deathPosition = demon.Bounds.Center.ToVector2();

                            demonDeathAnimations.Add(
                                new DeathAnimation(demonDeathTexture, deathPosition));

                            PlayRandomDemonDeathSound();

                            TryDropCoin(demon.Bounds.Center.ToVector2());
                            TryDropManaCrystal(demon.Bounds.Center.ToVector2());
                            demons.RemoveAt(j);
                        }

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
                        evilEye.Health.TakeDamage(PlayerProjectileDamage);
                        evilEye.ApplyKnockback(projectile.Bounds.Center.ToVector2(), Player.BasicAttackKnockbackForce);

                        if (evilEye.Health.IsDead)
                        {
                            TryDropCoin(evilEye.Bounds.Center.ToVector2());
                            TryDropManaCrystal(evilEye.Bounds.Center.ToVector2());

                            evilEye.Die();
                            PlayRandomEvilEyeDeathSound();
                        }

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
        private void UpdatePlayerResourceAnimations(GameTime gameTime)
        {
            float healthPercent =
                player.Health.CurrentHealth / (float)player.Health.MaxHealth;

            float manaPercent =
                player.Mana.CurrentMana / player.Mana.MaxMana;

            int targetHealthFrame = GetResourceFrame(healthPercent);
            int targetManaFrame = GetResourceFrame(manaPercent);

            AnimateResourceFrame(
                ref displayedHealthFrame,
                ref healthFrameTimer,
                targetHealthFrame,
                gameTime);

            AnimateResourceFrame(
                ref displayedManaFrame,
                ref manaFrameTimer,
                targetManaFrame,
                gameTime);
        }

        private int GetResourceFrame(float percent)
        {
            percent = MathHelper.Clamp(percent, 0f, 1f);

            int filledSteps = (int)MathF.Ceiling(
                percent * ResourceFrameCount);

            return Math.Clamp(
                ResourceFrameCount - filledSteps,
                0,
                ResourceFrameCount - 1);
        }

        private void AnimateResourceFrame(
            ref int displayedFrame,
            ref float timer,
            int targetFrame,
            GameTime gameTime)
        {
            if (displayedFrame == targetFrame)
            {
                timer = 0f;
                return;
            }

            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (timer < ResourceFrameDuration)
                return;

            timer = 0f;
            displayedFrame += Math.Sign(targetFrame - displayedFrame);
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
        private void TryDropManaCrystal(Vector2 enemyCenter)
        {
            if (random.Next(100) < ManaCrystalDropChancePercent)
            {
                manaCrystals.Add(
                    new ManaCrystal(manaCrystalTexture, enemyCenter));
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
        private void UpdateManaCrystals(GameTime gameTime)
        {
            for (int i = manaCrystals.Count - 1; i >= 0; i--)
            {
                ManaCrystal manaCrystal = manaCrystals[i];

                manaCrystal.Update(gameTime);

                bool playerCanReceiveMana =
                    player.Mana.CurrentMana < player.Mana.MaxMana;

                if (playerCanReceiveMana &&
                    player.Bounds.Intersects(manaCrystal.Bounds))
                {
                    player.Mana.Restore(ManaCrystalRestoreAmount);
                    manaCrystals.RemoveAt(i);

                    PlayManaCrystalCollectSound();
                }
            }
        }
        private void PlayNextCoinPickupSound()
        {
            if (coinPickupSounds == null || coinPickupSounds.Length == 0)
                return;

            int randomSoundIndex = random.Next(coinPickupSounds.Length);

            float coinVolume = MathHelper.Clamp(
                SoundEffectsVolume * CoinPickupVolumeMultiplier,
                0f,
                1f);

            coinPickupSounds[randomSoundIndex].Play(
                coinVolume,
                0f,
                0f);
        }
        private void PlayManaCrystalCollectSound()
        {
            manaCrystalCollectSound.Play(
                SoundEffectsVolume,
                0f,
                0f);
        }
        private void PlayRandomBasicAttackSound()
        {
            if (basicAttackSounds == null ||
                basicAttackSounds.Length == 0)
            {
                return;
            }

            int randomSoundIndex =
                random.Next(basicAttackSounds.Length);

            basicAttackSounds[randomSoundIndex].Play(
                SoundEffectsVolume,
                0f,
                0f);
        }
        private void PlayRandomDemonDeathSound()
        {
            if (demonDeathSounds == null ||
                demonDeathSounds.Length == 0)
            {
                return;
            }

            int randomSoundIndex =
                random.Next(demonDeathSounds.Length);

            demonDeathSounds[randomSoundIndex].Play(
                SoundEffectsVolume,
                0f,
                0f);
        }
        private void PlayRandomEvilEyeDeathSound()
        {
            if (evilEyeDeathSounds == null ||
                evilEyeDeathSounds.Length == 0)
            {
                return;
            }

            int randomSoundIndex =
                random.Next(evilEyeDeathSounds.Length);

            evilEyeDeathSounds[randomSoundIndex].Play(
                SoundEffectsVolume,
                0f,
                0f);
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

            Vector2 mouseWorldPosition = mouse.Position.ToVector2() + camera.Position;

            Vector2 direction = mouseWorldPosition - startPosition;

            if (direction != Vector2.Zero)
            {
                direction.Normalize();

                if (player.Mana.TrySpend(Player.BasicAttackManaCost))
                {
                    projectiles.Add(new PlayerProjectile(
                        playerProjectileTexture,
                        startPosition,
                        direction));
                    PlayRandomBasicAttackSound();
                }
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
