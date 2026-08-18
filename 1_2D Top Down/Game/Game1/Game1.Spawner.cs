using Microsoft.Xna.Framework;
using System;
using Tiled;

namespace _1_2D_Top_Down
{
    public partial class Game1
    {
        private void SpawnEnemy(EnemyType enemyType)
        {
            if (portalSpawnPoints.Count == 0)
            {
                return;
            }

            Vector2 spawnPosition = portalSpawnPoints[random.Next(portalSpawnPoints.Count)];

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
        private void LoadPreplacedMissionEnemies(string mapFileName)
        {
            TiledMissionObjects missionObjects = TiledMissionObjects.FromFile(Content,mapFileName,EnvironmentScale);

            playerStartPosition = missionObjects.PlayerSpawnPosition;
            player.Position = playerStartPosition;

            foreach (EnemySpawnPoint spawnPoint in missionObjects.EnemySpawnPoints)
            {
                if (!EnemyDefinitions.TryGet(
                        spawnPoint.EnemyType,
                        out EnemyDefinition? definition))
                {
                    throw new InvalidOperationException(
                        $"Unknown enemy definition '{spawnPoint.EnemyType}' in {mapFileName}.");
                }

                SpawnEnemy(definition.Id, spawnPoint.Position);
            }
        }
        private void StartNextWave()
        {
            int waveIndex = missionRuntime.Waves.CurrentWave;

            if (waveIndex >= missionRuntime.Definition.Waves.Count)
            {

                return;
            }

            WaveDefinition wave = missionRuntime.Definition.Waves[waveIndex];

            missionRuntime.Waves.StartNextWave(wave.TotalEnemyCount);

            enemyManager.StartSpawningWave(wave);
        }


    }
}
