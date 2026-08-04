using System;
using System.Collections.Generic;
using System.Linq;

namespace _1_2D_Top_Down
{
    public sealed class WaveDefinition
    {
        public float SpawnIntervalSeconds { get; }
        public IReadOnlyList<EnemySpawnGroup> SpawnGroups { get; }

        public int TotalEnemyCount =>
            SpawnGroups.Sum(group => group.Count);

        public WaveDefinition(float spawnIntervalSeconds,params EnemySpawnGroup[] spawnGroups)
        {
            if (spawnIntervalSeconds <= 0f)
                throw new ArgumentOutOfRangeException(
                    nameof(spawnIntervalSeconds));

            if (spawnGroups == null || spawnGroups.Length == 0)
                throw new ArgumentException(
                    "A wave must contain at least one spawn group.",
                    nameof(spawnGroups));

            SpawnIntervalSeconds = spawnIntervalSeconds;
            SpawnGroups = spawnGroups;
        }
    }
}