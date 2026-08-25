using System;
using System.Collections.Generic;

namespace _1_2D_Top_Down
{
    /// <summary>
    /// Owns the catalog of enemy data definitions. New definitions can be
    /// registered without changing EnemyFactory or adding runtime subclasses.
    /// </summary>
    public sealed class EnemyDefinitionRegistry
    {
        private readonly Dictionary<EnemyType, EnemyDefinition> byType = new();
        private readonly Dictionary<string, EnemyDefinition> byId = new(
            StringComparer.OrdinalIgnoreCase);

        public void Register(
            EnemyDefinition definition,
            params string[] aliases)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            byType[definition.Type] = definition;
            byId[definition.Id] = definition;

            foreach (string alias in aliases)
            {
                if (!string.IsNullOrWhiteSpace(alias))
                    byId[alias] = definition;
            }
        }

        public EnemyDefinition Get(EnemyType type) =>
            byType.TryGetValue(type, out EnemyDefinition definition)
                ? definition
                : throw new KeyNotFoundException(
                    $"No enemy definition is registered for '{type}'.");

        public EnemyDefinition Get(string id) =>
            TryGet(id, out EnemyDefinition definition)
                ? definition
                : throw new KeyNotFoundException(
                    $"No enemy definition is registered for '{id}'.");

        public bool TryGet(string id, out EnemyDefinition definition) =>
            byId.TryGetValue(id, out definition);
    }
}
