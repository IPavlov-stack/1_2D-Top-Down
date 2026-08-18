using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _1_2D_Top_Down
{
    public static class UiDrawing
    {
        public static void DrawNineSlicePanel(
            SpriteBatch spriteBatch,
            Texture2D texture,
            Rectangle destination,
            Color? tint = null)
        {
            const int sourceSliceSize = 74;
            const int sourceGap = 17;
            const int borderSize = sourceSliceSize;

            if (destination.Width < borderSize * 2 ||
                destination.Height < borderSize * 2)
            {
                return;
            }

            Color color = tint ?? Color.White;
            int[] sourcePositions =
            {
                0,
                sourceSliceSize + sourceGap,
                (sourceSliceSize + sourceGap) * 2
            };
            int[] destinationX =
            {
                destination.Left,
                destination.Left + borderSize,
                destination.Right - borderSize
            };
            int[] destinationY =
            {
                destination.Top,
                destination.Top + borderSize,
                destination.Bottom - borderSize
            };
            int[] destinationWidths =
            {
                borderSize,
                destination.Width - borderSize * 2,
                borderSize
            };
            int[] destinationHeights =
            {
                borderSize,
                destination.Height - borderSize * 2,
                borderSize
            };

            for (int row = 0; row < 3; row++)
            {
                for (int column = 0; column < 3; column++)
                {
                    spriteBatch.Draw(
                        texture,
                        new Rectangle(
                            destinationX[column],
                            destinationY[row],
                            destinationWidths[column],
                            destinationHeights[row]),
                        new Rectangle(
                            sourcePositions[column],
                            sourcePositions[row],
                            sourceSliceSize,
                            sourceSliceSize),
                        color);
                }
            }
        }
    }
}
