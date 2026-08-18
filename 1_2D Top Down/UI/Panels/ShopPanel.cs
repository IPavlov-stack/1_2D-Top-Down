using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using static _1_2D_Top_Down.ShopItem;

namespace _1_2D_Top_Down
{
    public sealed class ShopPanel : IGameplayPanel
    {
        private const int Width = 1120, Height = 900, SidePadding = 55;
        private const int ListTop = 110, ListBottom = 65, RowHeight = 160, RowSpacing = 10;
        private const int ScrollWidth = 18, ScrollGap = 10, BuyWidth = 245;
        private readonly Func<Rectangle> viewport;
        private readonly IReadOnlyList<ShopItem> items;
        private readonly Func<int> coins;
        private readonly Func<ShopItem, ShopPurchaseResult> purchase;
        private readonly Texture2D panelTexture, pixelTexture;
        private readonly SpriteFont font;
        private int selectedIndex;
        private readonly ScrollableList scrollableList = new();
        private string status = string.Empty;
        private Color statusColor = Color.White;

        public ShopPanel(Func<Rectangle> viewport, IReadOnlyList<ShopItem> items,
            Func<int> coins, Func<ShopItem, ShopPurchaseResult> purchase,
            Texture2D panelTexture, Texture2D pixelTexture, SpriteFont font)
        {
            this.viewport = viewport;
            this.items = items;
            this.coins = coins;
            this.purchase = purchase;
            this.panelTexture = panelTexture;
            this.pixelTexture = pixelTexture;
            this.font = font;
        }

        public string Id => GameplayPanelIds.Shop;
        public bool BlocksGameplayInput => true;
        public void Open() => status = string.Empty;
        public void Close() => scrollableList.Reset();

        public bool HandleInput(GameplayUiInput input)
        {
            if (HandleScroll(input.Mouse, input.PreviousMouse))
                return true;
            bool clicked = input.Mouse.LeftButton == ButtonState.Pressed &&
                input.PreviousMouse.LeftButton == ButtonState.Released;
            if (!clicked)
                return false;

            for (int row = 0; row < VisibleRows; row++)
            {
                int index = scrollableList.FirstVisibleIndex + row;
                if (index >= items.Count || !ItemBounds(row).Contains(input.Mouse.Position))
                    continue;
                selectedIndex = index;
                if (!BuyBounds(ItemBounds(row)).Contains(input.Mouse.Position))
                {
                    status = string.Empty;
                    return true;
                }
                SetResult(items[index], purchase(items[index]));
                return true;
            }
            return false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle bounds = PanelBounds;
            UiDrawing.DrawNineSlicePanel(spriteBatch, panelTexture, bounds);
            PanelTitle.Draw(spriteBatch, font, "SHOP", bounds, 25, Color.Gold);
            string coinText = $"Coins: {coins()}";
            Vector2 coinSize = font.MeasureString(coinText);
            spriteBatch.DrawString(font, coinText,
                new Vector2(bounds.Right - SidePadding - coinSize.X, bounds.Top + 30), Color.Gold);

            if (items.Count == 0)
                return;
            selectedIndex = Math.Clamp(selectedIndex, 0, items.Count - 1);
            scrollableList.Clamp(items.Count, VisibleRows);
            for (int row = 0; row < VisibleRows; row++)
            {
                int index = scrollableList.FirstVisibleIndex + row;
                if (index >= items.Count) break;
                DrawItem(spriteBatch, items[index], ItemBounds(row), index == selectedIndex);
            }
            DrawScrollbar(spriteBatch);
            if (!string.IsNullOrEmpty(status))
                DrawCentered(spriteBatch, status, bounds.Bottom - 55, statusColor);
        }

        private Rectangle PanelBounds
        {
            get
            {
                Rectangle view = viewport();
                return new Rectangle(view.Center.X - Width / 2, view.Center.Y - Height / 2, Width, Height);
            }
        }
        private Rectangle ListBounds => new(PanelBounds.Left + SidePadding, PanelBounds.Top + ListTop,
            Width - SidePadding * 2 - ScrollWidth - ScrollGap, Height - ListTop - ListBottom);
        private int VisibleRows => scrollableList.GetVisibleCount(ListBounds, RowHeight, RowSpacing);
        private int MaxFirstVisible => scrollableList.GetMaxFirstVisible(items.Count, VisibleRows);
        private Rectangle ItemBounds(int row) => new(ListBounds.Left,
            ListBounds.Top + row * (RowHeight + RowSpacing), ListBounds.Width, RowHeight);
        private Rectangle TrackBounds => new(PanelBounds.Right - SidePadding - ScrollWidth,
            ListBounds.Top, ScrollWidth, ListBounds.Height);
        private static Rectangle BuyBounds(Rectangle item) =>
            new(item.Right - BuyWidth, item.Top + 3, BuyWidth - 3, item.Height - 6);

