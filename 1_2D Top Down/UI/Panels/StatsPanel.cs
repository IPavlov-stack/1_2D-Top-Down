using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace _1_2D_Top_Down
{
    public sealed class StatsPanel : IGameplayPanel
    {
        private const float Scale = 0.8f;
        private readonly Func<Rectangle> viewport;
        private readonly Func<IReadOnlyList<StatSection>> sections;
        private readonly Texture2D panelTexture;
        private readonly Texture2D pixelTexture;
        private readonly SpriteFont font;

        public StatsPanel(Func<Rectangle> viewport, Func<IReadOnlyList<StatSection>> sections,
            Texture2D panelTexture, Texture2D pixelTexture, SpriteFont font)
        {
            this.viewport = viewport;
            this.sections = sections;
            this.panelTexture = panelTexture;
            this.pixelTexture = pixelTexture;
            this.font = font;
        }

        public string Id => GameplayPanelIds.Stats;
        public bool BlocksGameplayInput => true;
        public void Open() { }
        public void Close() { }
        public bool HandleInput(GameplayUiInput input) => false;

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle view = viewport();
            Rectangle bounds = new(10, view.Center.Y - 440, 560, 880);
            UiDrawing.DrawNineSlicePanel(spriteBatch, panelTexture, bounds);
            PanelTitle.Draw(spriteBatch, font, "STATS", bounds, 30, Color.Gold);

            int y = bounds.Top + 115;
            foreach (StatSection section in sections())
            {
                spriteBatch.DrawString(font, section.Title,
                    new Vector2(bounds.Left + 70, y), Color.Gold);
                y += 42;

                foreach (StatRow row in section.Rows)
                {
                    DrawRow(spriteBatch, row, bounds, y);
                    y += 48;
                }

                y += 24;
            }
        }

        private void DrawRow(SpriteBatch spriteBatch, StatRow row, Rectangle bounds, int y)
        {
            const int padding = 70;
            Vector2 valueSize = font.MeasureString(row.Value);
            spriteBatch.DrawString(font, row.Label, new Vector2(bounds.Left + padding, y), Color.White);
            spriteBatch.DrawString(font, row.Value,
                new Vector2(bounds.Right - padding - valueSize.X, y), Color.Gold);
            spriteBatch.Draw(pixelTexture,
                new Rectangle(bounds.Left + padding, y + 34, bounds.Width - padding * 2, 2),
                Color.Black * 0.45f);
        }

    }

    public sealed record StatSection(string Title, IReadOnlyList<StatRow> Rows);
    public sealed record StatRow(string Label, string Value);
}
