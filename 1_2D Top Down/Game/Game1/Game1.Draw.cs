using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _1_2D_Top_Down
{
    public partial class Game1
    {
        private void DrawNormalWorld()
        {
            DrawMap();
            DrawEnemyShadows();
            DrawEnemyShadow(
                playerShadowTexture,
                player.Bounds,
                scale: 0.34f,
                opacity: 0.70f,
                bottomOffset: 3f);

            foreach (Coin coin in coins)
                coin.Draw(_spriteBatch);

            foreach (ManaCrystal manaCrystal in manaCrystals)
                manaCrystal.Draw(_spriteBatch);

            player.Draw(_spriteBatch);
            propsLayer.DrawInFrontOfPlayer(_spriteBatch, player.Bounds.Bottom);

            foreach (Projectile projectile in projectiles)
                projectile.Draw(_spriteBatch);

            foreach (Demon demon in demons)
                demon.Draw(_spriteBatch);

            foreach (DeathAnimation deathAnimation in demonDeathAnimations)
                deathAnimation.Draw(_spriteBatch);

            foreach (Evil_Eye evilEye in evilEyes)
                evilEye.Draw(_spriteBatch);

            foreach (EnemyProjectile enemyProjectile in enemyProjectiles)
                enemyProjectile.Draw(_spriteBatch);

            DrawEnemyHealthBars();
        }
        private void DrawEnemyShadows()
        {
            foreach (Demon demon in demons)
            {
                DrawEnemyShadow(
                    demonShadowTexture,
                    demon.Bounds,
                    0.40f,
                    0.75f,
                    4f);
            }

            foreach (Evil_Eye evilEye in evilEyes)
            {
                if (!evilEye.IsDead)
                {
                    DrawEnemyShadow(
                        evilEyeShadowTexture,
                        evilEye.Bounds,
                        0.35f,
                        0.65f,
                        2f);
                }
            }
        }

        private void DrawEnemyShadow(
            Texture2D shadowTexture,
            Rectangle enemyBounds,
            float scale,
            float opacity,
            float bottomOffset)
        {
            Vector2 shadowPosition = new Vector2(
                enemyBounds.Center.X -
                shadowTexture.Width * scale / 2f,

                enemyBounds.Bottom +
                bottomOffset -
                shadowTexture.Height * scale / 2f);

            _spriteBatch.Draw(
                shadowTexture,
                shadowPosition,
                null,
                Color.White * opacity,
                0f,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                0f);
        }

        private void DrawUi()
        {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            DrawGameplayUI();
            DrawExitConfirmation();
            DrawWaveProgressUi();

            if (gameFlowState == GameFlowState.GameOver)
                DrawGameOverScreen();

            DrawWaveIntermissionUi();

            _spriteBatch.End();
        }
        private void DrawEnemyHealthBars()
        {
            foreach (Demon demon in demons)
            {
                DrawEnemyHealthBar(demon.Health, demon.Bounds);
            }

            foreach (Evil_Eye evilEye in evilEyes)
            {
                if (!evilEye.IsDead)
                {
                    DrawEnemyHealthBar(evilEye.Health, evilEye.Bounds);
                }
            }
        }

        private void DrawEnemyHealthBar(Health health, Rectangle bounds)
        {
            // Не показваме bar при пълна кръв или след смърт.
            if (health.CurrentHealth >= health.MaxHealth || health.IsDead)
            {
                return;
            }

            const int barWidth = 46;
            const int barHeight = 7;
            const int borderSize = 1;
            const int distanceAboveEnemy = 8;

            float healthPercent =
                health.CurrentHealth / (float)health.MaxHealth;

            int x = bounds.Center.X - barWidth / 2;
            int y = bounds.Top - distanceAboveEnemy;

            Rectangle borderRectangle =
                new Rectangle(x, y, barWidth, barHeight);

            Rectangle backgroundRectangle = new Rectangle(
                x + borderSize,
                y + borderSize,
                barWidth - borderSize * 2,
                barHeight - borderSize * 2);

            Rectangle currentHealthRectangle = new Rectangle(
                backgroundRectangle.X,
                backgroundRectangle.Y,
                (int)(backgroundRectangle.Width * healthPercent),
                backgroundRectangle.Height);

            _spriteBatch.Draw(pixelTexture, borderRectangle, Color.Black);
            _spriteBatch.Draw(pixelTexture, backgroundRectangle, Color.DarkRed);
            _spriteBatch.Draw(pixelTexture, currentHealthRectangle, Color.LimeGreen);
        }

        private void DrawGameOverScreen()
        {
            Rectangle screenBounds = GraphicsDevice.Viewport.Bounds;

            _spriteBatch.Draw(
                pixelTexture,
                screenBounds,
                Color.Black * 0.60f);

            const string title = "GAME OVER";
            const string restartText = "Press R to restart";
            const float titleScale = 3f;
            const float restartScale = 1.25f;
            const float spacing = 28f;

            Vector2 titleSize = boldpixels.MeasureString(title) * titleScale;
            Vector2 restartSize = boldpixels.MeasureString(restartText) * restartScale;
            float contentHeight = titleSize.Y + spacing + restartSize.Y;
            float top = (screenBounds.Height - contentHeight) / 2f;

            _spriteBatch.DrawString(
                boldpixels,
                title,
                new Vector2((screenBounds.Width - titleSize.X) / 2f, top),
                Color.Red,
                0f,
                Vector2.Zero,
                titleScale,
                SpriteEffects.None,
                0f);

            _spriteBatch.DrawString(
                boldpixels,
                restartText,
                new Vector2(
                    (screenBounds.Width - restartSize.X) / 2f,
                    top + titleSize.Y + spacing),
                Color.White,
                0f,
                Vector2.Zero,
                restartScale,
                SpriteEffects.None,
                0f);
        }

        private void DrawMap()
        {
            waterMap.Draw(_spriteBatch);
            worldMap.Draw(_spriteBatch);
            portalLayer.Draw(_spriteBatch);
            propsLayer.DrawBehindPlayer(_spriteBatch, player.Bounds.Bottom);
        }

        private void DrawPlayerResourceUi()
        {
            Vector2 screenBottomCenter = new Vector2(
                GraphicsDevice.Viewport.Width / 2f,
                GraphicsDevice.Viewport.Height);

            float healthPercent =
                player.Health.CurrentHealth / (float)player.Health.MaxHealth;

            float manaPercent =
                player.Mana.CurrentMana / player.Mana.MaxMana;

            DrawResourceMeter(
                healthMeterFrameTexture,
                healthMeterFillTexture,
                screenBottomCenter,
                HealthMeterOffsetFromBottomCenter,
                HealthFillOffset,
                healthPercent,
                HealthMeterScale,
                player.IsHealthFlashingWhite);

            DrawResourceMeter(
                manaMeterFrameTexture,
                manaMeterFillTexture,
                screenBottomCenter,
                ManaMeterOffsetFromBottomCenter,
                ManaFillOffset,
                manaPercent,
                ManaMeterScale,
                false);
        }

        private void DrawResourceMeter(
            Texture2D frameTexture,
            Texture2D fillTexture,
            Vector2 screenBottomCenter,
            Vector2 frameOffset,
            Vector2 fillOffset,
            float percent,
            float scale,
            bool flashWhite)
        {
            percent = MathHelper.Clamp(percent, 0f, 1f);

            Vector2 framePosition =
                screenBottomCenter + frameOffset;

            framePosition.X -= frameTexture.Width * scale / 2f;

            // Рамката се рисува цяла.
            _spriteBatch.Draw(
                frameTexture,
                framePosition,
                null,
                Color.White,
                0f,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                0f);

            int visibleFillWidth =
                (int)(fillTexture.Width * percent);

            if (visibleFillWidth <= 0)
                return;

            Rectangle fillSourceRectangle = new Rectangle(
                0,
                0,
                visibleFillWidth,
                fillTexture.Height);

            Vector2 fillPosition = framePosition + fillOffset * scale;

            // Рисуваме само частта, отговаряща на health/mana процента.
            _spriteBatch.Draw(
                fillTexture,
                fillPosition,
                fillSourceRectangle,
                Color.White,
                0f,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                0f);

            if (flashWhite)
            {
                Rectangle flashBounds = new Rectangle(
                    (int)fillPosition.X,
                    (int)fillPosition.Y,
                    (int)(visibleFillWidth * scale),
                    (int)(fillTexture.Height * scale));

                _spriteBatch.Draw(
                    pixelTexture,
                    flashBounds,
                    Color.White);
            }
        }
        private void DrawDeveloperMode()
        {
            foreach (Rectangle collisionRectangle in solidCollisionRectangles)
                DrawDebugRectangle(collisionRectangle, Color.White);

            DrawDebugRectangle(player.Bounds, Color.DodgerBlue);

            foreach (Coin coin in coins)
                DrawDebugRectangle(coin.Bounds, Color.Gold);

            foreach (ManaCrystal manaCrystal in manaCrystals)
            {
                DrawDebugRectangle(manaCrystal.Bounds, Color.DarkSlateBlue);
            }

            foreach (PlayerProjectile projectile in projectiles)
                DrawDebugRectangle(projectile.Bounds, Color.LimeGreen);

            foreach (Demon demon in demons)
                DrawDebugRectangle(demon.Bounds, Color.Red);

            foreach (Evil_Eye evilEye in evilEyes)
            {
                if (!evilEye.IsDead)
                    DrawDebugRectangle(evilEye.Bounds, Color.OrangeRed);
            }

            foreach (EnemyProjectile projectile in enemyProjectiles)
                DrawDebugRectangle(projectile.Bounds, Color.MediumPurple);
        }

        private void DrawDebugRectangle(Rectangle rectangle, Color color)
        {
            const int outlineThickness = 2;

            _spriteBatch.Draw(pixelTexture, rectangle, color * 0.25f);

            _spriteBatch.Draw(
                pixelTexture,
                new Rectangle(rectangle.X, rectangle.Y,
                    rectangle.Width, outlineThickness),
                color);

            _spriteBatch.Draw(
                pixelTexture,
                new Rectangle(rectangle.X, rectangle.Bottom - outlineThickness,
                    rectangle.Width, outlineThickness),
                color);

            _spriteBatch.Draw(
                pixelTexture,
                new Rectangle(rectangle.X, rectangle.Y,
                    outlineThickness, rectangle.Height),
                color);

            _spriteBatch.Draw(
                pixelTexture,
                new Rectangle(rectangle.Right - outlineThickness, rectangle.Y,
                    outlineThickness, rectangle.Height),
                color);
        }
        private void DrawWaveIntermissionUi()
        {
            if (gameFlowState != GameFlowState.WaveIntermission)
                return;

            DrawNextWavePreview();

            _spriteBatch.Draw(
                startNextWaveButtonTexture,
                startNextWaveButtonBounds,
                Color.White);
        }
        private void DrawNextWavePreview()
        {
            int nextWaveIndex = waveManager.CurrentWave;

            if (nextWaveIndex >= currentMission.Waves.Count)
                return;

            WaveDefinition nextWave = currentMission.Waves[nextWaveIndex];

            const int panelWidth = 350;
            const int panelHeight = 115;
            const int gapToButton = 16;
            const int iconSize = 52;
            const int groupSpacing = 34;

            Rectangle panelBounds = new Rectangle(
                startNextWaveButtonBounds.Left - panelWidth - gapToButton,
                startNextWaveButtonBounds.Bottom - panelHeight,
                panelWidth,
                panelHeight);

            DrawWavePreviewPanel(panelBounds);
            DrawCenteredPanelText(
                $"NEXT WAVE: {nextWaveIndex + 1}",
                panelBounds,
                22,
                Color.Gold);

            int x = panelBounds.Left + 40;
            int y = panelBounds.Top + 78;

            foreach (EnemySpawnGroup group in nextWave.SpawnGroups)
            {
                string countText = $"{group.Count}x";

                _spriteBatch.DrawString(
                    boldpixels,
                    countText,
                    new Vector2(x, y),
                    Color.White);

                x += (int)boldpixels.MeasureString(countText).X + 12;

                if (TryGetEnemyPreviewSprite(
                        group.EnemyType,
                        out Texture2D texture,
                        out Rectangle sourceRectangle))
                {
                    Rectangle iconBounds = new Rectangle(
                        x,
                        y - 14,
                        iconSize,
                        iconSize);

                    _spriteBatch.Draw(
                        texture,
                        iconBounds,
                        sourceRectangle,
                        Color.White);

                    x += iconSize + groupSpacing;
                }
            }
        }
        private void DrawWavePreviewPanel(Rectangle destinationBounds)
        {
            const int borderSize = 8;

            int sourceCenterWidth =
                wavePreviewPanelTexture.Width - borderSize * 2;

            int sourceCenterHeight =
                wavePreviewPanelTexture.Height - borderSize * 2;

            int destinationCenterWidth =
                destinationBounds.Width - borderSize * 2;

            int destinationCenterHeight =
                destinationBounds.Height - borderSize * 2;

            // Corners
            _spriteBatch.Draw(
                wavePreviewPanelTexture,
                new Rectangle(destinationBounds.Left, destinationBounds.Top,
                    borderSize, borderSize),
                new Rectangle(0, 0, borderSize, borderSize),
                Color.White);

            _spriteBatch.Draw(
                wavePreviewPanelTexture,
                new Rectangle(destinationBounds.Right - borderSize,
                    destinationBounds.Top, borderSize, borderSize),
                new Rectangle(wavePreviewPanelTexture.Width - borderSize,
                    0, borderSize, borderSize),
                Color.White);

            _spriteBatch.Draw(
                wavePreviewPanelTexture,
                new Rectangle(destinationBounds.Left,
                    destinationBounds.Bottom - borderSize,
                    borderSize, borderSize),
                new Rectangle(0,
                    wavePreviewPanelTexture.Height - borderSize,
                    borderSize, borderSize),
                Color.White);

            _spriteBatch.Draw(
                wavePreviewPanelTexture,
                new Rectangle(destinationBounds.Right - borderSize,
                    destinationBounds.Bottom - borderSize,
                    borderSize, borderSize),
                new Rectangle(wavePreviewPanelTexture.Width - borderSize,
                    wavePreviewPanelTexture.Height - borderSize,
                    borderSize, borderSize),
                Color.White);

            // Edges
            _spriteBatch.Draw(
                wavePreviewPanelTexture,
                new Rectangle(destinationBounds.Left + borderSize,
                    destinationBounds.Top,
                    destinationCenterWidth, borderSize),
                new Rectangle(borderSize, 0,
                    sourceCenterWidth, borderSize),
                Color.White);

            _spriteBatch.Draw(
                wavePreviewPanelTexture,
                new Rectangle(destinationBounds.Left + borderSize,
                    destinationBounds.Bottom - borderSize,
                    destinationCenterWidth, borderSize),
                new Rectangle(borderSize,
                    wavePreviewPanelTexture.Height - borderSize,
                    sourceCenterWidth, borderSize),
                Color.White);

            _spriteBatch.Draw(
                wavePreviewPanelTexture,
                new Rectangle(destinationBounds.Left,
                    destinationBounds.Top + borderSize,
                    borderSize, destinationCenterHeight),
                new Rectangle(0, borderSize,
                    borderSize, sourceCenterHeight),
                Color.White);

            _spriteBatch.Draw(
                wavePreviewPanelTexture,
                new Rectangle(destinationBounds.Right - borderSize,
                    destinationBounds.Top + borderSize,
                    borderSize, destinationCenterHeight),
                new Rectangle(wavePreviewPanelTexture.Width - borderSize,
                    borderSize, borderSize, sourceCenterHeight),
                Color.White);

            // Center
            _spriteBatch.Draw(
                wavePreviewPanelTexture,
                new Rectangle(destinationBounds.Left + borderSize,
                    destinationBounds.Top + borderSize,
                    destinationCenterWidth, destinationCenterHeight),
                new Rectangle(borderSize, borderSize,
                    sourceCenterWidth, sourceCenterHeight),
                Color.White);
        }
        private bool TryGetEnemyPreviewSprite(
            EnemyType enemyType,
            out Texture2D texture,
            out Rectangle sourceRectangle)
        {
            switch (enemyType)
            {
                case EnemyType.Demon:
                    texture = demonTexture;

                    // Demon FLYING sheet: 1 row, 4 frames
                    sourceRectangle = new Rectangle(
                        0,
                        0,
                        demonTexture.Width / 4,
                        demonTexture.Height);

                    return true;

                case EnemyType.EvilEye:
                    texture = evilEyeTexture;

                    // Evil Eye: 6 колони и 3 реда
                    // flying row, frame 1
                    sourceRectangle = new Rectangle(
                        0,
                        0,
                        evilEyeTexture.Width / 6,
                        evilEyeTexture.Height / 3);

                    return true;

                default:
                    texture = null;
                    sourceRectangle = Rectangle.Empty;
                    return false;
            }
        }
        private void DrawWaveProgressUi()
        {
            if (currentMission == null)
                return;

            string waveText =
                $"WAVE {waveManager.CurrentWave}/{currentMission.Waves.Count}";

            _spriteBatch.DrawString(
                boldpixels,
                waveText,
                new Vector2(24, 24),
                Color.White);
        }
        private void DrawNineSlicePanel(
                    Texture2D texture,
                    Rectangle destination,
                    Color? tint = null)
        {
            const int sourceSliceSize = 74;
            const int sourceGap = 17; // pixels between every slice 

            // Ъглите остават с оригиналния си размер
            const int borderSize = sourceSliceSize;

            if (destination.Width < borderSize * 2 ||
                destination.Height < borderSize * 2)
            {
                return;
            }

            Color color = tint ?? Color.White;

            // Начало на всяка от трите колони/редици в sprite sheet
            int[] sourcePositions = {
                                        0,
                                        sourceSliceSize + sourceGap,
                                        (sourceSliceSize + sourceGap) * 2
                                    };

            // Размери и позиции на деветте части в крайния panel
            int[] destinationX =
                                    {
                                destination.Left,
                                destination.Left + borderSize,
                                destination.Right - borderSize
                                    };

            int[] destinationY =
                                    {
                                destination.Top,
                                destination.Top + borderSize,
                                destination.Bottom - borderSize
                                    };

            int[] destinationWidths =
                                    {
                                borderSize,
                                destination.Width - borderSize * 2,
                                borderSize
                                    };

            int[] destinationHeights =
                                    {
                                borderSize,
                                destination.Height - borderSize * 2,
                                borderSize
                                    };

            // Рисува деветте части
            for (int row = 0; row < 3; row++)
            {
                for (int column = 0; column < 3; column++)
                {
                    Rectangle sourceRectangle = new Rectangle(
                        sourcePositions[column],
                        sourcePositions[row],
                        sourceSliceSize,
                        sourceSliceSize);

                    Rectangle destinationRectangle = new Rectangle(
                        destinationX[column],
                        destinationY[row],
                        destinationWidths[column],
                        destinationHeights[row]);

                    _spriteBatch.Draw(
                        texture,
                        destinationRectangle,
                        sourceRectangle,
                        color);
                }
            }
        }
    }
}