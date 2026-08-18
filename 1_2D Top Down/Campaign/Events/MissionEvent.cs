namespace _1_2D_Top_Down
{
    public abstract class MissionEvent
    {
    }

    public sealed class WaveCompletedMissionEvent : MissionEvent
    {
        public int WaveNumber { get; }

        public WaveCompletedMissionEvent(int waveNumber)
        {
            WaveNumber = waveNumber;
        }
    }

    public sealed class TriggerActivatedMissionEvent : MissionEvent
    {
        public string TriggerId { get; }

        public TriggerActivatedMissionEvent(string triggerId)
        {
            TriggerId = triggerId;
        }
    }

    public sealed class EnemyDefeatedMissionEvent : MissionEvent
    {
        public string EnemyId { get; }

        public EnemyDefeatedMissionEvent(string enemyId)
        {
            EnemyId = enemyId;
        }
    }

    public sealed class CollectibleCollectedMissionEvent : MissionEvent
    {
        public string CollectibleId { get; }
        public int Amount { get; }

        public CollectibleCollectedMissionEvent(
            string collectibleId,
            int amount)
        {
            CollectibleId = collectibleId;
            Amount = amount;
        }
    }
}
