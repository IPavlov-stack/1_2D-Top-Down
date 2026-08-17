using Microsoft.Xna.Framework;
using System;
using Tiled;

namespace _1_2D_Top_Down
{
    public partial class Game1
    {
        private float delayBetweenSpawnGroups;
        private void SpawnEnemy(EnemyType enemyType)
        {
            if (portalSpawnPoints.Count == 0)
                return;

            Vector2 spawnPosition = portalSpawnPoints[random.Next(portalSpawnPoints.Count)];

            switch (enemyType)
            {
                case EnemyType.Demon:
                    demons.Add(new Demon(demonTexture, spawnPosition));
                    break;

                case EnemyType.EvilEye:
                    evilEyes.Add(new Evil_Eye(
                        evilEyeTexture,
                        spawnPosition));
                    break;
            }
        }
        private void SpawnEnemy(EnemyType enemyType, Vector2 spawnPosition)
        {
            switch (enemyType)
            {
                case EnemyType.Demon:
                    demons.Add(new Demon(demonTexture, spawnPosition));
                    break;

                case EnemyType.EvilEye:
                    evilEyes.Add(new Evil_Eye(
                        evilEyeTexture,
                        spawnPosition));
                    break;
            }
        }

        private void LoadPreplacedMissionEnemies(string mapFileName)
        {
            TiledMissionObjects missionObjects = TiledMissionObjects.FromFile(Content,mapFileName,EnvironmentScale);

            playerStartPosition = missionObjects.PlayerSpawnPosition;
            player.Position = playerStartPosition;

            foreach (EnemySpawnPoint spawnPoint in missionObjects.EnemySpawnPoints)
            {
                if (!Enum.TryParse(spawnPoint.EnemyType, ignoreCase: true,out EnemyType enemyType))
                {
                    throw new InvalidOperationException($"Unknown EnemyType '{spawnPoint.EnemyType}' in {mapFileName}.");
                }

                SpawnEnemy(enemyType, spawnPoint.Position);
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
        private void SpawnDemon()
        {
            if (portalSpawnPoints.Count == 0)
                return;

            Vector2 spawnPosition = portalSpawnPoints[random.Next(portalSpawnPoints.Count)];

            demons.Add(new Demon(demonTexture, spawnPosition));
        }
        private void UpdateWaveSpawnQueue(GameTime gameTime)
        {
            if (hasFinishedSpawningWave)
                return;

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;


            if (delayBetweenSpawnGroups > 0f)
            {
                delayBetweenSpawnGroups -= deltaTime;

                if (delayBetweenSpawnGroups > 0f)
                    return;

                delayBetweenSpawnGroups = 0f;
            }

            // no active group = we take the next one
            if (activeSpawnGroup == null)
            {
                if (spawnGroupQueue.Count == 0)
                {
                    hasFinishedSpawningWave = true;
                    return;
                }

                activeSpawnGroup = spawnGroupQueue.Dequeue();
                remainingEnemiesInActiveGroup = activeSpawnGroup.Count;

                // first enemy from 1st group shows up with no delay
                spawnTimer = 0f;
            }

            spawnTimer -= deltaTime;

            if (spawnTimer > 0f)
                return;

            SpawnEnemy(activeSpawnGroup.EnemyType);

            remainingEnemiesInActiveGroup--;
            spawnTimer = currentSpawnInterval;


            if (remainingEnemiesInActiveGroup <= 0)
            {
                delayBetweenSpawnGroups =
                    activeSpawnGroup.DelayAfterGroupSeconds;

                activeSpawnGroup = null;


                if (spawnGroupQueue.Count == 0)
                {
                    delayBetweenSpawnGroups = 0f;
                    hasFinishedSpawningWave = true;
                }
            }
        }
    }
}