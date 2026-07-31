using _2D_Top_Down;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGameLibrary.Graphics;
using System;
using System.Collections.Generic;
using Tiled;


namespace _1_2D_Top_Down
{
    public partial class Game1 : Game
    {
        //game info
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont gamefont;
        private Color BackgroundColor = new Color(119, 167, 255); // sky blue-ish
        private Texture2D pixelTexture;
        private bool isGameOver;
        private bool isEnemySpawningEnabled = true;
        private const int WindowSizeX = 1920;
        private const int WindowSizeY = 1080;


        //input info
        private MouseState previousMouseState;
        private KeyboardState previousKeyboard;

        //player info
        private Player player;
        private Vector2 playerStartPosition = new Vector2(2150, 1850);
        private Texture2D playerProjectileTexture;
        private List<PlayerProjectile> projectiles = new List<PlayerProjectile>();

        //collectables info
        private const int CoinDropChancePercent = 25;
        private Texture2D coinTexture;
        private List<Coin> coins = new List<Coin>();
        private int coinsCollected;
        private SoundEffect[] coinPickupSounds;

        //demon info
        private Texture2D demonTexture;
        private List<Demon> demons = new List<Demon>();
        private Texture2D demonDeathTexture;
        private List<DeathAnimation> demonDeathAnimations = new List<DeathAnimation>();

        //evil eye info
        private Texture2D evilEyeProjectileTexture;
        private Texture2D evilEyeTexture;
        private List<Evil_Eye> evilEyes = new List<Evil_Eye>();
        private List<EnemyProjectile> enemyProjectiles = new List<EnemyProjectile>();

        //spawner info
        private float spawnTimer;
        private const float SpawnInterval = 0.5f;

        //camera info
        private Camera camera;

        //world map info
        private const int WorldWidth = 3000;
        private const int WorldHeight = 2000;
        private const int TileSize = 64;
        private const float EnvironmentScale = 0.25f;
        private Texture2D forestTileset;
        private TextureAtlas environmentGroundAtlas;
        private TextureAtlas environmentPropsAtlas;
        private TiledTileLayer waterMap;
        private TiledTileLayer worldMap;
        private TiledPropsLayer propsLayer;
        private TiledCollisionLayer collisionLayer;
        private List<Rectangle> solidCollisionRectangles;

        //ui info

        //sound effects
        private const float SoundEffectsVolume = 0.65f;

        //music
        private Song backgroundMusic;
        private const float MusicVolume = 0.3f;

        //others
        private Random random = new Random();
        private bool isDeveloperMode;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = WindowSizeX;
            _graphics.PreferredBackBufferHeight = WindowSizeY;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            DisplayMode displayMode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;

            _graphics.PreferredBackBufferWidth = displayMode.Width;
            _graphics.PreferredBackBufferHeight = displayMode.Height;
            _graphics.IsFullScreen = true;

            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            camera = new Camera();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
            pixelTexture.SetData(new[] { Color.White });
            Texture2D playerTexture = Content.Load<Texture2D>("player/Character");
            demonTexture = Content.Load<Texture2D>("enemies/Demon/FLYING");
            demonDeathTexture = Content.Load<Texture2D>("enemies/Demon/DEATH");
            playerProjectileTexture = Content.Load<Texture2D>("projectiles/magic_projectile");
            coinTexture = Content.Load<Texture2D>("Collectables/coin");
            coinPickupSounds = new[]
            {
                Content.Load<SoundEffect>("Sounds/Coin/coin_1"),
                Content.Load<SoundEffect>("Sounds/Coin/coin_2"),
                Content.Load<SoundEffect>("Sounds/Coin/coin_3"),
            };
            evilEyeProjectileTexture = Content.Load<Texture2D>("projectiles/evilEye/evilEye_projectile_sphere");
            evilEyeTexture = Content.Load<Texture2D>("enemies/Evil Eye/Evil Eye Sprite sheet");
            gamefont = Content.Load<SpriteFont>("Sprite fonts/GameFont");


            environmentGroundAtlas = TextureAtlas.FromFile(Content, "Environment/EnvironmentGroundAtlas.xml");
            environmentPropsAtlas = TextureAtlas.FromFile(Content, "Environment/EnvironmentPropsAtlas.xml");
            waterMap = TiledTileLayer.FromFile(
                Content,
                "Maps/ForestMap.tmx",
                "Environment/Water/tileset_water256x256",
                "tileset_water256x256.tsx",
                EnvironmentScale,
                "Water");

            worldMap = TiledTileLayer.FromFile(
                Content,
                "Maps/ForestMap.tmx",
                "Environment/EnvironmentGroundAtlas",
                "EnvironmentGround.tsx",
                EnvironmentScale,
                "Ground");
            propsLayer = TiledPropsLayer.FromFile(Content, "Maps/ForestMap.tmx", environmentPropsAtlas, EnvironmentScale);
            collisionLayer = TiledCollisionLayer.FromFile(Content, "Maps/ForestMap.tmx", EnvironmentScale);
            TiledWaterCollisionLayer waterCollisionLayer = TiledWaterCollisionLayer.FromFile(Content, "Maps/ForestMap.tmx", "tileset_water256x256.tsx", EnvironmentScale);

            solidCollisionRectangles = new List<Rectangle>(collisionLayer.Rectangles);
            solidCollisionRectangles.AddRange(waterCollisionLayer.Rectangles);
            LoadPortals();

            player = new Player(playerTexture, playerStartPosition);

            backgroundMusic = Content.Load<Song>("Music/ambient_forest");

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = MusicVolume;

            if (MediaPlayer.State != MediaState.Stopped)
            {
                MediaPlayer.Stop();
            }

            MediaPlayer.Play(backgroundMusic);
        }
        protected override void Update(GameTime gameTime)
        {
            portalLayer.Update(gameTime);

            KeyboardState keyboard = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();

            HandleExit(keyboard);
            HandleDeveloperMode(keyboard);
            Vector2 playerCenter = player.Position +
                       new Vector2(player.texture.Width / 2, player.texture.Height / 2);

            Vector2 screenCenter = new Vector2(
                GraphicsDevice.Viewport.Width / 2,
                GraphicsDevice.Viewport.Height / 2);

            camera.Follow(playerCenter - screenCenter);
            if (!isGameOver)
            {
                HandlePlayerShooting(mouse, keyboard);
                UpdateGameObjects(gameTime);
            }
            else if (keyboard.IsKeyDown(Keys.R))
            {
                RestartGame();
            }

            previousKeyboard = keyboard;
            previousMouseState = mouse;

            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(
                isDeveloperMode
                    ? Color.DimGray
                    : isGameOver ? Color.Black : BackgroundColor);

            _spriteBatch.Begin(
                transformMatrix: camera.Transform,
                samplerState: SamplerState.PointClamp);

            if (isDeveloperMode)
            {
                DrawDeveloperMode();
            }
            else
            {
                DrawNormalWorld();
            }

            _spriteBatch.End();

            DrawUi();

            base.Draw(gameTime);
        }

    }
}
