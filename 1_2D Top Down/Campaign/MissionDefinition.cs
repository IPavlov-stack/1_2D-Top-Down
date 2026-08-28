using System.Collections.Generic;

#nullable enable

namespace _1_2D_Top_Down
{
    public sealed class MissionDefinition
    {
        public string Id { get; }
        public string Name { get; }
        public MissionType Type { get; }
        public string? MapFileName { get; }
        public string MapThemeId { get; }
        public IReadOnlyList<WaveDefinition> Waves { get; }

        public MissionDefinition(
            string name,
            MissionType type,
            params WaveDefinition[] waves)
            : this(name, name, type, null, MapThemes.OriginalForestId, waves)
        {
        }

        public MissionDefinition(
            string name,
            MissionType type,
            string? mapFileName,
            params WaveDefinition[] waves)
            : this(
                name,
                name,
                type,
                mapFileName,
                MapThemes.OriginalForestId,
                waves)
        {
        }

        public MissionDefinition(
            string name,
            MissionType type,
            string? mapFileName,
            string mapThemeId,
            params WaveDefinition[] waves)
            : this(name, name, type, mapFileName, mapThemeId, waves)
        {
        }

        public MissionDefinition(
            string id,
            string name,
            MissionType type,
            string? mapFileName,
            string mapThemeId,
            params WaveDefinition[] waves)
        {
            Id = id;
            Name = name;
            Type = type;
            MapFileName = mapFileName;
            MapThemeId = mapThemeId;
            Waves = waves;
        }
    }
}
