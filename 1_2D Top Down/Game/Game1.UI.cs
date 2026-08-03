using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using static _1_2D_Top_Down.ShopItem;

namespace _1_2D_Top_Down
{
    public partial class Game1
    {
        private bool isInventoryOpen;
        private bool isQuestLogOpen;
        private bool isStatsOpen;

        // Hotbar
        private const int HotbarSlotCount = 6;
        private const int HotbarPanelWidth = 720;
        private const int HotbarPanelHeight = 240;
        private const int HotbarBottomMargin = -77;
        private const int HotbarHorizontalOffset = 0;
        private int selectedSpellSlot;

        private const int HotbarSlotLeftPadding = 68;
        private const int HotbarSlotRightPadding = 68;
        private const int HotbarSlotTop = 72;
        private const int HotbarSlotHeight = 120;

        // Inventory UI
        private const float InventoryPanelScale = 0.8f;
        private const int InventoryPanelWidth = 700;
        private const int InventoryPanelHeight = 1100;
        private const int InventoryPanelRightMargin = 10;

        private const int InventorySlotCount = 28;
        private const int InventorySlotSize = 100;
        private const int InventorySlotSpacing = 12;

        private const int NineSliceBorderSize = 74;
        private const int InventoryContentPadding = 28;
        private const int InventoryHeaderHeight = 64;

        // Stats UI
        private const float StatsPanelScale = 0.8f;
        private const int StatsPanelWidth = 700;
        private const int StatsPanelHeight = 1100;
        private const int StatsPanelLeft = 10;

        // Quick menu buttons
        private const int QuickMenuButtonSize = 64;
        private const int QuickMenuButtonSpacing = 10;
        private const int QuickMenuRightMargin = 25;
        private const int QuickMenuTopMargin = 25;
        private const int QuickMenuColumns = 7;

        private const int InventoryButtonIndex = 0;
        private const int StatsButtonIndex = 1;
        private const int ShopButtonIndex = 2;
        private const int MapButtonIndex = 3;
        private const int SkillTreeButtonIndex = 4;
        private const int SettingsButtonIndex = 5;
        private const int SoundVolumeButtonIndex = 6;


        // ===========================
        // Shop layout
        // ===========================
        // Main panel
        private const int ShopPanelWidth = 1120;
        private const int ShopPanelHeight = 900;
        private const int ShopPanelOffsetX = 0;
        private const int ShopPanelOffsetY = 0;

        // Header
        private const int ShopTitleTopOffset = 25;
        private const int ShopCoinsTopOffset = 30;

        // Items list
        private const int ShopContentSidePadding = 55;
        private const int ShopListTopOffset = 110;
        private const int ShopListBottomMargin = 65;

        private const int ShopItemHeight = 160;
        private const int ShopItemSpacing = 10;
        private const int ShopItemBorderSize = 3;

        // Icon 
        private const int ShopIconLeftOffset = ShopItemBorderSize;
        private const int ShopIconTopOffset = ShopItemBorderSize;
        private const int ShopIconTextureCrop = 12;

        private const int ShopIconSize = ShopItemHeight - ShopItemBorderSize * 2;
        private const int ShopIconBorderSize = 4;

        // Text 
        private const int ShopItemTextLeftMargin = 22;
        private const int ShopItemNameTopOffset = 25;
        private const int ShopItemDescriptionTopOffset = 80;

        // Buy 
        private const int ShopBuyCellWidth = 245;
        private const int ShopBuyCellMargin = 3;
        private const int ShopBuyMainTextTopOffset = 32;
        private const int ShopBuyPriceTextTopOffset = 88;

        // Scrollbar
        private const int ShopScrollbarWidth = 18;
        private const int ShopScrollbarGap = 10;
        private const int ShopScrollbarBorderSize = 3;
        private const int ShopScrollbarMinimumThumbHeight = 36;

        // Buy Message
        private const int ShopStatusBottomOffset = 55;

        private bool isShopOpen;
        private int selectedShopItemIndex;
        private string shopStatusMessage = string.Empty;
        private Color shopStatusColor = Color.White;
        private int shopFirstVisibleItem;
        private bool isShopScrollbarDragging;

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

