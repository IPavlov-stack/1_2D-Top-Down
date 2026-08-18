using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace _1_2D_Top_Down
{
    public sealed class InventoryPanel : IGameplayPanel
    {
        private const float PanelScale = 0.8f;
        private const int PanelWidth = 700;
        private const int PanelHeight = 1100;
        private const int PanelRightMargin = 10;
        private const int SlotCount = 28;
        private const int SlotSize = 100;
        private const int SlotSpacing = 12;
        private const int NineSliceBorderSize = 74;
        private const int ContentPadding = 28;
        private const int HeaderHeight = 64;
        private const int IconPadding = 18;

        private readonly Func<Rectangle> viewportBoundsProvider;
        private readonly IReadOnlyList<InventoryResource> resources;
        private readonly Texture2D panelTexture;
        private readonly Texture2D slotTexture;
        private readonly Texture2D pixelTexture;
        private readonly SpriteFont font;

        public InventoryPanel(
            Func<Rectangle> viewportBoundsProvider,
            IReadOnlyList<InventoryResource> resources,
            Texture2D panelTexture,
            Texture2D slotTexture,
            Texture2D pixelTexture,
            SpriteFont font)
        {
            this.viewportBoundsProvider = viewportBoundsProvider;
            this.resources = resources;
            this.panelTexture = panelTexture;
            this.slotTexture = slotTexture;
            this.pixelTexture = pixelTexture;
            this.font = font;
        }

        public string Id => GameplayPanelIds.Inventory;
        public bool BlocksGameplayInput => true;

        public void Open() { }
        public void Close() { }
        public bool HandleInput(GameplayUiInput input) => false;

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle viewport = viewportBoundsProvider();
            int panelWidth = ScaleUi(PanelWidth, PanelScale);
            int panelHeight = ScaleUi(PanelHeight, PanelScale);
            Rectangle panelBounds = new(
                viewport.Right - panelWidth - PanelRightMargin,
                viewport.Center.Y - panelHeight / 2,
                panelWidth,
                panelHeight);

            UiDrawing.DrawNineSlicePanel(
                spriteBatch,
                panelTexture,
                panelBounds);

            PanelTitle.Draw(spriteBatch, font, "Inventory", panelBounds, 28, Color.Gold);

            DrawSlots(spriteBatch, panelBounds);
        }

        private void DrawSlots(SpriteBatch spriteBatch, Rectangle panelBounds)
        {
            int borderSize = ScaleUi(NineSliceBorderSize, PanelScale);
            int contentPadding = ScaleUi(ContentPadding, PanelScale);
            int headerHeight = ScaleUi(HeaderHeight, PanelScale);
            int slotSize = ScaleUi(SlotSize, PanelScale);
            int slotSpacing = ScaleUi(SlotSpacing, PanelScale);
            int contentLeft = panelBounds.Left + borderSize + contentPadding;
            int contentTop = panelBounds.Top + borderSize + headerHeight;
            int contentWidth =
                panelBounds.Width - (borderSize + contentPadding) * 2;
            int contentHeight =
                panelBounds.Height - borderSize - headerHeight - contentPadding * 2;
            int columns = Math.Max(
                1,
                (contentWidth + slotSpacing) / (slotSize + slotSpacing));
            int rows = Math.Max(
                1,
                (contentHeight + slotSpacing) / (slotSize + slotSpacing));
            int slotsToDraw = Math.Min(SlotCount, columns * rows);
            int gridWidth = columns * slotSize + (columns - 1) * slotSpacing;
            int startX = contentLeft + (contentWidth - gridWidth) / 2;

            for (int slotIndex = 0; slotIndex < slotsToDraw; slotIndex++)
            {
                int column = slotIndex % columns;
                int row = slotIndex / columns;
                Rectangle slotBounds = new(
                    startX + column * (slotSize + slotSpacing),
                    contentTop + row * (slotSize + slotSpacing),
                    slotSize,
                    slotSize);

                spriteBatch.Draw(slotTexture, slotBounds, Color.White);

                if (slotIndex < resources.Count)
                    DrawResource(spriteBatch, resources[slotIndex], slotBounds);
            }
        }

        private void DrawResource(
            SpriteBatch spriteBatch,
            InventoryResource resource,
            Rectangle slotBounds)
        {
            Rectangle iconBounds = new(
                slotBounds.X + IconPadding,
                slotBounds.Y + IconPadding,
                slotBounds.Width - IconPadding * 2,
                slotBounds.Height - IconPadding * 2);
            spriteBatch.Draw(resource.Icon, iconBounds, Color.White);

            string amountText = resource.Amount.ToString();
            Vector2 textSize = font.MeasureString(amountText);
            int textX = slotBounds.Right - (int)textSize.X - 8;
            int textY = slotBounds.Bottom - (int)textSize.Y - 6;
            Rectangle countBackground = new(
                textX - 5,
                textY - 3,
                (int)textSize.X + 10,
                (int)textSize.Y + 6);

            spriteBatch.Draw(pixelTexture, countBackground, Color.Black * 0.75f);
            spriteBatch.DrawString(
                font,
                amountText,
                new Vector2(textX, textY),
                Color.White);
        }

        private static int ScaleUi(int value, float scale) =>
            Math.Max(1, (int)MathF.Round(value * scale));
    }
}
