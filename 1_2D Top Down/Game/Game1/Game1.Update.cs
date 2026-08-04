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

            player.Update( gameTime, worldBounds, solidCollisionRectangles, true);
            if (isEnemySpawningEnabled)
            {
                UpdateEnemySpawning(gameTime);

            }
            UpdateEvilEyes(gameTime);
            UpdateEnemyProjectiles(gameTime);
            UpdateDemons(gameTime);
            UpdatePlayerProjectiles(gameTime);
            RebuildEnemySpatialGrids();
            UpdateDemonDeathAnimations(gameTime);
            UpdateCoins(gameTime);
            UpdateManaCrystals(gameTime);
            UpdatePlayerResourceAnimations(gameTime);
        }
        private void RestartGame()
        {
            gameFlowState = GameFlowState.GameOver;
            projectiles.Clear();
            demons.Clear();
            evilEyes.Clear();
            enemyProjectiles.Clear();
            demonDeathAnimations.Clear();
            coins.Clear();
            manaCrystals.Clear();
            inventoryResources.Clear();
            player.Position = playerStartPosition;
            player.Health.Reset();
            player.ResetDamageEffects();
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
                        gameFlowState = GameFlowState.GameOver;
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
                if (enemyProjectile.HasReachedMaxTravelDistance)
                {
                    enemyProjectiles.RemoveAt(i);
                    continue;
                }

                if (IntersectsMapCollision(enemyProjectile.Bounds))
                {
                    enemyProjectiles.RemoveAt(i);
                    continue;
                }

                if (enemyProjectile.Bounds.Intersects(player.Bounds))
                {
                    player.TakeDamage(15);

                    enemyProjectiles.RemoveAt(i);

                    if (player.Health.IsDead)
                    {
                        gameFlowState = GameFlowState.GameOver;
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
            Rectangle worldBounds = new Rectangle(
                0,
                0,
                (int)worldMap.WorldWidth,
                (int)worldMap.WorldHeight);

            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                PlayerProjectile projectile = projectiles[i];

                projectile.Update(gameTime);

                if (!worldBounds.Intersects(projectile.Bounds) ||
                    IntersectsMapCollision(projectile.Bounds))
                {
                    projectiles.RemoveAt(i);
                    continue;
                }

                bool hitEnemy =
                    TryHitNearbyDemon(projectile) ||
                    TryHitNearbyEvilEye(projectile);

                if (hitEnemy)
                {
                    projectiles.RemoveAt(i);
                }
            }
        }
        private bool TryHitNearbyDemon(PlayerProjectile projectile)
        {
            demonSpatialGrid.QueryNearby(
                projectile.Bounds,
                nearbyDemons);

            for (int i = 0; i < nearbyDemons.Count; i++)
            {
                Demon demon = nearbyDemons[i];

                if (demon.Health.IsDead ||
                    !projectile.Bounds.Intersects(demon.Bounds))
                {
                    continue;
                }

                demon.Health.TakeDamage(player.Stats.Damage);
                demon.ApplyKnockback(
                    projectile.Bounds.Center.ToVector2(),
                    player.Stats.Knockback);

                if (demon.Health.IsDead)
                {
                    Vector2 deathPosition =
                        demon.Bounds.Center.ToVector2();

                    demonDeathAnimations.Add(
                        new DeathAnimation(
                            demonDeathTexture,
                            deathPosition));

                    PlayRandomDemonDeathSound();
                    RewardEnemyKill(demon);
                    TryDropCoin(deathPosition);
                    TryDropManaCrystal(deathPosition);

                    demons.Remove(demon);
                }

                return true;
            }

            return false;
        }

        private bool TryHitNearbyEvilEye(PlayerProjectile projectile)
        {
            evilEyeSpatialGrid.QueryNearby(
                projectile.Bounds,
                nearbyEvilEyes);

            for (int i = 0; i < nearbyEvilEyes.Count; i++)
            {
                Evil_Eye evilEye = nearbyEvilEyes[i];

                if (evilEye.IsDead ||
                    !projectile.Bounds.Intersects(evilEye.Bounds))
                {
                    continue;
                }

                evilEye.Health.TakeDamage(player.Stats.Damage);

                evilEye.ApplyKnockback(
                    projectile.Bounds.Center.ToVector2(),
                    player.Stats.Knockback);

                if (evilEye.Health.IsDead)
                {
                    Vector2 deathPosition =
                        evilEye.Bounds.Center.ToVector2();

                    TryDropCoin(deathPosition);
                    TryDropManaCrystal(deathPosition);
                    RewardEnemyKill(evilEye);

                    evilEye.Die();
                    PlayRandomEvilEyeDeathSound();
                }

                return true;
            }

            return false;
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
            return mapCollisionGrid.Intersects(bounds);
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
                if (coin.IsExpired)
                {
                    coins.RemoveAt(i);
                    continue;
                }

                if (player.Bounds.Intersects(coin.Bounds))
                {
                    coins.RemoveAt(i);
                    AddInventoryResource("coin", uiCoinTexture, 1);
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
                if (manaCrystal.IsExpired)
                {
                    manaCrystals.RemoveAt(i);
                    continue;
                }

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
                spawnTimer = 0f;

                if (ActiveEnemyCount < MaxActiveEnemies)
                {
                    SpawnEnemy();
                }
            }
        }
        private void HandlePlayerShooting(
            MouseState mouse,
            KeyboardState keyboard)
        {
            bool clickedLeftButton = mouse.LeftButton == ButtonState.Pressed &&
                        previousMouseState.LeftButton == ButtonState.Released;

            bool pressedE =  keyboard.IsKeyDown(Keys.E) && previousKeyboard.IsKeyUp(Keys.E);

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
                    player.EnterShootState();

                    SpawnPlayerProjectiles(
                        startPosition,
                        direction);
                    PlayRandomBasicAttackSound();
                }
            }
        }
        private void SpawnPlayerProjectiles( Vector2 startPosition,Vector2 baseDirection)
        {
            int projectileCount = Math.Max(
                1,
                player.Stats.ProjectileCount);

            float spreadAngleRadians = MathHelper.ToRadians(
                player.Stats.ProjectileSpreadAngleDegrees);

            float middleProjectileIndex =
                (projectileCount - 1) / 2f;

            for (int i = 0; i < projectileCount; i++)
            {
                float angle =
                    (i - middleProjectileIndex) *
                    spreadAngleRadians;

                Vector2 projectileDirection = Vector2.Transform(
                    baseDirection,
                    Matrix.CreateRotationZ(angle));

                projectiles.Add(new PlayerProjectile(
                    playerProjectileTexture,
                    startPosition,
                    projectileDirection,
                    player.Stats.ProjectileSpeed));
            }
        }
        private void RebuildEnemySpatialGrids()
        {
            demonSpatialGrid.Rebuild(demons);
            evilEyeSpatialGrid.Rebuild(evilEyes);
        }
        private void HandleDeveloperMode(KeyboardState keyboard)
        {
            if (keyboard.IsKeyDown(Keys.F3) &&
                previousKeyboard.IsKeyUp(Keys.F3))
            {
                isDeveloperMode = !isDeveloperMode;
            }
        }
        private void RewardEnemyKill(Enemy enemy)
        {
            player.GainExperience(enemy.ExperienceReward);
        }
    }
}
