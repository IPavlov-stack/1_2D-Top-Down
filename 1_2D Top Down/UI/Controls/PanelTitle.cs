using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _1_2D_Top_Down
{
    public static class PanelTitle
    {
        public static void Draw(SpriteBatch batch, SpriteFont font, string text,
            Rectangle bounds, int topOffset, Color color)
        {
            Vector2 size = font.MeasureString(text);
            batch.DrawString(font, text,
                new Vector2(bounds.Center.X - size.X / 2f, bounds.Top + topOffset), color);
        }
    }
}
