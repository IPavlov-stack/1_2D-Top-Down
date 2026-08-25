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
        private void UpdateGameObjects(GameTime gameTime, bool allowPlayerInput = true)
        {
            UpdatePlayerMovement(gameTime, allowPlayerInput);
            UpdateMissionTriggers();
            enemyManager.UpdateSpawnQueue(gameTime,SpawnEnemy);
            TryFinishCurrentWave();
            UpdateEnemies(gameTime);
            UpdateWorldEffects(gameTime);
            UpdateEnemyProjectiles(gameTime);
            enemyManager.RebuildSpatialGrid();
            UpdatePlayerProjectiles(gameTime);
            enemyManager.UpdateDeathAnimations(gameTime);
            UpdateCollectibles(gameTime);
        }
        private void RestartGame()
        {
            StartMission(missionRuntime.Definition, useSceneTransition: false);
        }
        private void UpdatePlayerMovement(GameTime gameTime, bool allowPlayerInput = true)
        {
            gameplaySession.UpdatePlayer(gameTime, allowPlayerInput);
        }
        private void UpdateEnemies(GameTime gameTime)
        {
            enemyManager.UpdateEnemies(
                gameTime,
                player,
                gameMap.WorldBounds,
                gameMap.WorldTileSize,
                IntersectsMapCollision);
            enemyActionProcessor.Process(enemyManager.PendingActions, player);

            if (player.Health.IsDead)
            {
                gameplaySession.ChangeFlowState(GameFlowState.GameOver);
            }
        }

        private void UpdateWorldEffects(GameTime gameTime)
        {
            if (worldEffectManager.Update(gameTime, player))
            {
                gameplaySession.ChangeFlowState(GameFlowState.GameOver);
            }
        }
        private void UpdateEnemyProjectiles(GameTime gameTime)
        {
            if (projectileManager.UpdateEnemyProjectiles(
                    gameTime,
                    gameMap.WorldBounds,
                    IntersectsMapCollision,
                    player))
            {
                gameplaySession.ChangeFlowState(GameFlowState.GameOver);
            }
        }
        private void UpdatePlayerProjectiles(GameTime gameTime)
        {
            projectileManager.UpdatePlayerProjectiles(
                gameTime,
                gameMap.WorldBounds,
                IntersectsMapCollision,
                TryHitNearbyEnemy);
        }

        private bool TryHitNearbyEnemy(PlayerProjectile projectile)
        {
            EnemyHitResult hit = enemyManager.TryHitEnemy(
                projectile.Bounds,
                projectile.CreateHit());

            if (!hit.HasHit)
            {
                return false;
            }

            if (!hit.HasDefeatedEnemy)
            {
                return true;
            }

            Enemy defeatedEnemy = hit.Enemy!;

            HandleEnemyDeath(defeatedEnemy);

            CreateEnemyDeathAnimation(defeatedEnemy);

            if (defeatedEnemy.Definition.Type == EnemyType.Demon)
            {
                PlayRandomDemonDeathSound();
            }
            else if (defeatedEnemy.Definition.Type == EnemyType.Lich)
            {
                PlayRandomLichDeathSound();
            }

            return true;
        }

        private void CreateEnemyDeathAnimation(Enemy enemy)
        {
            EnemyDefinition definition = enemy.Definition;

            deathAnimations.Add(
                new DeathAnimation(
                    enemyFactory.GetTexture(definition.DeathTextureAsset),
                    enemy.SpriteCenter,
                    definition.DeathFrameCount,
                    definition.DeathSheetColumnCount,
                    definition.DeathSheetRowCount,
                    definition.DeathAnimationRow,
                    definition.DeathFrameDuration,
                    definition.DeathScale));
        }

        private bool IntersectsMapCollision(Rectangle bounds)
        {
            return gameMap.IntersectsCollision(bounds);
        }

        private void TryDropCoin(Vector2 enemyCenter)
        {
            if (random.Next(100) < CoinDropChancePercent)
            {
                collectibleManager.Add(new Coin(coinTexture, enemyCenter));
            }
        }
        private void TryDropManaCrystal(Vector2 enemyCenter)
        {
            if (random.Next(100) < ManaCrystalDropChancePercent)
            {
                collectibleManager.Add(
                    new ManaCrystal(manaCrystalTexture, enemyCenter));
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
        private void PlayRandomLichDeathSound()
        {
            if (lichDeathSounds == null ||
                lichDeathSounds.Length == 0)
            {
                return;
            }

            int randomSoundIndex =
                random.Next(lichDeathSounds.Length);

            lichDeathSounds[randomSoundIndex].Play(
                SoundEffectsVolume,
                0f,
                0f);
        }
        private void HandlePlayerShooting(
            MouseState mouse,
            KeyboardState keyboard)
        {
            bool clickedLeftButton = mouse.LeftButton == ButtonState.Pressed &&
                        previousMouseState.LeftButton == ButtonState.Released;

            bool pressedE = keyboard.IsKeyDown(Keys.E) && previousKeyboard.IsKeyUp(Keys.E);

            if (!clickedLeftButton && !pressedE)
            {
                return;
            }

            Vector2 startPosition = player.Hurtbox.Center.ToVector2();

            Vector2 mouseWorldPosition = camera.ScreenToWorld(mouse.Position.ToVector2());
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
        private void SpawnPlayerProjectiles(Vector2 startPosition, Vector2 baseDirection)
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

                projectileManager.AddPlayerProjectile(new PlayerProjectile(
                    playerProjectileTexture,
                    startPosition,
                    projectileDirection,
                    player.Stats.ProjectileSpeed,
                    player.Stats.Damage,
                    player.Stats.Knockback));
            }
        }

        private void UpdateWaveIntermissionInput(MouseState mouse)
        {
            if (gameFlowState != GameFlowState.WaveIntermission)
                return;

            bool clickedStartButton =
                mouse.LeftButton == ButtonState.Pressed &&
                previousMouseState.LeftButton == ButtonState.Released &&
                startNextWaveButtonBounds.Contains(mouse.Position);

            if (!clickedStartButton)
                return;

            gameplaySession.TryStartNextWave();
        }
        private void UpdateDeathAnimations(GameTime gameTime)
        {
            enemyManager.UpdateDeathAnimations(gameTime);
        }
        private void UpdateCollectibles(GameTime gameTime)
        {
            bool playerCanReceiveMana =
                player.Mana.CurrentMana < player.Mana.MaxMana;

            collectibleManager.Update(
                gameTime,
                player.Hurtbox,
                playerCanReceiveMana,
                onCoinCollected: _ =>
                {
                    AddInventoryResource("coin", uiCoinTexture, 1);
                    PlayNextCoinPickupSound();
                    gameplaySession.PublishMissionEvent(
                        new CollectibleCollectedMissionEvent("coin", 1));
                },
                onManaCrystalCollected: _ =>
                {
                    player.Mana.Restore(ManaCrystalRestoreAmount);
                    PlayManaCrystalCollectSound();
                    gameplaySession.PublishMissionEvent(
                        new CollectibleCollectedMissionEvent(
                            "mana_crystal",
                            1));
                });
        }
    }
}
