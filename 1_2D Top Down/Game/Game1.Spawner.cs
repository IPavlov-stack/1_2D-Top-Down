using Microsoft.Xna.Framework;
using System;

namespace _1_2D_Top_Down
{
    public partial class Game1
    {
        private void SpawnEnemy()
        {
            if (portalSpawnPoints.Count == 0)
                return;

            Vector2 spawnPosition = portalSpawnPoints[random.Next(portalSpawnPoints.Count)];

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