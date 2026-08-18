using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace _1_2D_Top_Down
{
    public sealed class UiSlider
    {
        private const int ThumbSize = 32;
        private readonly string label;
        private readonly Func<float> value;
        private readonly Action<float> setValue;
        private bool isDragging;

        public UiSlider(string label, Func<float> value, Action<float> setValue)
        {
            this.label = label;
            this.value = value;
            this.setValue = setValue;
        }

        public bool HandleInput(MouseState mouse, Rectangle bounds)
        {
            Rectangle interaction = bounds;
            interaction.Inflate(ThumbSize / 2, ThumbSize / 2);
            if (mouse.LeftButton == ButtonState.Released)
            {
                bool handled = isDragging;
                isDragging = false;
                return handled;
            }
            if (!isDragging && !interaction.Contains(mouse.Position))
                return false;
            isDragging = true;
            setValue(MathHelper.Clamp((mouse.X - bounds.Left) / (float)bounds.Width, 0f, 1f));
            return true;
        }

        public void Draw(SpriteBatch batch, SpriteFont font, Texture2D pixel,
            Rectangle bounds, Color fillColor)
        {
            float current = MathHelper.Clamp(value(), 0f, 1f);
            string text = $"{label}: {(int)MathF.Round(current * 100f)}%";
            Vector2 size = font.MeasureString(text);
            batch.DrawString(font, text,
                new Vector2(bounds.Center.X - size.X / 2f, bounds.Y - 58), Color.White);
            batch.Draw(pixel, new Rectangle(bounds.X - 2, bounds.Y - 2,
                bounds.Width + 4, bounds.Height + 4), Color.Black);
            batch.Draw(pixel, bounds, Color.DarkSlateGray);
            int filled = (int)(bounds.Width * current);
            batch.Draw(pixel, new Rectangle(bounds.X, bounds.Y, filled, bounds.Height), fillColor);
            Rectangle thumb = new(bounds.X + filled - ThumbSize / 2,
                bounds.Center.Y - ThumbSize / 2, ThumbSize, ThumbSize);
            batch.Draw(pixel, thumb, Color.Black);
            batch.Draw(pixel, new Rectangle(thumb.X + 3, thumb.Y + 3,
                thumb.Width - 6, thumb.Height - 6), isDragging ? Color.Gold : Color.White);
        }

        public void Reset() => isDragging = false;
    }
}
