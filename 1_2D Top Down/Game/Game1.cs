using _2D_Top_Down;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using MonoGameLibrary.Graphics;


namespace _1_2D_Top_Down
{
    public partial class Game1 : Game
    {
        //game info
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont gamefont;
        private Color BackgroundColor = new Color(119, 167, 255);
        private const int WindowSizeX = 1920;
        private const int WindowSizeY = 1080;
        private Texture2D pixelTexture;
        private bool isGameOver;
        private bool isEnemySpawningEnabled = true;

        private Random random = new Random();
        private MouseState previousMouseState;
        private KeyboardState previousKeyboard;

        //player info
        private Player player;
        private Vector2 playerStartPosition = new Vector2(400, 500);
        private Texture2D playerProjectileTexture;
        private List<PlayerProjectile> projectiles = new List<PlayerProjectile>();

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
        private const float SpawnInterval = 0.3f;

        
        private Camera camera;
        private const int WorldWidth = 3000;
        private const int WorldHeight = 2000;

        private Texture2D forestTileset;
        private const int TileSize = 64;
        private const int TilesPerRow = 14;
        private TextureAtlas environmentGroundAtlas;
        private TextureAtlas environmentPropsAtlas;
        private const float EnvironmentScale = 0.25f;
        private TiledGroundMap worldMap;
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
            player = new Player(playerTexture, playerStartPosition);
            demonTexture = Content.Load<Texture2D>("enemies/Demon/FLYING");
            demonDeathTexture = Content.Load<Texture2D>("enemies/Demon/DEATH");
            playerProjectileTexture = Content.Load<Texture2D>("projectiles/magic_projectile");
            evilEyeProjectileTexture = Content.Load<Texture2D>("projectiles/evilEye/evilEye_projectile_sphere");
            gamefont = Content.Load<SpriteFont>("Sprite fonts/GameFont");
            evilEyeTexture = Content.Load<Texture2D>("enemies/Evil Eye/Evil Eye Sprite sheet");
            environmentGroundAtlas = TextureAtlas.FromFile(Content,"Environment/EnvironmentGroundAtlas.xml");
            environmentPropsAtlas = TextureAtlas.FromFile(Content,"Environment/EnvironmentPropsAtlas.xml");
            worldMap = TiledGroundMap.FromFile(Content,"Maps/ForestMap.tmx","Environment/EnvironmentGroundAtlas",EnvironmentScale);
        }
        private void DrawTile(int column, int row, Vector2 position)
        {
            Rectangle sourceRectangle = new Rectangle(
                column * TileSize,
                row * TileSize,
                TileSize,
                TileSize);

            _spriteBatch.Draw(
                forestTileset,
                position,
                sourceRectangle,
                Color.White);
        }
        private void DrawEnvironment(TextureAtlas atlas, string regionName, Vector2 position)
        {
            TextureRegion region = atlas.GetRegion(regionName);

            region.Draw(
                _spriteBatch,
                position,
                Color.White,
                0f,
                Vector2.Zero,
                EnvironmentScale,
                SpriteEffects.None,
                0f);
        }
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();

            HandleExit(keyboard);
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
            GraphicsDevice.Clear(isGameOver ? Color.Black : BackgroundColor);

            // Светът — мести се с камерата
            _spriteBatch.Begin(
                transformMatrix: camera.Transform,
                samplerState: SamplerState.PointClamp);
            DrawMap();

            player.Draw(_spriteBatch);

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

            _spriteBatch.End();

            // UI — остава неподвижно на екрана
            _spriteBatch.Begin();

            if (isGameOver)
            {
                _spriteBatch.DrawString(
                    gamefont,
                    "Game Over!\nPress R to restart.",
                    new Vector2(20, 80),
                    Color.Red);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
        private void DrawMap()
        {
            worldMap.Draw(_spriteBatch);
        }
    }
    }
