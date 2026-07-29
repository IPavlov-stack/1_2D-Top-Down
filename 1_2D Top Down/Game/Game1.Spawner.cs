using Microsoft.Xna.Framework;
using System;

namespace _1_2D_Top_Down
{
    public partial class Game1
    {
        private void SpawnEnemy()
        {
            int screenWidth = GraphicsDevice.Viewport.Width;
            int screenHeight = GraphicsDevice.Viewport.Height;

            Vector2 spawnPosition;

            int side = random.Next(4);

            switch (side)
            {
                case 0: // top
                    spawnPosition = new Vector2(random.Next(screenWidth), -50);
                    break;

                case 1: // bottom
                    spawnPosition = new Vector2(random.Next(screenWidth), screenHeight + 50);
                    break;

                case 2: // left
                    spawnPosition = new Vector2(-50, random.Next(screenHeight));
                    break;

                default: // right
                    spawnPosition = new Vector2(screenWidth + 50, random.Next(screenHeight));
                    break;
            }

            bool spawnEvilEye = Random.Shared.NextDouble() < 0.20;

            if (spawnEvilEye)
            {
                evilEyes.Add(new Evil_Eye(
                    evilEyeTexture,
                    spawnPosition));
            }
            else
            {
                demons.Add(new Demon(
                    demonTexture,
                    spawnPosition));
            }
        }
    }
}