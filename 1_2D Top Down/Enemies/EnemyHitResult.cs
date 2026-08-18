namespace _1_2D_Top_Down
{
    public sealed class EnemyHitResult
    {
        public Enemy? Enemy { get; }
        public bool HasHit => Enemy != null;

        public bool HasDefeatedEnemy { get; }

        private EnemyHitResult(
            Enemy? enemy,
            bool hasDefeatedEnemy)
        {
            Enemy = enemy;
            HasDefeatedEnemy = hasDefeatedEnemy;
        }

        public static EnemyHitResult Miss()
        {
            return new EnemyHitResult(null, false);
        }

        public static EnemyHitResult Hit(Enemy enemy)
        {
            return new EnemyHitResult(enemy, false);
        }

        public static EnemyHitResult Defeat(Enemy enemy)
        {
            return new EnemyHitResult(enemy, true);
        }
    }
}
