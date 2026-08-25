using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;
using _1_2D_Top_Down;

namespace Tiled
{
    /// <summary>
    /// Reads tile objects from the Props object layer of a Tiled map and draws
    /// them from an atlas, a grid tileset, or individual image-collection tiles.
    /// </summary>
    public sealed class TiledPropsLayer
    {
        private const uint TileIdMask = 0x0FFFFFFF;

        private readonly TextureAtlas _atlas;
        private readonly Texture2D _gridTexture;
        private readonly int _gridTileWidth;
        private readonly int _gridTileHeight;
        private readonly List<PropObject> _props;
        private readonly List<StaticCollisionShape> _collisionShapes;
        private readonly List<IYSortedWorldDrawable> _ySortedProps = new();
        private readonly float _mapScale;

        public IReadOnlyList<StaticCollisionShape> CollisionShapes => _collisionShapes;

        private TiledPropsLayer(TextureAtlas atlas, List<PropObject> props, float mapScale)
        {
            _atlas = atlas;
            _props = props;
            _collisionShapes = new List<StaticCollisionShape>();
            _mapScale = mapScale;
            BuildYSortedProps();
        }

        private TiledPropsLayer(
            Texture2D gridTexture,
            int gridTileWidth,
            int gridTileHeight,
            List<PropObject> props,
            List<StaticCollisionShape> collisionShapes,
            float mapScale)
        {
            _gridTexture = gridTexture;
            _gridTileWidth = gridTileWidth;
            _gridTileHeight = gridTileHeight;
            _props = props;
            _collisionShapes = collisionShapes;
            _mapScale = mapScale;
            BuildYSortedProps();
        }

        public static TiledPropsLayer FromFile(
            ContentManager content,
            string tmxFileName,
            TextureAtlas atlas,
            float mapScale,
            string propsTilesetFileName,
            IReadOnlyDictionary<int, string> regionNames,
            string layerName = "Props")
        {
            string path = Path.Combine(content.RootDirectory, tmxFileName);

            using Stream stream = TitleContainer.OpenStream(path);
            XDocument document = XDocument.Load(stream);
            XElement map = document.Root
                ?? throw new InvalidDataException("The TMX file has no <map> element.");

            XElement propsTileset = FindPropsTileset(map, propsTilesetFileName);
            int firstGid = ReadInt(propsTileset, "firstgid");

            XElement objectGroup = map.Elements("objectgroup")
                .FirstOrDefault(element => (string)element.Attribute("name") == layerName)
                ?? throw new InvalidDataException($"The TMX file has no object layer named '{layerName}'.");

            List<PropObject> props = new();
            foreach (XElement element in objectGroup.Elements("object"))
            {
                string gidText = (string)element.Attribute("gid");
                if (string.IsNullOrWhiteSpace(gidText))
                    continue;

                uint globalId = uint.Parse(gidText, CultureInfo.InvariantCulture);
                int tileId = (int)(globalId & TileIdMask) - firstGid;

                if (!regionNames.TryGetValue(tileId, out string regionName))
                    continue;

                props.Add(new PropObject(
                    regionName,
                    ReadDrawMode(element),
                    ReadFloat(element, "x"),
                    ReadFloat(element, "y"),
                    ReadFloat(element, "width"),
                    ReadFloat(element, "height")));
            }

            return new TiledPropsLayer(atlas, props, mapScale);
        }

