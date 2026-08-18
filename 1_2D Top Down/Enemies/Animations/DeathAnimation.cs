using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _1_2D_Top_Down
{
    public class DeathAnimation
    {
        private readonly Texture2D texture;
        private readonly int frameCount;
        private readonly int sheetColumnCount;
        private readonly int sheetRowCount;
        private readonly int animationRow;
        private readonly float frameDuration;
        private readonly float scale;

        private int currentFrame;
        private float animationTimer;

        public Vector2 Position { get; }
        public bool IsFinished { get; private set; }

        private int FrameWidth => texture.Width / sheetColumnCount;
        private int FrameHeight => texture.Height / sheetRowCount;

        public DeathAnimation(Texture2D texture, Vector2 position)
            : this(
                texture,
                position,
                frameCount: 7,
                sheetColumnCount: 7,
                sheetRowCount: 1,
                animationRow: 0,
                frameDuration: 0.1f,
                scale: 1f)
        {
        }

        public DeathAnimation(
            Texture2D texture,
            Vector2 position,
            int frameCount,
            int sheetColumnCount,
            int sheetRowCount,
            int animationRow,
            float frameDuration,
            float scale)
        {
            this.texture = texture;
            Position = position;
            this.frameCount = frameCount;
            this.sheetColumnCount = sheetColumnCount;
            this.sheetRowCount = sheetRowCount;
            this.animationRow = animationRow;
            this.frameDuration = frameDuration;
            this.scale = scale;
        }

        public void Update(GameTime gameTime)
        {
            animationTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (animationTimer < frameDuration)
                return;

            animationTimer -= frameDuration;
            currentFrame++;

            if (currentFrame >= frameCount)
                IsFinished = true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle sourceRectangle = new Rectangle(
                currentFrame * FrameWidth,
                animationRow * FrameHeight,
                FrameWidth,
                FrameHeight);

            Vector2 origin = new Vector2(
                FrameWidth / 2f,
                FrameHeight / 2f);

            spriteBatch.Draw(
                texture,
                Position,
                sourceRectangle,
                Color.White,
                0f,
                origin,
                scale,
                SpriteEffects.None,
                0f);
        }
    }
}
