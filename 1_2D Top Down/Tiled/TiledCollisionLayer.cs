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
    /// Loads rectangle objects from a Tiled object layer and exposes them as
    /// solid world-space collision rectangles.
    /// </summary>
    public sealed class TiledCollisionLayer
    {
        private readonly List<Rectangle> _rectangles;

        public IReadOnlyList<Rectangle> Rectangles => _rectangles;

        private TiledCollisionLayer(List<Rectangle> rectangles)
        {
            _rectangles = rectangles;
        }

        public static TiledCollisionLayer FromFile(
            ContentManager content,
            string tmxFileName,
            float mapScale,
            string layerName = "Collisions")
        {
            string path = Path.Combine(content.RootDirectory, tmxFileName);

            using Stream stream = TitleContainer.OpenStream(path);
            XDocument document = XDocument.Load(stream);
            XElement map = document.Root
                ?? throw new InvalidDataException("The TMX file has no <map> element.");

            XElement objectGroup = map.Elements("objectgroup")
                .FirstOrDefault(element => (string)element.Attribute("name") == layerName)
                ?? throw new InvalidDataException($"The TMX file has no object layer named '{layerName}'.");

            List<Rectangle> rectangles = new();
            foreach (XElement element in objectGroup.Elements("object"))
            {
                float width = ReadFloat(element, "width");
                float height = ReadFloat(element, "height");

                // Ignore points and any accidental objects without an area.
                if (width <= 0f || height <= 0f)
                    continue;

                rectangles.Add(new Rectangle(
                    Round(ReadFloat(element, "x") * mapScale),
                    Round(ReadFloat(element, "y") * mapScale),
                    Math.Max(1, Round(width * mapScale)),
                    Math.Max(1, Round(height * mapScale))));
            }

            return new TiledCollisionLayer(rectangles);
        }

        private static float ReadFloat(XElement element, string attributeName)
        {
            string value = (string)element.Attribute(attributeName) ?? "0";
            return float.Parse(value, CultureInfo.InvariantCulture);
        }

        private static int Round(float value) => (int)MathF.Round(value);
    }
}