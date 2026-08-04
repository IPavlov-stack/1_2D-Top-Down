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
    /// Reads all tile objects that use the portal tileset in the Props layer.
    /// Their centers are returned in the same scaled world coordinates as the map.
    /// </summary>
    public static class TiledPortalSpawns
    {
        private const uint TileIdMask = 0x0FFFFFFF;

        public static List<Vector2> FromFile(
            ContentManager content,
            string tmxFileName,
            float mapScale,
            string layerName = "Props")
        {
            string path = Path.Combine(content.RootDirectory, tmxFileName);

            using Stream stream = TitleContainer.OpenStream(path);
            XDocument document = XDocument.Load(stream);
            XElement map = document.Root
                ?? throw new InvalidDataException("The TMX file has no <map> element.");

            int portalFirstGid = FindPortalFirstGid(map);
            XElement propsLayer = map.Elements("objectgroup")
                .FirstOrDefault(element => (string)element.Attribute("name") == layerName)
                ?? throw new InvalidDataException($"The TMX file has no object layer named '{layerName}'.");

            List<Vector2> portalCenters = new();

            foreach (XElement element in propsLayer.Elements("object"))
            {
                string gidText = (string)element.Attribute("gid");
                if (string.IsNullOrWhiteSpace(gidText))
                    continue;

                uint globalId = uint.Parse(gidText, CultureInfo.InvariantCulture) & TileIdMask;

                // Every tile from portal_orange-1.tsx belongs to this tileset.
                // The next tileset (if one is added later) has a larger firstgid.
                if (globalId < portalFirstGid || IsFromAnotherTileset(map, globalId, portalFirstGid))
                    continue;

                float x = ReadFloat(element, "x");
                float y = ReadFloat(element, "y");
                float width = ReadFloat(element, "width");
                float height = ReadFloat(element, "height");

                // Tile objects in an orthogonal Tiled map are anchored at bottom-left.
                Vector2 center = new(
                    (x + width / 2f) * mapScale,
                    (y - height / 2f) * mapScale);

                portalCenters.Add(center);
            }

            if (portalCenters.Count == 0)
                throw new InvalidDataException("No portal objects were found in the Props layer.");

            return portalCenters;
        }

        private static int FindPortalFirstGid(XElement map)
        {
            XElement portalTileset = map.Elements("tileset")
                .FirstOrDefault(element =>
                    ((string)element.Attribute("source") ?? string.Empty)
                    .EndsWith("portal_orange-1.tsx", StringComparison.OrdinalIgnoreCase));

            if (portalTileset == null)
                throw new InvalidDataException("The map has no portal_orange-1.tsx tileset.");

            return int.Parse((string)portalTileset.Attribute("firstgid")!, CultureInfo.InvariantCulture);
        }

        private static bool IsFromAnotherTileset(XElement map, uint globalId, int portalFirstGid)
        {
            int nextFirstGid = map.Elements("tileset")
                .Select(element => int.Parse((string)element.Attribute("firstgid")!, CultureInfo.InvariantCulture))
                .Where(firstGid => firstGid > portalFirstGid)
                .DefaultIfEmpty(int.MaxValue)
                .Min();

            return globalId >= nextFirstGid;
        }

        private static float ReadFloat(XElement element, string attributeName)
        {
            string value = (string)element.Attribute(attributeName) ?? "0";
            return float.Parse(value, CultureInfo.InvariantCulture);
        }
    }
}