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
    public sealed class TiledMissionObjects
    {
        public Vector2 PlayerSpawnPosition { get; }
        public IReadOnlyList<EnemySpawnPoint> EnemySpawnPoints { get; }

        private TiledMissionObjects(Vector2 playerSpawnPosition, List<EnemySpawnPoint> enemySpawnPoints)
        {
            PlayerSpawnPosition = playerSpawnPosition;
            EnemySpawnPoints = enemySpawnPoints;
        }
        public static TiledMissionObjects FromFile(
            ContentManager content,
            string tmxFileName,
            float mapScale,
            string layerName = "MissionObjects")
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

            Vector2? playerSpawnPosition = null;
            List<EnemySpawnPoint> enemySpawnPoints = new();

            foreach (XElement element in objectGroup.Elements("object"))
            {
                string name = ((string)element.Attribute("name") ?? string.Empty)
                    .Trim();

                Vector2 position = new(
                    ReadFloat(element, "x") * mapScale,
                    ReadFloat(element, "y") * mapScale);

                if (name.Equals("PlayerSpawn",
                    StringComparison.OrdinalIgnoreCase))
                {
                    playerSpawnPosition = position;
                }
                else if (name.Equals("EnemySpawn",
                    StringComparison.OrdinalIgnoreCase))
                {
                    string enemyType = ReadProperty(
                        element,
                        "EnemyType");

                    if (string.IsNullOrWhiteSpace(enemyType))
                    {
                        throw new InvalidDataException(
                            "Every EnemySpawn must have an EnemyType property.");
                    }

                    enemySpawnPoints.Add(
                        new EnemySpawnPoint(position, enemyType));
                }
            }

            if (playerSpawnPosition is null)
            {
                throw new InvalidDataException(
                    "MissionObjects must contain one PlayerSpawn.");
            }

            return new TiledMissionObjects( playerSpawnPosition.Value, enemySpawnPoints );
        }

        private static string ReadProperty(
            XElement objectElement,
            string propertyName)
        {
            XElement property = objectElement
                .Element("properties")?
                .Elements("property")
                .FirstOrDefault(item =>
                    string.Equals(
                        (string)item.Attribute("name"),
                        propertyName,
                        StringComparison.OrdinalIgnoreCase));

            return (string)property?.Attribute("value")
                ?? property?.Value
                ?? string.Empty;
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

    public readonly struct EnemySpawnPoint
    {
        public Vector2 Position { get; }
        public string EnemyType { get; }

        public EnemySpawnPoint(
            Vector2 position,
            string enemyType)
        {
            Position = position;
            EnemyType = enemyType;
        }
    }
}