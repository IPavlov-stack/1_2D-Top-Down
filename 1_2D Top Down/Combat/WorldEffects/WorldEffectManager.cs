using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _1_2D_Top_Down
{
    public sealed class WorldEffectManager
    {
        private readonly List<WorldAreaEffect> effects = new();

        public IReadOnlyList<WorldAreaEffect> Effects => effects;

        public void Add(
            AreaEffectSpawnRequest request,
            Func<string, Texture2D> textureResolver = null)
        {
            effects.Add(new WorldAreaEffect(
                request.Definition,
                request.Center,
                request.WorldTileSize,
                request.SourceFaction,
                request.SourceId,
                textureResolver));
        }

        public bool Update(GameTime gameTime, Player player)
        {
            bool playerDied = false;

            for (int i = effects.Count - 1; i >= 0; i--)
            {
                WorldAreaEffect effect = effects[i];
                playerDied |= effect.Update(gameTime, player);

                if (effect.IsFinished)
                    effects.RemoveAt(i);
            }

            return playerDied;
        }

        public void Draw(
            SpriteBatch spriteBatch,
            Texture2D pixelTexture,
            Texture2D circleTexture)
        {
            foreach (WorldAreaEffect effect in effects)
                effect.Draw(spriteBatch, pixelTexture, circleTexture);
        }

        public void Clear() => effects.Clear();
    }
}
