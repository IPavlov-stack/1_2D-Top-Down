using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace _1_2D_Top_Down
{
    public partial class Game1
    {
        private const int MenuButtonWidth = 400;
        private const int MenuButtonHeight = 85;
        private const int MenuButtonSpacing = 40;

        private Rectangle GetMenuButtonBounds(int index)
        {
            int screenWidth = GraphicsDevice.Viewport.Width;
            int screenHeight = GraphicsDevice.Viewport.Height;

            int totalHeight =
                MenuButtonHeight * 3 + MenuButtonSpacing * 2;

            int startY = (screenHeight - totalHeight) / 2;

            return new Rectangle(
                screenWidth / 2 - MenuButtonWidth / 2,
                startY + index * (MenuButtonHeight + MenuButtonSpacing),
                MenuButtonWidth,
                MenuButtonHeight);
        }

        private void HandleMainMenuInput(MouseState mouse)
        {
            bool clickedLeftButton =
                mouse.LeftButton == ButtonState.Pressed &&
                previousMouseState.LeftButton == ButtonState.Released;

            if (!clickedLeftButton)
            {
                return;
            }

            Point mousePosition = mouse.Position;

            if (GetMenuButtonBounds(0).Contains(mousePosition))
            {
                StartSceneTransition(GameScene.Playing);
            }
            else if (GetMenuButtonBounds(1).Contains(mousePosition))
            {
                StartSceneTransition(GameScene.Options);
            }
            else if (GetMenuButtonBounds(2).Contains(mousePosition))
            {
                Exit();
            }
        }

        private void HandleOptionsInput(MouseState mouse)
        {
            bool clickedLeftButton =
                mouse.LeftButton == ButtonState.Pressed &&
                previousMouseState.LeftButton == ButtonState.Released;

            if (clickedLeftButton &&
                GetMenuButtonBounds(2).Contains(mouse.Position))
            {
                StartSceneTransition(GameScene.MainMenu);
            }
        }

        private void DrawMainMenu()
        {
            GraphicsDevice.Clear(new Color(25, 30, 40));

            _spriteBatch.Begin();

            DrawCenteredText("2D TOP DOWN", 160, Color.Gold, 2.8f);

            DrawMenuButton(GetMenuButtonBounds(0), "Start");
            DrawMenuButton(GetMenuButtonBounds(1), "Options");
            DrawMenuButton(GetMenuButtonBounds(2), "Exit");

            _spriteBatch.End();
        }

        private void DrawOptions()
        {
            GraphicsDevice.Clear(new Color(25, 30, 40));

            _spriteBatch.Begin();

            DrawCenteredText("OPTIONS", 180, Color.Gold, 2.2f);
            DrawCenteredText(
                "Sound settings will be added here.",
                300,
                Color.White,
                1f);

            DrawMenuButton(GetMenuButtonBounds(2), "Back");

            _spriteBatch.End();
        }

        private void DrawMenuButton(Rectangle bounds, string text)
        {
            bool isHovered = bounds.Contains(Mouse.GetState().Position);

            Color backgroundColor =
                isHovered ? Color.SlateGray : Color.DimGray;

            _spriteBatch.Draw(pixelTexture, bounds, Color.Black);

            Rectangle innerBounds = new Rectangle(
                bounds.X + 2,
                bounds.Y + 2,
                bounds.Width - 4,
                bounds.Height - 4);

            _spriteBatch.Draw(pixelTexture, innerBounds, backgroundColor);

            Vector2 textSize = boldpixels.MeasureString(text);
            Vector2 textPosition = new Vector2(
                bounds.Center.X - textSize.X / 2f,
                bounds.Center.Y - textSize.Y / 2f);

            _spriteBatch.DrawString(
                boldpixels,
                text,
                textPosition,
                Color.White);
        }

        private void DrawCenteredText(
            string text,
            float y,
            Color color,
            float scale)
        {
            Vector2 textSize = boldpixels.MeasureString(text) * scale;

            Vector2 position = new Vector2(
                (GraphicsDevice.Viewport.Width - textSize.X) / 2f,
                y);

            _spriteBatch.DrawString(
                boldpixels,
                text,
                position,
                color,
                0f,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                0f);
        }
        private void StartSceneTransition(GameScene scene)
        {
            if (isSceneTransitioning)
            {
                return;
            }

            nextScene = scene;
            sceneTransitionTimer = 0f;
            sceneChangedDuringTransition = false;
            isSceneTransitioning = true;
        }

        private void UpdateSceneTransition(GameTime gameTime)
        {
            if (!isSceneTransitioning)
            {
                return;
            }

            sceneTransitionTimer +=
                (float)gameTime.ElapsedGameTime.TotalSeconds;

            float halfDuration = SceneTransitionDuration / 2f;

            // В момента, в който екранът е изцяло черен.
            if (!sceneChangedDuringTransition &&
                sceneTransitionTimer >= halfDuration)
            {
                currentScene = nextScene;

                if (currentScene == GameScene.Playing)
                {
                    CenterCameraOnPlayer();
                }

                sceneChangedDuringTransition = true;
            }

            if (sceneTransitionTimer >= SceneTransitionDuration)
            {
                isSceneTransitioning = false;
            }
        }

        private void DrawSceneTransition()
        {
            if (!isSceneTransitioning)
            {
                return;
            }

            float halfDuration = SceneTransitionDuration / 2f;
            float opacity;

            if (sceneTransitionTimer <= halfDuration)
            {
                // Потъмняване.
                opacity = sceneTransitionTimer / halfDuration;
            }
            else
            {
                // Появяване на новата сцена.
                opacity = 1f -
                    (sceneTransitionTimer - halfDuration) / halfDuration;
            }

            _spriteBatch.Begin();

            _spriteBatch.Draw(
                pixelTexture,
                GraphicsDevice.Viewport.Bounds,
                Color.Black * MathHelper.Clamp(opacity, 0f, 1f));

            _spriteBatch.End();
        }
        private void CenterCameraOnPlayer()
        {
            Vector2 playerCenter = player.Position +
                new Vector2(
                    player.texture.Width / 2f,
                    player.texture.Height / 2f);

            Vector2 screenCenter = new Vector2(
                GraphicsDevice.Viewport.Width / 2f,
                GraphicsDevice.Viewport.Height / 2f);

            camera.Follow(playerCenter - screenCenter);
        }
    }

}