        public static TiledPropsLayer FromGridFile(
            ContentManager content,
            string tmxFileName,
            string textureAssetName,
            int sourceTileWidth,
            int sourceTileHeight,
            float mapScale,
            string propsTilesetFileName,
            string layerName = "Props")
        {
            string path = Path.Combine(content.RootDirectory, tmxFileName);
            using Stream stream = TitleContainer.OpenStream(path);
            XDocument document = XDocument.Load(stream);
            XElement map = document.Root
                ?? throw new InvalidDataException("The TMX file has no <map> element.");
            XElement propsTileset = FindPropsTileset(map, propsTilesetFileName);
            int firstGid = ReadInt(propsTileset, "firstgid");
            int nextFirstGid = map.Elements("tileset")
                .Select(element => ReadInt(element, "firstgid"))
                .Where(value => value > firstGid)
                .DefaultIfEmpty(int.MaxValue)
                .Min();
            XElement objectGroup = map.Elements("objectgroup")
                .FirstOrDefault(element => (string)element.Attribute("name") == layerName)
                ?? throw new InvalidDataException(
                    $"The TMX file has no object layer named '{layerName}'.");
            IReadOnlyDictionary<uint, ImageCollectionTile> imageCollectionTiles =
                LoadImageCollectionTiles(content, path, map, objectGroup);

            List<PropObject> props = new();
            List<StaticCollisionShape> collisionShapes = new();
            foreach (XElement element in objectGroup.Elements("object"))
            {
                string gidText = (string)element.Attribute("gid");
                if (string.IsNullOrWhiteSpace(gidText))
                    continue;

                uint globalId = uint.Parse(gidText, CultureInfo.InvariantCulture);
                uint cleanGlobalId = globalId & TileIdMask;
                if (cleanGlobalId >= firstGid && cleanGlobalId < nextFirstGid)
                {
                    int tileId = (int)cleanGlobalId - firstGid;
                    props.Add(new PropObject(
                        tileId,
                        ReadDrawMode(element),
                        ReadFloat(element, "x"),
                        ReadFloat(element, "y"),
                        ReadFloat(element, "width"),
                        ReadFloat(element, "height")));
                    continue;
                }

                if (imageCollectionTiles.TryGetValue(cleanGlobalId, out ImageCollectionTile tile))
                {
                    float x = ReadFloat(element, "x");
                    float y = ReadFloat(element, "y");
                    float width = ReadFloat(element, "width");
                    float height = ReadFloat(element, "height");
                    props.Add(new PropObject(
                        tile.Texture,
                        ReadDrawMode(element),
                        x,
                        y,
                        width,
                        height));
                    AddCollisionShapes(
                        collisionShapes,
                        tile,
                        x,
                        y,
                        width,
                        height,
                        mapScale);
                }
            }

            return new TiledPropsLayer(
                content.Load<Texture2D>(textureAssetName),
                sourceTileWidth,
                sourceTileHeight,
                props,
                collisionShapes,
                mapScale);
        }

        public void DrawBehind(SpriteBatch spriteBatch)
        {
            foreach (PropObject prop in _props)
            {
                if (prop.DrawMode == PropDrawMode.Behind)
                    DrawProp(spriteBatch, prop);
            }
        }

        public void AddYSortedItems(WorldRenderQueue renderQueue)
        {
            foreach (IYSortedWorldDrawable prop in _ySortedProps)
                renderQueue.Add(prop);
        }

        public void DrawInFront(SpriteBatch spriteBatch)
        {
            foreach (PropObject prop in _props)
            {
                if (prop.DrawMode == PropDrawMode.Front)
                    DrawProp(spriteBatch, prop);
            }
        }

        private void BuildYSortedProps()
        {
            foreach (PropObject prop in _props)
            {
                if (prop.DrawMode == PropDrawMode.YSort)
                    _ySortedProps.Add(new YSortedProp(this, prop));
            }
        }

        private void DrawProp(SpriteBatch spriteBatch, PropObject prop)
        {
            // A tile object in an orthogonal Tiled map is positioned from
            // its bottom-left corner. SpriteBatch expects top-left here.
            Rectangle destination = new(
                Round(prop.X * _mapScale),
                Round((prop.Y - prop.Height) * _mapScale),
                Math.Max(1, Round(prop.Width * _mapScale)),
                Math.Max(1, Round(prop.Height * _mapScale)));

            if (prop.ImageTexture != null)
            {
                spriteBatch.Draw(prop.ImageTexture, destination, Color.White);
                return;
            }

            if (_gridTexture != null)
            {
                int tilesPerRow = _gridTexture.Width / _gridTileWidth;
                Rectangle source = new(
                    prop.TileId % tilesPerRow * _gridTileWidth,
                    prop.TileId / tilesPerRow * _gridTileHeight,
                    _gridTileWidth,
                    _gridTileHeight);
                spriteBatch.Draw(_gridTexture, destination, source, Color.White);
                return;
            }

            TextureRegion region = _atlas.GetRegion(prop.RegionName);
            spriteBatch.Draw(region.Texture, destination, region.SourceRectangle, Color.White);
        }

