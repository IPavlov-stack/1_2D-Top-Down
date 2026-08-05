using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace _1_2D_Top_Down
{
    public partial class Game1
    {
        private Rectangle GetCampaignMapBounds()
        {
            float scale = MathF.Min(
                GraphicsDevice.Viewport.Width /
                    (float)campaignMapTexture.Width,

                GraphicsDevice.Viewport.Height /
                    (float)campaignMapTexture.Height);

            int width = (int)(campaignMapTexture.Width * scale);
            int height = (int)(campaignMapTexture.Height * scale);

            return new Rectangle(
                GraphicsDevice.Viewport.Width / 2 - width / 2,
                GraphicsDevice.Viewport.Height / 2 - height / 2,
                width,
                height);
        }

        private void DrawCampaign()
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin( samplerState: SamplerState.PointClamp);

            _spriteBatch.Draw(
                campaignMapTexture,
                GetCampaignMapBounds(),
                Color.White);
            Rectangle mission1Bounds = GetMission1NodeBounds();

            bool isHovered =
                mission1Bounds.Contains(Mouse.GetState().Position);

            Color nodeColor = isHovered
                ? Color.Gold
                : new Color(210, 170, 85);

            _spriteBatch.Draw(
                missionNodeTexture,
                mission1Bounds,
                nodeColor);
            const string missionNumber = "1";
            Vector2 numberSize = boldpixels.MeasureString(missionNumber);

            _spriteBatch.DrawString(
                boldpixels,
                missionNumber,
                new Vector2(
                    mission1Bounds.Center.X - numberSize.X / 2f,
                    mission1Bounds.Center.Y - numberSize.Y / 2f),
                Color.Black);

            DrawMenuButton( GetCampaignBackButtonBounds(),"BACK");

            _spriteBatch.End();
        }

        private void HandleCampaignInput( KeyboardState keyboard, MouseState mouse)
        {
            bool pressedEscape = keyboard.IsKeyDown(Keys.Escape) && previousKeyboard.IsKeyUp(Keys.Escape);

            if (pressedEscape)
            {
                StartSceneTransition(GameFlowState.MainMenu);
            }
            bool clickedBackButton = mouse.LeftButton == ButtonState.Pressed &&
                                    previousMouseState.LeftButton == ButtonState.Released &&
                                    GetCampaignBackButtonBounds().Contains(mouse.Position);

            bool clickedMission1 = mouse.LeftButton == ButtonState.Pressed &&
                                    previousMouseState.LeftButton == ButtonState.Released &&
                                    GetMission1NodeBounds().Contains(mouse.Position);

            if (clickedMission1)
            {
                StartSceneTransition(GameFlowState.WaveIntermission);
            }
            if (clickedBackButton)
            {
                StartSceneTransition(GameFlowState.MainMenu);
            }
        }
        private Rectangle GetCampaignBackButtonBounds()
        {
            const int buttonWidth = 220;
            const int buttonHeight = 58;
            const int margin = 15;

            return new Rectangle(
                margin,
                GraphicsDevice.Viewport.Height - buttonHeight - margin,
                buttonWidth,
                buttonHeight);
        }
        private Rectangle GetMission1NodeBounds()
        {
            const int nodeSize = 72;

            Rectangle mapBounds = GetCampaignMapBounds();

            // Position of the node 0.135; 0.515
            int centerX = mapBounds.X + (int)(mapBounds.Width * 0.135f);
            int centerY = mapBounds.Y + (int)(mapBounds.Height * 0.546f);

            return new Rectangle(
                centerX - nodeSize / 2,
                centerY - nodeSize / 2,
                nodeSize,
                nodeSize);
        }
        private Texture2D CreateCircleTexture(int size)
        {
            Texture2D texture = new Texture2D(
                GraphicsDevice,
                size,
                size);

            Color[] pixels = new Color[size * size];

            float radius = size / 2f;
            Vector2 center = new Vector2(radius, radius);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float distance = Vector2.Distance(
                        new Vector2(x, y),
                        center);

                    if (distance <= radius)
                    {
                        pixels[y * size + x] = Color.White;
                    }
                }
            }

            texture.SetData(pixels);

            return texture;
        }
    }
}