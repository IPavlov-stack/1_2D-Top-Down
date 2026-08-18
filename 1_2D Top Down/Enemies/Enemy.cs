using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace _1_2D_Top_Down
{
    public class Enemy
    {
        protected Texture2D texture;
        private readonly IEnemyBehavior behavior;

        // Размерът на цялата мрежа в sprite sheet
        protected int frameCount;
        protected int frameRows;
        protected float rotation;
        protected float frameDuration;
        protected float scale;

        protected int currentFrame;
        protected int animationRow;
        protected int framesInCurrentAnimation;
        protected float animationTimer;

        protected int FrameWidth => texture.Width / frameCount;
        protected int FrameHeight => texture.Height / frameRows;

        internal Vector2 SpriteCenter =>
            Position + new Vector2(
                FrameWidth * scale / 2f,
                FrameHeight * scale / 2f);

        public Vector2 Position;
        public EnemyDefinition Definition { get; }
        public Health Health { get; }
        public int ExperienceReward => Definition.ExperienceReward;

        public EnemyState CurrentState { get; private set; } = EnemyState.Idle;

        private const float KnockbackDeceleration = 9f;
        private readonly Knockback knockback = new Knockback(KnockbackDeceleration);

        public Rectangle Bounds
        {
            get
            {
                int spriteWidth = (int)(FrameWidth * scale);
                int spriteHeight = (int)(FrameHeight * scale);

                int hitboxWidth = (int)(spriteWidth * 0.7f);
                int hitboxHeight = (int)(spriteHeight * 0.7f);

                int offsetX = (spriteWidth - hitboxWidth) / 2;
                int offsetY = (spriteHeight - hitboxHeight) / 2;

                return new Rectangle(
                    (int)Position.X + offsetX,
                    (int)Position.Y + offsetY,
                    hitboxWidth,
                    hitboxHeight);
            }
        }

        public Enemy(
            Texture2D texture,
            Vector2 startPosition,
            int frameCount,
            int frameRows,
            float frameDuration,
            float scale,
            EnemyDefinition definition,
            IEnemyBehavior behavior)
        {
            this.texture = texture;
            Position = startPosition;
            Definition = definition;
            this.behavior = behavior;

            this.frameCount = frameCount;
            this.frameRows = frameRows;
            this.frameDuration = frameDuration;
            this.scale = scale;

            framesInCurrentAnimation = frameCount;
            Health = new Health(definition.MaxHealth);
        }
        public void Update(GameTime gameTime, EnemyUpdateContext context)
        {
            UpdateKnockback(gameTime);
            behavior.Update(this, gameTime, context);
        }

        internal void ChangeState(EnemyState newState)
        {
            if (CurrentState == newState)
                return;

            CurrentState = newState;
        }

        protected bool IsInState(EnemyState state)
        {
            return CurrentState == state;
        }

        internal void SetAnimation(int row, int animationFrameCount)
        {
            bool animationChanged =
                animationRow != row ||
                framesInCurrentAnimation != animationFrameCount;

            if (animationChanged)
            {
                animationRow = row;
                framesInCurrentAnimation = animationFrameCount;
                currentFrame = 0;
                animationTimer = 0f;
            }
        }

        // Връща true само ако non-looping анимацията е приключила.
        internal bool UpdateAnimation(GameTime gameTime, bool loop = true)
        {
            animationTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (animationTimer < frameDuration)
            {
                return false;
            }

            animationTimer -= frameDuration;
            currentFrame++;

            if (currentFrame < framesInCurrentAnimation)
            {
                return false;
            }

            if (loop)
            {
                currentFrame = 0;
                return false;
            }

            currentFrame = framesInCurrentAnimation - 1;
            return true;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            Rectangle sourceRectangle = new Rectangle(
                currentFrame * FrameWidth,
                animationRow * FrameHeight,
                FrameWidth,
                FrameHeight);

            spriteBatch.Draw(
                 texture,
                 SpriteCenter,
                 sourceRectangle,
                 Color.White,
                 rotation,
                 new Vector2(FrameWidth / 2f, FrameHeight / 2f),
                 scale,
                 SpriteEffects.None,
                 0f);
        }
        public void ApplyKnockback(
    Vector2 attackPosition,
    float force)
        {
            Vector2 direction =
                Bounds.Center.ToVector2() - attackPosition;

            knockback.Apply(direction, force);
        }

        internal void SetRotation(float value)
        {
            rotation = value;
        }

        private void UpdateKnockback(GameTime gameTime)
        {
            Position += knockback.Update(gameTime);
        }
    }
}
