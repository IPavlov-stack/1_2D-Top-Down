using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace _1_2D_Top_Down
{
    public sealed class QuestLogPanel : IGameplayPanel
    {
        private readonly Func<Rectangle> viewport;
        private readonly Texture2D texture;
        private readonly SpriteFont font;

        public QuestLogPanel(Func<Rectangle> viewport, Texture2D texture, SpriteFont font)
        {
            this.viewport = viewport;
            this.texture = texture;
            this.font = font;
        }

        public string Id => GameplayPanelIds.QuestLog;
        public bool BlocksGameplayInput => true;
        public void Open() { }
        public void Close() { }
        public bool HandleInput(GameplayUiInput input) => false;

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle view = viewport();
            Rectangle bounds = new(view.Left + 25, view.Top + 70, 360, 540);
            spriteBatch.Draw(texture, bounds, Color.White);
            PanelTitle.Draw(spriteBatch, font, "QUEST LOG  [Q]", bounds, 42, Color.Gold);
        }
    }
}
