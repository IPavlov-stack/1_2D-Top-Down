using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace _1_2D_Top_Down
{
    public sealed class HotbarHud
    {
        private const int SlotCount = 6;
        private readonly Func<Rectangle> viewport;
        private readonly Texture2D panelTexture;
        private readonly Texture2D pixelTexture;
        private readonly SpriteFont font;

        public HotbarHud(Func<Rectangle> viewport, Texture2D panelTexture,
            Texture2D pixelTexture, SpriteFont font)
        {
            this.viewport = viewport;
            this.panelTexture = panelTexture;
            this.pixelTexture = pixelTexture;
            this.font = font;
        }

        public int SelectedSlot { get; private set; }

        public void HandleInput(GameplayUiInput input)
        {
            Keys[] main = { Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6 };
            Keys[] numpad = { Keys.NumPad1, Keys.NumPad2, Keys.NumPad3,
                Keys.NumPad4, Keys.NumPad5, Keys.NumPad6 };
            for (int i = 0; i < SlotCount; i++)
            {
                if ((input.Keyboard.IsKeyDown(main[i]) && input.PreviousKeyboard.IsKeyUp(main[i])) ||
                    (input.Keyboard.IsKeyDown(numpad[i]) && input.PreviousKeyboard.IsKeyUp(numpad[i])))
                    SelectedSlot = i;
            }
        }

        public void Draw(SpriteBatch batch)
        {
            Rectangle view = viewport();
            int x = view.Center.X - 360;
            int y = view.Bottom - 240 + 77;
            Rectangle panel = new(x, y, 720, 240);
            batch.Draw(panelTexture, panel, Color.White);
            int slotWidth = (720 - 68 - 68) / SlotCount;
            for (int i = 0; i < SlotCount; i++)
            {
                Rectangle slot = new(x + 68 + i * slotWidth, y + 72, slotWidth, 120);
                if (i == SelectedSlot)
                    batch.Draw(pixelTexture, slot, Color.Gold * 0.18f);
                batch.DrawString(font, (i + 1).ToString(),
                    new Vector2(slot.X + 10, slot.Y + 8),
                    i == SelectedSlot ? Color.Gold : Color.White);
            }
        }
    }
}
