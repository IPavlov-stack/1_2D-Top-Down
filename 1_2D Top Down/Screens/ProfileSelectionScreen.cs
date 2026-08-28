using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _1_2D_Top_Down
{
    public sealed class ProfileSelectionScreen : IGameScreen
    {
        private readonly Game1 game;

        public ProfileSelectionScreen(Game1 game) => this.game = game;

        public string Id => ScreenIds.ProfileSelection;
        public void Enter() => game.OnProfileSelectionScreenEntered();
        public void Exit() => game.OnProfileSelectionScreenExited();
        public void Update(GameTime gameTime) =>
            game.UpdateProfileSelectionScreen();
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch) =>
            game.DrawProfileSelectionScreen();
    }
}
