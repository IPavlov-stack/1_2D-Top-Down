using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace _1_2D_Top_Down
{
    public partial class Game1
    {
        private bool isExitConfirmationOpen;
        private Rectangle GetPauseButtonBounds(int index)
        {
            const int buttonWidth = 280;
            const int buttonHeight = 52;
            const int spacing = 14;

            int totalHeight = buttonHeight * 5 + spacing * 4;
            int startY = GraphicsDevice.Viewport.Height / 2 - totalHeight / 2 + 45;

            return new Rectangle(
                GraphicsDevice.Viewport.Width / 2 - buttonWidth / 2,
                startY + index * (buttonHeight + spacing),
                buttonWidth,
                buttonHeight);
        }
        private void HandleExitConfirmationInput(
            KeyboardState keyboard,
            MouseState mouse)
        {
            bool pressedEscape =
                keyboard.IsKeyDown(Keys.Escape) &&
                previousKeyboard.IsKeyUp(Keys.Escape);

            if (pressedEscape)
            {
                isExitConfirmationOpen = false;
                return;
            }

            bool clickedLeftButton =
                mouse.LeftButton == ButtonState.Pressed &&
                previousMouseState.LeftButton == ButtonState.Released;

            if (!clickedLeftButton)
            {
                return;
            }

            if (GetPauseButtonBounds(0).Contains(mouse.Position))
            {
                // Main Menu
                isExitConfirmationOpen = false;
                StartSceneTransition(GameFlowState.MainMenu);
            }
            else if (GetPauseButtonBounds(1).Contains(mouse.Position))
            {
                // Restart
                RestartGame();
                isExitConfirmationOpen = false;
            }
            else if (GetPauseButtonBounds(2).Contains(mouse.Position))
            {
                // Exit
                Exit();
            }
            else if (GetPauseButtonBounds(3).Contains(mouse.Position))
            {
                // Cancel
                isExitConfirmationOpen = false;
            }
        }

        private void DrawExitConfirmation()
        {
            if (!isExitConfirmationOpen)
            {
                return;
            }

            _spriteBatch.Draw(
                pixelTexture,
                GraphicsDevice.Viewport.Bounds,
                Color.Black * 0.65f);

            Rectangle panelBounds = new Rectangle(
                GraphicsDevice.Viewport.Width / 2 - 280,
                GraphicsDevice.Viewport.Height / 2 - 260,
                560,
                520);
            _spriteBatch.Draw(
                pixelTexture,
                panelBounds,
                Color.Black);

            Rectangle innerPanelBounds = new Rectangle(
                panelBounds.X + 3,
                panelBounds.Y + 3,
                panelBounds.Width - 6,
                panelBounds.Height - 6);

            _spriteBatch.Draw(
                pixelTexture,
                innerPanelBounds,
                Color.DarkSlateGray);
            DrawCenteredText(
                "GAME PAUSED",
                panelBounds.Y + 80,
                Color.White,
                1.1f);

            DrawMenuButton(GetPauseButtonBounds(0), "Main Menu");
            DrawMenuButton(GetPauseButtonBounds(1), "Restart");
            DrawMenuButton(GetPauseButtonBounds(2), "Exit");
            DrawMenuButton(GetPauseButtonBounds(3), "Cancel");
        }
    }
}