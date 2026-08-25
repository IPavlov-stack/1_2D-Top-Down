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

        private static readonly ProjectileSpec LichProjectile = new(
            "projectiles/Lich/Projectile",
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
            return registry;
        }
    }
}
