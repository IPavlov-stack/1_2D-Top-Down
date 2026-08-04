using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

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
        private void StartNextWave()
        {
            int waveIndex = waveManager.CurrentWave;

            if (waveIndex >= currentMission.Waves.Count)
            {
                // По-късно: Mission Complete.
                return;
            }

            WaveDefinition wave = currentMission.Waves[waveIndex];

            waveManager.StartNextWave(wave.TotalEnemyCount);

            spawnGroupQueue.Clear();

            foreach (EnemySpawnGroup group in wave.SpawnGroups)
            {
                spawnGroupQueue.Enqueue(group);
            }

            activeSpawnGroup = null;
            remainingEnemiesInActiveGroup = 0;

            currentSpawnInterval = wave.SpawnIntervalSeconds;
            spawnTimer = 0f;
            delayBetweenSpawnGroups = 0f;

            hasFinishedSpawningWave = false;
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

            float deltaTime =
                (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Пауза след предишната група.
            if (delayBetweenSpawnGroups > 0f)
            {
                delayBetweenSpawnGroups -= deltaTime;

                if (delayBetweenSpawnGroups > 0f)
                    return;

                delayBetweenSpawnGroups = 0f;
            }

            // Ако в момента няма активна група, взимаме следващата.
            if (activeSpawnGroup == null)
            {
                if (spawnGroupQueue.Count == 0)
                {
                    hasFinishedSpawningWave = true;
                    return;
                }

                activeSpawnGroup = spawnGroupQueue.Dequeue();
                remainingEnemiesInActiveGroup = activeSpawnGroup.Count;

                // Първият враг от новата група излиза веднага.
                spawnTimer = 0f;
            }

            spawnTimer -= deltaTime;

            if (spawnTimer > 0f)
                return;

            SpawnEnemy(activeSpawnGroup.EnemyType);

            remainingEnemiesInActiveGroup--;
            spawnTimer = currentSpawnInterval;

            // Последният враг от групата е spawn-нат.
            if (remainingEnemiesInActiveGroup <= 0)
            {
                delayBetweenSpawnGroups =
                    activeSpawnGroup.DelayAfterGroupSeconds;

                activeSpawnGroup = null;

                // Ако това е последната група, няма причина да чакаме
                // нейната пауза.
                if (spawnGroupQueue.Count == 0)
                {
                    delayBetweenSpawnGroups = 0f;
                    hasFinishedSpawningWave = true;
                }
            }
        }
    }
}