using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _1_2D_Top_Down
{
    public sealed class PauseOverlay : IGameOverlay
    {
        private readonly Game1 game;

        public PauseOverlay(Game1 game) => this.game = game;

        public string Id => "pause";
        public bool BlocksInputBelow => true;
        public bool BlocksUpdateBelow => true;
        public void Enter() { }
        public void Exit() { }
        public void Update(GameTime gameTime) => game.UpdatePauseOverlay();
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch) => game.DrawPauseOverlay();
    }
}
