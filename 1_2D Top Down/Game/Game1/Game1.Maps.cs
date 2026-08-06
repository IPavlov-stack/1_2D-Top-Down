using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Tiled;

namespace _1_2D_Top_Down
{
    public partial class Game1
    {
        private const string DefaultMapFileName = "Maps/ForestMap.tmx";

        private void LoadMissionMap(
            string mapFileName,
            bool loadPortals)
        {
            waterMap = TiledTileLayer.FromFile(
                Content,
                mapFileName,
                "Environment/Water/tileset_water256x256",
                "tileset_water256x256.tsx",
                EnvironmentScale,
                "Water");

            worldMap = TiledTileLayer.FromFile(
                Content,
                mapFileName,
                "Environment/EnvironmentGroundAtlas",
                "EnvironmentGround.tsx",
                EnvironmentScale,
                "Ground");

            propsLayer = TiledPropsLayer.FromFile(
                Content,
                mapFileName,
                environmentPropsAtlas,
                EnvironmentScale);

            collisionLayer = TiledCollisionLayer.FromFile(
                Content,
                mapFileName,
                EnvironmentScale);

            TiledWaterCollisionLayer waterCollisionLayer =
                TiledWaterCollisionLayer.FromFile(
                    Content,
                    mapFileName,
                    "tileset_water256x256.tsx",
                    EnvironmentScale);

            solidCollisionRectangles =
                new List<Rectangle>(collisionLayer.Rectangles);

            solidCollisionRectangles.AddRange(
                waterCollisionLayer.Rectangles);

            mapCollisionGrid.Build(solidCollisionRectangles);

            arePortalsActive = loadPortals;

            if (arePortalsActive)
            {
                LoadPortals(mapFileName);
            }
            else
            {
                portalSpawnPoints.Clear();
            }
        }
    }
}