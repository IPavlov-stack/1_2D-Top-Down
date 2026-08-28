namespace _1_2D_Top_Down
{
    public static class CampaignMissions
    {
        public static MissionDefinition ForestOutskirts { get; } =
           new MissionDefinition(
                "chapter1.mission01",
                "Forest Outskirts",
                MissionType.Survival,
                "Maps/Chapter1/Mission01.tmx",
                MapThemes.Chapter1Id,

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

                    new EnemySpawnGroup(EnemyType.Lich, 1)),

                new WaveDefinition(
                    0.20f,
                    new EnemySpawnGroup(
                        EnemyType.Demon,
                        7,
                        delayAfterGroupSeconds: 1.5f),

                    new EnemySpawnGroup(EnemyType.Lich, 3),
                    new EnemySpawnGroup(EnemyType.Necromancer, 1)),

                new WaveDefinition(
                    0.18f,
                    new EnemySpawnGroup(EnemyType.Demon, 12)),

                new WaveDefinition(
                    0.16f,
                    new EnemySpawnGroup(
                        EnemyType.Demon,
                        15,
                        delayAfterGroupSeconds: 2f),

                    new EnemySpawnGroup(EnemyType.Lich, 3)),

                new WaveDefinition(
                    0.14f,
                    new EnemySpawnGroup(
                        EnemyType.Demon,
                        20,
                        delayAfterGroupSeconds: 2.5f),

                    new EnemySpawnGroup(EnemyType.Lich, 5)));

        public static MissionDefinition ForestPath { get; } =
            new MissionDefinition(
                "chapter1.mission02",
                "Forest Path",
                MissionType.Adventure,
                "Maps/Chapter1/Mission02.tmx",
                MapThemes.Chapter1Id);

        public static System.Collections.Generic.IReadOnlyList<MissionDefinition>
            All { get; } = new[]
            {
                ForestOutskirts,
                ForestPath
            };
    }
}
