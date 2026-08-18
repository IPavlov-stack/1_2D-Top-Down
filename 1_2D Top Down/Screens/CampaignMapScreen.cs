using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _1_2D_Top_Down
{
    public sealed class CampaignMapScreen : IGameScreen
    {
        private readonly Game1 game;

        public CampaignMapScreen(Game1 game) => this.game = game;

        public string Id => ScreenIds.CampaignMap;
        public void Enter() => game.OnMenuScreenEntered();
        public void Exit() { }
        public void Update(GameTime gameTime) => game.UpdateCampaignMapScreen();
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch) => game.DrawCampaignMapScreen();
    }
}
