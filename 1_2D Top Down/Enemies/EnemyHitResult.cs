namespace _1_2D_Top_Down
{
    /// <summary>
    /// Describes the result of one player projectile hitting an enemy.
    /// Game1 uses this to apply mission-wide effects such as XP, drops,
    /// sounds and visual death animations.
    /// </summary>
    public readonly struct EnemyHitResult
    {
        public Enemy? Enemy { get; }
        public EnemyType? DefeatedEnemyType { get; }

        public bool HasHit => Enemy != null;
        public bool HasDefeatedEnemy => DefeatedEnemyType != null;

        private EnemyHitResult(Enemy? enemy, EnemyType? defeatedEnemyType)
        {
            Enemy = enemy;
            DefeatedEnemyType = defeatedEnemyType;
        }

        public static EnemyHitResult Miss() => new(null, null);

        public static EnemyHitResult Hit(Enemy enemy) => new(enemy, null);

        public static EnemyHitResult Defeat(
            Enemy enemy,
            EnemyType enemyType) => new(enemy, enemyType);
    }
}