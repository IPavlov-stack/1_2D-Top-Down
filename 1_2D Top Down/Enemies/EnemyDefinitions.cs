using System;
using System.Collections.Generic;

namespace _1_2D_Top_Down
{
    public static class EnemyDefinitions
    {
        public static EnemyDefinition Demon { get; } = new EnemyDefinition(
            id: "demon",
            type: EnemyType.Demon,
            textureAsset: "enemies/Demon/FLYING",
            shadowTextureAsset: "enemies/Demon/shadow_demon",
            deathTextureAsset: "enemies/Demon/DEATH",
            maxHealth: 2,
            moveSpeed: 120f,
            contactDamage: 20,
            experienceReward: 10,
            deathFrameCount: 7,
            deathSheetColumnCount: 7,
            deathSheetRowCount: 1,
            deathAnimationRow: 0,
            deathFrameDuration: 0.1f,
            deathScale: 1f,
            shadowScale: 0.40f,
            shadowOpacity: 0.75f,
            shadowBottomOffset: 4f,
            contactDamageCooldownSeconds: 0.75f,
            attackStateDurationSeconds: 0.20f);

        public static EnemyDefinition EvilEye { get; } = new EnemyDefinition(
            id: "evil_eye",
            type: EnemyType.EvilEye,
            textureAsset: "enemies/Evil Eye/Evil Eye Sprite sheet",
            shadowTextureAsset: "enemies/Evil Eye/shadow_eye",
            deathTextureAsset: "enemies/Evil Eye/Evil Eye Sprite sheet",
            projectileTextureAsset: "projectiles/evilEye/evilEye_projectile_sphere",
            maxHealth: 2,
            moveSpeed: 100f,
            contactDamage: 0,
            experienceReward: 15,
            attackRange: 600f,
            attackCooldownSeconds: 1f,
            projectileReleaseTimeSeconds: 0.30f,
            attackDurationSeconds: 0.90f,
            deathFrameCount: 4,
            deathSheetColumnCount: 6,
            deathSheetRowCount: 3,
            deathAnimationRow: 2,
            deathFrameDuration: 0.15f,
            deathScale: 0.5f,
            shadowScale: 0.35f,
            shadowOpacity: 0.65f,
            shadowBottomOffset: 2f);

        private static readonly Dictionary<EnemyType, EnemyDefinition> ByType = new()
        {
            [EnemyType.Demon] = Demon,
            [EnemyType.EvilEye] = EvilEye
        };

        private static readonly Dictionary<string, EnemyDefinition> ById = new(
            StringComparer.OrdinalIgnoreCase)
        {
            [Demon.Id] = Demon,
            [EvilEye.Id] = EvilEye,
            [nameof(EnemyType.EvilEye)] = EvilEye
        };

        public static EnemyDefinition Get(EnemyType type)
        {
            if (ByType.TryGetValue(type, out EnemyDefinition? definition))
                return definition;

            throw new KeyNotFoundException($"No enemy definition is registered for '{type}'.");
        }

        public static bool TryGet(string id, out EnemyDefinition? definition)
        {
            return ById.TryGetValue(id, out definition);
        }
    }
}
