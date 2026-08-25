using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace _1_2D_Top_Down
{
    /// <summary>
    /// Owns the active enemy lifecycle, spatial lookup and queued AI actions.
    /// </summary>
    public sealed class EnemyManager
    {
        private const int SpatialCellSize = 128;
        private readonly SpatialGrid<Enemy> enemySpatialGrid = new(SpatialCellSize);
        private readonly Queue<EnemySpawnGroup> spawnGroupQueue = new();

        private EnemySpawnGroup? activeSpawnGroup;
        private int remainingEnemiesInActiveGroup;
        private float spawnTimer;
        private float spawnIntervalSeconds;
        private float delayBetweenSpawnGroups;
        public bool HasFinishedSpawningWave { get; private set; }
        private readonly List<Enemy> nearbyEnemies = new();
        private readonly List<Enemy> enemies = new();
        private readonly List<EnemyActionRequest> pendingActions = new();
        public IReadOnlyList<Enemy> Enemies => enemies;
        public IReadOnlyList<EnemyActionRequest> PendingActions => pendingActions;
        public List<DeathAnimation> DeathAnimations { get; } = new();

        public void Clear()
        {
            enemies.Clear();
            pendingActions.Clear();
            DeathAnimations.Clear();

            spawnGroupQueue.Clear();
            activeSpawnGroup = null;
            remainingEnemiesInActiveGroup = 0;
            spawnTimer = 0f;
            spawnIntervalSeconds = 0f;
            delayBetweenSpawnGroups = 0f;
            HasFinishedSpawningWave = false;
        }
        public void Add(Enemy enemy)
        {
            enemies.Add(enemy);
        }

        public void RebuildSpatialGrid()
        {
            enemySpatialGrid.Rebuild(enemies);
        }

        public Enemy? FindIntersectingEnemy(Rectangle bounds)
        {
            enemySpatialGrid.QueryNearby(bounds, nearbyEnemies);

            foreach (Enemy enemy in nearbyEnemies)
            {
                if (!enemy.Health.IsDead &&
                    bounds.Intersects(enemy.Bounds))
                {
                    return enemy;
                }
            }

            return null;
        }
        public EnemyHitResult TryHitEnemy(Rectangle projectileBounds, CombatHit hit)
        {
            Enemy? enemy = FindIntersectingEnemy(projectileBounds);

            if (enemy == null)
            {
                return EnemyHitResult.Miss();
            }

            enemy.TakeHit(hit);

            if (!enemy.Health.IsDead)
            {
                return EnemyHitResult.Hit(enemy);
            }

            enemies.Remove(enemy);

            return EnemyHitResult.Defeat(enemy);
        }
        public void UpdateEnemies(
            GameTime gameTime,
            Player player,
            Rectangle worldBounds,
            Vector2 worldTileSize,
            Func<Rectangle, bool> intersectsMapCollision)
        {
            pendingActions.Clear();
            EnemyUpdateContext context =
                new EnemyUpdateContext(
                    player,
                    pendingActions,
                    worldBounds,
                    worldTileSize,
                    intersectsMapCollision);

            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                Enemy enemy = enemies[i];
                enemy.Update(gameTime, context);
            }
        }
        public void UpdateDeathAnimations(GameTime gameTime)
        {
            for (int i = DeathAnimations.Count - 1; i >= 0; i--)
            {
                DeathAnimation animation = DeathAnimations[i];

                animation.Update(gameTime);

                if (animation.IsFinished)
                {
                    DeathAnimations.RemoveAt(i);
                }
            }
        }
        public void StartSpawningWave(WaveDefinition wave)
        {
            spawnGroupQueue.Clear();

            foreach (EnemySpawnGroup group in wave.SpawnGroups)
            {
                spawnGroupQueue.Enqueue(group);
            }

            activeSpawnGroup = null;
            remainingEnemiesInActiveGroup = 0;

            spawnIntervalSeconds = wave.SpawnIntervalSeconds;
            spawnTimer = 0f;
            delayBetweenSpawnGroups = 0f;

            HasFinishedSpawningWave = false;
        }
        public void UpdateSpawnQueue(GameTime gameTime, Action<EnemyType> spawnEnemy)
        {
            if (HasFinishedSpawningWave)
            {
                return;
            }

            float deltaTime =
                (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (delayBetweenSpawnGroups > 0f)
            {
                delayBetweenSpawnGroups -= deltaTime;

                if (delayBetweenSpawnGroups > 0f)
                {
                    return;
                }

                delayBetweenSpawnGroups = 0f;
            }

            if (activeSpawnGroup == null)
            {
                if (spawnGroupQueue.Count == 0)
                {
                    HasFinishedSpawningWave = true;
                    return;
                }

                activeSpawnGroup = spawnGroupQueue.Dequeue();

                remainingEnemiesInActiveGroup =
                    activeSpawnGroup.Count;

                // firt enemy immediately shows up
                spawnTimer = 0f;
            }

            spawnTimer -= deltaTime;

            if (spawnTimer > 0f)
            {
                return;
            }

            spawnEnemy(activeSpawnGroup.EnemyType);

            remainingEnemiesInActiveGroup--;

            spawnTimer = spawnIntervalSeconds;

            if (remainingEnemiesInActiveGroup > 0)
            {
                return;
            }

            delayBetweenSpawnGroups =
                activeSpawnGroup.DelayAfterGroupSeconds;

            activeSpawnGroup = null;

            if (spawnGroupQueue.Count == 0)
            {

                delayBetweenSpawnGroups = 0f;

                HasFinishedSpawningWave = true;
            }
        }
    }
}