        private bool HandleGameplayUIInput( KeyboardState keyboard, MouseState mouse)
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
            if (keyboard.IsKeyDown(Keys.C) &&
                previousKeyboard.IsKeyUp(Keys.C))
            {
                isStatsOpen = !isStatsOpen;
            }

            if (keyboard.IsKeyDown(Keys.Q) &&
                previousKeyboard.IsKeyUp(Keys.Q))
            {
                isQuestLogOpen = !isQuestLogOpen;
            }

            if (TryHandleQuickMenuClick(mouse))
                return true;

            if (TryHandleShopClick(mouse))
                return true;

            return false;
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
            DrawPlayerResourceUi();

            if (isInventoryOpen)
            {
                DrawInventoryPanel();
            }

            if (isStatsOpen)
            {
                DrawStatsPanel();
            }

            if (isQuestLogOpen)
            {
                DrawQuestLogPanel();
            }
            if (isShopOpen)
            {
                DrawShopPanel();
            }
            DrawQuickMenuButtons();
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
        private void DrawStatsPanel()
        {
            int panelWidth = ScaleUi(StatsPanelWidth, StatsPanelScale);
            int panelHeight = ScaleUi(StatsPanelHeight, StatsPanelScale);

            Rectangle panelBounds = new Rectangle(
                StatsPanelLeft,
                GraphicsDevice.Viewport.Height / 2 - panelHeight / 2,
                panelWidth,
                panelHeight);

            DrawNineSlicePanel(panel9SliceTexture, panelBounds);

            DrawCenteredPanelText(
                "STATS",
                panelBounds,
                30,
                Color.Gold);

            int rowY = panelBounds.Top + 115;

            DrawStatsSection("RESOURCES", panelBounds.Left + 70, rowY);
            rowY += 42;

            DrawStatsRow("Max Health", player.Health.MaxHealth.ToString(), panelBounds, rowY);
            rowY += 48;

            DrawStatsRow(
                "Health Regen",
                $"{player.Health.RegenPerSecond:0.##} / sec",
                panelBounds,
                rowY);
            rowY += 62;

            DrawStatsRow("Max Mana", $"{player.Mana.MaxMana:0}", panelBounds, rowY);
            rowY += 48;

            DrawStatsRow(
                "Mana Regen",
                $"{player.Mana.RegenPerSecond:0.##} / sec",
                panelBounds,
                rowY);
            rowY += 72;

            DrawStatsSection("COMBAT", panelBounds.Left + 70, rowY);
            rowY += 42;

            DrawStatsRow(
                "Damage",
                player.Stats.Damage.ToString(),
                panelBounds,
                rowY);
            rowY += 48;

            DrawStatsRow(
                "Knockback",
                $"{player.Stats.Knockback:0}",
                panelBounds,
                rowY);
            rowY += 48;

            DrawStatsRow(
                "Projectile Speed",
                $"{player.Stats.ProjectileSpeed:0}",
                panelBounds,
                rowY);
            rowY += 72;

            DrawStatsRow(
            "Projectile Count",
            player.Stats.ProjectileCount.ToString(),
            panelBounds,
            rowY);

            rowY += 72;

            DrawStatsSection("MOVEMENT", panelBounds.Left + 70, rowY);
            rowY += 42;

            DrawStatsRow(
                "Move Speed",
                $"{player.MoveSpeed:0}",
                panelBounds,
                rowY);
        }

        private void DrawStatsSection(string text, int x, int y)
        {
            _spriteBatch.DrawString(
                boldpixels,
                text,
                new Vector2(x, y),
                Color.Gold);
        }

