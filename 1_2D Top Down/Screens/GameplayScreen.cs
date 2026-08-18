using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _1_2D_Top_Down
{
    public sealed class GameplayScreen : IGameScreen
    {
        private readonly Game1 game;

        public GameplayScreen(Game1 game) => this.game = game;

        public string Id => ScreenIds.Gameplay;
        public void Enter() => game.OnGameplayScreenEntered();
        public void Exit() => game.OnGameplayScreenExited();
        public void Update(GameTime gameTime) => game.UpdateGameplayScreen(gameTime);
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch) => game.DrawGameplayScreen();
    }
}
