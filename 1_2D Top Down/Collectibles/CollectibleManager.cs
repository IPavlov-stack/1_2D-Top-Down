using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace _1_2D_Top_Down
{
    /// <summary>
    /// Owns collectible objects for the active mission and handles their
    /// shared lifetime and player-contact rules.
    /// </summary>
    public sealed class CollectibleManager
    {
        private readonly List<Coin> coins = new();
        private readonly List<ManaCrystal> manaCrystals = new();

        public IReadOnlyList<Coin> Coins => coins;
        public IReadOnlyList<ManaCrystal> ManaCrystals => manaCrystals;

        public void Clear()
        {
            coins.Clear();
            manaCrystals.Clear();
        }

        public void Add(Coin coin)
        {
            coins.Add(coin);
        }

        public void Add(ManaCrystal manaCrystal)
        {
            manaCrystals.Add(manaCrystal);
        }

        public void Update(
            GameTime gameTime,
            Rectangle playerBounds,
            bool playerCanReceiveMana,
            Action<Coin> onCoinCollected,
            Action<ManaCrystal> onManaCrystalCollected)
        {
            UpdateCoins(gameTime, playerBounds, onCoinCollected);
            UpdateManaCrystals(gameTime, playerBounds,playerCanReceiveMana,onManaCrystalCollected);
        }

        private void UpdateCoins(GameTime gameTime, Rectangle playerBounds, Action<Coin> onCoinCollected)
        {
            for (int i = coins.Count - 1; i >= 0; i--)
            {
                Coin coin = coins[i];
                coin.Update(gameTime);

                if (coin.IsExpired)
                {
                    coins.RemoveAt(i);
                    continue;
                }

                if (playerBounds.Intersects(coin.Bounds))
                {
                    coins.RemoveAt(i);
                    onCoinCollected(coin);
                }
            }
        }

        private void UpdateManaCrystals(
            GameTime gameTime,
            Rectangle playerBounds,
            bool playerCanReceiveMana,
            Action<ManaCrystal> onManaCrystalCollected)
        {
            for (int i = manaCrystals.Count - 1; i >= 0; i--)
            {
                ManaCrystal manaCrystal = manaCrystals[i];
                manaCrystal.Update(gameTime);

                if (manaCrystal.IsExpired)
                {
                    manaCrystals.RemoveAt(i);
                    continue;
                }

                if (playerCanReceiveMana &&
                    playerBounds.Intersects(manaCrystal.Bounds))
                {
                    manaCrystals.RemoveAt(i);
                    onManaCrystalCollected(manaCrystal);
                }
            }
        }
    }
}
