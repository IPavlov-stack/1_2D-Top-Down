using System;
using System.Collections.Generic;

namespace _1_2D_Top_Down
{
    /// <summary>
    /// Applies the commands produced by enemy AI after the enemy update pass.
    /// AI code stays independent from Game1 and from concrete world managers.
    /// </summary>
    public sealed class EnemyActionProcessor
    {
        private readonly EnemyManager enemyManager;
        private readonly ProjectileManager projectileManager;
        private readonly WorldEffectManager worldEffectManager;
        private readonly EnemyFactory enemyFactory;

        public EnemyActionProcessor(
            EnemyManager enemyManager,
            ProjectileManager projectileManager,
            WorldEffectManager worldEffectManager,
            EnemyFactory enemyFactory)
        {
            this.enemyManager = enemyManager;
            this.projectileManager = projectileManager;
            this.worldEffectManager = worldEffectManager;
            this.enemyFactory = enemyFactory;
        }

        public bool Process(
            IReadOnlyList<EnemyActionRequest> requests,
            Player player)
        {
            foreach (EnemyActionRequest request in requests)
            {
                switch (request)
                {
                    case DamagePlayerRequest damageRequest:
                        if (!player.Health.IsDead)
                            player.TakeHit(damageRequest.Hit);
                        break;

                    case ProjectileSpawnRequest projectileRequest:
                        projectileManager.AddEnemyProjectile(
                            new EnemyProjectile(
                                enemyFactory.GetTexture(
                                    projectileRequest.Spec.AssetName),
                                projectileRequest.Position,
                                projectileRequest.Direction,
                                projectileRequest.Spec,
                                projectileRequest.SourceId));
                        break;

                    case EnemySpawnRequest spawnRequest:
                        enemyManager.Add(enemyFactory.Create(
                            spawnRequest.EnemyId,
                            spawnRequest.Position));
                        break;

                    case AreaEffectSpawnRequest areaEffectRequest:
                        worldEffectManager.Add(areaEffectRequest);
                        break;

                    default:
                        throw new InvalidOperationException(
                            $"Unsupported enemy action request " +
                            $"'{request.GetType().Name}'.");
                }
            }

            return player.Health.IsDead;
        }
    }
}
