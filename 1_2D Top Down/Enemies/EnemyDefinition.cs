namespace _1_2D_Top_Down
{
    public sealed class EnemyDefinition
    {
        public string Id { get; }
        public EnemyType Type { get; }
        public EnemyVisualDefinition Visuals { get; }
        public EnemyStatsDefinition Stats { get; }
        public EnemyLocomotionDefinition Locomotion { get; }
        public EnemyBehaviorDefinition Behavior { get; }

        // Compatibility aliases keep mission and rendering code stable while
        // new enemy features consume the component definitions directly.
        public string TextureAsset => Visuals.TextureAsset;
        public string ShadowTextureAsset => Visuals.ShadowTextureAsset;
        public string DeathTextureAsset => Visuals.DeathTextureAsset;
        public int MaxHealth => Stats.MaxHealth;
        public int ExperienceReward => Stats.ExperienceReward;
        public float MoveSpeed => Locomotion.WalkSpeed;
        public int DeathFrameCount => Visuals.DeathFrameCount;
        public int DeathSheetColumnCount => Visuals.DeathSheetColumns;
        public int DeathSheetRowCount => Visuals.DeathSheetRows;
        public int DeathAnimationRow => Visuals.DeathAnimationRow;
        public float DeathFrameDuration => Visuals.DeathFrameDuration;
        public float DeathScale => Visuals.DeathScale;
        public float ShadowScale => Visuals.ShadowScale;
        public float ShadowOpacity => Visuals.ShadowOpacity;
        public float ShadowBottomOffset => Visuals.ShadowBottomOffset;

        public EnemyDefinition(
            string id,
            EnemyType type,
            EnemyVisualDefinition visuals,
            EnemyStatsDefinition stats,
            EnemyLocomotionDefinition locomotion,
            EnemyBehaviorDefinition behavior)
        {
            Id = id;
            Type = type;
            Visuals = visuals;
            Stats = stats;
            Locomotion = locomotion;
            Behavior = behavior;
        }
    }
}
