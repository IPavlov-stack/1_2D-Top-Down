using Microsoft.Xna.Framework;
using System;
using Tiled;

namespace _1_2D_Top_Down
{
    public partial class Game1
    {
        private void SpawnEnemy(EnemyType enemyType)
        {
            if (gameMap.PortalSpawnPoints.Count == 0)
            {
                return;
            }

            Vector2 spawnPosition = gameMap.PortalSpawnPoints[
                random.Next(gameMap.PortalSpawnPoints.Count)];

            enemyManager.Add(enemyFactory.Create(enemyType, spawnPosition));
        }
        private void SpawnEnemy(EnemyType enemyType, Vector2 spawnPosition)
        {
            enemyManager.Add(enemyFactory.Create(enemyType, spawnPosition));
        }
        private void SpawnEnemy(string enemyId, Vector2 spawnPosition)
        {
            enemyManager.Add(enemyFactory.Create(enemyId, spawnPosition));
        }
        private void LoadPreplacedMissionEnemies()
        {
            foreach (EnemySpawnPoint spawnPoint in gameMap.EnemySpawnPoints)
            {
                if (!EnemyDefinitions.TryGet(
                        spawnPoint.EnemyType,
                        out EnemyDefinition? definition))
                {
                    throw new InvalidOperationException(
                        $"Unknown enemy definition '{spawnPoint.EnemyType}' in " +
                        $"{missionRuntime.Definition.MapFileName}.");
                }

                SpawnEnemy(definition.Id, spawnPoint.Position);
            }
        }
    }
}
