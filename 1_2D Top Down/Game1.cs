using Microsoft.VisualBasic.Devices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;


namespace _1_2D_Top_Down
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Player player;
        private Enemy enemy;



        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;

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
            player = new Player(playerTexture, new Vector2(400, 500));
            Texture2D enemyTexture = Content.Load<Texture2D>("enemies/poopy");
            enemy = new Enemy(enemyTexture, new Vector2(200, 200));
        }

        protected override void Update(GameTime gameTime)
        {
            player.Update(gameTime, GraphicsDevice.Viewport.Bounds);
            enemy.Update(gameTime, player.Position);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();
            player.Draw(_spriteBatch);
            enemy.Draw(_spriteBatch);
            
            _spriteBatch.End();
            
            base.Draw(gameTime);
        }
    }
}
