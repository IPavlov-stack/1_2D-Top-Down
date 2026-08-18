namespace _1_2D_Top_Down
{
    public interface IShopEffect
    {
        void Apply(Player player);
    }

    public sealed class PlayerStatShopEffect : IShopEffect
    {
        public PlayerStatType Stat { get; }
        public float Amount { get; }

        public PlayerStatShopEffect(PlayerStatType stat, float amount)
        {
            Stat = stat;
            Amount = amount;
        }

        public void Apply(Player player)
        {
            player.AddStatBonus(Stat, Amount);
        }
    }
}
