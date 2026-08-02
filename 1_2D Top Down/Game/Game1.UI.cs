using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

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
        private const int HotbarBottomMargin = -77;
        private const int HotbarHorizontalOffset = 0;

        private const int HotbarSlotLeftPadding = 68;
        private const int HotbarSlotRightPadding = 68;
        private const int HotbarSlotTop = 72;
        private const int HotbarSlotHeight = 120;

        // Inventory
        private const int InventoryPanelWidth = 950;
        private const int InventoryPanelHeight = 950;
        private const int InventoryPanelRightMargin = 485;
        private const int InventoryPanelTop = 50;

        private const int InventorySlotCount = 25;
        private const int InventorySlotSize = 128;
        private const int InventorySlotSpacing = 12;

        private const int NineSliceBorderSize = 74;
        private const int InventoryContentPadding = 28;
        private const int InventoryHeaderHeight = 64;

        // Quest log
        private const int QuestPanelWidth = 360;
        private const int QuestPanelHeight = 540;
        private const int QuestPanelLeft = 25;
        private const int QuestPanelTop = 70;

        private const float HealthMeterScale = 0.55f;
        private const float ManaMeterScale = 0.55f;
        // =================
        // = X:  + right,  =
        // =     - left    =
        // = Y:  + down,   =
        // =     - up      =
        // =================
        private static readonly Vector2 HealthMeterOffsetFromBottomCenter = new Vector2(-200f, -175);
        private static readonly Vector2 ManaMeterOffsetFromBottomCenter = new Vector2(200f, -175f);

        // Позиция на fill текстурата вътре в рамката
        private static readonly Vector2 HealthFillOffset = new Vector2(85f, 44f);
        private static readonly Vector2 ManaFillOffset =  new Vector2(85f, 44f);

        // bottom HUD panel
        private const float BottomHudPanelScale = 0.55f;
        private static readonly Vector2 BottomHudPanelOffsetFromBottomCenter = new Vector2(0f, -280f);

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
            if (!isInventoryOpen)
            {
                DrawSpellHotbar();
                DrawPlayerResourceUi();
            }

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
                GraphicsDevice.Viewport.Width
                    - InventoryPanelWidth
                    - InventoryPanelRightMargin,
                InventoryPanelTop,
                InventoryPanelWidth,
                InventoryPanelHeight);

            //window bg
            DrawNineSlicePanel(panel9SliceTexture, panelBounds);

            //title
            const string title = "Inventory";

            Vector2 titleSize = boldpixels.MeasureString(title);
            Vector2 titlePosition = new Vector2(
                panelBounds.Center.X - titleSize.X / 2f,
                panelBounds.Top + 28);

            _spriteBatch.DrawString(
                boldpixels,
                title,
                titlePosition,
                Color.Gold);

            //inventory slots
            DrawInventorySlots(panelBounds);
        }
        private void DrawInventorySlots(Rectangle panelBounds)
        {
            int contentLeft =
                panelBounds.Left +
                NineSliceBorderSize +
                InventoryContentPadding;

            int contentTop =
                panelBounds.Top +
                NineSliceBorderSize +
                InventoryHeaderHeight;

            int contentWidth =
                panelBounds.Width -
                (NineSliceBorderSize + InventoryContentPadding) * 2;

            int contentHeight =
                panelBounds.Height -
                NineSliceBorderSize -
                InventoryHeaderHeight -
                InventoryContentPadding * 2;

            int columns = Math.Max(
                1,
                (contentWidth + InventorySlotSpacing) /
                (InventorySlotSize + InventorySlotSpacing));

            int rows = Math.Max(
                1,
                (contentHeight + InventorySlotSpacing) /
                (InventorySlotSize + InventorySlotSpacing));

            int slotsToDraw = Math.Min(
                InventorySlotCount,
                columns * rows);

            int gridWidth =
                columns * InventorySlotSize +
                (columns - 1) * InventorySlotSpacing;

            int startX =
                contentLeft +
                (contentWidth - gridWidth) / 2;

            for (int slotIndex = 0; slotIndex < slotsToDraw; slotIndex++)
            {
                int column = slotIndex % columns;
                int row = slotIndex / columns;

                int x = startX +
                        column * (InventorySlotSize + InventorySlotSpacing);

                int y = contentTop +
                        row * (InventorySlotSize + InventorySlotSpacing);

                Rectangle slotBounds = new Rectangle(
                    x,
                    y,
                    InventorySlotSize,
                    InventorySlotSize);

                _spriteBatch.Draw(
                    inventorySlotTexture,
                    slotBounds,
                    Color.White);

                if (slotIndex < inventoryResources.Count)
                {
                    DrawInventoryResource(
                        inventoryResources[slotIndex],
                        slotBounds);
                }
            }
        }
        private void DrawInventoryResource(
    InventoryResource resource,
    Rectangle slotBounds)
        {
            const int iconPadding = 18;

            Rectangle iconBounds = new Rectangle(
                slotBounds.X + iconPadding,
                slotBounds.Y + iconPadding,
                slotBounds.Width - iconPadding * 2,
                slotBounds.Height - iconPadding * 2);

            _spriteBatch.Draw(
                resource.Icon,
                iconBounds,
                Color.White);

            string amountText = resource.Amount.ToString();
            Vector2 textSize = boldpixels.MeasureString(amountText);

            int textX = slotBounds.Right - (int)textSize.X - 8;
            int textY = slotBounds.Bottom - (int)textSize.Y - 6;

            Rectangle countBackground = new Rectangle(
                textX - 5,
                textY - 3,
                (int)textSize.X + 10,
                (int)textSize.Y + 6);

            _spriteBatch.DrawString(
                boldpixels,
                amountText,
                new Vector2(textX, textY),
                Color.White);
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
        private void DrawBottomHudPanel()
        {
            float scale = BottomHudPanelScale;

            Vector2 panelPosition = new Vector2(
                GraphicsDevice.Viewport.Width / 2f -
                bottomHudPanelTexture.Width * scale / 2f +
                BottomHudPanelOffsetFromBottomCenter.X,

                GraphicsDevice.Viewport.Height +
                BottomHudPanelOffsetFromBottomCenter.Y);

            _spriteBatch.Draw(
                bottomHudPanelTexture,
                panelPosition,
                null,
                Color.White,
                0f,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                0f);
        }
        private void AddInventoryResource(
    string resourceId,
    Texture2D icon,
    int amount)
        {
            if (amount <= 0)
                return;

            foreach (InventoryResource resource in inventoryResources)
            {
                if (resource.Id == resourceId)
                {
                    resource.Add(amount);
                    return;
                }
            }

            inventoryResources.Add(
                new InventoryResource(resourceId, icon, amount));
        }

        private bool TrySpendInventoryResource(string resourceId, int amount)
        {
            for (int i = 0; i < inventoryResources.Count; i++)
            {
                InventoryResource resource = inventoryResources[i];

                if (resource.Id != resourceId)
                    continue;

                if (!resource.TryRemove(amount))
                    return false;

                // При количество 0 слотът се освобождава.
                if (resource.Amount == 0)
                    inventoryResources.RemoveAt(i);

                return true;
            }

            return false;
        }
    }
}