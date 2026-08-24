using System;
using System.Collections.Generic;

namespace _1_2D_Top_Down
{
    /// <summary>
    /// Describes the visual assets and Tiled contract used by one map biome.
    /// </summary>
    public sealed class MapThemeDefinition
    {
        public string Id { get; }
        public float Scale { get; }
        public string GroundTextureAsset { get; }
        public string GroundTilesetFile { get; }
        public string WaterTextureAsset { get; }
        public string WaterTilesetFile { get; }
        public string PropsAtlasAsset { get; }
        public string PropsTilesetFile { get; }
        public string PropsTextureAsset { get; }
        public int PropsTileWidth { get; }
        public int PropsTileHeight { get; }
        public bool UsesGridProps => !string.IsNullOrWhiteSpace(PropsTextureAsset);
        public string GroundLayerName { get; }
        public string WaterLayerName { get; }
        public string PropsLayerName { get; }
        public IReadOnlyDictionary<int, string> PropsRegionNames { get; }
        public IReadOnlyList<MapTileLayerDefinition> UnderGroundLayers { get; }
        public IReadOnlyList<MapTileLayerDefinition> OverGroundLayers { get; }
        public IReadOnlyList<MapTileLayerDefinition> ForegroundLayers { get; }

        public MapThemeDefinition(
            string id,
            float scale,
            string groundTextureAsset,
            string groundTilesetFile,
            string waterTextureAsset,
            string waterTilesetFile,
            string propsAtlasAsset,
            string propsTilesetFile,
            IReadOnlyDictionary<int, string> propsRegionNames,
            string groundLayerName = "Ground",
            string waterLayerName = "Water",
            string propsLayerName = "Props",
            IReadOnlyList<MapTileLayerDefinition> underGroundLayers = null,
            IReadOnlyList<MapTileLayerDefinition> overGroundLayers = null,
            IReadOnlyList<MapTileLayerDefinition> foregroundLayers = null)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("A map theme must have an ID.", nameof(id));
            if (scale <= 0f)
                throw new ArgumentOutOfRangeException(nameof(scale));

            Id = id;
            Scale = scale;
            GroundTextureAsset = groundTextureAsset;
            GroundTilesetFile = groundTilesetFile;
            WaterTextureAsset = waterTextureAsset;
            WaterTilesetFile = waterTilesetFile;
            PropsAtlasAsset = propsAtlasAsset;
            PropsTilesetFile = propsTilesetFile;
            PropsRegionNames = propsRegionNames;
            GroundLayerName = groundLayerName;
            WaterLayerName = waterLayerName;
            PropsLayerName = propsLayerName;
            PropsTextureAsset = string.Empty;
            UnderGroundLayers = underGroundLayers ?? Array.Empty<MapTileLayerDefinition>();
            OverGroundLayers = overGroundLayers ?? Array.Empty<MapTileLayerDefinition>();
            ForegroundLayers = foregroundLayers ?? Array.Empty<MapTileLayerDefinition>();
        }

        public MapThemeDefinition(
            string id,
            float scale,
            string groundTextureAsset,
            string groundTilesetFile,
            string waterTextureAsset,
            string waterTilesetFile,
            string propsTextureAsset,
            string propsTilesetFile,
            int propsTileWidth,
            int propsTileHeight,
            string groundLayerName = "Ground",
            string waterLayerName = "Water",
            string propsLayerName = "Props",
            IReadOnlyList<MapTileLayerDefinition> underGroundLayers = null,
            IReadOnlyList<MapTileLayerDefinition> overGroundLayers = null,
            IReadOnlyList<MapTileLayerDefinition> foregroundLayers = null)
            : this(
                id,
                scale,
                groundTextureAsset,
                groundTilesetFile,
                waterTextureAsset,
                waterTilesetFile,
                string.Empty,
                propsTilesetFile,
                new Dictionary<int, string>(),
                groundLayerName,
                waterLayerName,
                propsLayerName,
                underGroundLayers,
                overGroundLayers,
                foregroundLayers)
        {
            if (propsTileWidth <= 0 || propsTileHeight <= 0)
                throw new ArgumentOutOfRangeException(nameof(propsTileWidth));

            PropsTextureAsset = propsTextureAsset;
            PropsTileWidth = propsTileWidth;
            PropsTileHeight = propsTileHeight;
        }
    }
}
