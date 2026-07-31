using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _1_2D_Top_Down
{
    /// <summary>
    /// A world-space animated coin dropped by a defeated enemy.
    /// </summary>
    public sealed class Coin
    {
        private const int FrameCount = 6;
        private const float FrameDuration = 0.10f;
        private const float Scale = 0.6f;

        private readonly Texture2D _texture;
        private readonly int _frameWidth;
        private int _currentFrame;
        private float _animationTimer;

        public Vector2 Position { get; }
        public Rectangle Bounds
        {
            get
            {
                int width = (int)(_frameWidth * Scale);
                int height = (int)(_texture.Height * Scale);
                const int inset = 4;

                return new Rectangle(
                    (int)Position.X + inset,
                    (int)Position.Y + inset,
                    width - inset * 2,
                    height - inset * 2);
            }
        }

        public Coin(Texture2D texture, Vector2 centerPosition)
        {
            _texture = texture;
            _frameWidth = texture.Width / FrameCount;

            Vector2 drawSize = new(_frameWidth * Scale, texture.Height * Scale);
            Position = centerPosition - drawSize / 2f;
        }

        public void Update(GameTime gameTime)
        {
            _animationTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_animationTimer < FrameDuration)
                return;

            _animationTimer -= FrameDuration;
            _currentFrame = (_currentFrame + 1) % FrameCount;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle source = new(
                _currentFrame * _frameWidth,
                0,
                _frameWidth,
                _texture.Height);

            spriteBatch.Draw(
                _texture,
                Position,
                source,
                Color.White,
                0f,
                Vector2.Zero,
                Scale,
                SpriteEffects.None,
                0f);
        }
    }
}