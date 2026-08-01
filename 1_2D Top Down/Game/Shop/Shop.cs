namespace _1_2D_Top_Down
{
    public class Shop
    {
        public string Name { get; }
        public ShopCategory[] Categories { get; }

        public Shop(string name, params ShopCategory[] categories)
        {
            Name = name;
            Categories = categories;
        }
    }
}