namespace _1_2D_Top_Down
{
    /// <summary>
    /// Holds the mutable state of the mission currently being played.
    /// MissionDefinition remains immutable campaign data.
    /// </summary>
    public sealed class MissionRuntime
    {
        private bool isCompleted;

        public MissionDefinition Definition { get; private set; }
        public WaveManager Waves { get; } = new WaveManager();
        public MissionObjective? Objective { get; private set; }

        public bool IsCompleted => isCompleted;

        public MissionRuntime(MissionDefinition definition)
        {
            Definition = definition;
            Objective = CreateObjective(definition);
        }

        public void Start(MissionDefinition definition)
        {
            Definition = definition;
            Waves.Reset();
            Objective = CreateObjective(definition);
            isCompleted = false;
        }

        public void Restart()
        {
            Waves.Reset();
            Objective?.Reset();
            isCompleted = false;
        }

        public bool TryCompleteWave(bool hasFinishedSpawningWave)
        {
            bool isLastEnemyDefeated = Waves.RegisterEnemyDefeated();

            if (!hasFinishedSpawningWave || !isLastEnemyDefeated)
            {
                return false;
            }

            Objective?.OnWaveCompleted();
            isCompleted = Objective?.IsCompleted ?? false;
            return true;
        }

        public void Complete()
        {
            Objective?.MarkCompleted();
            isCompleted = true;
        }

        private static MissionObjective? CreateObjective(
            MissionDefinition definition)
        {
            return definition.Type == MissionType.Survival
                ? MissionObjectiveFactory.Create(definition)
                : null;
        }
    }
}