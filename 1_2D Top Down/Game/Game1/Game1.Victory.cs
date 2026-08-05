using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace _1_2D_Top_Down
{
    public partial class Game1
    {
        private Rectangle GetVictoryCampaignButtonBounds()
        {
            const int buttonWidth = 420;
            const int buttonHeight = 68;
            return new Rectangle(
                GraphicsDevice.Viewport.Width / 2 - buttonWidth / 2,
                GraphicsDevice.Viewport.Height / 2 + 65,
                buttonWidth,
                buttonHeight);
        }

        private void DrawVictoryScreen()
        {
            Rectangle screenBounds = GraphicsDevice.Viewport.Bounds;

            _spriteBatch.Draw(
                pixelTexture,
                screenBounds,
                Color.Black * 0.60f);

            const string title = "VICTORY!";
            const float titleScale = 3f;

            Vector2 titleSize =
                boldpixels.MeasureString(title) * titleScale;

            _spriteBatch.DrawString(
                boldpixels,
                title,
                new Vector2(
                    (screenBounds.Width - titleSize.X) / 2f,
                    screenBounds.Height / 2f - 95),
                Color.Gold,
                0f,
                Vector2.Zero,
                titleScale,
                SpriteEffects.None,
                0f);

            DrawMenuButton(
                GetVictoryCampaignButtonBounds(),
                "GO TO CAMPAIGN MAP");
        }

        private void HandleVictoryInput(MouseState mouse)
        {
            bool clickedCampaignButton =
                mouse.LeftButton == ButtonState.Pressed &&
                previousMouseState.LeftButton == ButtonState.Released &&
                GetVictoryCampaignButtonBounds().Contains(mouse.Position);

            if (clickedCampaignButton)
            {
                StartSceneTransition(GameFlowState.Campaign);
            }
        }
    }
}