using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _1_2D_Top_Down
{
    public sealed class DialogueOverlay : IGameOverlay
    {
        private readonly Game1 game;

        public DialogueOverlay(Game1 game) => this.game = game;

        public string Id => "dialogue";
        public bool BlocksInputBelow => true;
        public bool BlocksUpdateBelow => true;
        public string Speaker { get; private set; } = string.Empty;
        public string Text { get; private set; } = string.Empty;

        public void Show(string speaker, string text)
        {
            Speaker = speaker;
            Text = text;
        }

        public void Enter() { }
        public void Exit() { }
        public void Update(GameTime gameTime) => game.UpdateDialogueOverlay();
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch) => game.DrawDialogueOverlay(Speaker, Text);
    }
}
