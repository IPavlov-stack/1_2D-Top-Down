using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using _1_2D_Top_Down;

namespace Tiled
{
    /// <summary>
    /// Loads rectangle objects from a Tiled object layer and exposes them as
    /// solid world-space collision rectangles.
    /// </summary>
    public sealed class TiledCollisionLayer
    {
        private readonly List<Rectangle> _rectangles;
        private readonly List<StaticCollisionShape> _shapes;

        public IReadOnlyList<Rectangle> Rectangles => _rectangles;
        public IReadOnlyList<StaticCollisionShape> Shapes => _shapes;

        private TiledCollisionLayer(
            List<Rectangle> rectangles,
            List<StaticCollisionShape> shapes)
        {
            _rectangles = rectangles;
            _shapes = shapes;
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
            List<StaticCollisionShape> shapes = new();
            foreach (XElement element in objectGroup.Elements("object"))
            {
                XElement polygon = element.Element("polygon");
                if (polygon != null)
                {
                    float originX = ReadFloat(element, "x");
                    float originY = ReadFloat(element, "y");
                    List<Vector2> points = ReadPolygonPoints(polygon)
                        .Select(point => new Vector2(
                            (originX + point.X) * mapScale,
                            (originY + point.Y) * mapScale))
                        .ToList();
                    if (points.Count >= 3)
                        shapes.Add(StaticCollisionShape.FromPolygon(points));
                    continue;
                }

                float width = ReadFloat(element, "width");
                float height = ReadFloat(element, "height");

                // Ignore points and any accidental objects without an area.
                if (width <= 0f || height <= 0f)
                    continue;

                Rectangle rectangle = new(
                    Round(ReadFloat(element, "x") * mapScale),
                    Round(ReadFloat(element, "y") * mapScale),
                    Math.Max(1, Round(width * mapScale)),
                    Math.Max(1, Round(height * mapScale)));
                rectangles.Add(rectangle);
                shapes.Add(StaticCollisionShape.FromRectangle(rectangle));
            }

            return new TiledCollisionLayer(rectangles, shapes);
        }

        private static IReadOnlyList<Vector2> ReadPolygonPoints(XElement polygon)
        {
            string pointsText = (string)polygon.Attribute("points") ?? string.Empty;
            List<Vector2> points = new();
            foreach (string pair in pointsText.Split(
                new[] { ' ', '\r', '\n', '\t' },
                StringSplitOptions.RemoveEmptyEntries))
            {
                string[] coordinates = pair.Split(',');
                if (coordinates.Length != 2)
                    throw new InvalidDataException($"Invalid Tiled polygon point '{pair}'.");

                points.Add(new Vector2(
                    float.Parse(coordinates[0], CultureInfo.InvariantCulture),
                    float.Parse(coordinates[1], CultureInfo.InvariantCulture)));
            }

            return points;
        }

        private static float ReadFloat(XElement element, string attributeName)
        {
            string value = (string)element.Attribute(attributeName) ?? "0";
            return float.Parse(value, CultureInfo.InvariantCulture);
        }

        private static int Round(float value) => (int)MathF.Round(value);
    }
}
