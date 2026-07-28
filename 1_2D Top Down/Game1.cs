using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using System.Collections.Generic;


namespace _1_2D_Top_Down
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont gamefont;
        private Player player;
        private Enemy enemy;
        private bool isGameOver;
        private bool enemyIsAlive = true;

        private Vector2 playerStartPosition = new Vector2(400, 500);
        private Vector2 enemyStartPosition = new Vector2(200, 200);
        private const int WindowSizeX = 1280;
        private const int WindowSizeY = 720;

        private Texture2D projectileTexture;
        private List<Projectile> projectiles = new List<Projectile>();

        private MouseState previousMouseState;



        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = WindowSizeX;
            _graphics.PreferredBackBufferHeight = WindowSizeY;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Texture2D playerTexture = Content.Load<Texture2D>("player/noob");
            player = new Player(playerTexture, playerStartPosition);
            Texture2D enemyTexture = Content.Load<Texture2D>("enemies/poopy");
            enemy = new Enemy(enemyTexture, enemyStartPosition);
            projectileTexture = Content.Load<Texture2D>("projectiles/bullet");
            


            gamefont = Content.Load<SpriteFont>("Sprite fonts/GameFont");
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();

            bool clickedLeftButton =
                mouseState.LeftButton == ButtonState.Pressed &&
                previousMouseState.LeftButton == ButtonState.Released;

            if (!isGameOver)
            {
                player.Update(gameTime, GraphicsDevice.Viewport.Bounds);
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
                        continue;
                    }

                    if (enemyIsAlive && projectile.Bounds.Intersects(enemy.Bounds))
                    {
                        projectiles.RemoveAt(i);
                        enemyIsAlive = false;
                    }
                }
                previousMouseState = mouseState;

                //update enemy
                if (enemyIsAlive)
                {
                    enemy.Update(gameTime, player.Position);
                }
                if (player.Bounds.Intersects(enemy.Bounds))
                {
                    isGameOver = true;
                }
            }

            //restart game
            else if (keyboard.IsKeyDown(Keys.R))
            {
                player.Position = playerStartPosition;
                enemy.Position = enemyStartPosition;
                isGameOver = false;
                projectiles.Clear();
                enemyIsAlive = true;
            }

                base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(isGameOver ? Color.DarkRed : Color.DarkSlateGray); 
            
            _spriteBatch.Begin();

            player.Draw(_spriteBatch);
            foreach (Projectile projectile in projectiles)
            {
                projectile.Draw(_spriteBatch);
            }

            //draw enemy
            if (enemyIsAlive)
            {
                enemy.Draw(_spriteBatch);
            }
            if (isGameOver)
            {
                _spriteBatch.DrawString(
                    gamefont,
                    "Game Over!\nPress R to restart.",
                    new Vector2(500, 300),
                    Color.White
                    );
            }
            
            _spriteBatch.End();
            
            base.Draw(gameTime);
        }
    }
}
