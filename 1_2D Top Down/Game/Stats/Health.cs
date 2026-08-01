namespace _1_2D_Top_Down
{
    public class Health
    {
        public int MaxHealth { get; }
        public int CurrentHealth { get; private set; }
        public float RegenPerSecond { get; }

        private float regenProgress;

        public bool IsDead => CurrentHealth <= 0;

        public Health(int maxHealth, float regenPerSecond = 0f)
        {
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
            RegenPerSecond = regenPerSecond < 0f ? 0f : regenPerSecond;
        }

        public void Update(float deltaTime)
        {
            if (deltaTime <= 0f ||
                RegenPerSecond <= 0f ||
                IsDead ||
                CurrentHealth >= MaxHealth)
            {
                regenProgress = 0f;
                return;
            }

            regenProgress += RegenPerSecond * deltaTime;

            int healAmount = (int)regenProgress;

            if (healAmount <= 0)
            {
                return;
            }

            Heal(healAmount);
            regenProgress -= healAmount;

            if (CurrentHealth >= MaxHealth)
            {
                regenProgress = 0f;
            }
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
            regenProgress = 0f;
        }
    }
}