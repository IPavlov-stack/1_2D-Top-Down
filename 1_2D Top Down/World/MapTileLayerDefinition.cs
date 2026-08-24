using System;

namespace _1_2D_Top_Down
{
    /// <summary>
    /// Connects one Tiled tile layer to its texture and external TSX file.
    /// </summary>
    public sealed class MapTileLayerDefinition
    {
        public string LayerName { get; }
        public string TextureAsset { get; }
        public string TilesetFile { get; }

        public MapTileLayerDefinition(
            string layerName,
            string textureAsset,
            string tilesetFile)
        {
            if (string.IsNullOrWhiteSpace(layerName))
                throw new ArgumentException("A tile layer must have a name.", nameof(layerName));
            if (string.IsNullOrWhiteSpace(textureAsset))
                throw new ArgumentException("A tile layer must have a texture.", nameof(textureAsset));
            if (string.IsNullOrWhiteSpace(tilesetFile))
                throw new ArgumentException("A tile layer must have a tileset.", nameof(tilesetFile));

            LayerName = layerName;
            TextureAsset = textureAsset;
            TilesetFile = tilesetFile;
        }
    }
}
