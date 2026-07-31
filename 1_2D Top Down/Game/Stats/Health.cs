namespace _1_2D_Top_Down
{
    public class Health
    {
        public int MaxHealth { get; }
        public int CurrentHealth { get; private set; }

        public bool IsDead => CurrentHealth <= 0;

        public Health(int maxHealth)
        {
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
        }

        public void TakeDamage(int damage)
        {
            if (damage <= 0 || IsDead)
                return;

            CurrentHealth -= damage;

            if (CurrentHealth < 0)
                CurrentHealth = 0;
        }

        public void Heal(int amount)
        {
            if (amount <= 0 || IsDead)
                return;

            CurrentHealth += amount;

            if (CurrentHealth > MaxHealth)
                CurrentHealth = MaxHealth;
        }

        public void Reset()
        {
            CurrentHealth = MaxHealth;
        }
    }
}