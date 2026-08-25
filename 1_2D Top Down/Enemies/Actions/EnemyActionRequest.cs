using Microsoft.Xna.Framework;

namespace _1_2D_Top_Down
{
    public abstract class EnemyActionRequest
    {
    }

    public sealed class DamagePlayerRequest : EnemyActionRequest
    {
        public CombatHit Hit { get; }

        public DamagePlayerRequest(CombatHit hit)
        {
            Hit = hit;
        }
    }

    public sealed class ProjectileSpawnRequest : EnemyActionRequest
    {
        public ProjectileSpec Spec { get; }
        public string SourceId { get; }
        public Vector2 Position { get; }
        public Vector2 Direction { get; }

        public ProjectileSpawnRequest(
            ProjectileSpec spec,
            string sourceId,
            Vector2 position,
            Vector2 direction)
        {
            Spec = spec;
            SourceId = sourceId;
            Position = position;
            Direction = direction;
        }
    }

    public sealed class EnemySpawnRequest : EnemyActionRequest
    {
        public string EnemyId { get; }
        public Vector2 Position { get; }

        public EnemySpawnRequest(string enemyId, Vector2 position)
        {
            EnemyId = enemyId;
            Position = position;
        }
    }

    public sealed class AreaEffectSpawnRequest : EnemyActionRequest
    {
        public AreaEffectDefinition Definition { get; }
        public Vector2 Center { get; }
        public Vector2 WorldTileSize { get; }
        public CombatFaction SourceFaction { get; }
        public string SourceId { get; }

        public AreaEffectSpawnRequest(
            AreaEffectDefinition definition,
            Vector2 center,
            Vector2 worldTileSize,
            CombatFaction sourceFaction,
            string sourceId)
        {
            Definition = definition;
            Center = center;
            WorldTileSize = worldTileSize;
            SourceFaction = sourceFaction;
            SourceId = sourceId;
        }
    }
}
