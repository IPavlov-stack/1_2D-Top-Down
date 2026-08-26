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

    /// <summary>
    /// Decorates another behavior and periodically leaves a persistent area
    /// effect behind the enemy. This keeps locomotion/contact combat reusable.
    /// </summary>
    public sealed class PoisonTrailBehaviorDefinition : EnemyBehaviorDefinition
    {
        public EnemyBehaviorDefinition InnerBehavior { get; }
        public AreaEffectDefinition PoisonEffect { get; }
        public float SpawnInterval { get; }
        public float OffsetInTiles { get; }

        public PoisonTrailBehaviorDefinition(
            EnemyBehaviorDefinition innerBehavior,
            AreaEffectDefinition poisonEffect,
            float spawnInterval,
            float offsetInTiles)
        {
            if (innerBehavior == null)
                throw new System.ArgumentNullException(nameof(innerBehavior));
            if (poisonEffect == null)
                throw new System.ArgumentNullException(nameof(poisonEffect));
            if (spawnInterval <= 0f)
            {
                throw new System.ArgumentOutOfRangeException(
                    nameof(spawnInterval));
            }
            if (offsetInTiles < 0f)
            {
                throw new System.ArgumentOutOfRangeException(
                    nameof(offsetInTiles));
            }

            InnerBehavior = innerBehavior;
            PoisonEffect = poisonEffect;
            SpawnInterval = spawnInterval;
            OffsetInTiles = offsetInTiles;
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

    /// <summary>
    /// Stops within casting range and places a telegraphed area attack at the
    /// target's position. The target point is captured when casting starts,
    /// allowing the player to evade before the active phase.
    /// </summary>
    public sealed class TelegraphedAreaAttackBehaviorDefinition
        : EnemyBehaviorDefinition
    {
        public float AttackRange { get; }
        public float AttackCooldown { get; }
        public float AttackDuration { get; }
        public EnemyAnimationDefinition IdleAnimation { get; }
        public EnemyAnimationDefinition MovementAnimation { get; }
        public EnemyAnimationDefinition AttackAnimation { get; }
        public AreaEffectDefinition AreaEffect { get; }

        public TelegraphedAreaAttackBehaviorDefinition(
            float attackRange,
            float attackCooldown,
            float attackDuration,
            EnemyAnimationDefinition idleAnimation,
            EnemyAnimationDefinition movementAnimation,
            EnemyAnimationDefinition attackAnimation,
            AreaEffectDefinition areaEffect)
        {
            AttackRange = attackRange;
            AttackCooldown = attackCooldown;
            AttackDuration = attackDuration;
            IdleAnimation = idleAnimation;
            MovementAnimation = movementAnimation;
            AttackAnimation = attackAnimation;
            AreaEffect = areaEffect;
        }
    }

    /// <summary>
    /// Chases normally, accelerates while steering toward the target, then
    /// locks its final direction at maximum speed for a committed charge.
    /// </summary>
    public sealed class ChargerBehaviorDefinition : EnemyBehaviorDefinition
    {
        public float MinimumChargeDistance { get; }
        public float TriggerDistance { get; }
        public float LockedChargeDuration { get; }
        public float RecoveryDuration { get; }
        public float ChargeCooldown { get; }
        public int NormalContactDamage { get; }
        public float NormalContactDamageCooldown { get; }
        public int ContactDamage { get; }
        public float ContactKnockback { get; }
        public EnemyAnimationDefinition IdleAnimation { get; }
        public EnemyAnimationDefinition WalkAnimation { get; }
        public EnemyAnimationDefinition RunAnimation { get; }
        public EnemySteeringDefinition ChaseSteering { get; }
        public EnemySteeringDefinition ChargeSteering { get; }

        public ChargerBehaviorDefinition(
            float minimumChargeDistance,
            float triggerDistance,
            float lockedChargeDuration,
            float recoveryDuration,
            float chargeCooldown,
            int normalContactDamage,
            float normalContactDamageCooldown,
            int contactDamage,
            float contactKnockback,
            EnemyAnimationDefinition idleAnimation,
            EnemyAnimationDefinition walkAnimation,
            EnemyAnimationDefinition runAnimation,
            EnemySteeringDefinition chaseSteering,
            EnemySteeringDefinition chargeSteering)
        {
            if (minimumChargeDistance < 0f)
            {
                throw new System.ArgumentOutOfRangeException(
                    nameof(minimumChargeDistance));
            }
            if (triggerDistance <= minimumChargeDistance)
            {
                throw new System.ArgumentOutOfRangeException(
                    nameof(triggerDistance),
                    "Trigger distance must exceed the minimum charge distance.");
            }

            MinimumChargeDistance = minimumChargeDistance;
            TriggerDistance = triggerDistance;
            LockedChargeDuration = lockedChargeDuration;
            RecoveryDuration = recoveryDuration;
            ChargeCooldown = chargeCooldown;
            NormalContactDamage = normalContactDamage;
            NormalContactDamageCooldown = normalContactDamageCooldown;
            ContactDamage = contactDamage;
            ContactKnockback = contactKnockback;
            IdleAnimation = idleAnimation;
            WalkAnimation = walkAnimation;
            RunAnimation = runAnimation;
            ChaseSteering = chaseSteering ??
                throw new System.ArgumentNullException(
                    nameof(chaseSteering));
            ChargeSteering = chargeSteering ??
                throw new System.ArgumentNullException(
                    nameof(chargeSteering));
        }
    }

    /// <summary>
    /// Chases at walk/run speed and fades through a collision-safe teleport
    /// to the opposite side of the target.
    /// </summary>
    public sealed class TeleportChaserBehaviorDefinition
        : EnemyBehaviorDefinition
    {
        public float RunTriggerDistance { get; }
        public float TeleportTriggerDistance { get; }
        public float TeleportExitDistance { get; }
        public float TeleportCooldown { get; }
        public float FadeOutDuration { get; }
        public float FadeInDuration { get; }
        public float PostTeleportRunLockout { get; }
        public int ContactDamage { get; }
        public float ContactDamageCooldown { get; }
        public float ContactKnockback { get; }
        public float AttackDuration { get; }
        public float DamageReleaseTime { get; }
        public EnemyAnimationDefinition IdleAnimation { get; }
        public EnemyAnimationDefinition WalkAnimation { get; }
        public EnemyAnimationDefinition RunAnimation { get; }
        public EnemyAnimationDefinition AttackAnimation { get; }
        public EnemySteeringDefinition WalkSteering { get; }
        public EnemySteeringDefinition RunSteering { get; }

        public TeleportChaserBehaviorDefinition(
            float runTriggerDistance,
            float teleportTriggerDistance,
            float teleportExitDistance,
            float teleportCooldown,
            float fadeOutDuration,
            float fadeInDuration,
            float postTeleportRunLockout,
            int contactDamage,
            float contactDamageCooldown,
            float contactKnockback,
            float attackDuration,
            float damageReleaseTime,
            EnemyAnimationDefinition idleAnimation,
            EnemyAnimationDefinition walkAnimation,
            EnemyAnimationDefinition runAnimation,
            EnemyAnimationDefinition attackAnimation,
            EnemySteeringDefinition walkSteering,
            EnemySteeringDefinition runSteering)
        {
            if (runTriggerDistance <= teleportTriggerDistance)
            {
                throw new System.ArgumentOutOfRangeException(
                    nameof(runTriggerDistance),
                    "Run distance must exceed teleport distance.");
            }
            if (teleportTriggerDistance <= 0f)
            {
                throw new System.ArgumentOutOfRangeException(
                    nameof(teleportTriggerDistance));
            }
            if (teleportExitDistance <= 0f)
            {
                throw new System.ArgumentOutOfRangeException(
                    nameof(teleportExitDistance));
            }
            if (teleportCooldown < 0f || fadeOutDuration <= 0f ||
                fadeInDuration <= 0f || postTeleportRunLockout < 0f)
            {
                throw new System.ArgumentOutOfRangeException(
                    nameof(teleportCooldown));
            }
            if (attackDuration <= 0f || damageReleaseTime < 0f ||
                damageReleaseTime > attackDuration)
            {
                throw new System.ArgumentOutOfRangeException(
                    nameof(damageReleaseTime));
            }

            RunTriggerDistance = runTriggerDistance;
            TeleportTriggerDistance = teleportTriggerDistance;
            TeleportExitDistance = teleportExitDistance;
            TeleportCooldown = teleportCooldown;
            FadeOutDuration = fadeOutDuration;
            FadeInDuration = fadeInDuration;
            PostTeleportRunLockout = postTeleportRunLockout;
            ContactDamage = contactDamage;
            ContactDamageCooldown = contactDamageCooldown;
            ContactKnockback = contactKnockback;
            AttackDuration = attackDuration;
            DamageReleaseTime = damageReleaseTime;
            IdleAnimation = idleAnimation;
            WalkAnimation = walkAnimation;
            RunAnimation = runAnimation;
            AttackAnimation = attackAnimation;
            WalkSteering = walkSteering ??
                throw new System.ArgumentNullException(
                    nameof(walkSteering));
            RunSteering = runSteering ??
                throw new System.ArgumentNullException(
                    nameof(runSteering));
        }
    }
}