        private static IReadOnlyDictionary<uint, ImageCollectionTile> LoadImageCollectionTiles(
            ContentManager content,
            string mapPath,
            XElement map,
            XElement objectGroup)
        {
            Dictionary<uint, ImageCollectionTile> tiles = new();
            string mapDirectory = Path.GetDirectoryName(mapPath) ?? string.Empty;
            string contentRoot = Path.GetFullPath(content.RootDirectory);
            HashSet<uint> usedGlobalIds = objectGroup.Elements("object")
                .Select(element => (string)element.Attribute("gid"))
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Select(value => uint.Parse(value, CultureInfo.InvariantCulture) & TileIdMask)
                .ToHashSet();

            foreach (XElement mapTileset in map.Elements("tileset"))
            {
                uint firstGid = (uint)ReadInt(mapTileset, "firstgid");
                string tilesetSource = (string)mapTileset.Attribute("source") ?? string.Empty;
                string tilesetDirectory = mapDirectory;
                XElement tilesetRoot = mapTileset;

                if (!string.IsNullOrWhiteSpace(tilesetSource))
                {
                    string tilesetPath = Path.Combine(mapDirectory, tilesetSource);
                    using Stream tilesetStream = TitleContainer.OpenStream(tilesetPath);
                    XDocument tilesetDocument = XDocument.Load(tilesetStream);
                    tilesetRoot = tilesetDocument.Root
                        ?? throw new InvalidDataException(
                            $"Tileset '{tilesetSource}' has no <tileset> element.");
                    tilesetDirectory = Path.GetDirectoryName(tilesetPath) ?? string.Empty;
                }

                foreach (XElement tile in tilesetRoot.Elements("tile"))
                {
                    XElement image = tile.Element("image");
                    if (image == null)
                        continue;

                    string imageSource = (string)image.Attribute("source")
                        ?? throw new InvalidDataException(
                            $"An image tile in '{tilesetSource}' has no source.");
                    uint tileId = (uint)ReadInt(tile, "id");
                    uint globalId = firstGid + tileId;
                    if (!usedGlobalIds.Contains(globalId))
                        continue;

                    string imagePath = Path.GetFullPath(
                        Path.Combine(tilesetDirectory, imageSource));
                    string relativeImagePath = Path.GetRelativePath(contentRoot, imagePath);
                    if (relativeImagePath.StartsWith("..", StringComparison.Ordinal))
                    {
                        throw new InvalidDataException(
                            $"Image-collection asset '{imageSource}' must be inside the Content directory.");
                    }

                    string assetName = Path.ChangeExtension(relativeImagePath, null)
                        .Replace('\\', '/');
                    Texture2D texture = content.Load<Texture2D>(assetName);
                    int imageWidth = ReadOptionalInt(image, "width", texture.Width);
                    int imageHeight = ReadOptionalInt(image, "height", texture.Height);
                    tiles[globalId] = new ImageCollectionTile(
                        texture,
                        imageWidth,
                        imageHeight,
                        ReadLocalCollisionRectangles(tile),
                        ReadLocalCollisionPolygons(tile));
                }
            }

            return tiles;
        }

