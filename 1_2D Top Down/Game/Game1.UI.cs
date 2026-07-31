using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace _1_2D_Top_Down
{
    public partial class Game1
    {
        private const int HotbarSlotCount = 6;
        private int selectedSpellSlot;
        private bool isInventoryOpen;
        private bool isQuestLogOpen;

        // Hotbar
        private const int HotbarPanelWidth = 720;
        private const int HotbarPanelHeight = 240;
        private const int HotbarBottomMargin = -50;
        private const int HotbarHorizontalOffset = 0;

        private const int HotbarSlotLeftPadding = 68;
        private const int HotbarSlotRightPadding = 68;
        private const int HotbarSlotTop = 72;
        private const int HotbarSlotHeight = 120;

        // Inventory
        private const int InventoryPanelWidth = 950;
        private const int InventoryPanelHeight = 950;
        private const int InventoryPanelRightMargin = 485;
        private const int InventoryPanelTop = 0;

        // Quest log
        private const int QuestPanelWidth = 360;
        private const int QuestPanelHeight = 540;
        private const int QuestPanelLeft = 25;
        private const int QuestPanelTop = 70;

        private void HandleGameplayUIInput(KeyboardState keyboard)
        {
            if (WasSpellKeyPressed(keyboard, Keys.D1, Keys.NumPad1))
                selectedSpellSlot = 0;

            if (WasSpellKeyPressed(keyboard, Keys.D2, Keys.NumPad2))
                selectedSpellSlot = 1;

            if (WasSpellKeyPressed(keyboard, Keys.D3, Keys.NumPad3))
                selectedSpellSlot = 2;

            if (WasSpellKeyPressed(keyboard, Keys.D4, Keys.NumPad4))
                selectedSpellSlot = 3;

            if (WasSpellKeyPressed(keyboard, Keys.D5, Keys.NumPad5))
                selectedSpellSlot = 4;

            if (WasSpellKeyPressed(keyboard, Keys.D6, Keys.NumPad6))
                selectedSpellSlot = 5;

            if (keyboard.IsKeyDown(Keys.I) &&
                previousKeyboard.IsKeyUp(Keys.I))
            {
                isInventoryOpen = !isInventoryOpen;
            }

            if (keyboard.IsKeyDown(Keys.Q) &&
                previousKeyboard.IsKeyUp(Keys.Q))
            {
                isQuestLogOpen = !isQuestLogOpen;
            }
        }

        private bool WasSpellKeyPressed(
            KeyboardState keyboard,
            Keys numberKey,
            Keys numpadKey)
        {
            return
                (keyboard.IsKeyDown(numberKey) &&
                 previousKeyboard.IsKeyUp(numberKey))
                ||
                (keyboard.IsKeyDown(numpadKey) &&
                 previousKeyboard.IsKeyUp(numpadKey));
        }

        private void DrawGameplayUI()
        {
            DrawSpellHotbar();

            if (isInventoryOpen)
            {
                DrawInventoryPanel();
            }

            if (isQuestLogOpen)
            {
                DrawQuestLogPanel();
            }
        }

        private void DrawSpellHotbar()
        {
            int x = GraphicsDevice.Viewport.Width / 2 -
                    HotbarPanelWidth / 2 +
                    HotbarHorizontalOffset;

            int y = GraphicsDevice.Viewport.Height -
                    HotbarPanelHeight -
                    HotbarBottomMargin;

            Rectangle panelBounds = new Rectangle(
                x,
                y,
                HotbarPanelWidth,
                HotbarPanelHeight);
            _spriteBatch.Draw(
                spellsPanelTexture,
                panelBounds,
                Color.White);

            int usableWidth =
                HotbarPanelWidth -
                HotbarSlotLeftPadding -
                HotbarSlotRightPadding;

            int slotWidth = usableWidth / HotbarSlotCount;

            for (int i = 0; i < HotbarSlotCount; i++)
            {
                Rectangle slotBounds = new Rectangle(
                    x + HotbarSlotLeftPadding + i * slotWidth,
                    y + HotbarSlotTop,
                    slotWidth,
                    HotbarSlotHeight);

                bool isSelected = i == selectedSpellSlot;

                if (isSelected)
                {
                    _spriteBatch.Draw(
                        pixelTexture,
                        slotBounds,
                        Color.Gold * 0.18f);
                }

                string keyText = (i + 1).ToString();

                _spriteBatch.DrawString(
                    boldpixels,
                    keyText,
                    new Vector2(slotBounds.X + 10, slotBounds.Y + 8),
                    isSelected ? Color.Gold : Color.White);
            }
        }

        private void DrawInventoryPanel()
        {

            Rectangle panelBounds = new Rectangle(
                GraphicsDevice.Viewport.Width -
                InventoryPanelWidth -
                InventoryPanelRightMargin,

                InventoryPanelTop,

                InventoryPanelWidth,
                InventoryPanelHeight);
            _spriteBatch.Draw(
                inventoryPanelTexture,
                panelBounds,
                Color.White);

            DrawCenteredPanelText(
                "Inventory",
                panelBounds,
                85,
                Color.Gold);
        }

        private void DrawQuestLogPanel()
        {

            Rectangle panelBounds = new Rectangle(
                QuestPanelLeft,
                QuestPanelTop,
                QuestPanelWidth,
                QuestPanelHeight);

            _spriteBatch.Draw(
                questPanelTexture,
                panelBounds,
                Color.White);

            DrawCenteredPanelText(
                "QUEST LOG  [Q]",
                panelBounds,
                42,
                Color.Gold);
        }
        private void DrawCenteredPanelText(
                string text,
                Rectangle panelBounds,
                int yOffset,
                Color color)
        {
            Vector2 textSize = boldpixels.MeasureString(text);

            _spriteBatch.DrawString(
                boldpixels,
                text,
                new Vector2(
                    panelBounds.Center.X - textSize.X / 2f,
                    panelBounds.Y + yOffset),
                color);
        }

        private void DrawPanel(Rectangle bounds, string title)
        {
            _spriteBatch.Draw(pixelTexture, bounds, Color.Black * 0.85f);

            Rectangle innerBounds = new Rectangle(
                bounds.X + 3,
                bounds.Y + 3,
                bounds.Width - 6,
                bounds.Height - 6);

            _spriteBatch.Draw(
                pixelTexture,
                innerBounds,
                Color.DarkSlateGray * 0.95f);

            _spriteBatch.DrawString(
                boldpixels,
                title,
                new Vector2(bounds.X + 20, bounds.Y + 20),
                Color.Gold);
        }
    }
}