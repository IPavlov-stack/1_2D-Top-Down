using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Tiled
{
    /// <summary>
    /// Draws a named CSV tile layer from a finite orthogonal Tiled map.
    /// A separate instance is used for each texture/tileset pair.
    /// </summary>
    public sealed class TiledTileLayer
    {
        private const uint FlipHorizontalFlag = 0x80000000;
        private const uint FlipVerticalFlag = 0x40000000;
        private const uint FlipDiagonalFlag = 0x20000000;
        private const uint TileIdMask = 0x0FFFFFFF;

        private readonly Texture2D _texture;
        private readonly TileData[] _tiles;
        private readonly int _tilesPerRow;

        public int Columns { get; }
        public int Rows { get; }
        public int SourceTileWidth { get; }
        public int SourceTileHeight { get; }
        public float Scale { get; }

        public float TileWidth => SourceTileWidth * Scale;
        public float TileHeight => SourceTileHeight * Scale;
        public float WorldWidth => Columns * TileWidth;
        public float WorldHeight => Rows * TileHeight;

        private TiledTileLayer(
            Texture2D texture,
            TileData[] tiles,
            int columns,
            int rows,
            int sourceTileWidth,
            int sourceTileHeight,
            float scale)
        {
            _texture = texture;
            _tiles = tiles;
            Columns = columns;
            Rows = rows;
            SourceTileWidth = sourceTileWidth;
            SourceTileHeight = sourceTileHeight;
            Scale = scale;
            _tilesPerRow = texture.Width / sourceTileWidth;
        }

        public static TiledTileLayer FromFile(
            ContentManager content,
            string tmxFileName,
            string textureAssetName,
            string tilesetFileName,
            float scale,
            string layerName)
        {
            string path = Path.Combine(content.RootDirectory, tmxFileName);

            using Stream stream = TitleContainer.OpenStream(path);
            XDocument document = XDocument.Load(stream);
            XElement map = document.Root
                ?? throw new InvalidDataException("The TMX file has no <map> element.");

            int mapWidth = ReadInt(map, "width");
            int mapHeight = ReadInt(map, "height");
            int tileWidth = ReadInt(map, "tilewidth");
            int tileHeight = ReadInt(map, "tileheight");

            XElement tileset = map.Elements("tileset")
                .FirstOrDefault(element =>
                    ((string)element.Attribute("source") ?? string.Empty)
                    .EndsWith(tilesetFileName, StringComparison.OrdinalIgnoreCase))
                ?? throw new InvalidDataException($"The map has no tileset '{tilesetFileName}'.");

            int firstGid = ReadInt(tileset, "firstgid");

            XElement layer = map.Elements("layer")
                .FirstOrDefault(element => (string)element.Attribute("name") == layerName)
                ?? throw new InvalidDataException($"The TMX file has no layer named '{layerName}'.");

            string csv = layer.Element("data")?.Value
                ?? throw new InvalidDataException($"Layer '{layerName}' has no CSV data.");

            string[] values = csv.Split(
                new[] { ',', '\r', '\n', ' ', '\t' },
                StringSplitOptions.RemoveEmptyEntries);

            int expectedTileCount = mapWidth * mapHeight;
            if (values.Length != expectedTileCount)
                throw new InvalidDataException($"Layer '{layerName}' contains {values.Length} tiles; expected {expectedTileCount}.");

            TileData[] tiles = new TileData[expectedTileCount];
            for (int index = 0; index < values.Length; index++)
            {
                uint globalId = uint.Parse(values[index], CultureInfo.InvariantCulture);
                tiles[index] = TileData.FromGlobalId(globalId, firstGid);
            }

            return new TiledTileLayer(
                content.Load<Texture2D>(textureAssetName),
                tiles,
                mapWidth,
                mapHeight,
                tileWidth,
                tileHeight,
                scale);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 origin = new(SourceTileWidth / 2f, SourceTileHeight / 2f);

            for (int index = 0; index < _tiles.Length; index++)
            {
                TileData tile = _tiles[index];
                if (tile.LocalId < 0)
                    continue;

                int sourceColumn = tile.LocalId % _tilesPerRow;
                int sourceRow = tile.LocalId / _tilesPerRow;
                Rectangle source = new(
                    sourceColumn * SourceTileWidth,
                    sourceRow * SourceTileHeight,
                    SourceTileWidth,
                    SourceTileHeight);

                int mapColumn = index % Columns;
                int mapRow = index / Columns;
                Vector2 position = new(
                    mapColumn * TileWidth + TileWidth / 2f,
                    mapRow * TileHeight + TileHeight / 2f);

                GetTransform(tile, out float rotation, out SpriteEffects effects);
                spriteBatch.Draw(_texture, position, source, Color.White, rotation, origin, Scale, effects, 0f);
            }
        }

        private static int ReadInt(XElement element, string attributeName)
        {
            string value = (string)element.Attribute(attributeName)
                ?? throw new InvalidDataException($"Missing '{attributeName}' attribute.");
            return int.Parse(value, CultureInfo.InvariantCulture);
        }

        private static void GetTransform(TileData tile, out float rotation, out SpriteEffects effects)
        {
            rotation = 0f;
            effects = SpriteEffects.None;

            if (!tile.FlipDiagonal)
            {
                if (tile.FlipHorizontal) effects |= SpriteEffects.FlipHorizontally;
                if (tile.FlipVertical) effects |= SpriteEffects.FlipVertically;
                return;
            }

            if (tile.FlipHorizontal && tile.FlipVertical)
            {
                rotation = MathHelper.PiOver2;
                effects = SpriteEffects.FlipHorizontally;
            }
            else if (tile.FlipHorizontal)
                rotation = MathHelper.PiOver2;
            else if (tile.FlipVertical)
                rotation = -MathHelper.PiOver2;
            else
            {
                rotation = -MathHelper.PiOver2;
                effects = SpriteEffects.FlipHorizontally;
            }
        }

        private readonly struct TileData
        {
            public int LocalId { get; }
            public bool FlipHorizontal { get; }
            public bool FlipVertical { get; }
            public bool FlipDiagonal { get; }

            private TileData(int localId, bool flipHorizontal, bool flipVertical, bool flipDiagonal)
            {
                LocalId = localId;
                FlipHorizontal = flipHorizontal;
                FlipVertical = flipVertical;
                FlipDiagonal = flipDiagonal;
            }

            public static TileData FromGlobalId(uint globalId, int firstGid)
            {
                if (globalId == 0)
                    return new TileData(-1, false, false, false);

                return new TileData(
                    (int)(globalId & TileIdMask) - firstGid,
                    (globalId & FlipHorizontalFlag) != 0,
                    (globalId & FlipVerticalFlag) != 0,
                    (globalId & FlipDiagonalFlag) != 0);
            }
        }
    }
}