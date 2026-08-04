using Microsoft.Xna.Framework;
using System;

namespace _1_2D_Top_Down
{
    public class Mana
    {
        public float MaxMana { get; private set; }
        public float CurrentMana { get; private set; }
        public float RegenPerSecond { get; private set; }

        public Mana(float maxMana, float regenPerSecond)
        {
            MaxMana = maxMana;
            CurrentMana = maxMana;
            RegenPerSecond = regenPerSecond;
        }
        public void SetMaxMana(float maxMana)
        {
            MaxMana = Math.Max(1f, maxMana);

            if (CurrentMana > MaxMana)
                CurrentMana = MaxMana;
        }

        public void SetRegenPerSecond(float regenPerSecond)
        {
            RegenPerSecond = Math.Max(0f, regenPerSecond);
        }
        public void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            CurrentMana += RegenPerSecond * deltaTime;
            CurrentMana = MathHelper.Min(CurrentMana, MaxMana);
        }

        public bool TrySpend(float amount)
        {
            if (CurrentMana < amount)
                return false;

            CurrentMana -= amount;
            return true;
        }

        public void Restore(float amount)
        {
            if (amount <= 0f)
                return;

            CurrentMana += amount;

            if (CurrentMana > MaxMana)
                CurrentMana = MaxMana;
        }

        public void RestoreFull()
        {
            CurrentMana = MaxMana;
        }

    }
}