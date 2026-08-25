using Microsoft.Xna.Framework;

namespace _1_2D_Top_Down
{
    public static class EnemyDefinitions
    {
        private static readonly EnemyAnimationDefinition DemonIdle = new(
            "enemies/Demon/Tier1/Idle",
            sheetColumns: 4,
            sheetRows: 4,
            frameCount: 4,
            frameDuration: 0.16f,
            usesDirectionalRows: true,
            defaultRow: 0);

        private static readonly EnemyAnimationDefinition DemonWalk = new(
            "enemies/Demon/Tier1/Walk",
            sheetColumns: 6,
            sheetRows: 4,
            frameCount: 6,
            frameDuration: 0.10f,
            usesDirectionalRows: true,
            defaultRow: 0);

        private static readonly EnemyAnimationDefinition DemonAttack = new(
            "enemies/Demon/Tier1/Attack",
            sheetColumns: 10,
            sheetRows: 4,
            frameCount: 10,
            frameDuration: 0.07f,
            usesDirectionalRows: true,
            defaultRow: 0);

        private static readonly EnemyAnimationDefinition DemonHurt = new(
            "enemies/Demon/Tier1/Hurt",
            sheetColumns: 4,
            sheetRows: 4,
            frameCount: 4,
            frameDuration: 0.08f,
            usesDirectionalRows: true,
            defaultRow: 0);

        private static readonly EnemyAnimationDefinition LichIdle = new(
            "enemies/Lich/Tier1/Idle",
            sheetColumns: 4,
            sheetRows: 4,
            frameCount: 4,
            frameDuration: 0.16f,
            usesDirectionalRows: true,
            defaultRow: 0);

        private static readonly EnemyAnimationDefinition LichWalk = new(
            "enemies/Lich/Tier1/Walk",
            sheetColumns: 6,
            sheetRows: 4,
            frameCount: 6,
            frameDuration: 0.10f,
            usesDirectionalRows: true,
            defaultRow: 0);

        private static readonly EnemyAnimationDefinition LichAttack = new(
            "enemies/Lich/Tier1/Attack",
            sheetColumns: 8,
            sheetRows: 4,
            frameCount: 8,
            frameDuration: 0.08f,
            usesDirectionalRows: true,
            defaultRow: 0);

        private static readonly EnemyAnimationDefinition LichHurt = new(
            "enemies/Lich/Tier1/Hurt",
            sheetColumns: 4,
            sheetRows: 4,
            frameCount: 4,
            frameDuration: 0.08f,
            usesDirectionalRows: true,
            defaultRow: 0);

        private static readonly EnemyAnimationDefinition BeholderIdle = new(
            "enemies/Beholder/Tier1/Idle",
            sheetColumns: 12,
            sheetRows: 4,
            frameCount: 12,
            frameDuration: 0.15f,
            usesDirectionalRows: true,
            defaultRow: 0);

        private static readonly EnemyAnimationDefinition BeholderWalk = new(
            "enemies/Beholder/Tier1/Walk",
            sheetColumns: 8,
            sheetRows: 4,
            frameCount: 8,
            frameDuration: 0.11f,
            usesDirectionalRows: true,
            defaultRow: 0);

        private static readonly EnemyAnimationDefinition BeholderHurt = new(
            "enemies/Beholder/Tier1/Hurt",
            sheetColumns: 6,
            sheetRows: 4,
            frameCount: 6,
            frameDuration: 0.09f,
            usesDirectionalRows: true,
            defaultRow: 0);

        private static readonly EnemyAnimationDefinition MushroomIdle = new(
            "enemies/MushroomCharger/Tier1/Idle",
            sheetColumns: 4,
            sheetRows: 4,
            frameCount: 4,
            frameDuration: 0.16f,
            usesDirectionalRows: true,
            defaultRow: 0);

        private static readonly EnemyAnimationDefinition MushroomWalk = new(
            "enemies/MushroomCharger/Tier1/Walk",
            sheetColumns: 6,
            sheetRows: 4,
            frameCount: 6,
            frameDuration: 0.11f,
            usesDirectionalRows: true,
            defaultRow: 0);

        private static readonly EnemyAnimationDefinition MushroomRun = new(
            "enemies/MushroomCharger/Tier1/Run",
            sheetColumns: 6,
            sheetRows: 4,
            frameCount: 6,
            frameDuration: 0.075f,
            usesDirectionalRows: true,
            defaultRow: 0);

