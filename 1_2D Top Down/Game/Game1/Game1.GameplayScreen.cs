using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace _1_2D_Top_Down
{
    public partial class Game1
    {
        internal void OnGameplayScreenEntered()
        {
            PlayMusic(backgroundMusic);
            CenterCameraOnPlayer();
            overlayManager.Push(developerHudOverlay);
        }

        internal void OnGameplayScreenExited()
        {
            cutsceneDirector.Stop();
            overlayManager.Clear();
            CloseOpenGameplayPanels();
        }

        internal void UpdateGameplayScreen(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();

            developerMode.UpdateToggle(keyboard, previousKeyboard);

            if (overlayManager.HasOverlays)
                overlayManager.Update(gameTime);

            cutsceneDirector.Update(gameTime);

            if (cutsceneDirector.IsActive)
            {
                UpdateCamera(gameTime);
                return;
            }

            if (overlayManager.BlocksUpdateBelow)
            {
                return;
            }

            bool allowInput = !overlayManager.BlocksInputBelow;

            if (gameFlowState == GameFlowState.MissionComplete)
            {
                HandleVictoryInput(mouse);
                return;
            }

            if (gameFlowState == GameFlowState.GameOver)
            {
                bool pressedRestart = keyboard.IsKeyDown(Keys.R) && previousKeyboard.IsKeyUp(Keys.R);
                if (allowInput && pressedRestart)
                    RestartGame();

                return;
            }

            bool pressedEscape = keyboard.IsKeyDown(Keys.Escape) && previousKeyboard.IsKeyUp(Keys.Escape);
            if (allowInput && pressedEscape)
            {
                if (!CloseOpenGameplayPanels())
                    isExitConfirmationOpen = true;

                return;
            }

            if (allowInput)
            {
                developerMode.UpdateCommands(
                    keyboard,
                    previousKeyboard,
                    PlayCameraTestCutscene);
            }

            if (gameFlowState == GameFlowState.WaveIntermission)
            {
                if (allowInput)
                    HandleGameplayUIInput(keyboard, mouse);

                UpdatePlayerMovement(gameTime, allowInput);
                UpdateCamera(gameTime);
                UpdateDeathAnimations(gameTime);
                UpdateCollectibles(gameTime);
                enemyManager.RebuildSpatialGrid();
                UpdateEnemyProjectiles(gameTime);
                if (allowInput)
                {
                    UpdateWaveIntermissionInput(mouse);
                }
                return;
            }

            if (allowInput)
            {
                bool gameplayUiClickHandled = HandleGameplayUIInput(keyboard, mouse);
                if (!gameplayUiClickHandled)
                    HandlePlayerMeleeAttack(mouse, keyboard);
            }

            UpdateGameObjects(gameTime, allowInput);
            UpdateCamera(gameTime);
        }

        private void UpdateCamera(GameTime gameTime)
        {
            cameraController.Update(gameTime, GraphicsDevice.Viewport.Bounds.Size);
        }

        internal void DrawGameplayScreen()
        {
            GraphicsDevice.Clear(
                gameFlowState == GameFlowState.GameOver
                    ? Color.Black
                    : BackgroundColor);

            _spriteBatch.Begin(
                transformMatrix: camera.Transform,
                samplerState: SamplerState.PointClamp);

            DrawNormalWorld();

            if (developerMode.IsEnabled)
            {
                worldDebugRenderer.Draw(
                    _spriteBatch,
                    gameplaySession,
                    developerMode.View);
            }

            _spriteBatch.End();
            DrawUi();
        }
    }
}