        private Rectangle ThumbBounds
        {
            get
            {
                int height = Math.Max(36, (int)(TrackBounds.Height * VisibleRows /
                    (float)Math.Max(VisibleRows, items.Count)));
                int y = TrackBounds.Top;
                if (MaxFirstVisible > 0)
                    y += (int)((TrackBounds.Height - height) *
                        (scrollableList.FirstVisibleIndex / (float)MaxFirstVisible));
                return new Rectangle(TrackBounds.X, y, TrackBounds.Width, height);
            }
        }

        private bool HandleScroll(MouseState mouse, MouseState previous)
        {
            return scrollableList.HandleInput(
                mouse,
                previous,
                ListBounds,
                TrackBounds,
                ThumbBounds,
                items.Count,
                VisibleRows);
        }

        private void DrawItem(SpriteBatch batch, ShopItem item, Rectangle bounds, bool selected)
        {
            batch.Draw(pixelTexture, bounds, selected ? Color.Gold : Color.Black);
            Rectangle inner = new(bounds.X + 3, bounds.Y + 3, bounds.Width - 6, bounds.Height - 6);
            batch.Draw(pixelTexture, inner, item.IsSoldOut ? Color.Black * .6f : Color.DarkSlateGray * .8f);
            Rectangle icon = new(bounds.Left + 3, bounds.Top + 3, 154, 154);
            batch.Draw(pixelTexture, icon, Color.Black);
            Rectangle iconInner = new(icon.X + 4, icon.Y + 4, icon.Width - 8, icon.Height - 8);
            Rectangle source = new(12, 12, item.Icon.Width - 24, item.Icon.Height - 24);
            batch.Draw(item.Icon, iconInner, source, Color.White);
            Color text = item.IsSoldOut ? Color.Gray : Color.White;
            batch.DrawString(font, item.Name, new Vector2(icon.Right + 22, bounds.Top + 25), text);
            batch.DrawString(font, item.Description, new Vector2(icon.Right + 22, bounds.Top + 80),
                item.IsSoldOut ? Color.DarkGray : Color.LightGray);
            DrawBuyCell(batch, item, BuyBounds(bounds));
        }

        private void DrawBuyCell(SpriteBatch batch, ShopItem item, Rectangle bounds)
        {
            bool canBuy = !item.IsSoldOut && coins() >= item.Price;
            Color background = item.IsSoldOut ? Color.Black * .65f :
                canBuy ? (bounds.Contains(Mouse.GetState().Position) ? Color.ForestGreen : Color.DarkGreen) : Color.DarkRed;
            batch.Draw(pixelTexture, bounds, Color.Black);
            batch.Draw(pixelTexture, new Rectangle(bounds.X + 3, bounds.Y + 3, bounds.Width - 6, bounds.Height - 6), background);
            string main = item.IsSoldOut ? "SOLD OUT" : "BUY";
            DrawCentered(batch, main, bounds.Top + 32, item.IsSoldOut ? Color.Gray : Color.White, bounds);
            if (!item.IsSoldOut)
                DrawCentered(batch, $"{item.Price} coins", bounds.Top + 88, Color.Gold, bounds);
        }

        private void DrawScrollbar(SpriteBatch batch)
        {
            if (MaxFirstVisible <= 0) return;
            batch.Draw(pixelTexture, TrackBounds, Color.Black);
            batch.Draw(pixelTexture, new Rectangle(TrackBounds.X + 3, TrackBounds.Y + 3,
                TrackBounds.Width - 6, TrackBounds.Height - 6), Color.DimGray);
            batch.Draw(pixelTexture, ThumbBounds, Color.Black);
            batch.Draw(pixelTexture, new Rectangle(ThumbBounds.X + 3, ThumbBounds.Y + 3,
                ThumbBounds.Width - 6, ThumbBounds.Height - 6), Color.White);
        }

        private void SetResult(ShopItem item, ShopPurchaseResult result)
        {
            (status, statusColor) = result switch
            {
                ShopPurchaseResult.Success => ($"{item.Name} purchased!", Color.LimeGreen),
                ShopPurchaseResult.NotEnoughCoins => ("Not enough coins.", Color.IndianRed),
                _ => ("This item is sold out.", Color.Gray)
            };
        }

        private void DrawCentered(SpriteBatch batch, string text, int y, Color color, Rectangle? area = null)
        {
            Rectangle bounds = area ?? PanelBounds;
            Vector2 size = font.MeasureString(text);
            batch.DrawString(font, text, new Vector2(bounds.Center.X - size.X / 2f, y), color);
        }
    }
}
