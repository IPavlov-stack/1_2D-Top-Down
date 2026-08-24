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

namespace Tiled
{
    /// <summary>
    /// Reads tile objects from the Props object layer of a Tiled map and draws
    /// them from the compact EnvironmentProps texture atlas.
    /// </summary>
    public sealed class TiledPropsLayer
    {
        private const uint TileIdMask = 0x0FFFFFFF;

        private readonly TextureAtlas _atlas;
        private readonly Texture2D _gridTexture;
        private readonly int _gridTileWidth;
        private readonly int _gridTileHeight;
        private readonly List<PropObject> _props;
        private readonly float _mapScale;

        private TiledPropsLayer(TextureAtlas atlas, List<PropObject> props, float mapScale)
        {
            _atlas = atlas;
            _props = props;
            _mapScale = mapScale;
        }

        private TiledPropsLayer(
            Texture2D gridTexture,
            int gridTileWidth,
            int gridTileHeight,
            List<PropObject> props,
            float mapScale)
        {
            _gridTexture = gridTexture;
            _gridTileWidth = gridTileWidth;
            _gridTileHeight = gridTileHeight;
            _props = props;
            _mapScale = mapScale;
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

            List<PropObject> props = new();
            foreach (XElement element in objectGroup.Elements("object"))
            {
                string gidText = (string)element.Attribute("gid");
                if (string.IsNullOrWhiteSpace(gidText))
                    continue;

                uint globalId = uint.Parse(gidText, CultureInfo.InvariantCulture);
                uint cleanGlobalId = globalId & TileIdMask;
                if (cleanGlobalId < firstGid || cleanGlobalId >= nextFirstGid)
                    continue;

                int tileId = (int)cleanGlobalId - firstGid;
                props.Add(new PropObject(
                    tileId,
                    ReadDrawMode(element),
                    ReadFloat(element, "x"),
                    ReadFloat(element, "y"),
                    ReadFloat(element, "width"),
                    ReadFloat(element, "height")));
            }

            return new TiledPropsLayer(
                content.Load<Texture2D>(textureAssetName),
                sourceTileWidth,
                sourceTileHeight,
                props,
                mapScale);
        }

        /// <summary>
        /// Draws props whose base is above (or level with) the player's feet.
        /// They belong behind the player.
        /// </summary>
        public void DrawBehindPlayer(SpriteBatch spriteBatch, int playerFeetY)
        {
            foreach (PropObject prop in _props)
            {
                if (prop.DrawMode == PropDrawMode.Behind ||
                    (prop.DrawMode == PropDrawMode.YSort &&
                     Round(prop.Y * _mapScale) <= playerFeetY))
                    DrawProp(spriteBatch, prop);
            }
        }

        /// <summary>
        /// Draws props whose base is below the player's feet. They cover the
        /// player while the player is visually behind them.
        /// </summary>
        public void DrawInFrontOfPlayer(SpriteBatch spriteBatch, int playerFeetY)
        {
            foreach (PropObject prop in _props)
            {
                if (prop.DrawMode == PropDrawMode.Front ||
                    (prop.DrawMode == PropDrawMode.YSort &&
                     Round(prop.Y * _mapScale) > playerFeetY))
                    DrawProp(spriteBatch, prop);
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

        private readonly struct PropObject
        {
            public string RegionName { get; }
            public int TileId { get; }
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
                DrawMode = drawMode;
                X = x;
                Y = y;
                Width = width;
                Height = height;
            }
        }
    }
}
