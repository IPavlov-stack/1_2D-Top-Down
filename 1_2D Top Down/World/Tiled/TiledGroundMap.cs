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
    /// Draws one CSV tile layer from a finite orthogonal Tiled (.tmx) map.
    /// The map may contain rotated and flipped tiles.
    /// </summary>
    public sealed class TiledGroundMap
    {
        private const uint FlippedHorizontallyFlag = 0x80000000;
        private const uint FlippedVerticallyFlag = 0x40000000;
        private const uint FlippedDiagonallyFlag = 0x20000000;
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

        private TiledGroundMap(
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

        public static TiledGroundMap FromFile(
            ContentManager content,
            string tmxFileName,
            string textureAssetName,
            float scale = 1f,
            string layerName = "Ground")
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

            XElement tileset = map.Element("tileset")
                ?? throw new InvalidDataException("The TMX file has no tileset.");
            int firstGid = ReadInt(tileset, "firstgid");

            XElement layer = map.Elements("layer")
                .FirstOrDefault(element => (string)element.Attribute("name") == layerName)
                ?? throw new InvalidDataException($"The TMX file has no layer named '{layerName}'.");

            string csv = layer.Element("data")?.Value
                ?? throw new InvalidDataException("The map layer has no CSV data.");

            string[] values = csv.Split(
                new[] { ',', '\r', '\n', ' ', '\t' },
                StringSplitOptions.RemoveEmptyEntries);

            int expectedTileCount = mapWidth * mapHeight;
            if (values.Length != expectedTileCount)
            {
                throw new InvalidDataException(
                    $"Layer '{layerName}' contains {values.Length} tiles; expected {expectedTileCount}.");
            }

            TileData[] tiles = new TileData[expectedTileCount];
            for (int index = 0; index < values.Length; index++)
            {
                uint gid = uint.Parse(values[index], CultureInfo.InvariantCulture);
                tiles[index] = TileData.FromGlobalId(gid, firstGid);
            }

            Texture2D texture = content.Load<Texture2D>(textureAssetName);
            return new TiledGroundMap(texture, tiles, mapWidth, mapHeight, tileWidth, tileHeight, scale);
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
                spriteBatch.Draw(
                    _texture,
                    position,
                    source,
                    Color.White,
                    rotation,
                    origin,
                    Scale,
                    effects,
                    0f);
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
                if (tile.FlipHorizontal)
                    effects |= SpriteEffects.FlipHorizontally;
                if (tile.FlipVertical)
                    effects |= SpriteEffects.FlipVertically;
                return;
            }

            // Tiled applies the diagonal flip first, then the horizontal and
            // vertical flips. SpriteBatch uses a different coordinate
            // convention for this operation, so the diagonal flip is handled
            // as a transpose (swap of the two axes) here.
            if (tile.FlipHorizontal && tile.FlipVertical)
            {
                rotation = MathHelper.PiOver2;
                effects = SpriteEffects.FlipHorizontally;
            }
            else if (tile.FlipHorizontal)
            {
                rotation = MathHelper.PiOver2;
            }
            else if (tile.FlipVertical)
            {
                rotation = -MathHelper.PiOver2;
            }
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

            public static TileData FromGlobalId(uint gid, int firstGid)
            {
                if (gid == 0)
                    return new TileData(-1, false, false, false);

                bool flipHorizontal = (gid & FlippedHorizontallyFlag) != 0;
                bool flipVertical = (gid & FlippedVerticallyFlag) != 0;
                bool flipDiagonal = (gid & FlippedDiagonallyFlag) != 0;
                int localId = (int)(gid & TileIdMask) - firstGid;

                return new TileData(localId, flipHorizontal, flipVertical, flipDiagonal);
            }
        }
    }
}