        private static IReadOnlyList<LocalRectangle> ReadLocalCollisionRectangles(XElement tile)
        {
            List<LocalRectangle> rectangles = new();
            XElement objectGroup = tile.Element("objectgroup");
            if (objectGroup == null)
                return rectangles;

            foreach (XElement element in objectGroup.Elements("object"))
            {
                float width = ReadFloat(element, "width");
                float height = ReadFloat(element, "height");
                if (width <= 0f || height <= 0f ||
                    element.Element("polygon") != null ||
                    element.Element("ellipse") != null)
                {
                    continue;
                }

                rectangles.Add(new LocalRectangle(
                    ReadFloat(element, "x"),
                    ReadFloat(element, "y"),
                    width,
                    height));
            }

            return rectangles;
        }

        private static IReadOnlyList<IReadOnlyList<Vector2>> ReadLocalCollisionPolygons(
            XElement tile)
        {
            List<IReadOnlyList<Vector2>> polygons = new();
            XElement objectGroup = tile.Element("objectgroup");
            if (objectGroup == null)
                return polygons;

            foreach (XElement element in objectGroup.Elements("object"))
            {
                XElement polygon = element.Element("polygon");
                string pointsText = (string)polygon?.Attribute("points");
                if (string.IsNullOrWhiteSpace(pointsText))
                    continue;

                float objectX = ReadFloat(element, "x");
                float objectY = ReadFloat(element, "y");
                List<Vector2> points = new();
                foreach (string pair in pointsText.Split(
                    new[] { ' ', '\r', '\n', '\t' },
                    StringSplitOptions.RemoveEmptyEntries))
                {
                    string[] coordinates = pair.Split(',');
                    if (coordinates.Length != 2)
                        throw new InvalidDataException($"Invalid Tiled polygon point '{pair}'.");

                    points.Add(new Vector2(
                        objectX + float.Parse(coordinates[0], CultureInfo.InvariantCulture),
                        objectY + float.Parse(coordinates[1], CultureInfo.InvariantCulture)));
                }

                if (points.Count >= 3)
                    polygons.Add(points);
            }

            return polygons;
        }

        private static void AddCollisionShapes(
            List<StaticCollisionShape> destination,
            ImageCollectionTile tile,
            float propX,
            float propY,
            float propWidth,
            float propHeight,
            float mapScale)
        {
            if (tile.ImageWidth <= 0 || tile.ImageHeight <= 0 ||
                propWidth <= 0f || propHeight <= 0f)
            {
                return;
            }

            float scaleX = propWidth / tile.ImageWidth;
            float scaleY = propHeight / tile.ImageHeight;
            float propTop = propY - propHeight;

            foreach (LocalRectangle local in tile.CollisionRectangles)
            {
                destination.Add(StaticCollisionShape.FromRectangle(new Rectangle(
                    Round((propX + local.X * scaleX) * mapScale),
                    Round((propTop + local.Y * scaleY) * mapScale),
                    Math.Max(1, Round(local.Width * scaleX * mapScale)),
                    Math.Max(1, Round(local.Height * scaleY * mapScale)))));
            }

            foreach (IReadOnlyList<Vector2> localPolygon in tile.CollisionPolygons)
            {
                destination.Add(StaticCollisionShape.FromPolygon(
                    localPolygon.Select(point => new Vector2(
                        (propX + point.X * scaleX) * mapScale,
                        (propTop + point.Y * scaleY) * mapScale))));
            }
        }

