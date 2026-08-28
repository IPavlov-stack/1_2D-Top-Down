using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace _1_2D_Top_Down
{
    public partial class Game1
    {
        private const int MenuButtonWidth = 400;
        private const int MenuButtonHeight = 85;
        private const int MenuButtonSpacing = 40;

        // Options - Sound Effects slider
        private const int SoundEffectsSliderWidth = 460;
        private const int SoundEffectsSliderHeight = 14;
        private const int MusicSliderTop = 390;
        private const int SoundEffectsSliderTop = 480;
        private bool isMusicSliderDragging; 
        private const int SoundEffectsSliderThumbSize = 32;
        private bool isSoundEffectsSliderDragging;

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
                StartScreenTransition(GameFlowState.ProfileSelection);
            }
            else if (GetMenuButtonBounds(1).Contains(mousePosition))
            {
                StartScreenTransition(GameFlowState.Options);
            }
            else if (GetMenuButtonBounds(2).Contains(mousePosition))
            {
                Exit();
            }
        }

        private void HandleOptionsInput(MouseState mouse)
        {
            UpdateVolumeSliders(
                mouse,
                GetMusicSliderBounds(),
                GetSoundEffectsSliderBounds());
            bool clickedLeftButton =
                mouse.LeftButton == ButtonState.Pressed &&
                previousMouseState.LeftButton == ButtonState.Released;

            if (clickedLeftButton &&
                GetMenuButtonBounds(2).Contains(mouse.Position))
            {
                ReturnFromOptions();
            }
        }

        private void DrawMainMenu()
        {
            GraphicsDevice.Clear(new Color(25, 30, 40));

            _spriteBatch.Begin();

            DrawCenteredText("A guy called Pesho", 160, Color.Gold, 2.8f);

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
            DrawMusicSlider();
            DrawSoundEffectsSlider();
            DrawMenuButton(GetMenuButtonBounds(2), "Back");

            _spriteBatch.End();
        }
        private void ReturnFromOptions()
        {
            StartScreenBackTransition();
        }
        private Rectangle GetSoundEffectsSliderBounds()
        {
            return new Rectangle(
                GraphicsDevice.Viewport.Width / 2 -
                SoundEffectsSliderWidth / 2,

                SoundEffectsSliderTop,

                SoundEffectsSliderWidth,
                SoundEffectsSliderHeight);
        }

        private void SetSoundEffectsVolumeFromMouse(int mouseX, Rectangle sliderBounds)
        {
            float percent = (mouseX - sliderBounds.Left) /(float)sliderBounds.Width;

            SoundEffectsVolume = MathHelper.Clamp(percent, 0f, 1f);
        }
        private void DrawMusicSlider()
        {
            DrawMusicSlider(GetMusicSliderBounds());
        }
        private void DrawMusicSlider(Rectangle sliderBounds)
        {
            DrawVolumeSlider(
                sliderBounds,
                "MUSIC",
                MusicVolume,
                isMusicSliderDragging);
        }
        private void DrawSoundEffectsSlider()
        {
            DrawSoundEffectsSlider(GetSoundEffectsSliderBounds());
        }
        private void DrawSoundEffectsSlider(Rectangle sliderBounds)
        {
            DrawVolumeSlider(
                sliderBounds,
                "SOUND EFFECTS",
                SoundEffectsVolume,
                isSoundEffectsSliderDragging);
        }


        private void DrawVolumeSlider(
            Rectangle sliderBounds,
            string label,
            float volume,
            bool isDragging)
        {
            int volumePercent =
                (int)MathF.Round(volume * 100f);

            DrawCenteredText(
                $"{label}: {volumePercent}%",
                sliderBounds.Y - 58,
                Color.White,
                1f);

            Rectangle outerBounds = new Rectangle(
                sliderBounds.X - 2,
                sliderBounds.Y - 2,
                sliderBounds.Width + 4,
                sliderBounds.Height + 4);

            _spriteBatch.Draw(pixelTexture, outerBounds, Color.Black);
            _spriteBatch.Draw(pixelTexture, sliderBounds, Color.DarkSlateGray);

            int filledWidth =
                (int)(sliderBounds.Width * volume);

            Rectangle filledBounds = new Rectangle(
                sliderBounds.X,
                sliderBounds.Y,
                filledWidth,
                sliderBounds.Height);

            _spriteBatch.Draw(
                pixelTexture,
                filledBounds,
                Color.DodgerBlue);

            int thumbCenterX =
                sliderBounds.X + filledWidth;

            Rectangle thumbBounds = new Rectangle(
                thumbCenterX - SoundEffectsSliderThumbSize / 2,
                sliderBounds.Center.Y -
                    SoundEffectsSliderThumbSize / 2,
                SoundEffectsSliderThumbSize,
                SoundEffectsSliderThumbSize);

            _spriteBatch.Draw(pixelTexture, thumbBounds, Color.Black);

            Rectangle thumbInnerBounds = new Rectangle(
                thumbBounds.X + 3,
                thumbBounds.Y + 3,
                thumbBounds.Width - 6,
                thumbBounds.Height - 6);

            _spriteBatch.Draw(
                pixelTexture,
                thumbInnerBounds,
                isDragging ? Color.Gold : Color.White);
        }
        private Rectangle GetMusicSliderBounds()
        {
            return new Rectangle(
                GraphicsDevice.Viewport.Width / 2 -
                SoundEffectsSliderWidth / 2,

                MusicSliderTop,

                SoundEffectsSliderWidth,
                SoundEffectsSliderHeight);
        }

        private void SetMusicVolumeFromMouse(int mouseX, Rectangle sliderBounds)
        {
            float percent = (mouseX - sliderBounds.Left) / (float)sliderBounds.Width;

            MusicVolume = MathHelper.Clamp(percent, 0f, 1f);
        }
        private void DrawMenuButton(Rectangle bounds, string text)
        {
            bool isHovered = bounds.Contains(Mouse.GetState().Position);

            Color backgroundColor = isHovered ? Color.SlateGray : Color.DimGray;

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
        private void StartSceneTransition(GameFlowState scene)
        {
            if (transitionManager.IsActive)
            {
                return;
            }

            nextGameFlowState = scene;
            pendingScreenNavigation = ScreenNavigation.Direct;
            transitionManager.Start();
        }

        private void UpdateSceneTransition(GameTime gameTime)
        {
            if (transitionManager.Update(gameTime))
            {
                gameplaySession.ChangeFlowState(nextGameFlowState);
                ActivateScreenForFlowState(gameFlowState);
                UpdateMusicForgameFlowState();

                if (gameFlowState == GameFlowState.Playing ||
                    gameFlowState == GameFlowState.WaveIntermission)
                {
                    CenterCameraOnPlayer();
                }
            }
        }

        private void DrawSceneTransition()
        {
            if (!transitionManager.IsActive)
            {
                return;
            }

            _spriteBatch.Begin();

            _spriteBatch.Draw(
                pixelTexture,
                GraphicsDevice.Viewport.Bounds,
                Color.Black * transitionManager.Opacity);

            _spriteBatch.End();
        }
        private void CenterCameraOnPlayer()
        {
            cameraController.WorldBounds = gameMap.WorldBounds;

            cameraController.SnapToFollow(GraphicsDevice.Viewport.Bounds.Size);
        }
        private void UpdateVolumeSliders( MouseState mouse,Rectangle musicSliderBounds, Rectangle soundEffectsSliderBounds)
        {
            Rectangle musicInteractionBounds = musicSliderBounds;
            musicInteractionBounds.Inflate(
                SoundEffectsSliderThumbSize / 2,
                SoundEffectsSliderThumbSize / 2);

            Rectangle soundEffectsInteractionBounds = soundEffectsSliderBounds;
            soundEffectsInteractionBounds.Inflate(
                SoundEffectsSliderThumbSize / 2,
                SoundEffectsSliderThumbSize / 2);

            if (mouse.LeftButton == ButtonState.Released)
            {
                isMusicSliderDragging = false;
                isSoundEffectsSliderDragging = false;
            }
            else if (isMusicSliderDragging ||
                     musicInteractionBounds.Contains(mouse.Position))
            {
                isMusicSliderDragging = true;
                SetMusicVolumeFromMouse(mouse.X, musicSliderBounds);
            }
            else if (isSoundEffectsSliderDragging ||
                     soundEffectsInteractionBounds.Contains(mouse.Position))
            {
                isSoundEffectsSliderDragging = true;
                SetSoundEffectsVolumeFromMouse(
                    mouse.X,
                    soundEffectsSliderBounds);
            }
        }
    }

}
