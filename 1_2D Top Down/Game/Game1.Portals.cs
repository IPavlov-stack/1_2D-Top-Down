using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Tiled;

namespace _1_2D_Top_Down
{
    public partial class Game1
    {
        private List<Vector2> portalSpawnPoints = new();
        private TiledPortalLayer portalLayer;

        private void LoadPortals()
        {
            portalSpawnPoints = TiledPortalSpawns.FromFile(
                Content,
                "Maps/ForestMap.tmx",
                EnvironmentScale);

            portalLayer = TiledPortalLayer.FromFile(
                Content,
                "Maps/ForestMap.tmx",
                Content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Objects/Portal_orange-sheet"),
                EnvironmentScale);
        }
    }
}