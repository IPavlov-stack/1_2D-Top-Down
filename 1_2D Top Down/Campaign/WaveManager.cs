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


        // Реалният край на даден wave се потвърждава от MissionRuntime,
        // след като spawnerът е приключил и няма активни enemies в света
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

        public bool TryFinishActiveWave()
        {
            if (!IsWaveActive)
            {
                return false;
            }

            EnemiesRemaining = 0;
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