using System;

namespace _1_2D_Top_Down
{
    public static class MissionObjectiveFactory
    {
        public static MissionObjective Create(
            MissionDefinition mission)
        {
            return mission.Type switch
            {
                MissionType.Survival =>
                    new SurvivalObjective(mission.Waves.Count),

                MissionType.Adventure =>
                    new ReachTriggerObjective("Exit"),

                _ => throw new NotSupportedException(
                    $"Mission type {mission.Type} is not implemented yet.")
            };
        }
    }
}
