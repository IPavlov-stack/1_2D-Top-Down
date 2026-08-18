using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace _1_2D_Top_Down
{
    public sealed class ScrollableList
    {
        private bool isDragging;
        public int FirstVisibleIndex { get; private set; }

        public int GetVisibleCount(Rectangle bounds, int rowHeight, int spacing) =>
            Math.Max(1, (bounds.Height + spacing) / (rowHeight + spacing));

        public int GetMaxFirstVisible(int itemCount, int visibleCount) =>
            Math.Max(0, itemCount - visibleCount);

        public void Clamp(int itemCount, int visibleCount) =>
            FirstVisibleIndex = Math.Clamp(FirstVisibleIndex, 0,
                GetMaxFirstVisible(itemCount, visibleCount));

        public bool HandleInput(MouseState mouse, MouseState previous, Rectangle listBounds,
            Rectangle trackBounds, Rectangle thumbBounds, int itemCount, int visibleCount)
        {
            int max = GetMaxFirstVisible(itemCount, visibleCount);
            if (max <= 0) { Reset(); return false; }
            int wheel = mouse.ScrollWheelValue - previous.ScrollWheelValue;
            if (wheel != 0 && listBounds.Contains(mouse.Position))
            {
                FirstVisibleIndex = Math.Clamp(FirstVisibleIndex - Math.Sign(wheel), 0, max);
                return true;
            }
            if (mouse.LeftButton == ButtonState.Released) { Reset(); return false; }
            if (previous.LeftButton == ButtonState.Released && trackBounds.Contains(mouse.Position))
                isDragging = true;
            if (!isDragging) return false;
            float travel = trackBounds.Height - thumbBounds.Height;
            float percent = travel <= 0 ? 0 :
                (mouse.Y - trackBounds.Top - thumbBounds.Height / 2f) / travel;
            FirstVisibleIndex = (int)MathF.Round(MathHelper.Clamp(percent, 0, 1) * max);
            return true;
        }

        public void Reset() => isDragging = false;
    }
}
