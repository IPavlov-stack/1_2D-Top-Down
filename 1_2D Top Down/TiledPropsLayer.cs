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

namespace _1_2D_Top_Down
{
    /// <summary>
    /// Reads tile objects from the Props object layer of a Tiled map and draws
    /// them from the compact EnvironmentProps texture atlas.
    /// </summary>
    public sealed class TiledPropsLayer
    {
        private const uint TileIdMask = 0x0FFFFFFF;

        // These are the actual tile IDs in EnvironmentProps.tsx. They are
        // sparse because tiles were removed/re-added in Tiled, so an array
        // indexed from 0 would associate the wrong sprite with a tile.
        private static readonly Dictionary<int, string> RegionNames = new()
        {
            [27] = "prop_blue_banner",
            [28] = "prop_bushes_large",
            [29] = "prop_bushes_medium",
            [30] = "prop_bushes_small",
            [31] = "prop_campfire",
            [32] = "prop_castle_round",
            [33] = "prop_castle_square",
            [34] = "prop_flag",
            [35] = "prop_house",
            [36] = "prop_magic_stone_tower",
            [37] = "prop_red_banner",
            [38] = "prop_rock_01",
            [39] = "prop_rock_02",
            [40] = "prop_rock_03",
            [41] = "prop_rock_04",
            [42] = "prop_rock_05",
            [43] = "prop_tent",
            [44] = "prop_treasure_chest",
            [45] = "prop_tree_large",
            [46] = "prop_tree_medium",
            [47] = "prop_tree_small",
            [48] = "prop_tree_stump_short",
            [49] = "prop_tree_stump_tall",
            [50] = "prop_watchtower_short",
            [51] = "prop_watchtower_tall",
            [52] = "prop_well",
            [53] = "prop_windmill",
            [54] = "prop_wooden_barrel",
            [55] = "prop_wooden_bridge_horizontal",
            [56] = "prop_wooden_bridge_vertical",
            [57] = "prop_wooden_cart",
            [58] = "prop_wooden_fence_horizontal",
            [59] = "prop_wooden_fence_vertical"
        };

        private readonly TextureAtlas _atlas;
        private readonly List<PropObject> _props;
        private readonly float _mapScale;

        private TiledPropsLayer(TextureAtlas atlas, List<PropObject> props, float mapScale)
        {
            _atlas = atlas;
            _props = props;
            _mapScale = mapScale;
        }

        public static TiledPropsLayer FromFile(
            ContentManager content,
            string tmxFileName,
            TextureAtlas atlas,
            float mapScale,
            string layerName = "Props")
        {
            string path = Path.Combine(content.RootDirectory, tmxFileName);

            using Stream stream = TitleContainer.OpenStream(path);
            XDocument document = XDocument.Load(stream);
            XElement map = document.Root
                ?? throw new InvalidDataException("The TMX file has no <map> element.");

            XElement propsTileset = FindPropsTileset(map);
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

                if (!RegionNames.TryGetValue(tileId, out string regionName))
                    continue;

                props.Add(new PropObject(
                    regionName,
                    ReadFloat(element, "x"),
                    ReadFloat(element, "y"),
                    ReadFloat(element, "width"),
                    ReadFloat(element, "height")));
            }

            return new TiledPropsLayer(atlas, props, mapScale);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (PropObject prop in _props)
            {
                TextureRegion region = _atlas.GetRegion(prop.RegionName);

                // A tile object in an orthogonal Tiled map is positioned from
                // its bottom-left corner. SpriteBatch expects top-left here.
                Rectangle destination = new(
                    Round(prop.X * _mapScale),
                    Round((prop.Y - prop.Height) * _mapScale),
                    Math.Max(1, Round(prop.Width * _mapScale)),
                    Math.Max(1, Round(prop.Height * _mapScale)));

                spriteBatch.Draw(
                    region.Texture,
                    destination,
                    region.SourceRectangle,
                    Color.White);
            }
        }

        private static XElement FindPropsTileset(XElement map)
        {
            foreach (XElement tileset in map.Elements("tileset"))
            {
                string source = (string)tileset.Attribute("source") ?? string.Empty;
                string name = (string)tileset.Attribute("name") ?? string.Empty;

                if (source.Contains("EnvironmentProps", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(name, "EnvironmentProps", StringComparison.OrdinalIgnoreCase))
                    return tileset;
            }

            throw new InvalidDataException("The map has no EnvironmentProps.tsx tileset.");
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

        private readonly struct PropObject
        {
            public string RegionName { get; }
            public float X { get; }
            public float Y { get; }
            public float Width { get; }
            public float Height { get; }

            public PropObject(string regionName, float x, float y, float width, float height)
            {
                RegionName = regionName;
                X = x;
                Y = y;
                Width = width;
                Height = height;
            }
        }
    }
}