using System.Collections.Generic;

namespace _1_2D_Top_Down
{
    public static class ShopDefinitions
    {
        public static IReadOnlyList<ShopItemDefinition> All { get; } =
            new ShopItemDefinition[]
            {
                Stat("vitality_training", "Vitality Training", "+20 Max Health",
                    "health", 5, PlayerStatType.MaxHealth, 20, 10),
                Stat("mana_training", "Mana Training", "+20 Max Mana",
                    "mana", 5, PlayerStatType.MaxMana, 20, 10),
                Stat("strength_training", "Strength Training", "+1 Damage",
                    "damage", 10, PlayerStatType.Damage, 1, 10),
                Stat("forceful_magic", "Forceful Magic", "+50 Knockback",
                    "knockback", 8, PlayerStatType.Knockback, 50, 10),
                Stat("swift_boots", "Swift Boots", "+25 Move Speed",
                    "movement-speed", 12, PlayerStatType.MoveSpeed, 25, 5),
                Stat("arcane_acceleration", "Arcane Acceleration",
                    "+75 Projectile Speed", "projectile-speed", 10,
                    PlayerStatType.ProjectileSpeed, 75, 5),
                Stat("multishot", "Multishot", "Fire 3 projectiles in a spread.",
                    "multishot", 30, PlayerStatType.ProjectileCount, 2, 1)
            };

        private static ShopItemDefinition Stat(
            string id,
            string name,
            string description,
            string iconId,
            int price,
            PlayerStatType stat,
            float amount,
            int maxPurchases)
        {
            return new ShopItemDefinition(
                id,
                name,
                description,
                iconId,
                price,
                new PlayerStatShopEffect(stat, amount),
                maxPurchases);
        }
    }
}
