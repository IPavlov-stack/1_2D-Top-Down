using System.Collections.Generic;

namespace _1_2D_Top_Down
{
    public sealed class MissionDefinition
    {
        public string Name { get; }
        public IReadOnlyList<WaveDefinition> Waves { get; }

        public MissionDefinition(
            string name,
            params WaveDefinition[] waves)
        {
            Name = name;
            Waves = waves;
        }
    }
}