using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace _1_2D_Top_Down
{
    public class Enemy : IYSortedWorldDrawable
    {
        protected Texture2D texture;
        private readonly IEnemyBehavior behavior;
        private Func<string, Texture2D> textureResolver;
        private EnemyAnimationDefinition currentAnimation;
        private Vector2 facingDirection = Vector2.UnitY;
        private float hurtStateTimer;

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
        public EnemyMotor Motor { get; } = new();
        public int ExperienceReward => Definition.ExperienceReward;

        public EnemyState CurrentState { get; private set; } = EnemyState.Idle;

        private const float KnockbackDeceleration = 9f;
        private readonly Knockback knockback = new Knockback(KnockbackDeceleration);

        public Rectangle SpriteBounds => new(
            (int)MathF.Round(Position.X),
            (int)MathF.Round(Position.Y),
            Math.Max(1, (int)MathF.Round(FrameWidth * scale)),
            Math.Max(1, (int)MathF.Round(FrameHeight * scale)));

        public Rectangle MovementBounds =>
            GetConfiguredBounds(Definition.Visuals.Hitbox) ??
            GetLegacyBounds();

        public Rectangle Hurtbox =>
            GetConfiguredBounds(Definition.Visuals.Hurtbox) ??
            GetLegacyBounds();

        // Compatibility alias for gameplay systems that need the combat body.
        public Rectangle Bounds => Hurtbox;

        private Rectangle? GetConfiguredBounds(EnemyHitboxDefinition hitbox)
        {
            if (hitbox == null)
                return null;

            int width = Math.Max(1, (int)MathF.Round(hitbox.Width * scale));
            int height = Math.Max(1, (int)MathF.Round(hitbox.Height * scale));
            int centerX = (int)MathF.Round(
                Position.X + hitbox.CenterX * scale);
            int bottomY = (int)MathF.Round(
                Position.Y + hitbox.BottomY * scale);

            return new Rectangle(
                centerX - width / 2,
                bottomY - height,
                width,
                height);
        }

        private Rectangle GetLegacyBounds()
        {
            Rectangle spriteBounds = SpriteBounds;
            int width = (int)(spriteBounds.Width * 0.7f);
            int height = (int)(spriteBounds.Height * 0.7f);

            return new Rectangle(
                spriteBounds.X + (spriteBounds.Width - width) / 2,
                spriteBounds.Y + (spriteBounds.Height - height) / 2,
                width,
                height);
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
            textureResolver = _ => texture;

            this.frameCount = frameCount;
            this.frameRows = frameRows;
            this.frameDuration = frameDuration;
            this.scale = scale;

            framesInCurrentAnimation = frameCount;
            Health = new Health(definition.MaxHealth);
        }

        public Enemy(
            Texture2D texture,
            Vector2 startPosition,
            EnemyDefinition definition,
            IEnemyBehavior behavior,
            Func<string, Texture2D> textureResolver = null)
            : this(
                texture,
                startPosition,
                definition.Visuals.SheetColumns,
                definition.Visuals.SheetRows,
                definition.Visuals.FrameDuration,
                definition.Visuals.Scale,
                definition,
                behavior)
        {
            if (textureResolver != null)
                this.textureResolver = textureResolver;

            SetAnimation(definition.Visuals.DefaultAnimation);
        }

        public int SortY => MovementBounds.Bottom;
        public void Update(GameTime gameTime, EnemyUpdateContext context)
        {
            UpdateKnockback(gameTime, context);

            if (hurtStateTimer > 0f &&
                Definition.Visuals.HurtAnimation != null)
            {
                hurtStateTimer = MathF.Max(
                    0f,
                    hurtStateTimer -
                    (float)gameTime.ElapsedGameTime.TotalSeconds);
                Motor.Stop();
                ChangeState(EnemyState.Hurt);
                SetAnimation(Definition.Visuals.HurtAnimation);
                UpdateAnimation(gameTime);
                return;
            }

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
            currentAnimation = null;
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

        internal void SetAnimation(EnemyAnimationDefinition animation)
        {
            if (animation == null)
                return;

            int targetRow = animation.UsesDirectionalRows
                ? ResolveDirectionalRow(facingDirection)
                : animation.Row;

            bool clipChanged = !ReferenceEquals(currentAnimation, animation);
            bool rowChanged = animationRow != targetRow;

            if (clipChanged)
            {
                if (!string.IsNullOrWhiteSpace(animation.TextureAsset))
                    texture = textureResolver(animation.TextureAsset);

                if (animation.SheetColumns > 0)
                    frameCount = animation.SheetColumns;
                if (animation.SheetRows > 0)
                    frameRows = animation.SheetRows;
                if (animation.FrameDuration > 0f)
                    frameDuration = animation.FrameDuration;

                currentAnimation = animation;
                framesInCurrentAnimation = animation.FrameCount;
            }

            if (clipChanged || rowChanged)
            {
                animationRow = targetRow;
                currentFrame = 0;
                animationTimer = 0f;
            }
        }

        internal void SetFacingDirection(Vector2 direction)
        {
            if (direction != Vector2.Zero)
                facingDirection = direction;
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

        public void TakeHit(CombatHit hit)
        {
            Health.TakeDamage(hit.Damage);
            ApplyKnockback(hit.HitPosition, hit.Knockback);

            EnemyAnimationDefinition hurtAnimation =
                Definition.Visuals.HurtAnimation;

            if (!Health.IsDead && hurtAnimation != null)
            {
                hurtStateTimer = MathF.Max(
                    hurtStateTimer,
                    hurtAnimation.FrameCount * hurtAnimation.FrameDuration);
            }
        }

        internal void SetRotation(float value)
        {
            rotation = value;
        }

        private void UpdateKnockback(GameTime gameTime, EnemyUpdateContext context)
        {
            Motor.MoveDelta(this, knockback.Update(gameTime), context);
        }

        private static int ResolveDirectionalRow(Vector2 direction)
        {
            if (MathF.Abs(direction.X) > MathF.Abs(direction.Y))
                return direction.X < 0f ? 2 : 3;

            return direction.Y < 0f ? 1 : 0;
        }
    }
}
