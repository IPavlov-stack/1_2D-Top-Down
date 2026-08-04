using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Tiled
{
    /// <summary>
    /// Turns collision rectangles defined on water tiles in a TSX file into
    /// world-space rectangles. A water tile is ignored when Ground has a tile
    /// at the same cell, because Water is the background layer of this map.
    /// </summary>
    public sealed class TiledWaterCollisionLayer
    {
        private const uint FlipHorizontalFlag = 0x80000000;
        private const uint FlipVerticalFlag = 0x40000000;
        private const uint FlipDiagonalFlag = 0x20000000;
        private const uint TileIdMask = 0x0FFFFFFF;

        private readonly List<Rectangle> _rectangles;
        public IReadOnlyList<Rectangle> Rectangles => _rectangles;

        private TiledWaterCollisionLayer(List<Rectangle> rectangles)
        {
            _rectangles = rectangles;
        }

        public static TiledWaterCollisionLayer FromFile(
            ContentManager content,
            string tmxFileName,
            string waterTilesetFileName,
            float mapScale,
            string waterLayerName = "Water",
            string groundLayerName = "Ground")
        {
            string mapPath = Path.Combine(content.RootDirectory, tmxFileName);
            using Stream mapStream = TitleContainer.OpenStream(mapPath);
            XDocument mapDocument = XDocument.Load(mapStream);
            XElement map = mapDocument.Root
                ?? throw new InvalidDataException("The TMX file has no <map> element.");

            int columns = ReadInt(map, "width");
            int rows = ReadInt(map, "height");
            int tileWidth = ReadInt(map, "tilewidth");
            int tileHeight = ReadInt(map, "tileheight");

            XElement waterTileset = map.Elements("tileset")
                .FirstOrDefault(element =>
                    ((string)element.Attribute("source") ?? string.Empty)
                    .EndsWith(waterTilesetFileName, StringComparison.OrdinalIgnoreCase))
                ?? throw new InvalidDataException($"The map has no tileset '{waterTilesetFileName}'.");

            int waterFirstGid = ReadInt(waterTileset, "firstgid");
            string tilesetPath = Path.Combine(
                content.RootDirectory,
                Path.GetDirectoryName(tmxFileName) ?? string.Empty,
                waterTilesetFileName);

            using Stream tilesetStream = TitleContainer.OpenStream(tilesetPath);
            XDocument tilesetDocument = XDocument.Load(tilesetStream);
            Dictionary<int, List<LocalRectangle>> localCollisions = ReadTileCollisions(tilesetDocument.Root!);

            uint[] waterTiles = ReadLayer(map, waterLayerName, columns * rows);
            uint[] groundTiles = ReadLayer(map, groundLayerName, columns * rows);
            List<Rectangle> rectangles = new();

            for (int index = 0; index < waterTiles.Length; index++)
            {
                // Ground covers water visually, so it must also make that cell walkable.
                if ((groundTiles[index] & TileIdMask) != 0)
                    continue;

                uint waterGid = waterTiles[index];
                uint cleanGid = waterGid & TileIdMask;
                if (cleanGid < waterFirstGid)
                    continue;

                int localTileId = (int)cleanGid - waterFirstGid;
                if (!localCollisions.TryGetValue(localTileId, out List<LocalRectangle> localRectangles))
                    continue;

                int column = index % columns;
                int row = index / columns;

                foreach (LocalRectangle local in localRectangles)
                {
                    LocalRectangle transformed = Transform(local, waterGid, tileWidth, tileHeight);
                    rectangles.Add(new Rectangle(
                        Round((column * tileWidth + transformed.X) * mapScale),
                        Round((row * tileHeight + transformed.Y) * mapScale),
                        Math.Max(1, Round(transformed.Width * mapScale)),
                        Math.Max(1, Round(transformed.Height * mapScale))));
                }
            }

            return new TiledWaterCollisionLayer(rectangles);
        }

        private static Dictionary<int, List<LocalRectangle>> ReadTileCollisions(XElement tileset)
        {
            Dictionary<int, List<LocalRectangle>> result = new();

            foreach (XElement tile in tileset.Elements("tile"))
            {
                XElement objectGroup = tile.Element("objectgroup");
                if (objectGroup == null)
                    continue;

                int tileId = ReadInt(tile, "id");
                List<LocalRectangle> rectangles = new();

                foreach (XElement element in objectGroup.Elements("object"))
                {
                    float width = ReadFloat(element, "width");
                    float height = ReadFloat(element, "height");

                    // This first version deliberately supports rectangular tile collisions.
                    if (width <= 0f || height <= 0f || element.Element("polygon") != null || element.Element("ellipse") != null)
                        continue;

                    rectangles.Add(new LocalRectangle(
                        ReadFloat(element, "x"),
                        ReadFloat(element, "y"),
                        width,
                        height));
                }

                if (rectangles.Count > 0)
                    result[tileId] = rectangles;
            }

            return result;
        }

        private static uint[] ReadLayer(XElement map, string layerName, int expectedCount)
        {
            XElement layer = map.Elements("layer")
                .FirstOrDefault(element => (string)element.Attribute("name") == layerName)
                ?? throw new InvalidDataException($"The TMX file has no layer named '{layerName}'.");

            string csv = layer.Element("data")?.Value
                ?? throw new InvalidDataException($"Layer '{layerName}' has no CSV data.");

            string[] values = csv.Split(
                new[] { ',', '\r', '\n', ' ', '\t' },
                StringSplitOptions.RemoveEmptyEntries);

            if (values.Length != expectedCount)
                throw new InvalidDataException($"Layer '{layerName}' contains {values.Length} tiles; expected {expectedCount}.");

            uint[] tiles = new uint[expectedCount];
            for (int index = 0; index < values.Length; index++)
                tiles[index] = uint.Parse(values[index], CultureInfo.InvariantCulture);

            return tiles;
        }

        private static LocalRectangle Transform(LocalRectangle rectangle, uint gid, int tileWidth, int tileHeight)
        {
            float x = rectangle.X;
            float y = rectangle.Y;
            float width = rectangle.Width;
            float height = rectangle.Height;

            // Tiled applies diagonal flip first, then horizontal and vertical flips.
            if ((gid & FlipDiagonalFlag) != 0)
            {
                (x, y) = (y, x);
                (width, height) = (height, width);
            }

            if ((gid & FlipHorizontalFlag) != 0)
                x = tileWidth - x - width;

            if ((gid & FlipVerticalFlag) != 0)
                y = tileHeight - y - height;

            return new LocalRectangle(x, y, width, height);
        }

        private static int ReadInt(XElement element, string attributeName)
        {
            string value = (string)element.Attribute(attributeName)
                ?? throw new InvalidDataException($"Missing '{attributeName}' attribute.");
            return int.Parse(value, CultureInfo.InvariantCulture);
        }

        private static float ReadFloat(XElement element, string attributeName)
        {
            string value = (string)element.Attribute(attributeName) ?? "0";
            return float.Parse(value, CultureInfo.InvariantCulture);
        }

        private static int Round(float value) => (int)MathF.Round(value);

        private readonly struct LocalRectangle
        {
            public float X { get; }
            public float Y { get; }
            public float Width { get; }
            public float Height { get; }

            public LocalRectangle(float x, float y, float width, float height)
            {
                X = x;
                Y = y;
                Width = width;
                Height = height;
            }
        }
    }
}