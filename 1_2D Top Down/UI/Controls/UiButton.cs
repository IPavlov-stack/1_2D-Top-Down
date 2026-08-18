using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace _1_2D_Top_Down
{
    public sealed class UiButton
    {
        private readonly Texture2D texture;
        private readonly Texture2D overlayTexture;
        private readonly Action clicked;

        public UiButton(Texture2D texture, Texture2D overlayTexture, Action clicked, bool isEnabled = true)
        {
            this.texture = texture;
            this.overlayTexture = overlayTexture;
            this.clicked = clicked;
            IsEnabled = isEnabled;
        }

        public bool IsEnabled { get; set; }

        public bool HandleInput(GameplayUiInput input, Rectangle bounds)
        {
            if (input.Mouse.LeftButton != Microsoft.Xna.Framework.Input.ButtonState.Pressed ||
                input.PreviousMouse.LeftButton != Microsoft.Xna.Framework.Input.ButtonState.Released ||
                !bounds.Contains(input.Mouse.Position))
                return false;

            if (IsEnabled)
                clicked();
            return true;
        }

        public void Draw(SpriteBatch batch, Rectangle bounds, Point mousePosition)
        {
            batch.Draw(texture, bounds, IsEnabled ? Color.White : Color.White * 0.5f);
            if (IsEnabled && bounds.Contains(mousePosition))
                batch.Draw(overlayTexture, bounds, Color.Gold * 0.18f);
        }
    }
}
