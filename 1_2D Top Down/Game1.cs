using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;


namespace _1_2D_Top_Down
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont gamefont;
        private Player player;
        private bool isGameOver;

        private Vector2 playerStartPosition = new Vector2(400, 500);
        private const int WindowSizeX = 1920;
        private const int WindowSizeY = 1080;

        private Texture2D projectileTexture;
        private List<Projectile> projectiles = new List<Projectile>();

        private MouseState previousMouseState;

        private Texture2D enemyTexture;
        private List<Enemy> enemies = new List<Enemy>();
        private Random random = new Random();
        private float spawnTimer;
        private const float SpawnInterval = 0.3f;

        private int missedSpells;
        private const int MaxMissedSpells = 10;

        private Texture2D deathTexture;
        private List<DeathAnimation> deathAnimations = new List<DeathAnimation>();

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
            enemyTexture = Content.Load<Texture2D>("enemies/FLYING");
            deathTexture = Content.Load<Texture2D>("enemies/DEATH");
            projectileTexture = Content.Load<Texture2D>("projectiles/magic_projectile");
            gamefont = Content.Load<SpriteFont>("Sprite fonts/GameFont");
            SpawnEnemy();
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();

            bool clickedLeftButton =
                mouseState.LeftButton == ButtonState.Pressed &&
                previousMouseState.LeftButton == ButtonState.Released;
            if (keyboard.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            if (!isGameOver)
            {
                player.Update(gameTime, GraphicsDevice.Viewport.Bounds);
                spawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (spawnTimer >= SpawnInterval)
                {
                    SpawnEnemy();
                    spawnTimer = 0f;
                }

                for (int i = enemies.Count - 1; i >= 0; i--)
                {
                    Enemy enemy = enemies[i];

                    enemy.Update(gameTime, player.Position);

                    if (player.Bounds.Intersects(enemy.Bounds))
                    {
                        isGameOver = true;
                    }
                }
                if (clickedLeftButton)
                {
                    Vector2 startPosition = player.Bounds.Center.ToVector2();
                    Vector2 mousePosition = mouseState.Position.ToVector2();

                    Vector2 direction = mousePosition - startPosition;

                    if (direction != Vector2.Zero)
                    {
                        direction.Normalize();

                        Projectile projectile = new Projectile(
                            projectileTexture,
                            startPosition,
                            direction);

                        projectiles.Add(projectile);
                    }
                }

                for (int i = projectiles.Count - 1; i >= 0; i--)
                {
                    Projectile projectile = projectiles[i];

                    projectile.Update(gameTime);

                    bool isOutsideScreen =
                        !GraphicsDevice.Viewport.Bounds.Intersects(projectile.Bounds);

                    if (isOutsideScreen)
                    {
                        projectiles.RemoveAt(i);

                        missedSpells++;

                        if (missedSpells >= MaxMissedSpells)
                        {
                            isGameOver = true;
                        }

                        continue;
                    }
                    bool projectileHitEnemy = false;

                    for (int j = enemies.Count - 1; j >= 0; j--)
                    {
                        if (projectile.Bounds.Intersects(enemies[j].Bounds))
                        {
                            Vector2 deathPosition = enemies[j].Bounds.Center.ToVector2();

                            deathAnimations.Add(
                                new DeathAnimation(deathTexture, deathPosition));

                            enemies.RemoveAt(j);

                            projectileHitEnemy = true;
                            break;
                        }
                    }

                    if (projectileHitEnemy)
                    {
                        projectiles.RemoveAt(i);
                    }

                }
                for (int i = deathAnimations.Count - 1; i >= 0; i--)
                {
                    deathAnimations[i].Update(gameTime);

                    if (deathAnimations[i].IsFinished)
                    {
                        deathAnimations.RemoveAt(i);
                    }
                }
                previousMouseState = mouseState;

            }

            //restart game
            else if (keyboard.IsKeyDown(Keys.R))
            {
                isGameOver = false;
                projectiles.Clear();
                enemies.Clear();
                deathAnimations.Clear();

                player.Position = playerStartPosition;
                spawnTimer = 0f;
                missedSpells = 0;
                SpawnEnemy();
            }

                base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(isGameOver ? Color.Black : new Color(121, 191, 86));
                                                            //Color.DarkSlateGray
            _spriteBatch.Begin();

            player.Draw(_spriteBatch);
            foreach (Projectile projectile in projectiles)
            {
                projectile.Draw(_spriteBatch);
            }

            //draw enemy
            foreach (Enemy enemy in enemies)
            {
                enemy.Draw(_spriteBatch);
            }
            foreach (DeathAnimation deathAnimation in deathAnimations)
            {
                deathAnimation.Draw(_spriteBatch);
            }
            _spriteBatch.DrawString(
            gamefont,
            $"Missed spells: {missedSpells}/{MaxMissedSpells}",
            new Vector2(20, 20),
            Color.White);
            if (isGameOver)
            {
                _spriteBatch.DrawString(
                    gamefont,
                    "Game Over!\nPress R to restart.",
                    new Vector2(20, 80),
                    Color.Red
                    );
            }
            
            _spriteBatch.End();
            
            base.Draw(gameTime);
        }
        private void SpawnEnemy()
        {
            int screenWidth = GraphicsDevice.Viewport.Width;
            int screenHeight = GraphicsDevice.Viewport.Height;

            Vector2 spawnPosition;

            int side = random.Next(4);

            switch (side)
            {
                case 0: // отгоре
                    spawnPosition = new Vector2(random.Next(screenWidth), -50);
                    break;

                case 1: // отдолу
                    spawnPosition = new Vector2(random.Next(screenWidth), screenHeight + 50);
                    break;

                case 2: // отляво
                    spawnPosition = new Vector2(-50, random.Next(screenHeight));
                    break;

                default: // отдясно
                    spawnPosition = new Vector2(screenWidth + 50, random.Next(screenHeight));
                    break;
            }

            enemies.Add(new Enemy(enemyTexture, spawnPosition));
        }
    }
}
