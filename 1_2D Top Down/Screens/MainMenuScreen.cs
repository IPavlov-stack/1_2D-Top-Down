using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _1_2D_Top_Down
{
    public sealed class MainMenuScreen : IGameScreen
    {
        private readonly Game1 game;

        public MainMenuScreen(Game1 game) => this.game = game;

        public string Id => ScreenIds.MainMenu;
        public void Enter() => game.OnMenuScreenEntered();
        public void Exit() { }
        public void Update(GameTime gameTime) => game.UpdateMainMenuScreen();
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch) => game.DrawMainMenuScreen();
    }
}
