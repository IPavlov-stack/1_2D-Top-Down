using System;
using System.Collections.Generic;

namespace _1_2D_Top_Down
{
    public static class PlayerVisualDefinitions
    {
        private static readonly PlayerVisualDefinition[] MeleeLevels =
            CreateMeleeLevels();

        public static PlayerVisualDefinition MeleeLevel1 => MeleeLevels[0];
        public static IReadOnlyList<PlayerVisualDefinition> AllMeleeLevels =>
            MeleeLevels;

        public static PlayerVisualDefinition GetMeleeLevel(int level)
        {
            if (level < 1 || level > MeleeLevels.Length)
                throw new ArgumentOutOfRangeException(nameof(level));

            return MeleeLevels[level - 1];
        }

        private static PlayerVisualDefinition[] CreateMeleeLevels()
        {
            PlayerVisualDefinition[] result =
                new PlayerVisualDefinition[9];

            for (int level = 1; level <= result.Length; level++)
                result[level - 1] = CreateMeleeLevel(level);

            return result;
        }

        private static PlayerVisualDefinition CreateMeleeLevel(int level)
        {
            string root = $"player/Melee/Level{level}";
            bool hasNormalAttackVariants = level >= 4;
            int attackFrames = level <= 3 ? 8 : 7;

            return new PlayerVisualDefinition(
                level: level,
                scale: 2.2f,
                idle: Clip(
                    root,
                    "Idle",
                    12,
                    0.12f,
                    frameCountsByRow: new[] { 12, 12, 12, 4 }),
                walk: Clip(root, "Walk", 6, 0.10f),
                run: Clip(root, "Run", 8, 0.07f),
                attack: Clip(
                    root,
                    "Attack",
                    attackFrames,
                    0.07f,
                    loop: false),
                normalAttack: hasNormalAttackVariants
                    ? Clip(root, "AttackNormal", 7, 0.07f, loop: false)
                    : null,
                walkAttack: Clip(
                    root,
                    "WalkAttack",
                    6,
                    0.08f,
                    loop: false),
                normalWalkAttack: hasNormalAttackVariants
                    ? Clip(root, "WalkAttackNormal", 6, 0.08f, loop: false)
                    : null,
                runAttack: Clip(
                    root,
                    "RunAttack",
                    8,
                    0.07f,
                    loop: false),
                normalRunAttack: hasNormalAttackVariants
                    ? Clip(root, "RunAttackNormal", 8, 0.07f, loop: false)
                    : null,
                hurt: Clip(root, "Hurt", 5, 0.08f, loop: false),
                death: Clip(root, "Death", 7, 0.10f, loop: false),
                shadowTextureAsset: $"{root}/Shadow",
                deathShadowTextureAsset: $"{root}/DeathShadow",
                shadowScale: 2.2f,
                shadowOpacity: 0.65f,
                shadowBottomOffset: -4f,
                footAnchorX: 32f,
                footAnchorY: 44f,
                movementHitboxWidth: 15f,
                movementHitboxHeight: 7f,
                hurtboxWidth: 18f,
                hurtboxHeight: 29f);
        }

        private static PlayerAnimationDefinition Clip(
            string root,
            string name,
            int columns,
            float frameDuration,
            bool loop = true,
            int[] frameCountsByRow = null) =>
            new(
                $"{root}/{name}",
                sheetColumns: columns,
                sheetRows: 4,
                frameCount: columns,
                frameDuration: frameDuration,
                loop: loop,
                frameCountsByRow: frameCountsByRow);
    }
}
