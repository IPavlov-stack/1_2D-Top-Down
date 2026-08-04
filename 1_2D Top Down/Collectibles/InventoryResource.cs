using Microsoft.Xna.Framework.Graphics;

namespace _1_2D_Top_Down
{
    public sealed class InventoryResource
    {
        public string Id { get; }
        public Texture2D Icon { get; }
        public int Amount { get; private set; }

        public InventoryResource(string id, Texture2D icon, int amount)
        {
            Id = id;
            Icon = icon;
            Amount = amount;
        }

        public void Add(int amount)
        {
            Amount += amount;
        }

        public bool TryRemove(int amount)
        {
            if (amount <= 0 || Amount < amount)
                return false;

            Amount -= amount;
            return true;
        }
    }
}