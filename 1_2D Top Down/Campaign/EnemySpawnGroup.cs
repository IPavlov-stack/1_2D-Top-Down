using System;

namespace _1_2D_Top_Down
{
    public sealed class EnemySpawnGroup
    {
        public EnemyType EnemyType { get; }
        public int Count { get; }
        public float DelayAfterGroupSeconds { get; }

        public EnemySpawnGroup(EnemyType enemyType, int count,float delayAfterGroupSeconds = 0f)
        {
            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            if (delayAfterGroupSeconds < 0f)
                throw new ArgumentOutOfRangeException(
                    nameof(delayAfterGroupSeconds));

            EnemyType = enemyType;
            Count = count;
            DelayAfterGroupSeconds = delayAfterGroupSeconds;
        }
    }
}