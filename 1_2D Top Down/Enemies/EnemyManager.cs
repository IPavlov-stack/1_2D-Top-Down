using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace _1_2D_Top_Down
{
    /// <summary>
    /// Owns enemy runtime collections for the active mission.
    /// Enemy behaviour remains in Game1 during this transitional step.
    /// </summary>
    public sealed class EnemyManager
    {
        private const int SpatialCellSize = 128;
        private readonly SpatialGrid<Demon> demonSpatialGrid = new(SpatialCellSize);
        private readonly SpatialGrid<Evil_Eye> evilEyeSpatialGrid = new(SpatialCellSize);
        private readonly Queue<EnemySpawnGroup> spawnGroupQueue = new();

        private EnemySpawnGroup? activeSpawnGroup;
        private int remainingEnemiesInActiveGroup;
        private float spawnTimer;
        private float spawnIntervalSeconds;
        private float delayBetweenSpawnGroups;
        public bool HasFinishedSpawningWave { get; private set; }
        private readonly List<Demon> nearbyDemons = new();
        private readonly List<Evil_Eye> nearbyEvilEyes = new();
        public List<Demon> Demons { get; } = new();
        public List<Evil_Eye> EvilEyes { get; } = new();
        public List<DeathAnimation> DemonDeathAnimations { get; } = new();

        public void Clear()
        {
            Demons.Clear();
            EvilEyes.Clear();
            DemonDeathAnimations.Clear();

            spawnGroupQueue.Clear();
            activeSpawnGroup = null;
            remainingEnemiesInActiveGroup = 0;
            spawnTimer = 0f;
            spawnIntervalSeconds = 0f;
            delayBetweenSpawnGroups = 0f;
            HasFinishedSpawningWave = false;
        }
        public void RebuildSpatialGrids()
        {
            demonSpatialGrid.Rebuild(Demons);
            evilEyeSpatialGrid.Rebuild(EvilEyes);
        }

        public Demon? FindIntersectingDemon(Rectangle bounds)
        {
            demonSpatialGrid.QueryNearby(bounds, nearbyDemons);

            foreach (Demon demon in nearbyDemons)
            {
                if (!demon.Health.IsDead &&
                    bounds.Intersects(demon.Bounds))
                {
                    return demon;
                }
            }

            return null;
        }

        public Evil_Eye? FindIntersectingEvilEye(Rectangle bounds)
        {
            evilEyeSpatialGrid.QueryNearby(bounds, nearbyEvilEyes);

            foreach (Evil_Eye evilEye in nearbyEvilEyes)
            {
                if (!evilEye.IsDead &&
                    bounds.Intersects(evilEye.Bounds))
                {
                    return evilEye;
                }
            }

            return null;
        }
        public EnemyHitResult TryHitEnemy(Rectangle projectileBounds,int damage,Vector2 attackPosition,float knockbackForce)
        {
            Demon? demon = FindIntersectingDemon(projectileBounds);

            if (demon != null)
            {
                demon.Health.TakeDamage(damage);

                demon.ApplyKnockback(attackPosition,knockbackForce);

                if (!demon.Health.IsDead)
                {
                    return EnemyHitResult.Hit(demon);
                }

                Demons.Remove(demon);

                return EnemyHitResult.Defeat( demon, EnemyType.Demon);
            }

            Evil_Eye? evilEye = FindIntersectingEvilEye(projectileBounds);

            if (evilEye == null)
            {
                return EnemyHitResult.Miss();
            }

            evilEye.Health.TakeDamage(damage);

            evilEye.ApplyKnockback(attackPosition, knockbackForce);

            if (!evilEye.Health.IsDead)
            {
                return EnemyHitResult.Hit(evilEye);
            }

            evilEye.Die();

            return EnemyHitResult.Defeat(evilEye,EnemyType.EvilEye);
        }
        public bool UpdateDemons( GameTime gameTime, Player player)
        {
            for (int i = Demons.Count - 1; i >= 0; i--)
            {
                Demon demon = Demons[i];

                demon.Update(gameTime, player);

                if (player.Bounds.Intersects(demon.Bounds) &&
                    demon.TryDamagePlayer(player, 20) &&
                    player.Health.IsDead)
                {
                    return true;
                }
            }

            return false;
        }
        public void UpdateEvilEyes(GameTime gameTime, Player player, Texture2D projectileTexture, Action<EnemyProjectile> spawnProjectile)
        {
            for (int i = EvilEyes.Count - 1; i >= 0; i--)
            {
                Evil_Eye evilEye = EvilEyes[i];

                EnemyProjectile? projectile = evilEye.Update(gameTime, player,projectileTexture);

                if (projectile != null)
                {
                    spawnProjectile(projectile);
                }

                if (evilEye.IsDeathAnimationFinished)
                {
                    EvilEyes.RemoveAt(i);
                }
            }
        }
        public void UpdateDemonDeathAnimations( GameTime gameTime)
        {
            for (int i = DemonDeathAnimations.Count - 1; i >= 0; i--)
            {
                DeathAnimation animation =DemonDeathAnimations[i];

                animation.Update(gameTime);

                if (animation.IsFinished)
                {
                    DemonDeathAnimations.RemoveAt(i);
                }
            }
        }

        public void UpdateEvilEyeDeathAnimations(GameTime gameTime,Player player,Texture2D projectileTexture)
        {
            for (int i = EvilEyes.Count - 1; i >= 0; i--)
            {
                Evil_Eye evilEye = EvilEyes[i];

                if (!evilEye.IsDead)
                {
                    continue;
                }

                evilEye.Update(
                    gameTime,
                    player,
                    projectileTexture);

                if (evilEye.IsDeathAnimationFinished)
                {
                    EvilEyes.RemoveAt(i);
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
        public void SpawnEnemy( EnemyType enemyType, Vector2 spawnPosition, Texture2D demonTexture, Texture2D evilEyeTexture)
        {
            switch (enemyType)
            {
                case EnemyType.Demon:
                    Demons.Add(new Demon(demonTexture,spawnPosition));
                    break;

                case EnemyType.EvilEye:
                    EvilEyes.Add(new Evil_Eye( evilEyeTexture,spawnPosition));
                    break;
            }
        }
    }
}