using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _1_2D_Top_Down
{
    public sealed class EnemyFactory
    {
        private readonly Func<string, Texture2D> loadTexture;
        private readonly Dictionary<string, Texture2D> textureCache = new(
            StringComparer.OrdinalIgnoreCase);

        public EnemyFactory(Func<string, Texture2D> loadTexture)
        {
            this.loadTexture = loadTexture;
        }

        public Enemy Create(EnemyType type, Vector2 position)
        {
            return Create(EnemyDefinitions.Get(type), position);
        }

        public Enemy Create(string id, Vector2 position)
        {
            if (!EnemyDefinitions.TryGet(id, out EnemyDefinition? definition))
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

            return definition.Type switch
            {
                EnemyType.Demon => new Demon(
                    texture,
                    position,
                    definition,
                    new ChaseContactBehavior(
                        definition.ContactDamageCooldownSeconds,
                        definition.AttackStateDurationSeconds)),
                EnemyType.EvilEye => new Evil_Eye(
                    texture,
                    position,
                    definition,
                    new KeepDistanceRangedBehavior(
                        movementAnimationRow: 0,
                        movementFrameCount: 4,
                        attackAnimationRow: 1,
                        attackFrameCount: 6,
                        projectileReleaseTime:
                            definition.ProjectileReleaseTimeSeconds,
                        attackDuration:
                            definition.AttackDurationSeconds)),
                EnemyType.Necromancer => new Necromancer(
                    texture,
                    position,
                    definition,
                    new NecromancerBehavior(
                        new KeepDistanceRangedBehavior(
                            movementAnimationRow: 0,
                            movementFrameCount: 4,
                            attackAnimationRow: 1,
                            attackFrameCount: 4,
                            projectileReleaseTime:
                                definition.ProjectileReleaseTimeSeconds,
                            attackDuration:
                                definition.AttackDurationSeconds,
                            rotateDuringAttack: false),
                        definition.SummonEnemyId!,
                        definition.SummonCooldownSeconds,
                        definition.SummonRadius)),
                _ => throw new InvalidOperationException(
                    $"No runtime enemy is registered for '{definition.Id}'.")
            };
        }
    }
}
