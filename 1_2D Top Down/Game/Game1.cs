using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;


namespace _1_2D_Top_Down
{
    public partial class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont gamefont;
        private Player player;
        private bool isGameOver;
        private Color BackgroundColor = new Color(121, 191, 86);

        private Vector2 playerStartPosition = new Vector2(400, 500);
        private const int WindowSizeX = 1920;
        private const int WindowSizeY = 1080;

        private Texture2D playerProjectileTexture;
        private List<PlayerProjectile> projectiles = new List<PlayerProjectile>();

        private Texture2D evilEyeProjectileTexture;

        private MouseState previousMouseState;
        private KeyboardState previousKeyboard;

        private Texture2D demonTexture;
        private List<Demon> demons = new List<Demon>();
        private Random random = new Random();
        private float spawnTimer;
        private const float SpawnInterval = 0.25f;

        private Texture2D demonDeathTexture;
        private List<DeathAnimation> demonDeathAnimations = new List<DeathAnimation>();
        
        private Texture2D evilEyeTexture;
        private List<Evil_Eye> evilEyes = new List<Evil_Eye>();

        private List<EnemyProjectile> enemyProjectiles = new List<EnemyProjectile>();

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
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Texture2D playerTexture = Content.Load<Texture2D>("player/Character");
            player = new Player(playerTexture, playerStartPosition);
            demonTexture = Content.Load<Texture2D>("enemies/Demon/FLYING");
            demonDeathTexture = Content.Load<Texture2D>("enemies/Demon/DEATH");
            playerProjectileTexture = Content.Load<Texture2D>("projectiles/magic_projectile");
            evilEyeProjectileTexture = Content.Load<Texture2D>("projectiles/evil eye/evilEye_projectile_magenta");
            gamefont = Content.Load<SpriteFont>("Sprite fonts/GameFont");
            evilEyeTexture = Content.Load<Texture2D>("enemies/Evil Eye/Evil Eye Sprite sheet");

            SpawnEnemy();
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();

            HandleExit(keyboard);

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
                                                            //Color.DarkSlateGray
            _spriteBatch.Begin();

            player.Draw(_spriteBatch);
            foreach (Projectile projectile in projectiles)
            {
                projectile.Draw(_spriteBatch);
            }

            //draw enemy
            foreach (Demon demon in demons)
            {
                demon.Draw(_spriteBatch);
            }
            foreach (DeathAnimation deathAnimation in demonDeathAnimations)
            {
                deathAnimation.Draw(_spriteBatch);
            }
            
            if (isGameOver)
            {
                _spriteBatch.DrawString(
                    gamefont,
                    "Game Over!\nPress R to restart.",
                    new Vector2(20, 80),
                    Color.Red
                    );
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
            
            base.Draw(gameTime);
        }
        
    }
}
