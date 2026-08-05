namespace _1_2D_Top_Down
{
    public sealed class SurvivalObjective : MissionObjective
    {
        public int TotalWaves { get; }
        public int CompletedWaves { get; private set; }

        public override string Description =>
            $"Survive {TotalWaves} waves";

        public SurvivalObjective(int totalWaves)
        {
            TotalWaves = totalWaves;
        }

        public void CompleteWave()
        {
            if (IsCompleted)
                return;

            CompletedWaves++;

            if (CompletedWaves >= TotalWaves)
            {
                IsCompleted = true;
            }
        }
        public override void OnWaveCompleted()
        {
            CompleteWave();
        }
        public override void Reset()
        {
            base.Reset();

            CompletedWaves = 0;
        }
    }
}