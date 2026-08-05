namespace _1_2D_Top_Down
{
    public abstract class MissionObjective
    {
        public abstract string Description { get; }

        public bool IsCompleted { get; protected set; }
        public bool IsFailed { get; protected set; }

        public virtual void OnWaveCompleted()
        {
        }

        public virtual void Reset()
        {
            IsCompleted = false;
            IsFailed = false;
        }
    }
}