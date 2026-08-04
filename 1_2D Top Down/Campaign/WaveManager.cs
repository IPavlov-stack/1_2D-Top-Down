using System;

namespace _1_2D_Top_Down
{
    public sealed class WaveManager
    {
        public int CurrentWave { get; private set; }
        public int EnemiesRemaining { get; private set; }
        public bool IsWaveActive { get; private set; }

        public void StartNextWave(int enemyCount)
        {
            if (enemyCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(enemyCount));

            CurrentWave++;
            EnemiesRemaining = enemyCount;
            IsWaveActive = true;
        }

        // Връща true само когато е убит последният враг от wave-а.
        public bool RegisterEnemyDefeated()
        {
            if (!IsWaveActive || EnemiesRemaining <= 0)
                return false;

            EnemiesRemaining--;

            if (EnemiesRemaining > 0)
                return false;

            IsWaveActive = false;
            return true;
        }

        public void Reset()
        {
            CurrentWave = 0;
            EnemiesRemaining = 0;
            IsWaveActive = false;
        }
    }
}