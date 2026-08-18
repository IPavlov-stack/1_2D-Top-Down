using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _1_2D_Top_Down
{
    /// <summary>
    /// Displays non-blocking, screen-space runtime information for developers.
    /// </summary>
    public sealed class DeveloperHudOverlay : IGameOverlay
    {
        private readonly DeveloperModeController developerMode;
        private readonly GameplaySession session;
        private readonly Camera2D camera;
        private readonly CameraController cameraController;
        private readonly Texture2D pixelTexture;
        private readonly SpriteFont font;

        public DeveloperHudOverlay(
            DeveloperModeController developerMode,
            GameplaySession session,
            Camera2D camera,
            CameraController cameraController,
            Texture2D pixelTexture,
            SpriteFont font)
        {
            this.developerMode = developerMode;
            this.session = session;
            this.camera = camera;
            this.cameraController = cameraController;
            this.pixelTexture = pixelTexture;
            this.font = font;
        }

        public string Id => "developer-hud";
        public bool BlocksInputBelow => false;
        public bool BlocksUpdateBelow => false;

        public void Enter() { }
        public void Exit() { }
        public void Update(GameTime gameTime) { }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!developerMode.IsEnabled || !developerMode.View.ShowHud)
                return;

            string text =
                "DEVELOPER MODE\n" +
                "F3 Mode  F4 Collision  F5 Entities\n" +
                "F6 Cutscene  F7 Projectiles  F8 Spawns  F9 HUD\n" +
                $"State: {session.FlowState}\n" +
                $"Mission: {session.Mission.Definition.Name}\n" +
                $"Wave: {session.Mission.Waves.CurrentWave}/" +
                $"{session.Mission.Definition.Waves.Count}  " +
                $"Enemies: {session.World.Enemies.Enemies.Count}\n" +
                $"Camera: {cameraController.Mode}  Zoom: {camera.Zoom:0.00}\n" +
                $"Player: {(int)session.Player.Position.X}, " +
                $"{(int)session.Player.Position.Y}";

            const int margin = 16;
            const int padding = 12;
            const float scale = 0.65f;
            Vector2 textSize = font.MeasureString(text) * scale;
            Rectangle panel = new(
                margin,
                margin,
                (int)textSize.X + padding * 2,
                (int)textSize.Y + padding * 2);

            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            spriteBatch.Draw(pixelTexture, panel, Color.Black * 0.78f);
            spriteBatch.DrawString(
                font,
                text,
                new Vector2(panel.X + padding, panel.Y + padding),
                Color.White,
                0f,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                0f);
            spriteBatch.End();
        }
    }
}
