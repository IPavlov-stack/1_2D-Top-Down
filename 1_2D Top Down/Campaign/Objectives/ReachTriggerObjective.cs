using System;

namespace _1_2D_Top_Down
{
    public sealed class ReachTriggerObjective : MissionObjective
    {
        public string TargetTriggerId { get; }

        public override string Description =>
            $"Reach {TargetTriggerId}";

        public ReachTriggerObjective(string targetTriggerId)
        {
            TargetTriggerId = targetTriggerId;
        }

        public override void HandleEvent(MissionEvent missionEvent)
        {
            if (missionEvent is TriggerActivatedMissionEvent triggerEvent &&
                triggerEvent.TriggerId.Equals(
                    TargetTriggerId,
                    StringComparison.OrdinalIgnoreCase))
            {
                MarkCompleted();
            }
        }
    }
}
