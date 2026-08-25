namespace _1_2D_Top_Down
{
    public sealed class EnemyStatsDefinition
    {
        public int MaxHealth { get; }
        public int ExperienceReward { get; }

        public EnemyStatsDefinition(int maxHealth, int experienceReward)
        {
            MaxHealth = maxHealth;
            ExperienceReward = experienceReward;
        }
    }

    public sealed class EnemyLocomotionDefinition
    {
        public float WalkSpeed { get; }
        public float RunSpeed { get; }

        public EnemyLocomotionDefinition(float walkSpeed, float runSpeed = 0f)
        {
            WalkSpeed = walkSpeed;
            RunSpeed = runSpeed > 0f ? runSpeed : walkSpeed;
        }
    }

    public sealed class EnemyAnimationDefinition
    {
        public string TextureAsset { get; }
        public int SheetColumns { get; }
        public int SheetRows { get; }
        public int Row { get; }
        public int FrameCount { get; }
        public float FrameDuration { get; }
        public bool UsesDirectionalRows { get; }

        public EnemyAnimationDefinition(int row, int frameCount)
        {
            TextureAsset = string.Empty;
            Row = row;
            FrameCount = frameCount;
        }

        public EnemyAnimationDefinition(
            string textureAsset,
            int sheetColumns,
            int sheetRows,
            int frameCount,
            float frameDuration,
            bool usesDirectionalRows,
            int defaultRow = 0)
        {
            TextureAsset = textureAsset;
            SheetColumns = sheetColumns;
            SheetRows = sheetRows;
            Row = defaultRow;
            FrameCount = frameCount;
            FrameDuration = frameDuration;
            UsesDirectionalRows = usesDirectionalRows;
        }
    }

    public sealed class EnemyHitboxDefinition
    {
        public float Width { get; }
        public float Height { get; }
        public float CenterX { get; }
        public float BottomY { get; }

        public EnemyHitboxDefinition(
            float width,
            float height,
            float centerX,
            float bottomY)
        {
            Width = width;
            Height = height;
            CenterX = centerX;
            BottomY = bottomY;
        }
    }

    public sealed class EnemyVisualDefinition
    {
        public string TextureAsset { get; }
        public int SheetColumns { get; }
        public int SheetRows { get; }
        public float FrameDuration { get; }
        public float Scale { get; }
        public EnemyAnimationDefinition DefaultAnimation { get; }
        public string ShadowTextureAsset { get; }
        public float ShadowScale { get; }
        public float ShadowOpacity { get; }
        public float ShadowBottomOffset { get; }
        public string DeathTextureAsset { get; }
        public int DeathFrameCount { get; }
        public int DeathSheetColumns { get; }
        public int DeathSheetRows { get; }
        public int DeathAnimationRow { get; }
        public float DeathFrameDuration { get; }
        public float DeathScale { get; }
        public EnemyAnimationDefinition HurtAnimation { get; }
        public EnemyHitboxDefinition Hitbox { get; }
        public EnemyHitboxDefinition Hurtbox { get; }

        public EnemyVisualDefinition(
            string textureAsset, int sheetColumns, int sheetRows,
            float frameDuration, float scale,
            EnemyAnimationDefinition defaultAnimation,
            string shadowTextureAsset, float shadowScale,
            float shadowOpacity, float shadowBottomOffset,
            string deathTextureAsset, int deathFrameCount,
            int deathSheetColumns, int deathSheetRows,
            int deathAnimationRow, float deathFrameDuration,
            float deathScale,
            EnemyAnimationDefinition hurtAnimation = null,
            EnemyHitboxDefinition hitbox = null,
            EnemyHitboxDefinition hurtbox = null)
        {
            TextureAsset = textureAsset;
            SheetColumns = sheetColumns;
            SheetRows = sheetRows;
            FrameDuration = frameDuration;
            Scale = scale;
            DefaultAnimation = defaultAnimation;
            ShadowTextureAsset = shadowTextureAsset;
            ShadowScale = shadowScale;
            ShadowOpacity = shadowOpacity;
            ShadowBottomOffset = shadowBottomOffset;
            DeathTextureAsset = deathTextureAsset;
            DeathFrameCount = deathFrameCount;
            DeathSheetColumns = deathSheetColumns;
            DeathSheetRows = deathSheetRows;
            DeathAnimationRow = deathAnimationRow;
            DeathFrameDuration = deathFrameDuration;
            DeathScale = deathScale;
            HurtAnimation = hurtAnimation;
            Hitbox = hitbox;
            Hurtbox = hurtbox;
        }
    }

    public abstract class EnemyBehaviorDefinition { }

    public sealed class ChaseContactBehaviorDefinition : EnemyBehaviorDefinition
    {
        public int ContactDamage { get; }
        public float DamageCooldown { get; }
        public float AttackStateDuration { get; }
        public float DamageReleaseTime { get; }
        public EnemyAnimationDefinition MovementAnimation { get; }
        public EnemyAnimationDefinition AttackAnimation { get; }

        public ChaseContactBehaviorDefinition(
            int contactDamage, float damageCooldown, float attackStateDuration)
            : this(
                contactDamage,
                damageCooldown,
                attackStateDuration,
                movementAnimation: null,
                attackAnimation: null,
                damageReleaseTime: attackStateDuration * 0.5f)
        {
        }

        public ChaseContactBehaviorDefinition(
            int contactDamage,
            float damageCooldown,
            float attackStateDuration,
            EnemyAnimationDefinition movementAnimation,
            EnemyAnimationDefinition attackAnimation,
            float damageReleaseTime = 0f)
        {
            ContactDamage = contactDamage;
            DamageCooldown = damageCooldown;
            AttackStateDuration = attackStateDuration;
            DamageReleaseTime = damageReleaseTime > 0f
                ? damageReleaseTime
                : attackStateDuration * 0.5f;
            MovementAnimation = movementAnimation;
            AttackAnimation = attackAnimation;
        }
    }

    public sealed class KeepDistanceRangedBehaviorDefinition : EnemyBehaviorDefinition
    {
        public float AttackRange { get; }
        public float AttackCooldown { get; }
        public EnemyAnimationDefinition IdleAnimation { get; }
        public EnemyAnimationDefinition MovementAnimation { get; }
        public EnemyAnimationDefinition AttackAnimation { get; }
        public float ReleaseTime { get; }
        public float AttackDuration { get; }
        public bool RotateDuringAttack { get; }
        public ProjectileSpec Projectile { get; }

        public KeepDistanceRangedBehaviorDefinition(
            float attackRange, float attackCooldown,
            EnemyAnimationDefinition movementAnimation,
            EnemyAnimationDefinition attackAnimation,
            float releaseTime, float attackDuration,
            bool rotateDuringAttack, ProjectileSpec projectile)
            : this(
                attackRange,
                attackCooldown,
                movementAnimation,
                movementAnimation,
                attackAnimation,
                releaseTime,
                attackDuration,
                rotateDuringAttack,
                projectile)
        {
        }

        public KeepDistanceRangedBehaviorDefinition(
            float attackRange,
            float attackCooldown,
            EnemyAnimationDefinition idleAnimation,
            EnemyAnimationDefinition movementAnimation,
            EnemyAnimationDefinition attackAnimation,
            float releaseTime,
            float attackDuration,
            bool rotateDuringAttack,
            ProjectileSpec projectile)
        {
            AttackRange = attackRange;
            AttackCooldown = attackCooldown;
            IdleAnimation = idleAnimation;
            MovementAnimation = movementAnimation;
            AttackAnimation = attackAnimation;
            ReleaseTime = releaseTime;
            AttackDuration = attackDuration;
            RotateDuringAttack = rotateDuringAttack;
            Projectile = projectile;
        }
    }

    public sealed class NecromancerBehaviorDefinition : EnemyBehaviorDefinition
    {
        public KeepDistanceRangedBehaviorDefinition Ranged { get; }
        public string SummonEnemyId { get; }
        public float SummonCooldown { get; }
        public float SummonRadius { get; }

        public NecromancerBehaviorDefinition(
            KeepDistanceRangedBehaviorDefinition ranged,
            string summonEnemyId, float summonCooldown, float summonRadius)
        {
            Ranged = ranged;
            SummonEnemyId = summonEnemyId;
            SummonCooldown = summonCooldown;
            SummonRadius = summonRadius;
        }
    }
}
