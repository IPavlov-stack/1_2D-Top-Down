using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using static _1_2D_Top_Down.ShopItem;

namespace _1_2D_Top_Down
{
    public partial class Game1
    {
        private readonly List<ShopItem> shopItems = new();

        private void InitializeShopItems()
        {
            shopItems.Clear();

            shopItems.Add(new ShopItem(
                id: "vitality_training",
                name: "Vitality Training",
                description: "+20 Max Health",
                icon: shopUpgradeIcons["health"],
                price: 5,
                stat: PlayerStatType.MaxHealth,
                statAmount: 20,
                maxPurchases: 10));

            shopItems.Add(new ShopItem(
                id: "mana_training",
                name: "Mana Training",
                description: "+20 Max Mana",
                icon: shopUpgradeIcons["mana"],
                price: 5,
                stat: PlayerStatType.MaxMana,
                statAmount: 20,
                maxPurchases: 10));

            shopItems.Add(new ShopItem(
                id: "strength_training",
                name: "Strength Training",
                description: "+1 Damage",
                icon: shopUpgradeIcons["damage"],
                price: 10,
                stat: PlayerStatType.Damage,
                statAmount: 1,
                maxPurchases: 10));

            shopItems.Add(new ShopItem(
                id: "forceful_magic",
                name: "Forceful Magic",
                description: "+50 Knockback",
                icon: shopUpgradeIcons["knockback"],
                price: 8,
                stat: PlayerStatType.Knockback,
                statAmount: 50,
                maxPurchases: 10));

            shopItems.Add(new ShopItem(
                id: "swift_boots",
                name: "Swift Boots",
                description: "+25 Move Speed",
                icon: shopUpgradeIcons["movement-speed"],
                price: 12,
                stat: PlayerStatType.MoveSpeed,
                statAmount: 25,
                maxPurchases: 5));

            shopItems.Add(new ShopItem(
                id: "arcane_acceleration",
                name: "Arcane Acceleration",
                description: "+75 Projectile Speed",
                icon: shopUpgradeIcons["projectile-speed"],
                price: 10,
                stat: PlayerStatType.ProjectileSpeed,
                statAmount: 75,
                maxPurchases: 5));

            shopItems.Add(new ShopItem(
                id: "multishot",
                name: "Multishot",
                description: "Fire 3 projectiles in a spread.",
                icon: shopUpgradeIcons["multishot"],
                price: 30,
                stat: PlayerStatType.ProjectileCount,
                statAmount: 2,
                maxPurchases: 1));

        }
        private ShopPurchaseResult TryPurchaseShopItem(ShopItem item)
        {
            if (item.IsSoldOut)
                return ShopPurchaseResult.SoldOut;

            bool coinsSpent = TrySpendInventoryResource(
                "coin",
                item.Price);

            if (!coinsSpent)
                return ShopPurchaseResult.NotEnoughCoins;

            player.AddStatBonus(
                item.Stat,
                item.StatAmount);

            item.RegisterPurchase();

            return ShopPurchaseResult.Success;
        }
        private void LoadShopUpgradeIcons()
        {
            shopUpgradeIcons.Clear();

            shopUpgradeIcons["health"] =
                Content.Load<Texture2D>("UI/Shop Upgrades/health");

            shopUpgradeIcons["mana"] =
                Content.Load<Texture2D>("UI/Shop Upgrades/mana");

            shopUpgradeIcons["damage"] =
                Content.Load<Texture2D>("UI/Shop Upgrades/damage");

            shopUpgradeIcons["movement-speed"] =
                Content.Load<Texture2D>("UI/Shop Upgrades/movement-speed");

            shopUpgradeIcons["critical-chance"] =
                Content.Load<Texture2D>("UI/Shop Upgrades/critical-chance");

            shopUpgradeIcons["health-regeneration"] =
                Content.Load<Texture2D>("UI/Shop Upgrades/health-regeneration");

            shopUpgradeIcons["mana-regeneration"] =
                Content.Load<Texture2D>("UI/Shop Upgrades/mana-regeneration");

            shopUpgradeIcons["projectile-speed"] =
                Content.Load<Texture2D>("UI/Shop Upgrades/projectile-speed");

            shopUpgradeIcons["multishot"] =
                Content.Load<Texture2D>("UI/Shop Upgrades/multishot");

            shopUpgradeIcons["armor"] =
                Content.Load<Texture2D>("UI/Shop Upgrades/armor");

            shopUpgradeIcons["lifesteal"] =
                Content.Load<Texture2D>("UI/Shop Upgrades/lifesteal");

            shopUpgradeIcons["pickup-radius"] =
                Content.Load<Texture2D>("UI/Shop Upgrades/pickup-radius");

            shopUpgradeIcons["cooldown-reduction"] =
                Content.Load<Texture2D>("UI/Shop Upgrades/cooldown-reduction");

            shopUpgradeIcons["knockback"] =
                Content.Load<Texture2D>("UI/Shop Upgrades/knockback");

            shopUpgradeIcons["gold-find"] =
                Content.Load<Texture2D>("UI/Shop Upgrades/gold-find");

            shopUpgradeIcons["experience-gain"] =
                Content.Load<Texture2D>("UI/Shop Upgrades/experience-gain");
        }
    }
}