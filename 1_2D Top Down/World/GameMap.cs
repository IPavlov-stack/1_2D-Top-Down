using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;
using System.Collections.Generic;
using Tiled;

namespace _1_2D_Top_Down
{
    /// <summary>
    /// Owns the loaded Tiled layers and their runtime collision and portal data.
    /// </summary>
    public sealed class GameMap
    {
        private readonly StaticCollisionGrid collisionGrid = new(128);
        private readonly List<Rectangle> solidCollisionRectangles = new();
        private IReadOnlyList<Vector2> portalSpawnPoints = new List<Vector2>();
        private IReadOnlyList<EnemySpawnPoint> enemySpawnPoints =
            new List<EnemySpawnPoint>();
        private IReadOnlyList<MissionTrigger> missionTriggers =
            new List<MissionTrigger>();

        public TiledTileLayer WaterLayer { get; private set; }
        public TiledTileLayer GroundLayer { get; private set; }
        public TiledPropsLayer PropsLayer { get; private set; }
        public TiledPortalLayer PortalLayer { get; private set; }
        public IReadOnlyList<Rectangle> SolidCollisionRectangles => solidCollisionRectangles;
        public IReadOnlyList<Vector2> PortalSpawnPoints => portalSpawnPoints;
        public IReadOnlyList<EnemySpawnPoint> EnemySpawnPoints => enemySpawnPoints;
        public IReadOnlyList<MissionTrigger> MissionTriggers => missionTriggers;
        public Vector2 PlayerSpawnPosition { get; private set; }
        public bool ArePortalsActive { get; private set; }

        public Rectangle WorldBounds => new(
            0,
            0,
            (int)GroundLayer.WorldWidth,
            (int)GroundLayer.WorldHeight);

        public void Load(
            ContentManager content,
            string mapFileName,
            TextureAtlas propsAtlas,
            Texture2D portalTexture,
            float mapScale,
            bool loadPortals,
            bool loadMissionData)
        {
            WaterLayer = TiledTileLayer.FromFile(
                content,
                mapFileName,
                "Environment/Water/tileset_water256x256",
                "tileset_water256x256.tsx",
                mapScale,
                "Water");

            GroundLayer = TiledTileLayer.FromFile(
                content,
                mapFileName,
                "Environment/EnvironmentGroundAtlas",
                "EnvironmentGround.tsx",
                mapScale,
                "Ground");

            PropsLayer = TiledPropsLayer.FromFile(
                content,
                mapFileName,
                propsAtlas,
                mapScale);

            TiledCollisionLayer collisionLayer =
                TiledCollisionLayer.FromFile(content, mapFileName, mapScale);

            TiledWaterCollisionLayer waterCollisionLayer =
                TiledWaterCollisionLayer.FromFile(
                    content,
                    mapFileName,
                    "tileset_water256x256.tsx",
                    mapScale);

            solidCollisionRectangles.Clear();
            solidCollisionRectangles.AddRange(collisionLayer.Rectangles);
            solidCollisionRectangles.AddRange(waterCollisionLayer.Rectangles);
            collisionGrid.Build(solidCollisionRectangles);

            ArePortalsActive = loadPortals;

            if (loadPortals)
            {
                portalSpawnPoints = TiledPortalSpawns.FromFile(
                    content,
                    mapFileName,
                    mapScale);

                PortalLayer = TiledPortalLayer.FromFile(
                    content,
                    mapFileName,
                    portalTexture,
                    mapScale);
            }
            else
            {
                portalSpawnPoints = new List<Vector2>();
                PortalLayer = null;
            }

            if (loadMissionData)
            {
                TiledMissionObjects missionObjects =
                    TiledMissionObjects.FromFile(
                        content,
                        mapFileName,
                        mapScale);

                TiledMissionTriggers tiledTriggers =
                    TiledMissionTriggers.FromFile(
                        content,
                        mapFileName,
                        mapScale);

                PlayerSpawnPosition = missionObjects.PlayerSpawnPosition;
                enemySpawnPoints = missionObjects.EnemySpawnPoints;
                missionTriggers = tiledTriggers.Triggers;
            }
            else
            {
                PlayerSpawnPosition = Vector2.Zero;
                enemySpawnPoints = new List<EnemySpawnPoint>();
                missionTriggers = new List<MissionTrigger>();
            }
        }

        public bool IntersectsCollision(Rectangle bounds) =>
            collisionGrid.Intersects(bounds);

        public void Update(GameTime gameTime)
        {
            if (ArePortalsActive)
                PortalLayer.Update(gameTime);
        }
    }
}
