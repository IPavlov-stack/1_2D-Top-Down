using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _1_2D_Top_Down
{
    public partial class Game1
    {
        

        private void DrawNormalWorld()
        {
            DrawMap();

            foreach (Coin coin in coins)
                coin.Draw(_spriteBatch);

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

        private void DrawUi()
        {
            _spriteBatch.Begin();

            _spriteBatch.DrawString(
                boldpixels,
                $"Coins: {coinsCollected}",
                new Vector2(20, 20),
                Color.Gold);

            DrawGameplayUI();

            DrawExitConfirmation();

            if (isGameOver)
                DrawGameOverScreen();

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
                HealthMeterScale);

            DrawResourceMeter(
                manaMeterFrameTexture,
                manaMeterFillTexture,
                screenBottomCenter,
                ManaMeterOffsetFromBottomCenter,
                ManaFillOffset,
                manaPercent,
                ManaMeterScale);
        }

        private void DrawResourceMeter(
            Texture2D frameTexture,
            Texture2D fillTexture,
            Vector2 screenBottomCenter,
            Vector2 frameOffset,
            Vector2 fillOffset,
            float percent,
            float scale)
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

            Vector2 fillPosition =
                framePosition + fillOffset * scale;

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
        }
        private void DrawDeveloperMode()
        {
            foreach (Rectangle collisionRectangle in solidCollisionRectangles)
                DrawDebugRectangle(collisionRectangle, Color.White);

            DrawDebugRectangle(player.Bounds, Color.DodgerBlue);

            foreach (Coin coin in coins)
                DrawDebugRectangle(coin.Bounds, Color.Gold);

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
    }
}