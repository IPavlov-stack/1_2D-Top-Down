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
        public List<Coin> Coins { get; } = new();
        public List<ManaCrystal> ManaCrystals { get; } = new();

        public void Clear()
        {
            Coins.Clear();
            ManaCrystals.Clear();
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
            for (int i = Coins.Count - 1; i >= 0; i--)
            {
                Coin coin = Coins[i];
                coin.Update(gameTime);

                if (coin.IsExpired)
                {
                    Coins.RemoveAt(i);
                    continue;
                }

                if (playerBounds.Intersects(coin.Bounds))
                {
                    Coins.RemoveAt(i);
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
            for (int i = ManaCrystals.Count - 1; i >= 0; i--)
            {
                ManaCrystal manaCrystal = ManaCrystals[i];
                manaCrystal.Update(gameTime);

                if (manaCrystal.IsExpired)
                {
                    ManaCrystals.RemoveAt(i);
                    continue;
                }

                if (playerCanReceiveMana &&
                    playerBounds.Intersects(manaCrystal.Bounds))
                {
                    ManaCrystals.RemoveAt(i);
                    onManaCrystalCollected(manaCrystal);
                }
            }
        }
    }
}