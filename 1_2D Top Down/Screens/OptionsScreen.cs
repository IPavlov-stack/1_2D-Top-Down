using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _1_2D_Top_Down
{
    public sealed class OptionsScreen : IGameScreen
    {
        private readonly Game1 game;

        public OptionsScreen(Game1 game) => this.game = game;

        public string Id => ScreenIds.Options;
        public void Enter() => game.OnMenuScreenEntered();
        public void Exit() => game.OnOptionsScreenExited();
        public void Update(GameTime gameTime) => game.UpdateOptionsScreen();
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch) => game.DrawOptionsScreen();
    }
}
