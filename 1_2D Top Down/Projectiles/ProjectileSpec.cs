namespace _1_2D_Top_Down
{
    public sealed class ProjectileSpec
    {
        public string AssetName { get; }
        public int Damage { get; }
        public DamageType DamageType { get; }
        public CombatFaction Faction { get; }
        public float Speed { get; }
        public float MaxTravelDistance { get; }
        public float Knockback { get; }
        public bool IsReflectable { get; }
        public float Scale { get; }
        public int FrameCount { get; }
        public int FrameRows { get; }
        public int AnimationRow { get; }
        public float FrameDuration { get; }
        public float HitboxWidth { get; }
        public float HitboxHeight { get; }

        public ProjectileSpec(
            string assetName,
            int damage,
            DamageType damageType,
            CombatFaction faction,
            float speed,
            float maxTravelDistance,
            float knockback,
            bool isReflectable,
            float scale,
            int frameCount,
            int frameRows,
            float frameDuration,
            int animationRow = 0,
            float hitboxWidth = 0f,
            float hitboxHeight = 0f)
        {
            AssetName = assetName;
            Damage = damage;
            DamageType = damageType;
            Faction = faction;
            Speed = speed;
            MaxTravelDistance = maxTravelDistance;
            Knockback = knockback;
            IsReflectable = isReflectable;
            Scale = scale;
            FrameCount = frameCount;
            FrameRows = frameRows;
            AnimationRow = animationRow;
            FrameDuration = frameDuration;
            HitboxWidth = hitboxWidth;
            HitboxHeight = hitboxHeight;
        }
    }
}
