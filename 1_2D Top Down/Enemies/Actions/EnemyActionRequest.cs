using Microsoft.Xna.Framework;

namespace _1_2D_Top_Down
{
    public abstract class EnemyActionRequest
    {
    }

    public sealed class DamagePlayerRequest : EnemyActionRequest
    {
        public int Damage { get; }

        public DamagePlayerRequest(int damage)
        {
            Damage = damage;
        }
    }

    public sealed class ProjectileSpawnRequest : EnemyActionRequest
    {
        public string ProjectileAsset { get; }
        public Vector2 Position { get; }
        public Vector2 Direction { get; }

        public ProjectileSpawnRequest(
            string projectileAsset,
            Vector2 position,
            Vector2 direction)
        {
            ProjectileAsset = projectileAsset;
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
}
