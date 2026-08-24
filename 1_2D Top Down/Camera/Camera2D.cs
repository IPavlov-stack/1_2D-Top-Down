using Microsoft.Xna.Framework;
using System;

namespace _1_2D_Top_Down
{
    public sealed class Camera2D
    {
        public Vector2 Position { get; private set; }
        public float Zoom { get; private set; } = 1f;

        public Matrix Transform =>
            Matrix.CreateTranslation(-Position.X, -Position.Y, 0f) *
            Matrix.CreateScale(Zoom);

        public void SetPosition(Vector2 position)
        {
            // Keep the world-to-screen translation on whole pixels. This
            // prevents seams between adjacent pixel-art tiles while moving.
            Position = new Vector2(
                MathF.Round(position.X * Zoom) / Zoom,
                MathF.Round(position.Y * Zoom) / Zoom);
        }

        public void SetZoom(float zoom)
        {
            Zoom = MathHelper.Clamp(zoom, 0.25f, 4f);
        }

        public Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            return screenPosition / Zoom + Position;
        }
    }
}

namespace _2D_Top_Down
{
    // Kept temporarily so older files with this using directive keep compiling.
}
