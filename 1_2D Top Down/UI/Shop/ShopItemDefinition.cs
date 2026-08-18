namespace _1_2D_Top_Down
{
    public sealed class ShopItemDefinition
    {
        public string Id { get; }
        public string Name { get; }
        public string Description { get; }
        public string IconId { get; }
        public int Price { get; }
        public int MaxPurchases { get; }
        public IShopEffect Effect { get; }

        public ShopItemDefinition(
            string id,
            string name,
            string description,
            string iconId,
            int price,
            IShopEffect effect,
            int maxPurchases = -1)
        {
            Id = id;
            Name = name;
            Description = description;
            IconId = iconId;
            Price = price;
            Effect = effect;
            MaxPurchases = maxPurchases;
        }
    }
}
