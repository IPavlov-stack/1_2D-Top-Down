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
    /// Loads reusable wave-spawn positions from EnemySpawner point objects.
    /// Wave definitions remain responsible for enemy types and counts.
    /// </summary>
    public static class TiledEnemySpawners
    {
        public static IReadOnlyList<Vector2> FromFile(
            ContentManager content,
            string tmxFileName,
            float mapScale,
            string layerName = "EnemySpawners",
            string objectName = "EnemySpawner")
        {
            string path = Path.Combine(content.RootDirectory, tmxFileName);

            using Stream stream = TitleContainer.OpenStream(path);
            XDocument document = XDocument.Load(stream);
            XElement map = document.Root
                ?? throw new InvalidDataException("The TMX file has no <map> element.");

            XElement layer = map.Elements("objectgroup")
                .FirstOrDefault(element =>
                    (string)element.Attribute("name") == layerName)
                ?? throw new InvalidDataException(
                    $"The TMX file has no object layer named '{layerName}'.");

            List<Vector2> positions = new();
            foreach (XElement element in layer.Elements("object")
                         .Where(element =>
                             (string)element.Attribute("name") == objectName))
            {
                if (element.Element("point") == null)
                {
                    throw new InvalidDataException(
                        $"'{objectName}' objects in '{layerName}' must be points.");
                }

                positions.Add(new Vector2(
                    ReadFloat(element, "x") * mapScale,
                    ReadFloat(element, "y") * mapScale));
            }

            if (positions.Count == 0)
            {
                throw new InvalidDataException(
                    $"No '{objectName}' points were found in '{layerName}'.");
            }

            return positions;
        }

        private static float ReadFloat(XElement element, string attributeName)
        {
            string value = (string)element.Attribute(attributeName)
                ?? throw new InvalidDataException(
                    $"Object is missing '{attributeName}'.");

            return float.Parse(value, CultureInfo.InvariantCulture);
        }
    }
}