        private static readonly EnemyAnimationDefinition MushroomHurt = new(
            "enemies/MushroomCharger/Tier1/Hurt",
            sheetColumns: 4,
            sheetRows: 4,
            frameCount: 4,
            frameDuration: 0.09f,
            usesDirectionalRows: true,
            defaultRow: 0);

        private static readonly EnemyAnimationDefinition ZombieIdle = new(
            "enemies/Zombie/Tier1/Idle",
            sheetColumns: 4,
            sheetRows: 4,
            frameCount: 4,
            frameDuration: 0.16f,
            usesDirectionalRows: true,
            defaultRow: 0);

        private static readonly EnemyAnimationDefinition ZombieWalk = new(
            "enemies/Zombie/Tier1/Walk",
            sheetColumns: 6,
            sheetRows: 4,
            frameCount: 6,
            frameDuration: 0.12f,
            usesDirectionalRows: true,
            defaultRow: 0);

        private static readonly EnemyAnimationDefinition ZombieAttack = new(
            "enemies/Zombie/Tier1/Attack",
            sheetColumns: 10,
            sheetRows: 4,
            frameCount: 10,
            frameDuration: 0.10f,
            usesDirectionalRows: true,
            defaultRow: 0);

        private static readonly EnemyAnimationDefinition ZombieHurt = new(
            "enemies/Zombie/Tier1/Hurt",
            sheetColumns: 4,
            sheetRows: 4,
            frameCount: 4,
            frameDuration: 0.09f,
            usesDirectionalRows: true,
            defaultRow: 0);

        private static readonly AreaEffectDefinition BeholderExplosion =
            AreaEffectDefinition.Circle(
                id: "beholder-explosion",
                radiusInTiles: 3f,
                telegraphDuration: 1.0f,
                activeDuration: 1.0f,
                damage: 25,
                damageType: DamageType.Arcane,
                hitOnce: true,
                knockback: 1200f,
                telegraphColor: Color.Red,
                activeColor: Color.OrangeRed,
                animation: new AreaEffectAnimationDefinition(
                    "Effects/Magic/Explosion",
                    sheetColumns: 10,
                    sheetRows: 1,
                    frameCount: 10,
                    frameDuration: 0.10f,
                    visualScale: 1.0f),
                damageOnlyOnActivation: true);

        private static readonly AreaEffectDefinition ZombiePoison =
            AreaEffectDefinition.Rectangle(
                id: "zombie-poison",
                widthInTiles: 1.5f,
                heightInTiles: 1.5f,
                telegraphDuration: 0f,
                activeDuration: 7.5f,
                damage: 5,
                damageType: DamageType.Poison,
                damageInterval: 0.75f,
                hitOnce: false,
                activeColor: new Color(45, 220, 80),
                animation: new AreaEffectAnimationDefinition(
                    "Effects/Poison/PoisonAoe",
                    sheetColumns: 10,
                    sheetRows: 1,
                    frameCount: 10,
                    frameDuration: 0.10f,
                    visualScale: 2.5f,
                    loop: true,
                    visualOffsetInTiles: new Vector2(0.02f, -1.2f)),
                showActiveIndicator: true,
                pulseActiveIndicator: true);

        private static readonly ProjectileSpec LichProjectile = new(
            "projectiles/Lich/Projectile",
            damage: 15,
            DamageType.Arcane,
            CombatFaction.Enemy,
            speed: 650f,
            maxTravelDistance: 700f,
            knockback: 0f,
            isReflectable: true,
            scale: 1.9f,
            frameCount: 9,
            frameRows: 4,
            frameDuration: 0.12f,
            animationRow: 3,
            hitboxWidth: 30f,
            hitboxHeight: 30f);

        private static readonly ProjectileSpec NecromancerProjectile = new(
            "projectiles/mage_arcane_projectile",
            damage: 15,
            DamageType.Arcane,
            CombatFaction.Enemy,
            speed: 325f,
            maxTravelDistance: 700f,
            knockback: 0f,
            isReflectable: true,
            scale: 1.75f,
            frameCount: 11,
            frameRows: 1,
            frameDuration: 0.04f);

