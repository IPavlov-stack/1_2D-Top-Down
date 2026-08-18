using Microsoft.Xna.Framework.Graphics;

namespace _1_2D_Top_Down
{
    public sealed class ShopItem
    {
        public enum ShopPurchaseResult
        {
            Success,
            NotEnoughCoins,
            SoldOut
        }
        public ShopItemDefinition Definition { get; }
        public Texture2D Icon { get; }
        public string Id => Definition.Id;
        public string Name => Definition.Name;
        public string Description => Definition.Description;
        public int Price => Definition.Price;

        // -1 означава неограничен брой покупки.
        public int MaxPurchases => Definition.MaxPurchases;
        public int PurchasedCount { get; private set; }

        public bool IsSoldOut =>
            MaxPurchases >= 0 &&
            PurchasedCount >= MaxPurchases;

        public ShopItem(
            ShopItemDefinition definition,
            Texture2D icon)
        {
            Definition = definition;
            Icon = icon;
        }

        public void RegisterPurchase()
        {
            if (!IsSoldOut)
                PurchasedCount++;
        }
    }
}
