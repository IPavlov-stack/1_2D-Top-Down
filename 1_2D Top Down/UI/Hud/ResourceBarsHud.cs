using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _1_2D_Top_Down
{
    public sealed class ResourceBarsHud
    {
        private const float Scale = 2.5f;
        private const int Margin = 16;
        private static readonly Rectangle FrameSource = new(0, 0, 131, 38);
        private static readonly Rectangle HealthFillSource = new(16, 45, 40, 10);
        private static readonly Rectangle ManaFillSource = new(76, 45, 40, 10);
        private static readonly Point HealthFillPosition = new(42, 14);
        private static readonly Point ManaFillPosition = new(86, 14);

        private readonly Texture2D texture;
        private readonly Texture2D pixelTexture;

        public ResourceBarsHud(Texture2D texture, Texture2D pixelTexture)
        {
            this.texture = texture;
            this.pixelTexture = pixelTexture;
        }

        public int Bottom => Margin + (int)(FrameSource.Height * Scale);

        public void Draw(
            SpriteBatch spriteBatch,
            float healthPercent,
            float manaPercent,
            bool flashHealth)
        {
            Vector2 position = new(Margin, Margin);

            spriteBatch.Draw(
                texture,
                position,
                FrameSource,
                Color.White,
                0f,
                Vector2.Zero,
                Scale,
                SpriteEffects.None,
                0f);

            DrawFill(
                spriteBatch,
                position,
                HealthFillSource,
                HealthFillPosition,
                healthPercent,
                Color.White);

            if (flashHealth)
            {
                DrawFlash(
                    spriteBatch,
                    position,
                    HealthFillSource,
                    HealthFillPosition,
                    healthPercent);
            }

            DrawFill(
                spriteBatch,
                position,
                ManaFillSource,
                ManaFillPosition,
                manaPercent,
                Color.White);
        }

        private void DrawFill(
            SpriteBatch spriteBatch,
            Vector2 framePosition,
            Rectangle fullSource,
            Point fillPosition,
            float percent,
            Color tint)
        {
            percent = MathHelper.Clamp(percent, 0f, 1f);
            int visibleWidth = (int)(fullSource.Width * percent);

            if (visibleWidth <= 0)
                return;

            Rectangle source = new(
                fullSource.X,
                fullSource.Y,
                visibleWidth,
                fullSource.Height);

            spriteBatch.Draw(
                texture,
                framePosition + fillPosition.ToVector2() * Scale,
                source,
                tint,
                0f,
                Vector2.Zero,
                Scale,
                SpriteEffects.None,
                0f);
        }

        private void DrawFlash(
            SpriteBatch spriteBatch,
            Vector2 framePosition,
            Rectangle fullSource,
            Point fillPosition,
            float percent)
        {
            int visibleWidth = (int)(
                fullSource.Width * MathHelper.Clamp(percent, 0f, 1f) * Scale);

            if (visibleWidth <= 0)
                return;

            Rectangle bounds = new(
                (int)(framePosition.X + fillPosition.X * Scale),
                (int)(framePosition.Y + fillPosition.Y * Scale),
                visibleWidth,
                (int)(fullSource.Height * Scale));

            spriteBatch.Draw(pixelTexture, bounds, Color.White);
        }
    }
}
