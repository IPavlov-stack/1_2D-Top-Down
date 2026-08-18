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

            foreach (ShopItemDefinition definition in ShopDefinitions.All)
            {
                shopItems.Add(
                    new ShopItem(
                        definition,
                        shopUpgradeIcons[definition.IconId]));
            }

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

            item.Definition.Effect.Apply(player);

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
