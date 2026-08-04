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
        public Texture2D Icon { get; }
        public string Id { get; }
        public string Name { get; }
        public string Description { get; }
        public int Price { get; }

        public PlayerStatType Stat { get; }
        public float StatAmount { get; }

        // -1 означава неограничен брой покупки.
        public int MaxPurchases { get; }
        public int PurchasedCount { get; private set; }

        public bool IsSoldOut =>
            MaxPurchases >= 0 &&
            PurchasedCount >= MaxPurchases;

        public ShopItem(
            string id,
            string name,
            string description,
            Texture2D icon,
            int price,
            PlayerStatType stat,
            float statAmount,
            int maxPurchases = -1)
        {
            Id = id;
            Name = name;
            Description = description;
            Icon = icon;
            Price = price;
            Stat = stat;
            StatAmount = statAmount;
            MaxPurchases = maxPurchases;
        }

        public void RegisterPurchase()
        {
            if (!IsSoldOut)
                PurchasedCount++;
        }
    }
}