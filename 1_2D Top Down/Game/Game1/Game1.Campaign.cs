using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace _1_2D_Top_Down
{
    public partial class Game1
    {

        private Vector2? adventureExitPosition;
        private Rectangle GetCampaignMapBounds()
        {
            float scale = MathF.Min(
                GraphicsDevice.Viewport.Width / (float)campaignMapTexture.Width,

                GraphicsDevice.Viewport.Height / (float)campaignMapTexture.Height);

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

            Rectangle mission2Bounds = GetMission2NodeBounds();

            bool isMission2Hovered =
                mission2Bounds.Contains(Mouse.GetState().Position);

            Color mission2Color = isMission2Hovered
                ? Color.Gold
                : new Color(210, 170, 85);

            _spriteBatch.Draw(
                missionNodeTexture,
                mission2Bounds,
                mission2Color);

            const string mission2Number = "2";
            Vector2 mission2NumberSize =
                boldpixels.MeasureString(mission2Number);

            _spriteBatch.DrawString(
                boldpixels,
                mission2Number,
                new Vector2(
                    mission2Bounds.Center.X - mission2NumberSize.X / 2f,
                    mission2Bounds.Center.Y - mission2NumberSize.Y / 2f),
                Color.Black);

            DrawMenuButton( GetCampaignBackButtonBounds(),"BACK");

            _spriteBatch.End();
        }
        private void StartMission(MissionDefinition mission)
        {
            currentMission = mission;
            adventureExitPosition = null;
            projectiles.Clear();
            demons.Clear();
            evilEyes.Clear();
            enemyProjectiles.Clear();
            demonDeathAnimations.Clear();
            coins.Clear();
            manaCrystals.Clear();

            player.Health.Reset();
            player.ResetDamageEffects();
            waveManager.Reset();

            string mapFileName = mission.MapFileName ?? DefaultMapFileName;

            LoadMissionMap(mapFileName, loadPortals: mission.Type == MissionType.Survival);
            UpdateAdventureExit();
            if (mission.Type == MissionType.Adventure)
            {
                if (string.IsNullOrWhiteSpace(mission.MapFileName))
                {
                    throw new InvalidOperationException(
                        $"Adventure mission '{mission.Name}' has no map file.");
                }

                LoadPreplacedMissionEnemies(mission.MapFileName);
                StartSceneTransition(GameFlowState.Playing);
                return;
            }

            player.Position = playerStartPosition;
            StartSceneTransition(GameFlowState.WaveIntermission);
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

            bool clickedMission2 = mouse.LeftButton == ButtonState.Pressed &&
                                   previousMouseState.LeftButton == ButtonState.Released &&
                                   GetMission2NodeBounds().Contains(mouse.Position);

            if (clickedMission1)
            {
                StartMission(CampaignMissions.ForestOutskirts);
            }
            if (clickedMission2)
            {
                StartMission(CampaignMissions.ForestPath);
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
        private Rectangle GetMission2NodeBounds()
        {
            const int nodeSize = 72;

            Rectangle mapBounds = GetCampaignMapBounds();

            int centerX = mapBounds.X + (int)(mapBounds.Width * 0.285f);
            int centerY = mapBounds.Y + (int)(mapBounds.Height * 0.475f);

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

        private void UpdateAdventureExit()
        {
            if (gameFlowState != GameFlowState.Playing ||
                currentMission.Type != MissionType.Adventure ||
                adventureExitPosition is null)
            {
                return;
            }

            const float exitReachDistance = 48f;

            if (Vector2.DistanceSquared(
                    player.Position,
                    adventureExitPosition.Value) <=
                exitReachDistance * exitReachDistance)
            {
                StartSceneTransition(GameFlowState.MissionComplete);
            }
        }
    }
}