        public static EnemyDefinition Demon { get; } = new(
            "demon",
            EnemyType.Demon,
            new EnemyVisualDefinition(
                "enemies/Demon/Tier1/Idle", 4, 4, 0.16f, 1.75f,
                DemonIdle,
                "enemies/Demon/Tier1/Shadow", 2.15f, 0.60f, -18f,
                "enemies/Demon/Tier1/Death", 13, 13, 4, 0, 0.08f, 1.75f,
                hurtAnimation: DemonHurt,
                hitbox: new EnemyHitboxDefinition(
                    width: 34f,
                    height: 24f,
                    centerX: 64f,
                    bottomY: 94f),
                hurtbox: new EnemyHitboxDefinition(
                    width: 48f,
                    height: 58f,
                    centerX: 64f,
                    bottomY: 94f)),
            new EnemyStatsDefinition(maxHealth: 2, experienceReward: 10),
            new EnemyLocomotionDefinition(walkSpeed: 120f),
            new ChaseContactBehaviorDefinition(
                contactDamage: 20,
                damageCooldown: 0.75f,
                attackStateDuration: 0.70f,
                movementAnimation: DemonWalk,
                attackAnimation: DemonAttack,
                damageReleaseTime: 0.35f));

        public static EnemyDefinition Lich { get; } = new(
            "lich",
            EnemyType.Lich,
            new EnemyVisualDefinition(
                "enemies/Lich/Tier1/Idle", 4, 4, 0.16f, 2f,
                LichIdle,
                "enemies/Lich/Tier1/Shadow", 2f, 0.60f, 8f,
                "enemies/Lich/Tier1/Death", 10, 10, 4, 0, 0.10f, 2f,
                hurtAnimation: LichHurt,
                hitbox: new EnemyHitboxDefinition(
                    width: 14f,
                    height: 12f,
                    centerX: 32f,
                    bottomY: 46f),
                hurtbox: new EnemyHitboxDefinition(
                    width: 22f,
                    height: 34f,
                    centerX: 32f,
                    bottomY: 46f)),
            new EnemyStatsDefinition(maxHealth: 2, experienceReward: 15),
            new EnemyLocomotionDefinition(walkSpeed: 100f),
            new KeepDistanceRangedBehaviorDefinition(
                attackRange: 600f,
                attackCooldown: 1f,
                LichIdle,
                LichWalk,
                LichAttack,
                releaseTime: 0.45f,
                attackDuration: 0.70f,
                rotateDuringAttack: false,
                LichProjectile));

        public static EnemyDefinition Necromancer { get; } = new(
            "necromancer",
            EnemyType.Necromancer,
            new EnemyVisualDefinition(
                "enemies/Skeleton Mage/skeleton_mage", 4, 3, 0.15f, 0.55f,
                new EnemyAnimationDefinition(0, 4),
                "enemies/Demon/shadow_demon", 0.45f, 0.70f, 4f,
                "enemies/Skeleton Mage/skeleton_mage", 4, 4, 3, 2, 0.15f, 0.55f),
            new EnemyStatsDefinition(maxHealth: 8, experienceReward: 35),
            new EnemyLocomotionDefinition(walkSpeed: 75f),
            new NecromancerBehaviorDefinition(
                new KeepDistanceRangedBehaviorDefinition(
                    attackRange: 500f,
                    attackCooldown: 1.8f,
                    new EnemyAnimationDefinition(0, 4),
                    new EnemyAnimationDefinition(1, 4),
                    releaseTime: 0.45f,
                    attackDuration: 0.90f,
                    rotateDuringAttack: false,
                    NecromancerProjectile),
                summonEnemyId: "demon",
                summonCooldown: 8f,
                summonRadius: 110f));

        public static EnemyDefinition Beholder { get; } = new(
            "beholder",
            EnemyType.Beholder,
            new EnemyVisualDefinition(
                "enemies/Beholder/Tier1/Idle", 12, 4, 0.15f, 2f,
                BeholderIdle,
                "enemies/Beholder/Tier1/Shadow", 2f, 0.60f, 4f,
                "enemies/Beholder/Tier1/Death", 9, 9, 4, 0, 0.12f, 2f,
                hurtAnimation: BeholderHurt,
                hitbox: new EnemyHitboxDefinition(
                    width: 18f,
                    height: 14f,
                    centerX: 32f,
                    bottomY: 51f),
                hurtbox: new EnemyHitboxDefinition(
                    width: 34f,
                    height: 38f,
                    centerX: 32f,
                    bottomY: 51f)),
            new EnemyStatsDefinition(maxHealth: 4, experienceReward: 20),
            new EnemyLocomotionDefinition(walkSpeed: 90f),
            new TelegraphedAreaAttackBehaviorDefinition(
                attackRange: 600f,
                attackCooldown: 1.8f,
                attackDuration: 2.0f,
                idleAnimation: BeholderIdle,
                movementAnimation: BeholderWalk,
                attackAnimation: null,
                areaEffect: BeholderExplosion));