        private void DrawStatsRow(
            string label,
            string value,
            Rectangle panelBounds,
            int y)
        {
            const int sidePadding = 70;

            Vector2 valueSize = boldpixels.MeasureString(value);

            _spriteBatch.DrawString(
                boldpixels,
                label,
                new Vector2(panelBounds.Left + sidePadding, y),
                Color.White);

            _spriteBatch.DrawString(
                boldpixels,
                value,
                new Vector2(
                    panelBounds.Right - sidePadding - valueSize.X,
                    y),
                Color.Gold);

            Rectangle separator = new Rectangle(
                panelBounds.Left + sidePadding,
                y + 34,
                panelBounds.Width - sidePadding * 2,
                2);

            _spriteBatch.Draw(
                pixelTexture,
                separator,
                Color.Black * 0.45f);
        }
        private void DrawInventoryPanel()
        {
            int panelWidth = ScaleUi(InventoryPanelWidth, InventoryPanelScale);
            int panelHeight = ScaleUi(InventoryPanelHeight, InventoryPanelScale);

            Rectangle panelBounds = new Rectangle(
                GraphicsDevice.Viewport.Width - panelWidth - InventoryPanelRightMargin,
                GraphicsDevice.Viewport.Height / 2 - panelHeight / 2,
                panelWidth,
                panelHeight); 
            
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
            DrawInventorySlots(panelBounds, InventoryPanelScale);
        }
        private void DrawInventorySlots(Rectangle panelBounds, float scale)
        {

            int borderSize = ScaleUi(NineSliceBorderSize, scale);
            int contentPadding = ScaleUi(InventoryContentPadding, scale);
            int headerHeight = ScaleUi(InventoryHeaderHeight, scale);
            int slotSize = ScaleUi(InventorySlotSize, scale);
            int slotSpacing = ScaleUi(InventorySlotSpacing, scale);
            int contentLeft = panelBounds.Left + NineSliceBorderSize + InventoryContentPadding;

            int contentTop =
                panelBounds.Top +
                borderSize +
                headerHeight;

            int contentWidth =
                panelBounds.Width -
                (NineSliceBorderSize + contentPadding) * 2;

            int contentHeight =
                panelBounds.Height -
                NineSliceBorderSize -
                headerHeight -
                contentPadding * 2;

            int columns = Math.Max(
                1,
                (contentWidth + slotSpacing) /
                (slotSize + InventorySlotSpacing));

            int rows = Math.Max(
                1,
                (contentHeight + slotSpacing) /
                (slotSize + slotSpacing));

            int slotsToDraw = Math.Min(
                InventorySlotCount,
                columns * rows);

            int gridWidth =
                columns * slotSize +
                (columns - 1) * slotSpacing;

            int startX =
                contentLeft +
                (contentWidth - gridWidth) / 2;

            for (int slotIndex = 0; slotIndex < slotsToDraw; slotIndex++)
            {
                int column = slotIndex % columns;
                int row = slotIndex / columns;

                int x = startX +
                        column * (slotSize + slotSpacing);

                int y = contentTop +
                        row * (slotSize + InventorySlotSpacing);

                Rectangle slotBounds = new Rectangle(
                    x,
                    y,
                    slotSize,
                    slotSize);

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
        private void DrawInventoryResource( InventoryResource resource,Rectangle slotBounds)
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

        private Rectangle GetQuickMenuButtonBounds(int index)
        {
            int column = index % QuickMenuColumns;
            int row = index / QuickMenuColumns;

            int gridWidth =
                QuickMenuColumns * QuickMenuButtonSize +
                (QuickMenuColumns - 1) * QuickMenuButtonSpacing;

            int startX =
                GraphicsDevice.Viewport.Width -
                QuickMenuRightMargin -
                gridWidth;

            return new Rectangle(
                startX + column *
                    (QuickMenuButtonSize + QuickMenuButtonSpacing),

                QuickMenuTopMargin + row *
                    (QuickMenuButtonSize + QuickMenuButtonSpacing),

                QuickMenuButtonSize,
                QuickMenuButtonSize);
        }

        private void DrawQuickMenuButtons()
        {
            DrawQuickMenuButton(
                inventoryButtonTexture,
                GetQuickMenuButtonBounds(InventoryButtonIndex),
                isEnabled: true);

            DrawQuickMenuButton(
                mapButtonTexture,
                GetQuickMenuButtonBounds(MapButtonIndex),
                isEnabled: false);

            DrawQuickMenuButton(
                statsButtonTexture,
                GetQuickMenuButtonBounds(StatsButtonIndex),
                isEnabled: true);

            DrawQuickMenuButton(
                shopButtonTexture,
                GetQuickMenuButtonBounds(ShopButtonIndex),
                isEnabled: true);

            DrawQuickMenuButton(
                skillTreeButtonTexture,
                GetQuickMenuButtonBounds(SkillTreeButtonIndex),
                isEnabled: false);

            DrawQuickMenuButton(
                soundVolumeButtonTexture,
                GetQuickMenuButtonBounds(SoundVolumeButtonIndex),
                isEnabled: true);

            DrawQuickMenuButton(
                settingsButtonTexture,
                GetQuickMenuButtonBounds(SettingsButtonIndex),
                isEnabled: false);
        }

        private void DrawQuickMenuButton(
            Texture2D texture,
            Rectangle bounds,
            bool isEnabled)
        {
            bool isHovered = bounds.Contains(Mouse.GetState().Position);

            Color tint = isEnabled
                ? Color.White
                : Color.White * 0.50f;

            _spriteBatch.Draw(
                texture,
                bounds,
                tint);

            if (isEnabled && isHovered)
            {
                _spriteBatch.Draw(
                    pixelTexture,
                    bounds,
                    Color.Gold * 0.18f);
            }
        }

        private bool TryHandleQuickMenuClick(MouseState mouse)
        {
            bool clickedLeftButton =
                mouse.LeftButton == ButtonState.Pressed &&
                previousMouseState.LeftButton == ButtonState.Released;

            if (!clickedLeftButton)
                return false;

            Point mousePosition = mouse.Position;

            if (GetQuickMenuButtonBounds(InventoryButtonIndex).Contains(mousePosition))
            {
                isInventoryOpen = !isInventoryOpen;
                return true;
            }

            if (GetQuickMenuButtonBounds(StatsButtonIndex).Contains(mousePosition))
            {
                isStatsOpen = !isStatsOpen;
                return true;
            }

            if (GetQuickMenuButtonBounds(SoundVolumeButtonIndex).Contains(mousePosition))
            {
                optionsReturnScene = GameScene.Playing;
                reopenPauseAfterOptions = false;

                StartSceneTransition(GameScene.Options);
                return true;
            }

            if (GetQuickMenuButtonBounds(ShopButtonIndex).Contains(mousePosition))
            {
                isShopOpen = !isShopOpen;
                shopStatusMessage = string.Empty;

                return true;
            }
            // за да не се изстрелва projectile зад тях.
            for (int i = 2; i <= 5; i++)
            {
                if (GetQuickMenuButtonBounds(i).Contains(mousePosition))
                    return true;
            }

            return false;
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
        private void AddInventoryResource( string resourceId, Texture2D icon,  int amount)
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
        private int GetInventoryResourceAmount(string resourceId)
        {
            foreach (InventoryResource resource in inventoryResources)
            {
                if (resource.Id == resourceId)
                    return resource.Amount;
            }

            return 0;
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

        private bool CloseOpenGameplayPanels()
        {
            bool hadOpenPanel =
                isInventoryOpen ||
                isStatsOpen ||
                isQuestLogOpen ||
                isShopOpen;

            isInventoryOpen = false;
            isStatsOpen = false;
            isQuestLogOpen = false;
            isShopOpen = false;

            return hadOpenPanel;
        }

        private Rectangle GetShopPanelBounds()
        {
            return new Rectangle(
                GraphicsDevice.Viewport.Width / 2
                    - ShopPanelWidth / 2
                    + ShopPanelOffsetX,

                GraphicsDevice.Viewport.Height / 2
                    - ShopPanelHeight / 2
                    + ShopPanelOffsetY,

                ShopPanelWidth,
                ShopPanelHeight);
        }
        private Rectangle GetShopListBounds()
        {
            Rectangle panelBounds = GetShopPanelBounds();

            int listLeft = panelBounds.Left + ShopContentSidePadding;
            int listTop = panelBounds.Top + ShopListTopOffset;

            int listRight = panelBounds.Right - ShopContentSidePadding - ShopScrollbarWidth - ShopScrollbarGap;

            int listBottom = panelBounds.Bottom - ShopListBottomMargin;
            return new Rectangle(
                listLeft,
                listTop,
                listRight - listLeft,
                listBottom - listTop);
        }

        private Rectangle GetShopVisibleItemBounds(int visibleIndex)
        {
            Rectangle listBounds = GetShopListBounds();

            return new Rectangle(
                listBounds.Left,
                listBounds.Top +
                    visibleIndex * (ShopItemHeight + ShopItemSpacing),
                listBounds.Width,
                ShopItemHeight);
        }

        private Rectangle GetShopScrollbarTrackBounds()
        {
            Rectangle panelBounds = GetShopPanelBounds();
            Rectangle listBounds = GetShopListBounds();

            return new Rectangle(
                panelBounds.Right -
                ShopContentSidePadding -
                ShopScrollbarWidth,

                listBounds.Top,
                ShopScrollbarWidth,
                listBounds.Height);
        }
        private int GetShopVisibleRowCount()
        {
            Rectangle listBounds = GetShopListBounds();

            return Math.Max(
                1,
                (listBounds.Height + ShopItemSpacing) /
                (ShopItemHeight + ShopItemSpacing));
        }

        private int GetShopMaxFirstVisibleItem()
        {
            return Math.Max(0,shopItems.Count - GetShopVisibleRowCount());
        }

        private Rectangle GetShopScrollbarThumbBounds()
        {
            Rectangle trackBounds = GetShopScrollbarTrackBounds();

            int visibleRows = GetShopVisibleRowCount();

            int thumbHeight = Math.Max(ShopScrollbarMinimumThumbHeight, (int)(trackBounds.Height * visibleRows / (float)Math.Max(visibleRows, shopItems.Count)));

            int maxFirstVisibleItem = GetShopMaxFirstVisibleItem();
            int travelDistance = trackBounds.Height - thumbHeight;

            int thumbY = trackBounds.Top;

            if (maxFirstVisibleItem > 0)
            {
                thumbY += (int)(
                    travelDistance *
                    (shopFirstVisibleItem /
                    (float)maxFirstVisibleItem));
            }

            return new Rectangle(
                trackBounds.X,
                thumbY,
                trackBounds.Width,
                thumbHeight);
        }
        private Rectangle GetShopBuyCellBounds(Rectangle itemBounds)
        {
            return new Rectangle(
                itemBounds.Right - ShopBuyCellWidth,
                itemBounds.Top + ShopBuyCellMargin,
                ShopBuyCellWidth - ShopBuyCellMargin,
                itemBounds.Height - ShopBuyCellMargin * 2);
        }
        private void DrawShopPanel()
        {
            Rectangle panelBounds = GetShopPanelBounds();

            DrawNineSlicePanel(panel9SliceTexture, panelBounds);

            DrawCenteredPanelText(
                "SHOP",
                panelBounds, ShopTitleTopOffset, Color.Gold);

            string coinText =
                $"Coins: {GetInventoryResourceAmount("coin")}";

            Vector2 coinTextSize = boldpixels.MeasureString(coinText);

            _spriteBatch.DrawString(
                boldpixels,
                coinText,
                new Vector2(
                    panelBounds.Right -
                    ShopContentSidePadding -
                    coinTextSize.X,

                    panelBounds.Top +
                    ShopCoinsTopOffset),
                Color.Gold);

            if (shopItems.Count == 0)
                return;

            selectedShopItemIndex = Math.Clamp(
                selectedShopItemIndex,
                0,
                shopItems.Count - 1);

            int visibleRows = GetShopVisibleRowCount();

            shopFirstVisibleItem = Math.Clamp(
                shopFirstVisibleItem,
                0,
                GetShopMaxFirstVisibleItem());

            for (int visibleIndex = 0;
                 visibleIndex < visibleRows;
                 visibleIndex++)
            {
                int itemIndex =
                    shopFirstVisibleItem + visibleIndex;

                if (itemIndex >= shopItems.Count)
                    break;

                DrawShopItem(
                    shopItems[itemIndex],
                    GetShopVisibleItemBounds(visibleIndex),
                    itemIndex == selectedShopItemIndex);
            }

            DrawShopScrollbar();
            if (!string.IsNullOrEmpty(shopStatusMessage))
            {
                DrawCenteredPanelText(
                    shopStatusMessage,
                    panelBounds,
                    ShopPanelHeight - ShopStatusBottomOffset, shopStatusColor);
            }
        }
        private void DrawShopScrollbar()
        {
            if (GetShopMaxFirstVisibleItem() <= 0)
                return;

            Rectangle trackBounds = GetShopScrollbarTrackBounds();
            Rectangle thumbBounds = GetShopScrollbarThumbBounds();

            _spriteBatch.Draw(
                pixelTexture,
                trackBounds,
                Color.Black);

            Rectangle trackInnerBounds = new Rectangle(
                trackBounds.X + 3,
                trackBounds.Y + 3,
                trackBounds.Width - 6,
                trackBounds.Height - 6);

            _spriteBatch.Draw(
                pixelTexture,
                trackInnerBounds,
                Color.DimGray);

            _spriteBatch.Draw(
                pixelTexture,
                thumbBounds,
                Color.Black);

            Rectangle thumbInnerBounds = new Rectangle(
                thumbBounds.X + 3,
                thumbBounds.Y + 3,
                thumbBounds.Width - 6,
                thumbBounds.Height - 6);

            _spriteBatch.Draw(
                pixelTexture,
                thumbInnerBounds,
                Color.White);
        }

        private void DrawShopItem(
            ShopItem item,
            Rectangle bounds,
            bool isSelected)
        {
            Color outerColor = isSelected
                ? Color.Gold
                : Color.Black;

            _spriteBatch.Draw(pixelTexture, bounds, outerColor);

            Rectangle innerBounds = new Rectangle(
                bounds.X + ShopItemBorderSize,
                bounds.Y + ShopItemBorderSize,
                bounds.Width - ShopItemBorderSize * 2,
                bounds.Height - ShopItemBorderSize * 2);

            _spriteBatch.Draw(
                pixelTexture,
                innerBounds,
                item.IsSoldOut
                    ? Color.Black * 0.60f
                    : Color.DarkSlateGray * 0.80f);

            // Празна рамка за бъдещата item/upgrade икона
            Rectangle iconBounds = new Rectangle(
                bounds.Left + ShopIconLeftOffset,
                bounds.Top + ShopIconTopOffset,
                ShopIconSize,
                ShopIconSize);
            _spriteBatch.Draw(pixelTexture, iconBounds, Color.Black);

            Rectangle iconInnerBounds = new Rectangle(
                iconBounds.X + ShopIconBorderSize,
                iconBounds.Y + ShopIconBorderSize,
                iconBounds.Width - ShopIconBorderSize * 2,
                iconBounds.Height - ShopIconBorderSize * 2);

            //icon image
            Rectangle iconSourceBounds = new Rectangle(
                ShopIconTextureCrop,
                ShopIconTextureCrop,
                item.Icon.Width - ShopIconTextureCrop * 2,
                item.Icon.Height - ShopIconTextureCrop * 2);

            _spriteBatch.Draw(
                item.Icon,
                iconInnerBounds,
                iconSourceBounds,
                Color.White);
            Rectangle buyBounds = GetShopBuyCellBounds(bounds);

            Color textColor = item.IsSoldOut
                ? Color.Gray
                : Color.White;

            _spriteBatch.DrawString(
                boldpixels,
                item.Name,
                new Vector2( iconBounds.Right + ShopItemTextLeftMargin, bounds.Top + ShopItemNameTopOffset),textColor);

            _spriteBatch.DrawString(
                boldpixels,
                item.Description,
                new Vector2( iconBounds.Right + ShopItemTextLeftMargin,bounds.Top + ShopItemDescriptionTopOffset),
                item.IsSoldOut
                    ? Color.DarkGray
                    : Color.LightGray);

            DrawShopBuyCell(item, buyBounds);
        }

        private void DrawShopBuyCell(
            ShopItem item,
            Rectangle bounds)
        {
            bool canBuy =
                !item.IsSoldOut &&
                GetInventoryResourceAmount("coin") >= item.Price;

            bool isHovered = bounds.Contains(Mouse.GetState().Position);

            Color backgroundColor;

            if (item.IsSoldOut)
            {
                backgroundColor = Color.Black * 0.65f;
            }
            else if (canBuy)
            {
                backgroundColor = isHovered
                    ? Color.ForestGreen
                    : Color.DarkGreen;
            }
            else
            {
                backgroundColor = Color.DarkRed;
            }

            _spriteBatch.Draw(pixelTexture, bounds, Color.Black);

            Rectangle innerBounds = new Rectangle(
                bounds.X + 3,
                bounds.Y + 3,
                bounds.Width - 6,
                bounds.Height - 6);

            _spriteBatch.Draw(
                pixelTexture,
                innerBounds,
                backgroundColor);

            string mainText = item.IsSoldOut
                ? "SOLD OUT"
                : "BUY";

            string priceText = item.IsSoldOut
                ? string.Empty
                : $"{item.Price} coins";

            Vector2 mainTextSize =
                boldpixels.MeasureString(mainText);

            _spriteBatch.DrawString(
                boldpixels,
                mainText,
                new Vector2(
                    bounds.Center.X - mainTextSize.X / 2f,
                    bounds.Top + ShopBuyMainTextTopOffset),
                item.IsSoldOut ? Color.Gray : Color.White);

            if (!string.IsNullOrEmpty(priceText))
            {
                Vector2 priceTextSize =
                    boldpixels.MeasureString(priceText);

                _spriteBatch.DrawString(
                    boldpixels,
                    priceText,
                    new Vector2(
                        bounds.Center.X - priceTextSize.X / 2f,
                        bounds.Top + ShopBuyPriceTextTopOffset),
                    Color.Gold);
            }
        }

        private bool TryHandleShopClick(MouseState mouse)
        {
            if (!isShopOpen)
                return false;

            if (HandleShopScrollbarInput(mouse))
                return true;

            bool clickedLeftButton =
                mouse.LeftButton == ButtonState.Pressed &&
                previousMouseState.LeftButton == ButtonState.Released;

            if (!clickedLeftButton)
                return false;

            int visibleRows = GetShopVisibleRowCount();

            for (int visibleIndex = 0;
                 visibleIndex < visibleRows;
                 visibleIndex++)
            {
                int itemIndex =
                    shopFirstVisibleItem + visibleIndex;

                if (itemIndex >= shopItems.Count)
                    break;

                Rectangle itemBounds =
                    GetShopVisibleItemBounds(visibleIndex);

                if (!itemBounds.Contains(mouse.Position))
                    continue;

                selectedShopItemIndex = itemIndex;

                Rectangle buyBounds =
                    GetShopBuyCellBounds(itemBounds);

                if (!buyBounds.Contains(mouse.Position))
                {
                    shopStatusMessage = string.Empty;
                    return true;
                }

                ShopItem selectedItem = shopItems[itemIndex];

                ShopPurchaseResult result =
                    TryPurchaseShopItem(selectedItem);

                switch (result)
                {
                    case ShopPurchaseResult.Success:
                        shopStatusMessage =
                            $"{selectedItem.Name} purchased!";
                        shopStatusColor = Color.LimeGreen;
                        break;

                    case ShopPurchaseResult.NotEnoughCoins:
                        shopStatusMessage = "Not enough coins.";
                        shopStatusColor = Color.IndianRed;
                        break;

                    case ShopPurchaseResult.SoldOut:
                        shopStatusMessage = "This item is sold out.";
                        shopStatusColor = Color.Gray;
                        break;
                }

                return true;
            }

            return false;
        }
        private bool HandleShopScrollbarInput(MouseState mouse)
        {
            int maxFirstVisibleItem =
                GetShopMaxFirstVisibleItem();

            if (maxFirstVisibleItem <= 0)
            {
                isShopScrollbarDragging = false;
                return false;
            }

            Rectangle listBounds = GetShopListBounds();
            Rectangle trackBounds = GetShopScrollbarTrackBounds();

            int wheelDelta =
                mouse.ScrollWheelValue -
                previousMouseState.ScrollWheelValue;

            if (wheelDelta != 0 &&
                listBounds.Contains(mouse.Position))
            {
                shopFirstVisibleItem = Math.Clamp(
                    shopFirstVisibleItem - Math.Sign(wheelDelta),
                    0,
                    maxFirstVisibleItem);

                return true;
            }

            if (mouse.LeftButton == ButtonState.Released)
            {
                isShopScrollbarDragging = false;
                return false;
            }

            bool clickedLeftButton =
                mouse.LeftButton == ButtonState.Pressed &&
                previousMouseState.LeftButton == ButtonState.Released;

            if (clickedLeftButton &&
                trackBounds.Contains(mouse.Position))
            {
                isShopScrollbarDragging = true;
            }

            if (!isShopScrollbarDragging)
                return false;

            Rectangle thumbBounds = GetShopScrollbarThumbBounds();

            float availableTravel =
                trackBounds.Height - thumbBounds.Height;

            float percent = availableTravel <= 0f
                ? 0f
                : (mouse.Y - trackBounds.Top -
                   thumbBounds.Height / 2f) / availableTravel;

            percent = MathHelper.Clamp(percent, 0f, 1f);

            shopFirstVisibleItem = (int)MathF.Round(
                percent * maxFirstVisibleItem);

            return true;
        }

    }
}