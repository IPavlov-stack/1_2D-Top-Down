using System;
using System.Collections.Generic;

namespace _1_2D_Top_Down
{
    /// <summary>
    /// Maps data-only behavior definitions to their runtime implementations.
    /// Adding a behavior no longer requires another EnemyFactory switch case.
    /// </summary>
    public sealed class EnemyBehaviorRegistry
    {
        private readonly Dictionary<Type, Func<EnemyBehaviorDefinition, IEnemyBehavior>>
            factories = new();

        public EnemyBehaviorRegistry()
        {
            Register<ChaseContactBehaviorDefinition>(
                definition => new ChaseContactBehavior(definition));

            Register<PoisonTrailBehaviorDefinition>(
                definition => new PoisonTrailBehavior(
                    Create(definition.InnerBehavior),
                    definition.PoisonEffect,
                    definition.SpawnInterval,
                    definition.OffsetInTiles));

            Register<KeepDistanceRangedBehaviorDefinition>(
                definition => new KeepDistanceRangedBehavior(definition));

            Register<TelegraphedAreaAttackBehaviorDefinition>(
                definition =>
                    new TelegraphedAreaAttackBehavior(definition));

            Register<ChargerBehaviorDefinition>(
                definition => new ChargerBehavior(definition));

            Register<NecromancerBehaviorDefinition>(
                definition => new NecromancerBehavior(
                    Create(definition.Ranged),
                    definition.SummonEnemyId,
                    definition.SummonCooldown,
                    definition.SummonRadius));
        }

        public void Register<TDefinition>(Func<TDefinition, IEnemyBehavior> factory)
            where TDefinition : EnemyBehaviorDefinition
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            factories[typeof(TDefinition)] =
                definition => factory((TDefinition)definition);
        }

        public IEnemyBehavior Create(EnemyBehaviorDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            if (!factories.TryGetValue(definition.GetType(), out var factory))
            {
                throw new InvalidOperationException(
                    $"No enemy behavior is registered for " +
                    $"'{definition.GetType().Name}'.");
            }

            return factory(definition);
        }
    }
}
