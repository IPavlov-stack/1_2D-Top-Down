using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tiled;

namespace _1_2D_Top_Down
{
    /// <summary>
    /// Draws optional world-space developer visualizations over normal gameplay.
    /// </summary>
    public sealed class WorldDebugRenderer
    {
        private readonly Texture2D pixelTexture;

        public WorldDebugRenderer(Texture2D pixelTexture)
        {
            this.pixelTexture = pixelTexture;
        }

        public void Draw(
            SpriteBatch spriteBatch,
            GameplaySession session,
            DeveloperViewSettings view)
        {
            if (view.ShowCollisions)
            {
                foreach (Rectangle bounds in session.Map.SolidCollisionRectangles)
                    DrawRectangle(spriteBatch, bounds, Color.White);
            }

            if (view.ShowEntityBounds)
            {
                DrawRectangle(spriteBatch, session.Player.Bounds, Color.DodgerBlue);

                foreach (Enemy enemy in session.World.Enemies.Enemies)
                    DrawRectangle(spriteBatch, enemy.Bounds, Color.Red);
            }

            if (view.ShowCollectibleBounds)
            {
                foreach (Coin coin in session.World.Collectibles.Coins)
                    DrawRectangle(spriteBatch, coin.Bounds, Color.Gold);

                foreach (ManaCrystal crystal in session.World.Collectibles.ManaCrystals)
                    DrawRectangle(spriteBatch, crystal.Bounds, Color.DarkSlateBlue);
            }

            if (view.ShowProjectileBounds)
            {
                foreach (PlayerProjectile projectile in
                         session.World.Projectiles.PlayerProjectiles)
                {
                    DrawRectangle(spriteBatch, projectile.Bounds, Color.LimeGreen);
                }

                foreach (EnemyProjectile projectile in
                         session.World.Projectiles.EnemyProjectiles)
                {
                    DrawRectangle(spriteBatch, projectile.Bounds, Color.MediumPurple);
                }
            }

            if (view.ShowSpawnPoints)
                DrawSpawnData(spriteBatch, session.Map);
        }

        private void DrawSpawnData(SpriteBatch spriteBatch, GameMap map)
        {
            foreach (Vector2 position in map.EnemySpawnerPoints)
                DrawPoint(spriteBatch, position, Color.Orange);

            foreach (EnemySpawnPoint spawnPoint in map.EnemySpawnPoints)
                DrawPoint(spriteBatch, spawnPoint.Position, Color.HotPink);

            foreach (MissionTrigger trigger in map.MissionTriggers)
                DrawRectangle(spriteBatch, trigger.Bounds, Color.Cyan);

            if (map.PlayerSpawnPosition != Vector2.Zero)
                DrawPoint(spriteBatch, map.PlayerSpawnPosition, Color.DodgerBlue);
        }

        private void DrawPoint(SpriteBatch spriteBatch, Vector2 position, Color color)
        {
            const int markerSize = 14;
            DrawRectangle(
                spriteBatch,
                new Rectangle(
                    (int)position.X - markerSize / 2,
                    (int)position.Y - markerSize / 2,
                    markerSize,
                    markerSize),
                color);
        }

        private void DrawRectangle(
            SpriteBatch spriteBatch,
            Rectangle rectangle,
            Color color)
        {
            const int outlineThickness = 2;

            spriteBatch.Draw(pixelTexture, rectangle, color * 0.25f);
            spriteBatch.Draw(
                pixelTexture,
                new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, outlineThickness),
                color);
            spriteBatch.Draw(
                pixelTexture,
                new Rectangle(
                    rectangle.X,
                    rectangle.Bottom - outlineThickness,
                    rectangle.Width,
                    outlineThickness),
                color);
            spriteBatch.Draw(
                pixelTexture,
                new Rectangle(rectangle.X, rectangle.Y, outlineThickness, rectangle.Height),
                color);
            spriteBatch.Draw(
                pixelTexture,
                new Rectangle(
                    rectangle.Right - outlineThickness,
                    rectangle.Y,
                    outlineThickness,
                    rectangle.Height),
                color);
        }
    }
}
