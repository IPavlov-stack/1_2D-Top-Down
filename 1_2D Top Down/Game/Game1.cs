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
                Content.Load<SoundEffect>("Sounds/Coin/coin_4"),
                Content.Load<SoundEffect>("Sounds/Coin/coin_5")
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

            // Светът се мести се с камерата
            _spriteBatch.Begin(
                transformMatrix: camera.Transform,
                samplerState: SamplerState.PointClamp);

            if (isDeveloperMode)
            {
                DrawDeveloperMode();
            }
            else
            {
                DrawMap();

                foreach (Coin coin in coins)
                {
                    coin.Draw(_spriteBatch);
                }

                player.Draw(_spriteBatch);
                propsLayer.DrawInFrontOfPlayer(_spriteBatch, player.Bounds.Bottom);

                foreach (Projectile projectile in projectiles)
                {
                    projectile.Draw(_spriteBatch);
                }

                foreach (Demon demon in demons)
                {
                    demon.Draw(_spriteBatch);
                }

                foreach (DeathAnimation deathAnimation in demonDeathAnimations)
                {
                    deathAnimation.Draw(_spriteBatch);
                }

                foreach (Evil_Eye evilEye in evilEyes)
                {
                    evilEye.Draw(_spriteBatch);
                }

                foreach (EnemyProjectile enemyProjectile in enemyProjectiles)
                {
                    enemyProjectile.Draw(_spriteBatch);
                }

                DrawPlayerHealthBar();
            }

            _spriteBatch.End();
            //UI остава неподвижно на екрана
            _spriteBatch.Begin();

            _spriteBatch.DrawString(
                gamefont,
                $"Coins: {coinsCollected}",
                new Vector2(20, 20),
                Color.Gold);

            if (isGameOver)
            {
                Rectangle screenBounds = GraphicsDevice.Viewport.Bounds;

                _spriteBatch.Draw(
                    pixelTexture,
                    screenBounds,
                    Color.Black * 0.60f);

                const string title = "GAME OVER";
                const string restartText = "Press R to restart";
                const float titleScale = 3f;
                const float restartScale = 1.25f;
                const float spacing = 28f;

                Vector2 titleSize = gamefont.MeasureString(title) * titleScale;
                Vector2 restartSize = gamefont.MeasureString(restartText) * restartScale;
                float contentHeight = titleSize.Y + spacing + restartSize.Y;
                float top = (screenBounds.Height - contentHeight) / 2f;

                _spriteBatch.DrawString(
                    gamefont,
                    title,
                    new Vector2((screenBounds.Width - titleSize.X) / 2f, top),
                    Color.Red,
                    0f,
                    Vector2.Zero,
                    titleScale,
                    SpriteEffects.None,
                    0f);

                _spriteBatch.DrawString(
                    gamefont,
                    restartText,
                    new Vector2(
                        (screenBounds.Width - restartSize.X) / 2f,
                        top + titleSize.Y + spacing),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    restartScale,
                    SpriteEffects.None,
                    0f);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
        private void DrawMap()
        {
            waterMap.Draw(_spriteBatch);
            worldMap.Draw(_spriteBatch);
            portalLayer.Draw(_spriteBatch);
            propsLayer.DrawBehindPlayer(_spriteBatch, player.Bounds.Bottom);
        }
        private void DrawPlayerHealthBar()
        {
            const int barWidth = 80;
            const int barHeight = 11;
            const int borderSize = 2;
            const int distanceAbovePlayer = 18;

            float healthPercent =
                player.Health.CurrentHealth / (float)player.Health.MaxHealth;

            int x = player.Bounds.Center.X - barWidth / 2;
            int y = player.Bounds.Top - distanceAbovePlayer;

            Rectangle borderRectangle = new Rectangle(x, y, barWidth, barHeight);

            Rectangle backgroundRectangle = new Rectangle(
                x + borderSize,
                y + borderSize,
                barWidth - borderSize * 2,
                barHeight - borderSize * 2);

            Rectangle healthRectangle = new Rectangle(
                x + borderSize,
                y + borderSize,
                (int)(backgroundRectangle.Width * healthPercent),
                backgroundRectangle.Height);

            _spriteBatch.Draw(pixelTexture, borderRectangle, Color.Black);
            _spriteBatch.Draw(pixelTexture, backgroundRectangle, Color.DarkRed);
            _spriteBatch.Draw(pixelTexture, healthRectangle, Color.LimeGreen);
        }

        private void DrawDeveloperMode()
        {
            foreach (Rectangle collisionRectangle in solidCollisionRectangles)
            {
                DrawDebugRectangle(collisionRectangle, Color.White);
            }

            DrawDebugRectangle(player.Bounds, Color.DodgerBlue);

            foreach (Coin coin in coins)
            {
                DrawDebugRectangle(coin.Bounds, Color.Gold);
            }

            foreach (PlayerProjectile projectile in projectiles)
            {
                DrawDebugRectangle(projectile.Bounds, Color.LimeGreen);
            }

            foreach (Demon demon in demons)
            {
                DrawDebugRectangle(demon.Bounds, Color.Red);
            }

            foreach (Evil_Eye evilEye in evilEyes)
            {
                if (!evilEye.IsDead)
                {
                    DrawDebugRectangle(evilEye.Bounds, Color.OrangeRed);
                }
            }

            foreach (EnemyProjectile projectile in enemyProjectiles)
            {
                DrawDebugRectangle(projectile.Bounds, Color.MediumPurple);
            }
        }

        private void DrawDebugRectangle(Rectangle rectangle, Color color)
        {
            const int outlineThickness = 2;

            _spriteBatch.Draw(pixelTexture, rectangle, color * 0.25f);

            _spriteBatch.Draw(
                pixelTexture,
                new Rectangle(rectangle.X, rectangle.Y,
                    rectangle.Width, outlineThickness),
                color);

            _spriteBatch.Draw(
                pixelTexture,
                new Rectangle(rectangle.X, rectangle.Bottom - outlineThickness,
                    rectangle.Width, outlineThickness),
                color);

            _spriteBatch.Draw(
                pixelTexture,
                new Rectangle(rectangle.X, rectangle.Y,
                    outlineThickness, rectangle.Height),
                color);

            _spriteBatch.Draw(
                pixelTexture,
                new Rectangle(rectangle.Right - outlineThickness, rectangle.Y,
                    outlineThickness, rectangle.Height),
                color);
        }

    }
}