        private static XElement FindPropsTileset(
            XElement map,
            string propsTilesetFileName)
        {
            foreach (XElement tileset in map.Elements("tileset"))
            {
                string source = (string)tileset.Attribute("source") ?? string.Empty;
                string name = (string)tileset.Attribute("name") ?? string.Empty;

                if (source.EndsWith(propsTilesetFileName, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(name,
                        Path.GetFileNameWithoutExtension(propsTilesetFileName),
                        StringComparison.OrdinalIgnoreCase))
                    return tileset;
            }

            throw new InvalidDataException(
                $"The map has no props tileset '{propsTilesetFileName}'.");
        }

        private static int ReadInt(XElement element, string attributeName)
        {
            string value = (string)element.Attribute(attributeName)
                ?? throw new InvalidDataException($"Missing '{attributeName}' attribute.");
            return int.Parse(value, CultureInfo.InvariantCulture);
        }

        private static int ReadOptionalInt(
            XElement element,
            string attributeName,
            int defaultValue)
        {
            string value = (string)element.Attribute(attributeName);
            return string.IsNullOrWhiteSpace(value)
                ? defaultValue
                : int.Parse(value, CultureInfo.InvariantCulture);
        }

        private static float ReadFloat(XElement element, string attributeName)
        {
            string value = (string)element.Attribute(attributeName) ?? "0";
            return float.Parse(value, CultureInfo.InvariantCulture);
        }

        private static int Round(float value) => (int)MathF.Round(value);

        private static PropDrawMode ReadDrawMode(XElement element)
        {
            XElement property = element.Element("properties")?
                .Elements("property")
                .FirstOrDefault(item =>
                    string.Equals((string)item.Attribute("name"), "DrawMode", StringComparison.OrdinalIgnoreCase));

            string value = (string)property?.Attribute("value") ?? property?.Value ?? string.Empty;

            return value.Trim().ToLowerInvariant() switch
            {
                "behind" => PropDrawMode.Behind,
                "front" => PropDrawMode.Front,
                _ => PropDrawMode.YSort
            };
        }

        private enum PropDrawMode
        {
            Behind,
            YSort,
            Front
        }

        private sealed class ImageCollectionTile
        {
            public Texture2D Texture { get; }
            public int ImageWidth { get; }
            public int ImageHeight { get; }
            public IReadOnlyList<LocalRectangle> CollisionRectangles { get; }
            public IReadOnlyList<IReadOnlyList<Vector2>> CollisionPolygons { get; }

            public ImageCollectionTile(
                Texture2D texture,
                int imageWidth,
                int imageHeight,
                IReadOnlyList<LocalRectangle> collisionRectangles,
                IReadOnlyList<IReadOnlyList<Vector2>> collisionPolygons)
            {
                Texture = texture;
                ImageWidth = imageWidth;
                ImageHeight = imageHeight;
                CollisionRectangles = collisionRectangles;
                CollisionPolygons = collisionPolygons;
            }
        }

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

        private readonly struct PropObject
        {
            public string RegionName { get; }
            public int TileId { get; }
            public Texture2D ImageTexture { get; }
            public PropDrawMode DrawMode { get; }
            public float X { get; }
            public float Y { get; }
            public float Width { get; }
            public float Height { get; }

            public PropObject(
                string regionName,
                PropDrawMode drawMode,
                float x,
                float y,
                float width,
                float height)
            {
                RegionName = regionName;
                TileId = -1;
                ImageTexture = null;
                DrawMode = drawMode;
                X = x;
                Y = y;
                Width = width;
                Height = height;
            }

            public PropObject(
                int tileId,
                PropDrawMode drawMode,
                float x,
                float y,
                float width,
                float height)
            {
                RegionName = string.Empty;
                TileId = tileId;
                ImageTexture = null;
                DrawMode = drawMode;
                X = x;
                Y = y;
                Width = width;
                Height = height;
            }

            public PropObject(
                Texture2D imageTexture,
                PropDrawMode drawMode,
                float x,
                float y,
                float width,
                float height)
            {
                RegionName = string.Empty;
                TileId = -1;
                ImageTexture = imageTexture;
                DrawMode = drawMode;
                X = x;
                Y = y;
                Width = width;
                Height = height;
            }
        }

        private sealed class YSortedProp : IYSortedWorldDrawable
        {
            private readonly TiledPropsLayer owner;
            private readonly PropObject prop;

            public YSortedProp(TiledPropsLayer owner, PropObject prop)
            {
                this.owner = owner;
                this.prop = prop;
            }

            public int SortY => Round(prop.Y * owner._mapScale);

            public void Draw(SpriteBatch spriteBatch)
            {
                owner.DrawProp(spriteBatch, prop);
            }
        }
    }
}
