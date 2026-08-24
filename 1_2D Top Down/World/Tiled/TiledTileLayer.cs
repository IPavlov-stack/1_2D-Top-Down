using System;
using System.Collections.Generic;
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
    /// Tilesets are resolved per GID, so one layer may mix tilesets just like Tiled.
    /// </summary>
    public sealed class TiledTileLayer
    {
        private const uint FlipHorizontalFlag = 0x80000000;
        private const uint FlipVerticalFlag = 0x40000000;
        private const uint FlipDiagonalFlag = 0x20000000;
        private const uint TileIdMask = 0x0FFFFFFF;

        private readonly TileData[] _tiles;
        private readonly TilesetData[] _tilesets;
        private double _animationTimeMilliseconds;

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
            TileData[] tiles,
            TilesetData[] tilesets,
            int columns,
            int rows,
            int sourceTileWidth,
            int sourceTileHeight,
            float scale)
        {
            _tiles = tiles;
            _tilesets = tilesets;
            Columns = columns;
            Rows = rows;
            SourceTileWidth = sourceTileWidth;
            SourceTileHeight = sourceTileHeight;
            Scale = scale;
        }

        public static TiledTileLayer FromFile(
            ContentManager content,
            string tmxFileName,
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
            TilesetData[] tilesets = LoadTilesets(content, path, map);

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
                tiles[index] = TileData.FromGlobalId(uint.Parse(values[index], CultureInfo.InvariantCulture));

            return new TiledTileLayer(
                tiles,
                tilesets,
                mapWidth,
                mapHeight,
                tileWidth,
                tileHeight,
                scale);
        }

        public static TiledTileLayer FromFile(
            ContentManager content,
            string tmxFileName,
            string textureAssetName,
            string tilesetFileName,
            float scale,
            string layerName)
        {
            return FromFile(content, tmxFileName, scale, layerName);
        }

        public void Update(GameTime gameTime)
        {
            _animationTimeMilliseconds = gameTime.TotalGameTime.TotalMilliseconds;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 origin = new(SourceTileWidth / 2f, SourceTileHeight / 2f);

            for (int index = 0; index < _tiles.Length; index++)
            {
                TileData tile = _tiles[index];
                if (tile.GlobalId == 0)
                    continue;

                TilesetData tileset = ResolveTileset(tile.GlobalId);
                int localId = tileset.ResolveLocalId(tile.GlobalId - tileset.FirstGid, _animationTimeMilliseconds);
                int sourceColumn = localId % tileset.TilesPerRow;
                int sourceRow = localId / tileset.TilesPerRow;
                Rectangle source = new(
                    sourceColumn * tileset.TileWidth,
                    sourceRow * tileset.TileHeight,
                    tileset.TileWidth,
                    tileset.TileHeight);

                int mapColumn = index % Columns;
                int mapRow = index / Columns;
                Vector2 position = new(
                    mapColumn * TileWidth + TileWidth / 2f,
                    mapRow * TileHeight + TileHeight / 2f);

                GetTransform(tile, out float rotation, out SpriteEffects effects);
                spriteBatch.Draw(tileset.Texture, position, source, Color.White, rotation, origin, Scale, effects, 0f);
            }
        }

        private static int ReadInt(XElement element, string attributeName)
        {
            string value = (string)element.Attribute(attributeName)
                ?? throw new InvalidDataException($"Missing '{attributeName}' attribute.");
            return int.Parse(value, CultureInfo.InvariantCulture);
        }

        private TilesetData ResolveTileset(int globalId)
        {
            for (int index = _tilesets.Length - 1; index >= 0; index--)
            {
                if (globalId >= _tilesets[index].FirstGid && globalId <= _tilesets[index].LastGid)
                    return _tilesets[index];
            }

            throw new InvalidDataException($"Tile GID {globalId} does not belong to a map tileset.");
        }

        private static TilesetData[] LoadTilesets(
            ContentManager content,
            string mapPath,
            XElement map)
        {
            string mapDirectory = Path.GetDirectoryName(mapPath) ?? string.Empty;
            string contentRoot = Path.GetFullPath(content.RootDirectory);
            List<TilesetData> result = new();

            foreach (XElement mapTileset in map.Elements("tileset"))
            {
                string tilesetSource = (string)mapTileset.Attribute("source");
                string tilesetDirectory = mapDirectory;
                XElement root;

                if (!string.IsNullOrWhiteSpace(tilesetSource))
                {
                    string tilesetPath = Path.Combine(mapDirectory, tilesetSource);
                    using Stream stream = TitleContainer.OpenStream(tilesetPath);
                    XDocument document = XDocument.Load(stream);
                    root = document.Root
                        ?? throw new InvalidDataException($"Tileset '{tilesetSource}' has no root element.");
                    tilesetDirectory = Path.GetDirectoryName(tilesetPath) ?? string.Empty;
                }
                else
                {
                    root = mapTileset;
                }

                string tilesetName = (string)root.Attribute("name") ?? "unnamed";
                // Image-collection tilesets are used by the legacy Props object
                // layer and are rendered by TiledPropsLayer, not tile layers.
                XElement image = root.Element("image");
                if (image == null)
                    continue;
                string imageSource = (string)image.Attribute("source")
                    ?? throw new InvalidDataException($"Tileset '{tilesetName}' image has no source.");
                string imagePath = Path.GetFullPath(Path.Combine(
                    tilesetDirectory,
                    imageSource));
                string assetName = ResolveTextureAssetName(
                    contentRoot,
                    imagePath,
                    tilesetName);

                result.Add(new TilesetData(
                    ReadInt(mapTileset, "firstgid"),
                    ReadInt(root, "tilecount"),
                    ReadInt(root, "tilewidth"),
                    ReadInt(root, "tileheight"),
                    content.Load<Texture2D>(assetName),
                    ReadAnimations(root)));
            }

            return result.OrderBy(tileset => tileset.FirstGid).ToArray();
        }

        private static string ResolveTextureAssetName(
            string contentRoot,
            string imagePath,
            string tilesetName)
        {
            string relativePath = Path.GetRelativePath(contentRoot, imagePath);
            if (!relativePath.StartsWith(".." + Path.DirectorySeparatorChar, StringComparison.Ordinal) &&
                !Path.IsPathRooted(relativePath))
            {
                return Path.ChangeExtension(relativePath, null)!.Replace('\\', '/');
            }

            Dictionary<string, string> chapter1Assets = new(StringComparer.OrdinalIgnoreCase)
            {
                ["Water_detilazation"] = "Environment/Chapter1/WaterDetails",
                ["Water_detilazation2"] = "Environment/Chapter1/WaterDetails2",
                ["Water_coasts"] = "Environment/Chapter1/Water",
                ["Ground_grass"] = "Environment/Chapter1/Ground",
                ["spots"] = "Environment/Chapter1/Spots",
                ["stairs_grass"] = "Environment/Chapter1/GrassStairs",
                ["spots_rock"] = "Environment/Chapter1/RockSpots",
                ["lianas"] = "Environment/Chapter1/Lianas",
                ["Objects"] = "Environment/Chapter1/Objects"
            };

            if (chapter1Assets.TryGetValue(tilesetName, out string assetName))
                return assetName;

            throw new InvalidDataException(
                $"Inline tileset '{tilesetName}' points outside Content and has no registered asset mapping.");
        }

        private static IReadOnlyDictionary<int, TileAnimation> ReadAnimations(XElement tileset)
        {
            Dictionary<int, TileAnimation> animations = new();
            foreach (XElement tile in tileset.Elements("tile"))
            {
                XElement animationElement = tile.Element("animation");
                if (animationElement == null)
                    continue;

                AnimationFrame[] frames = animationElement.Elements("frame")
                    .Select(frame => new AnimationFrame(
                        ReadInt(frame, "tileid"),
                        ReadInt(frame, "duration")))
                    .ToArray();
                if (frames.Length > 0)
                    animations.Add(ReadInt(tile, "id"), new TileAnimation(frames));
            }

            return animations;
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
            public int GlobalId { get; }
            public bool FlipHorizontal { get; }
            public bool FlipVertical { get; }
            public bool FlipDiagonal { get; }

            private TileData(int globalId, bool flipHorizontal, bool flipVertical, bool flipDiagonal)
            {
                GlobalId = globalId;
                FlipHorizontal = flipHorizontal;
                FlipVertical = flipVertical;
                FlipDiagonal = flipDiagonal;
            }

            public static TileData FromGlobalId(uint globalId)
            {
                if (globalId == 0)
                    return new TileData(0, false, false, false);

                return new TileData(
                    (int)(globalId & TileIdMask),
                    (globalId & FlipHorizontalFlag) != 0,
                    (globalId & FlipVerticalFlag) != 0,
                    (globalId & FlipDiagonalFlag) != 0);
            }
        }

        private sealed class TilesetData
        {
            public int FirstGid { get; }
            public int LastGid { get; }
            public int TileWidth { get; }
            public int TileHeight { get; }
            public Texture2D Texture { get; }
            public int TilesPerRow => Texture.Width / TileWidth;
            private IReadOnlyDictionary<int, TileAnimation> Animations { get; }

            public TilesetData(
                int firstGid,
                int tileCount,
                int tileWidth,
                int tileHeight,
                Texture2D texture,
                IReadOnlyDictionary<int, TileAnimation> animations)
            {
                FirstGid = firstGid;
                LastGid = firstGid + tileCount - 1;
                TileWidth = tileWidth;
                TileHeight = tileHeight;
                Texture = texture;
                Animations = animations;
            }

            public int ResolveLocalId(int localId, double animationTimeMilliseconds)
            {
                if (!Animations.TryGetValue(localId, out TileAnimation animation))
                    return localId;

                int elapsed = (int)(animationTimeMilliseconds % animation.TotalDuration);
                foreach (AnimationFrame frame in animation.Frames)
                {
                    if (elapsed < frame.Duration)
                        return frame.TileId;
                    elapsed -= frame.Duration;
                }

                return animation.Frames[animation.Frames.Length - 1].TileId;
            }
        }

        private readonly struct AnimationFrame
        {
            public int TileId { get; }
            public int Duration { get; }

            public AnimationFrame(int tileId, int duration)
            {
                TileId = tileId;
                Duration = duration;
            }
        }

        private sealed class TileAnimation
        {
            public AnimationFrame[] Frames { get; }
            public int TotalDuration { get; }

            public TileAnimation(AnimationFrame[] frames)
            {
                Frames = frames;
                TotalDuration = frames.Sum(frame => frame.Duration);

                if (TotalDuration <= 0)
                    throw new InvalidDataException("A tile animation must have a positive duration.");
            }
        }
    }
}
