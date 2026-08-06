using Microsoft.Xna.Framework;

namespace Tiled
{
    public sealed class MissionTrigger
    {
        public string Name { get; }
        public Rectangle Bounds { get; }
        public bool IsActivated { get; private set; }

        public MissionTrigger(string name, Rectangle bounds)
        {
            Name = name;
            Bounds = bounds;
        }

        public void Activate()
        {
            IsActivated = true;
        }
    }
}