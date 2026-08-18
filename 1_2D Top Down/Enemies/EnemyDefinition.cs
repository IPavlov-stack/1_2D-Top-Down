namespace _1_2D_Top_Down
{
    public sealed class EnemyDefinition
    {
        public string Id { get; }
        public EnemyType Type { get; }
        public string TextureAsset { get; }
        public string ShadowTextureAsset { get; }
        public string DeathTextureAsset { get; }
        public string? ProjectileTextureAsset { get; }

        public int MaxHealth { get; }
        public float MoveSpeed { get; }
        public int ContactDamage { get; }
        public int ExperienceReward { get; }
        public float AttackRange { get; }
        public float AttackCooldownSeconds { get; }
        public float ContactDamageCooldownSeconds { get; }
        public float AttackStateDurationSeconds { get; }
        public float ProjectileReleaseTimeSeconds { get; }
        public float AttackDurationSeconds { get; }

        public int DeathFrameCount { get; }
        public int DeathSheetColumnCount { get; }
        public int DeathSheetRowCount { get; }
        public int DeathAnimationRow { get; }
        public float DeathFrameDuration { get; }
        public float DeathScale { get; }

        public float ShadowScale { get; }
        public float ShadowOpacity { get; }
        public float ShadowBottomOffset { get; }

        public EnemyDefinition(
            string id,
            EnemyType type,
            string textureAsset,
            string shadowTextureAsset,
            string deathTextureAsset,
            int maxHealth,
            float moveSpeed,
            int contactDamage,
            int experienceReward,
            int deathFrameCount,
            int deathSheetColumnCount,
            int deathSheetRowCount,
            int deathAnimationRow,
            float deathFrameDuration,
            float deathScale,
            float shadowScale,
            float shadowOpacity,
            float shadowBottomOffset,
            string? projectileTextureAsset = null,
            float attackRange = 0f,
            float attackCooldownSeconds = 0f,
            float contactDamageCooldownSeconds = 0f,
            float attackStateDurationSeconds = 0f,
            float projectileReleaseTimeSeconds = 0f,
            float attackDurationSeconds = 0f)
        {
            Id = id;
            Type = type;
            TextureAsset = textureAsset;
            ShadowTextureAsset = shadowTextureAsset;
            DeathTextureAsset = deathTextureAsset;
            ProjectileTextureAsset = projectileTextureAsset;
            MaxHealth = maxHealth;
            MoveSpeed = moveSpeed;
            ContactDamage = contactDamage;
            ExperienceReward = experienceReward;
            AttackRange = attackRange;
            AttackCooldownSeconds = attackCooldownSeconds;
            ContactDamageCooldownSeconds = contactDamageCooldownSeconds;
            AttackStateDurationSeconds = attackStateDurationSeconds;
            ProjectileReleaseTimeSeconds = projectileReleaseTimeSeconds;
            AttackDurationSeconds = attackDurationSeconds;
            DeathFrameCount = deathFrameCount;
            DeathSheetColumnCount = deathSheetColumnCount;
            DeathSheetRowCount = deathSheetRowCount;
            DeathAnimationRow = deathAnimationRow;
            DeathFrameDuration = deathFrameDuration;
            DeathScale = deathScale;
            ShadowScale = shadowScale;
            ShadowOpacity = shadowOpacity;
            ShadowBottomOffset = shadowBottomOffset;
        }
    }
}