        public static EnemyDefinition MushroomCharger { get; } = new(
            "mushroom-charger",
            EnemyType.MushroomCharger,
            new EnemyVisualDefinition(
                "enemies/MushroomCharger/Tier1/Idle", 4, 4, 0.16f, 2f,
                MushroomIdle,
                "enemies/MushroomCharger/Tier1/Shadow", 2f, 0.60f, 4f,
                "enemies/MushroomCharger/Tier1/Death", 9, 9, 4, 0, 0.11f, 2f,
                hurtAnimation: MushroomHurt,
                hitbox: new EnemyHitboxDefinition(
                    width: 14f,
                    height: 10f,
                    centerX: 32f,
                    bottomY: 43f),
                hurtbox: new EnemyHitboxDefinition(
                    width: 24f,
                    height: 29f,
                    centerX: 32f,
                    bottomY: 43f)),
            new EnemyStatsDefinition(maxHealth: 5, experienceReward: 18),
            new EnemyLocomotionDefinition(
                walkSpeed: 150f,
                runSpeed: 720f),
            new ChargerBehaviorDefinition(
                minimumChargeDistance: 300f,
                triggerDistance: 800f,
                acceleration: 700f,
                lockedChargeDuration: 0.90f,
                recoveryDuration: 1.5f,
                chargeCooldown: 2.0f,
                normalContactDamage: 10,
                normalContactDamageCooldown: 0.75f,
                contactDamage: 30,
                contactKnockback: 1200f,
                idleAnimation: MushroomIdle,
                walkAnimation: MushroomWalk,
                runAnimation: MushroomRun));

        public static EnemyDefinition Zombie { get; } = new(
            "zombie",
            EnemyType.Zombie,
            new EnemyVisualDefinition(
                "enemies/Zombie/Tier1/Idle", 4, 4, 0.16f, 2.5f,
                ZombieIdle,
                "enemies/Zombie/Tier1/Shadow", 2.5f, 0.60f, 0f,
                "enemies/Zombie/Tier1/Death", 9, 9, 4, 0, 0.11f, 2.5f,
                hurtAnimation: ZombieHurt,
                hitbox: new EnemyHitboxDefinition(
                    width: 14f,
                    height: 10f,
                    centerX: 32f,
                    bottomY: 40f),
                hurtbox: new EnemyHitboxDefinition(
                    width: 24f,
                    height: 34f,
                    centerX: 32f,
                    bottomY: 40f)),
            new EnemyStatsDefinition(maxHealth: 6, experienceReward: 22),
            new EnemyLocomotionDefinition(walkSpeed: 110f),
            new PoisonTrailBehaviorDefinition(
                new ChaseContactBehaviorDefinition(
                    contactDamage: 12,
                    damageCooldown: 0.9f,
                    attackStateDuration: 1.0f,
                    movementAnimation: ZombieWalk,
                    attackAnimation: ZombieAttack,
                    damageReleaseTime: 0.55f),
                poisonEffect: ZombiePoison,
                spawnInterval: 0.54f,
                offsetInTiles: 0.7f));

        public static EnemyDefinitionRegistry Registry { get; } =
            CreateRegistry();

        public static EnemyDefinition Get(EnemyType type) =>
            Registry.Get(type);

        public static bool TryGet(string id, out EnemyDefinition definition) =>
            Registry.TryGet(id, out definition);

        private static EnemyDefinitionRegistry CreateRegistry()
        {
            EnemyDefinitionRegistry registry = new();
            registry.Register(Demon);
            registry.Register(Lich);
            registry.Register(Necromancer);
            registry.Register(Beholder);
            registry.Register(
                MushroomCharger,
                "MushroomCharger",
                "Mushroom Charger");
            registry.Register(Zombie, "Zombie", "Zombie Tier1");
            return registry;
        }
    }
}
