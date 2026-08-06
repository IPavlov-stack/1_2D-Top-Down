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
    public sealed class TiledMissionTriggers
    {
        public IReadOnlyList<MissionTrigger> Triggers { get; }

        private TiledMissionTriggers(List<MissionTrigger> triggers)
        {
            Triggers = triggers;
        }

        public static TiledMissionTriggers FromFile(
            ContentManager content,
            string tmxFileName,
            float mapScale,
            string layerName = "MissionTriggers")
        {
            string path = Path.Combine(content.RootDirectory, tmxFileName);

            using Stream stream = TitleContainer.OpenStream(path);
            XDocument document = XDocument.Load(stream);

            XElement map = document.Root
                ?? throw new InvalidDataException(
                    "The TMX file has no <map> element.");

            XElement objectGroup = map.Elements("objectgroup")
                .FirstOrDefault(element =>
                    (string)element.Attribute("name") == layerName)
                ?? throw new InvalidDataException(
                    $"The TMX file has no object layer named '{layerName}'.");

            float layerOffsetX = ReadFloat(objectGroup, "offsetx");
            float layerOffsetY = ReadFloat(objectGroup, "offsety");

            List<MissionTrigger> triggers = new();

            foreach (XElement element in objectGroup.Elements("object"))
            {
                string name = ((string)element.Attribute("name") ?? string.Empty)
                    .Trim();

                if (string.IsNullOrWhiteSpace(name))
                {
                    continue;
                }

                float width = ReadFloat(element, "width");
                float height = ReadFloat(element, "height");

                if (width <= 0 || height <= 0)
                {
                    throw new InvalidDataException(
                        $"Mission trigger '{name}' must be a rectangle with width and height.");
                }

                Rectangle bounds = new Rectangle(
                    (int)((ReadFloat(element, "x") + layerOffsetX) * mapScale),
                    (int)((ReadFloat(element, "y") + layerOffsetY) * mapScale),
                    (int)(width * mapScale),
                    (int)(height * mapScale));

                triggers.Add(new MissionTrigger(name, bounds));
            }

            return new TiledMissionTriggers(triggers);
        }

        private static float ReadFloat(
            XElement element,
            string attributeName)
        {
            string value = (string)element.Attribute(attributeName) ?? "0";

            return float.Parse(
                value,
                CultureInfo.InvariantCulture);
        }
    }
}