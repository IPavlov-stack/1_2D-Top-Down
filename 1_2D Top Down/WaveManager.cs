namespace _1_2D_Top_Down
{
    public sealed class WaveManager
    {
        public int CurrentWave { get; private set; }
        public bool IsWaveActive { get; private set; }

        public void StartNextWave()
        {
            CurrentWave++;
            IsWaveActive = true;
        }

        public void FinishCurrentWave()
        {
            IsWaveActive = false;
        }

        public void Reset()
        {
            CurrentWave = 0;
            IsWaveActive = false;
        }
    }
}