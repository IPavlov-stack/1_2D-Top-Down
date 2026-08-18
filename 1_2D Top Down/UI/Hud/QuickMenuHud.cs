using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace _1_2D_Top_Down
{
    public sealed class QuickMenuHud
    {
        private const int ButtonSize = 64;
        private const int Spacing = 10;
        private const int Columns = 7;
        private readonly Func<Rectangle> viewport;
        private readonly List<UiButton> buttons;

        public QuickMenuHud(Func<Rectangle> viewport, Texture2D pixelTexture,
            Texture2D inventory, Texture2D stats, Texture2D shop, Texture2D map,
            Texture2D skillTree, Texture2D settings, Texture2D sound,
            Action openInventory, Action openStats, Action openShop, Action openSound)
        {
            this.viewport = viewport;
            buttons = new List<UiButton>
            {
                new(inventory, pixelTexture, openInventory),
                new(stats, pixelTexture, openStats),
                new(shop, pixelTexture, openShop),
                new(map, pixelTexture, () => { }, false),
                new(skillTree, pixelTexture, () => { }, false),
                new(settings, pixelTexture, () => { }, false),
                new(sound, pixelTexture, openSound)
            };
        }

        public bool HandleInput(GameplayUiInput input)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                if (buttons[i].HandleInput(input, GetBounds(i)))
                    return true;
            }
            return false;
        }

        public void Draw(SpriteBatch batch, Point mousePosition)
        {
            for (int i = 0; i < buttons.Count; i++)
                buttons[i].Draw(batch, GetBounds(i), mousePosition);
        }

        private Rectangle GetBounds(int index)
        {
            Rectangle view = viewport();
            int gridWidth = Columns * ButtonSize + (Columns - 1) * Spacing;
            int startX = view.Right - 25 - gridWidth;
            return new Rectangle(startX + index % Columns * (ButtonSize + Spacing),
                view.Top + 25 + index / Columns * (ButtonSize + Spacing), ButtonSize, ButtonSize);
        }
    }
}
