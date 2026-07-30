using Microsoft.Xna.Framework;

namespace _2D_Top_Down
{
    public class Camera
    {
        public Vector2 Position { get; private set; }

        public Matrix Transform =>
            Matrix.CreateTranslation(-Position.X, -Position.Y, 0);

        public void Follow(Vector2 targetPosition)
        {
            Position = targetPosition;
        }
    }
}