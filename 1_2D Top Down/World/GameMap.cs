using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MonoGameLibrary.Graphics;
using System.Collections.Generic;
using Tiled;

namespace _1_2D_Top_Down
{
    /// <summary>
    /// Owns the loaded Tiled layers and their runtime collision and spawn data.
    /// </summary>
    public sealed class GameMap
    {
        private readonly StaticCollisionGrid collisionGrid = new(128);
        private readonly List<Rectangle> solidCollisionRectangles = new();
        private readonly List<StaticCollisionShape> solidCollisionShapes = new();
        private IReadOnlyList<Vector2> enemySpawnerPoints = new List<Vector2>();
        private IReadOnlyList<EnemySpawnPoint> enemySpawnPoints =
            new List<EnemySpawnPoint>();
        private IReadOnlyList<MissionTrigger> missionTriggers =
            new List<MissionTrigger>();

        public TiledTileLayer WaterLayer { get; private set; }
        public TiledTileLayer GroundLayer { get; private set; }
        public IReadOnlyList<TiledTileLayer> UnderGroundLayers { get; private set; } =
            new List<TiledTileLayer>();
        public IReadOnlyList<TiledTileLayer> OverGroundLayers { get; private set; } =
            new List<TiledTileLayer>();
        public IReadOnlyList<TiledTileLayer> ForegroundLayers { get; private set; } =
            new List<TiledTileLayer>();
        public TiledPropsLayer PropsLayer { get; private set; }
        public IReadOnlyList<Rectangle> SolidCollisionRectangles => solidCollisionRectangles;
        public IReadOnlyList<StaticCollisionShape> SolidCollisionShapes => solidCollisionShapes;
        public IReadOnlyList<Vector2> EnemySpawnerPoints => enemySpawnerPoints;
        public IReadOnlyList<EnemySpawnPoint> EnemySpawnPoints => enemySpawnPoints;
        public IReadOnlyList<MissionTrigger> MissionTriggers => missionTriggers;
        public Vector2 PlayerSpawnPosition { get; private set; }

        public Rectangle WorldBounds => new(
            0,
            0,
            (int)GroundLayer.WorldWidth,
            (int)GroundLayer.WorldHeight);

        public void Load(
            ContentManager content,
            string mapFileName,
            MapThemeDefinition theme,
            bool loadEnemySpawners,
            bool loadMissionData)
        {
            WaterLayer = TiledTileLayer.FromFile(
                content,
                mapFileName,
                theme.Scale,
                theme.WaterLayerName);

            GroundLayer = TiledTileLayer.FromFile(
                content,
                mapFileName,
                theme.Scale,
                theme.GroundLayerName);

            UnderGroundLayers = LoadTileLayers(
                content,
                mapFileName,
                theme,
                theme.UnderGroundLayers);

            OverGroundLayers = LoadTileLayers(
                content,
                mapFileName,
                theme,
                theme.OverGroundLayers);

            ForegroundLayers = LoadTileLayers(
                content,
                mapFileName,
                theme,
                theme.ForegroundLayers);

            PropsLayer = theme.UsesGridProps
                ? TiledPropsLayer.FromGridFile(
                    content,
                    mapFileName,
                    theme.PropsTextureAsset,
                    theme.PropsTileWidth,
                    theme.PropsTileHeight,
                    theme.Scale,
                    theme.PropsTilesetFile,
                    theme.PropsLayerName)
                : TiledPropsLayer.FromFile(
                    content,
                    mapFileName,
                    TextureAtlas.FromFile(content, theme.PropsAtlasAsset),
                    theme.Scale,
                    theme.PropsTilesetFile,
                    theme.PropsRegionNames,
                    theme.PropsLayerName);

            TiledCollisionLayer collisionLayer =
                TiledCollisionLayer.FromFile(content, mapFileName, theme.Scale);

            TiledWaterCollisionLayer waterCollisionLayer =
                TiledWaterCollisionLayer.FromFile(
                    content,
                    mapFileName,
                    theme.WaterTilesetFile,
                    theme.Scale,
                    theme.WaterLayerName,
                    theme.GroundLayerName);

            solidCollisionRectangles.Clear();
            solidCollisionRectangles.AddRange(collisionLayer.Rectangles);
            solidCollisionRectangles.AddRange(waterCollisionLayer.Rectangles);
            solidCollisionShapes.Clear();
            solidCollisionShapes.AddRange(collisionLayer.Shapes);
            solidCollisionShapes.AddRange(waterCollisionLayer.Shapes);
            solidCollisionShapes.AddRange(PropsLayer.CollisionShapes);
            collisionGrid.Build(solidCollisionShapes);

            if (loadEnemySpawners)
            {
                enemySpawnerPoints = TiledEnemySpawners.FromFile(
                    content,
                    mapFileName,
                    theme.Scale);
            }
            else
            {
                enemySpawnerPoints = new List<Vector2>();
            }

            TiledMissionObjects missionObjects =
                TiledMissionObjects.FromFile(
                    content,
                    mapFileName,
                    theme.Scale);

            PlayerSpawnPosition = missionObjects.PlayerSpawnPosition;

            if (loadMissionData)
            {
                TiledMissionTriggers tiledTriggers =
                    TiledMissionTriggers.FromFile(
                        content,
                        mapFileName,
                        theme.Scale);

                enemySpawnPoints = missionObjects.EnemySpawnPoints;
                missionTriggers = tiledTriggers.Triggers;
            }
            else
            {
                enemySpawnPoints = new List<EnemySpawnPoint>();
                missionTriggers = new List<MissionTrigger>();
            }
        }

        public bool IntersectsCollision(Rectangle bounds) =>
            collisionGrid.Intersects(bounds);

        public void Update(GameTime gameTime)
        {
            WaterLayer.Update(gameTime);
            GroundLayer.Update(gameTime);

            foreach (TiledTileLayer layer in UnderGroundLayers)
                layer.Update(gameTime);

            foreach (TiledTileLayer layer in OverGroundLayers)
                layer.Update(gameTime);

            foreach (TiledTileLayer layer in ForegroundLayers)
                layer.Update(gameTime);

        }

        private static IReadOnlyList<TiledTileLayer> LoadTileLayers(
            ContentManager content,
            string mapFileName,
            MapThemeDefinition theme,
            IReadOnlyList<MapTileLayerDefinition> definitions)
        {
            List<TiledTileLayer> layers = new(definitions.Count);

            foreach (MapTileLayerDefinition definition in definitions)
            {
                layers.Add(TiledTileLayer.FromFile(
                    content,
                    mapFileName,
                    theme.Scale,
                    definition.LayerName));
            }

            return layers;
        }
    }
}
