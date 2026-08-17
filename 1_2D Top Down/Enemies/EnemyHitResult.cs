namespace _1_2D_Top_Down
{
    public sealed class EnemyHitResult
    {
        public Enemy? Enemy { get; }
        public EnemyType? DefeatedEnemyType { get; }

        public bool HasHit => Enemy != null;

        public bool HasDefeatedEnemy =>  DefeatedEnemyType.HasValue;

        private EnemyHitResult(
            Enemy? enemy,
            EnemyType? defeatedEnemyType)
        {
            Enemy = enemy;
            DefeatedEnemyType = defeatedEnemyType;
        }

        public static EnemyHitResult Miss()
        {
            return new EnemyHitResult(null, null);
        }

        public static EnemyHitResult Hit(Enemy enemy)
        {
            return new EnemyHitResult(enemy, null);
        }

        public static EnemyHitResult Defeat(
            Enemy enemy,
            EnemyType enemyType)
        {
            return new EnemyHitResult(enemy, enemyType);
        }
    }
}