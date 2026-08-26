using System;

namespace _1_2D_Top_Down
{
    /// <summary>
    /// Complete visual set for one player class level. Attack variants are
    /// registered now so progression can swap the whole set later without
    /// another content import.
    /// </summary>
    public sealed class PlayerVisualDefinition
    {
        public int Level { get; }
        public float Scale { get; }
        public PlayerAnimationDefinition Idle { get; }
        public PlayerAnimationDefinition Walk { get; }
        public PlayerAnimationDefinition Run { get; }
        public PlayerAnimationDefinition Attack { get; }
        public PlayerAnimationDefinition NormalAttack { get; }
        public PlayerAnimationDefinition WalkAttack { get; }
        public PlayerAnimationDefinition NormalWalkAttack { get; }
        public PlayerAnimationDefinition RunAttack { get; }
        public PlayerAnimationDefinition NormalRunAttack { get; }
        public PlayerAnimationDefinition Hurt { get; }
        public PlayerAnimationDefinition Death { get; }
        public string ShadowTextureAsset { get; }
        public string DeathShadowTextureAsset { get; }
        public float ShadowScale { get; }
        public float ShadowOpacity { get; }
        public float ShadowBottomOffset { get; }

        public PlayerVisualDefinition(
            int level,
            float scale,
            PlayerAnimationDefinition idle,
            PlayerAnimationDefinition walk,
            PlayerAnimationDefinition run,
            PlayerAnimationDefinition attack,
            PlayerAnimationDefinition normalAttack,
            PlayerAnimationDefinition walkAttack,
            PlayerAnimationDefinition normalWalkAttack,
            PlayerAnimationDefinition runAttack,
            PlayerAnimationDefinition normalRunAttack,
            PlayerAnimationDefinition hurt,
            PlayerAnimationDefinition death,
            string shadowTextureAsset,
            string deathShadowTextureAsset,
            float shadowScale,
            float shadowOpacity,
            float shadowBottomOffset)
        {
            if (level <= 0)
                throw new ArgumentOutOfRangeException(nameof(level));
            if (scale <= 0f)
                throw new ArgumentOutOfRangeException(nameof(scale));

            Level = level;
            Scale = scale;
            Idle = idle ?? throw new ArgumentNullException(nameof(idle));
            Walk = walk ?? throw new ArgumentNullException(nameof(walk));
            Run = run ?? throw new ArgumentNullException(nameof(run));
            Attack = attack ?? throw new ArgumentNullException(nameof(attack));
            NormalAttack = normalAttack;
            WalkAttack = walkAttack ??
                throw new ArgumentNullException(nameof(walkAttack));
            NormalWalkAttack = normalWalkAttack;
            RunAttack = runAttack ??
                throw new ArgumentNullException(nameof(runAttack));
            NormalRunAttack = normalRunAttack;
            Hurt = hurt ?? throw new ArgumentNullException(nameof(hurt));
            Death = death ?? throw new ArgumentNullException(nameof(death));
            ShadowTextureAsset = shadowTextureAsset ??
                throw new ArgumentNullException(nameof(shadowTextureAsset));
            DeathShadowTextureAsset = deathShadowTextureAsset ??
                throw new ArgumentNullException(
                    nameof(deathShadowTextureAsset));
            ShadowScale = MathF.Max(0f, shadowScale);
            ShadowOpacity = Math.Clamp(shadowOpacity, 0f, 1f);
            ShadowBottomOffset = shadowBottomOffset;
        }
    }
}
