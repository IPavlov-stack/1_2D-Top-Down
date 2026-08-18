using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace _1_2D_Top_Down
{
    public partial class Game1
    {
        private void PlayCameraTestCutscene()
        {
            if (enemies.Count == 0 || cutsceneDirector.IsActive)
                return;

            Enemy focusEnemy = enemies[0];
            cutsceneDirector.Play(new ICutsceneStep[]
            {
                new FocusDialogueCutsceneStep(
                    () => focusEnemy.Bounds.Center.ToVector2(),
                    () => GraphicsDevice.Viewport.Bounds.Size,
                    1.55f,
                    "PESHO",
                    "This enemy is blocking the way. Be careful!")
            });
        }

        internal void UpdateDialogueOverlay()
        {
            KeyboardState keyboard = Keyboard.GetState();
            bool dismiss =
                (keyboard.IsKeyDown(Keys.Enter) && previousKeyboard.IsKeyUp(Keys.Enter)) ||
                (keyboard.IsKeyDown(Keys.Space) && previousKeyboard.IsKeyUp(Keys.Space)) ||
                (keyboard.IsKeyDown(Keys.Escape) && previousKeyboard.IsKeyUp(Keys.Escape));

            if (dismiss)
                overlayManager.Remove(dialogueOverlay.Id);
        }

        internal void DrawDialogueOverlay(string speaker, string text)
        {
            const int horizontalMargin = 180;
            const int bottomMargin = 90;
            const int panelHeight = 210;
            Rectangle panel = new Rectangle(
                horizontalMargin,
                GraphicsDevice.Viewport.Height - panelHeight - bottomMargin,
                GraphicsDevice.Viewport.Width - horizontalMargin * 2,
                panelHeight);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.Draw(pixelTexture, panel, Color.Black * 0.92f);

            Rectangle innerPanel = new Rectangle(
                panel.X + 4,
                panel.Y + 4,
                panel.Width - 8,
                panel.Height - 8);

            _spriteBatch.Draw(pixelTexture, innerPanel, new Color(42, 48, 60));
            _spriteBatch.DrawString(
                boldpixels,
                speaker,
                new Vector2(panel.X + 32, panel.Y + 26),
                Color.Gold);
            _spriteBatch.DrawString(
                boldpixels,
                text,
                new Vector2(panel.X + 32, panel.Y + 82),
                Color.White);

            const string continueText = "ENTER / SPACE";
            Vector2 continueSize = boldpixels.MeasureString(continueText);
            _spriteBatch.DrawString(
                boldpixels,
                continueText,
                new Vector2(panel.Right - continueSize.X - 28, panel.Bottom - continueSize.Y - 22),
                Color.LightGray);
            _spriteBatch.End();
        }
    }
}
