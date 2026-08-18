namespace _1_2D_Top_Down
{
    /// <summary>
    /// Controls which developer visualizations are shown while developer mode is active.
    /// </summary>
    public sealed class DeveloperViewSettings
    {
        public bool ShowCollisions { get; set; } = true;
        public bool ShowEntityBounds { get; set; } = true;
        public bool ShowProjectileBounds { get; set; } = true;
        public bool ShowCollectibleBounds { get; set; } = true;
        public bool ShowSpawnPoints { get; set; }
        public bool ShowHud { get; set; } = true;
    }
}
