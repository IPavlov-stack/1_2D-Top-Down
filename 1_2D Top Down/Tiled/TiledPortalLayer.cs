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
    /// Draws the portal tile objects from the Props layer using one animated
    /// horizontal spritesheet.
    /// </summary>
    public sealed class TiledPortalLayer
    {
        private const uint TileIdMask = 0x0FFFFFFF;
        private const uint FlipHorizontalFlag = 0x80000000;
        private const int FrameWidth = 64;
        private const int FrameHeight = 64;
        private const int FrameCount = 8;
        private const float FrameDuration = 0.1f;

        private readonly Texture2D _texture;
        private readonly List<PortalObject> _portals;
        private readonly float _mapScale;
        private float _frameTimer;
        private int _currentFrame;

        private TiledPortalLayer(Texture2D texture, List<PortalObject> portals, float mapScale)
        {
            _texture = texture;
            _portals = portals;
            _mapScale = mapScale;
        }

        public static TiledPortalLayer FromFile(
            ContentManager content,
            string tmxFileName,
            Texture2D portalTexture,
            float mapScale,
            string layerName = "Props")
        {
            string path = Path.Combine(content.RootDirectory, tmxFileName);

            using Stream stream = TitleContainer.OpenStream(path);
            XDocument document = XDocument.Load(stream);
            XElement map = document.Root
                ?? throw new InvalidDataException("The TMX file has no <map> element.");

            int portalFirstGid = FindPortalFirstGid(map);
            int nextTilesetFirstGid = map.Elements("tileset")
                .Select(element => ReadInt(element, "firstgid"))
                .Where(firstGid => firstGid > portalFirstGid)
                .DefaultIfEmpty(int.MaxValue)
                .Min();

            XElement propsLayer = map.Elements("objectgroup")
                .FirstOrDefault(element => (string)element.Attribute("name") == layerName)
                ?? throw new InvalidDataException($"The TMX file has no object layer named '{layerName}'.");

            List<PortalObject> portals = new();
            foreach (XElement element in propsLayer.Elements("object"))
            {
                string gidText = (string)element.Attribute("gid");
                if (string.IsNullOrWhiteSpace(gidText))
                    continue;

                uint rawGlobalId = uint.Parse(gidText, CultureInfo.InvariantCulture);
                uint globalId = rawGlobalId & TileIdMask;

                if (globalId < portalFirstGid || globalId >= nextTilesetFirstGid)
                    continue;

                portals.Add(new PortalObject(
                    ReadFloat(element, "x"),
                    ReadFloat(element, "y"),
                    ReadFloat(element, "width"),
                    ReadFloat(element, "height"),
                    (rawGlobalId & FlipHorizontalFlag) != 0));
            }

            return new TiledPortalLayer(portalTexture, portals, mapScale);
        }

        public void Update(GameTime gameTime)
        {
            _frameTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            while (_frameTimer >= FrameDuration)
            {
                _frameTimer -= FrameDuration;
                _currentFrame = (_currentFrame + 1) % FrameCount;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle source = new(_currentFrame * FrameWidth, 0, FrameWidth, FrameHeight);

            foreach (PortalObject portal in _portals)
            {
                // Like all tile objects in an orthogonal Tiled map, the portal
                // position is its bottom-left corner.
                Rectangle destination = new(
                    Round(portal.X * _mapScale),
                    Round((portal.Y - portal.Height) * _mapScale),
                    Math.Max(1, Round(portal.Width * _mapScale)),
                    Math.Max(1, Round(portal.Height * _mapScale)));

                spriteBatch.Draw(
                    _texture,
                    destination,
                    source,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    portal.IsFlippedHorizontally ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    0f);
            }
        }

        private static int FindPortalFirstGid(XElement map)
        {
            XElement portalTileset = map.Elements("tileset")
                .FirstOrDefault(element =>
                    ((string)element.Attribute("source") ?? string.Empty)
                    .EndsWith("portal_orange-1.tsx", StringComparison.OrdinalIgnoreCase));

            if (portalTileset == null)
                throw new InvalidDataException("The map has no portal_orange-1.tsx tileset.");

            return ReadInt(portalTileset, "firstgid");
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

        private readonly struct PortalObject
        {
            public float X { get; }
            public float Y { get; }
            public float Width { get; }
            public float Height { get; }
            public bool IsFlippedHorizontally { get; }

            public PortalObject(float x, float y, float width, float height, bool isFlippedHorizontally)
            {
                X = x;
                Y = y;
                Width = width;
                Height = height;
                IsFlippedHorizontally = isFlippedHorizontally;
            }
        }
    }
}