namespace _1_2D_Top_Down
{
    public static class CampaignMissions
    {
        public static MissionDefinition ForestOutskirts { get; } =
           new MissionDefinition( "Forest Outskirts", MissionType.Survival,

                new WaveDefinition(
                    0.25f,
                    new EnemySpawnGroup(EnemyType.Demon, 2)),

                new WaveDefinition(
                    0.25f,
                    new EnemySpawnGroup(EnemyType.Demon, 5)),

                new WaveDefinition(
                    0.25f,
                    new EnemySpawnGroup(
                        EnemyType.Demon,
                        6,
                        delayAfterGroupSeconds: 1f),

                    new EnemySpawnGroup(EnemyType.EvilEye, 1)),

                new WaveDefinition(
                    0.20f,
                    new EnemySpawnGroup(
                        EnemyType.Demon,
                        7,
                        delayAfterGroupSeconds: 1.5f),

                    new EnemySpawnGroup(EnemyType.EvilEye, 3)),

                new WaveDefinition(
                    0.18f,
                    new EnemySpawnGroup(EnemyType.Demon, 12)),

                new WaveDefinition(
                    0.16f,
                    new EnemySpawnGroup(
                        EnemyType.Demon,
                        15,
                        delayAfterGroupSeconds: 2f),

                    new EnemySpawnGroup(EnemyType.EvilEye, 3)),

                new WaveDefinition(
                    0.14f,
                    new EnemySpawnGroup(
                        EnemyType.Demon,
                        20,
                        delayAfterGroupSeconds: 2.5f),

                    new EnemySpawnGroup(EnemyType.EvilEye, 5)));

        public static MissionDefinition ForestPath { get; } =
            new MissionDefinition("Forest Path", MissionType.Adventure, "Maps/Mission_2.tmx");
    }
}