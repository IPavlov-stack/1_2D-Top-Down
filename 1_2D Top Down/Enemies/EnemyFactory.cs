using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _1_2D_Top_Down
{
    public sealed class EnemyFactory
    {
        private readonly Func<string, Texture2D> loadTexture;
        private readonly EnemyBehaviorRegistry behaviorRegistry;
        private readonly EnemyDefinitionRegistry definitionRegistry;
        private readonly Dictionary<string, Texture2D> textureCache = new(
            StringComparer.OrdinalIgnoreCase);

        public EnemyFactory(
            Func<string, Texture2D> loadTexture,
            EnemyBehaviorRegistry? behaviorRegistry = null,
            EnemyDefinitionRegistry? definitionRegistry = null)
        {
            this.loadTexture = loadTexture;
            this.behaviorRegistry =
                behaviorRegistry ?? new EnemyBehaviorRegistry();
            this.definitionRegistry =
                definitionRegistry ?? EnemyDefinitions.Registry;
        }

        public Enemy Create(EnemyType type, Vector2 position)
        {
            return Create(definitionRegistry.Get(type), position);
        }

        public Enemy Create(string id, Vector2 position)
        {
            if (!definitionRegistry.TryGet(id, out EnemyDefinition definition))
                throw new InvalidOperationException($"Unknown enemy definition '{id}'.");

            return Create(definition, position);
        }

        public Texture2D GetTexture(string assetName)
        {
            if (!textureCache.TryGetValue(assetName, out Texture2D? texture))
            {
                texture = loadTexture(assetName);
                textureCache.Add(assetName, texture);
            }

            return texture;
        }

        private Enemy Create(EnemyDefinition definition, Vector2 position)
        {
            Texture2D texture = GetTexture(definition.TextureAsset);

            return new Enemy(
                texture,
                position,
                definition,
                behaviorRegistry.Create(definition.Behavior),
                GetTexture);
        }
    }
}
