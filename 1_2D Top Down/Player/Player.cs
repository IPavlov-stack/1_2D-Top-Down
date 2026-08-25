using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace _1_2D_Top_Down
{
    public class Player : IYSortedWorldDrawable
    {
        // Combat
        public const float BasicAttackManaCost = 8f;
        private const float ShootStateDuration = 0.20f;

        private float shootStateTimer;
        public PlayerState CurrentState { get; private set; } = PlayerState.Idle;

        public float MoveSpeed => Stats.MoveSpeed;
        public Texture2D texture;

        private const int FrameCount = 4;
        private const float FrameDuration = 0.15f;

        private int currentFrame;
        private float animationTimer;
        private int FrameWidth => texture.Width / FrameCount;
        private int FrameHeight => texture.Height;

        private const float Scale = 1.3f;

        public Vector2 Position;
        public Vector2 playerPosition = new Vector2(400, 500);

        private const float DamageFlashDuration = 0.75f;
        private const float DamageFlashInterval = 0.08f;
        private float damageFlashTimer;
        private const float HealthFlashDuration = 0.48f;
        private const float HealthFlashInterval = 0.08f;
        private float healthFlashTimeLeft;
        public bool IsHealthFlashingWhite => healthFlashTimeLeft > 0f && (int)(healthFlashTimeLeft / HealthFlashInterval) % 2 == 0;

        public Health Health { get; }
        public Mana Mana { get; }
        public PlayerProfile Profile { get; }
        public PlayerStats Stats { get; }

        public Rectangle SpriteBounds
        {
            get
            {
                int spriteWidth = (int)(FrameWidth * Scale);
                int spriteHeight = (int)(FrameHeight * Scale);

                return new Rectangle(
                    (int)Position.X,
                    (int)Position.Y,
                    spriteWidth,
                    spriteHeight);
            }
        }

        public Rectangle MovementBounds
        {
            get
            {
                Rectangle spriteBounds = SpriteBounds;
                int hitboxWidth = (int)(spriteBounds.Width * 0.50f);
                int hitboxHeight = (int)(spriteBounds.Height * 0.24f);
                int offsetX = (spriteBounds.Width - hitboxWidth) / 2;

                return new Rectangle(
                    spriteBounds.X + offsetX,
                    spriteBounds.Bottom - hitboxHeight,
                    hitboxWidth,
                    hitboxHeight);
            }
        }

        public Rectangle Hurtbox
        {
            get
            {
                Rectangle spriteBounds = SpriteBounds;
                int hitboxWidth = (int)(spriteBounds.Width * 0.60f);
                int hitboxHeight = (int)(spriteBounds.Height * 0.70f);
                int offsetX = (spriteBounds.Width - hitboxWidth) / 2;
                int offsetY = (int)(spriteBounds.Height * 0.18f);

                return new Rectangle(
                    spriteBounds.X + offsetX,
                    spriteBounds.Y + offsetY,
                    hitboxWidth,
                    hitboxHeight);
            }
        }

        // Temporary compatibility alias for systems that still need the
        // player's combat body rather than its movement footprint.
        public Rectangle Bounds => Hurtbox;
        public int SortY => MovementBounds.Bottom;
        public Vector2 Center => Hurtbox.Center.ToVector2();

        public Player(Texture2D texture, Vector2 startPosition, PlayerProfile profile)
        {
            this.texture = texture;
            Position = startPosition;
            Profile = profile ?? throw new ArgumentNullException(nameof(profile));

            Stats = new PlayerStats();
            Health = new Health(Stats.MaxHealth, Stats.HealthRegen);
            Mana = new Mana(Stats.MaxMana, Stats.ManaRegen);
        }
        public void GainExperience(int amount)
        {
            Profile.Experience.AddExperience(amount);
        }
        public void Update(
            GameTime gameTime,
            Rectangle arena,
            Func<Rectangle, bool> intersectsCollision,
            bool canMove)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            bool isMoving = false;
            healthFlashTimeLeft = MathF.Max( 0f, healthFlashTimeLeft - (float)gameTime.ElapsedGameTime.TotalSeconds);
            Health.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            if (damageFlashTimer > 0f)
            {
                damageFlashTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            damageFlashTimer = MathF.Max( 0f, damageFlashTimer - (float)gameTime.ElapsedGameTime.TotalSeconds);
            KeyboardState keyboard = Keyboard.GetState();
            if (canMove)
            {

                Vector2 direction = Vector2.Zero;

                if (keyboard.IsKeyDown(Keys.Left) || keyboard.IsKeyDown(Keys.A))
                    direction.X -= 1f;
                if (keyboard.IsKeyDown(Keys.Right) || keyboard.IsKeyDown(Keys.D))
                    direction.X += 1f;
                if (keyboard.IsKeyDown(Keys.Up) || keyboard.IsKeyDown(Keys.W))
                    direction.Y -= 1f;
                if (keyboard.IsKeyDown(Keys.Down) || keyboard.IsKeyDown(Keys.S))
                    direction.Y += 1f;

                if (direction != Vector2.Zero)
                    direction.Normalize();
                isMoving = direction != Vector2.Zero;

                float movementDistance = Stats.MoveSpeed * deltaTime;
                // Each axis is tried independently. If X is blocked but Y is
                // clear, the player still moves along the obstacle instead of
                // getting stuck against its corner.
                TryMoveHorizontally(direction.X * movementDistance, arena, intersectsCollision);
                TryMoveVertically(direction.Y * movementDistance, arena, intersectsCollision);

                animationTimer += deltaTime;

                if (animationTimer >= FrameDuration)
                {
                    currentFrame++;
                    animationTimer = 0f;

                    if (currentFrame >= FrameCount)
                        currentFrame = 0;
                }
            }
            Mana.Update(gameTime);
            UpdateState(deltaTime, isMoving);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            //=== while player is invulnerable after taking damage: effect
            if (damageFlashTimer > 0f &&
                (int)(damageFlashTimer * 12f) % 2 == 0)
            {
                return;
            }
            //===

            if (damageFlashTimer > 0f &&  (int)(damageFlashTimer / DamageFlashInterval) % 2 == 0)
            {
                return;
            }
            Rectangle sourceRectangle = new Rectangle(
                currentFrame * FrameWidth,
                0,
                FrameWidth,
                FrameHeight);

            spriteBatch.Draw(
                texture,
                Position,
                sourceRectangle,
                Color.White,
                0f,
                Vector2.Zero,
                Scale,
                SpriteEffects.None,
                0f);
        }
        private void TryMoveHorizontally(
            float distance,
            Rectangle arena,
            Func<Rectangle, bool> intersectsCollision)
        {
            float previousX = Position.X;
            Position.X += distance;
            KeepInsideArena(arena);

            if (intersectsCollision(MovementBounds))
                Position.X = previousX;
        }

        private void TryMoveVertically(
            float distance,
            Rectangle arena,
            Func<Rectangle, bool> intersectsCollision)
        {
            float previousY = Position.Y;
            Position.Y += distance;
            KeepInsideArena(arena);

            if (intersectsCollision(MovementBounds))
                Position.Y = previousY;
        }
        private void KeepInsideArena(Rectangle arena)
        {
            Rectangle movementBounds = MovementBounds;

            if (movementBounds.Left < arena.Left)
                Position.X += arena.Left - movementBounds.Left;
            else if (movementBounds.Right > arena.Right)
                Position.X -= movementBounds.Right - arena.Right;

            movementBounds = MovementBounds;

            if (movementBounds.Top < arena.Top)
                Position.Y += arena.Top - movementBounds.Top;
            else if (movementBounds.Bottom > arena.Bottom)
                Position.Y -= movementBounds.Bottom - arena.Bottom;
        }

        public void TakeDamage(int damage)
        {
            float healthBeforeDamage = Health.CurrentHealth;

            // Damage винаги се нанася.
            Health.TakeDamage(damage);

            if (Health.CurrentHealth < healthBeforeDamage)
            {
                // Започваме blink само ако предишният вече е приключил.
                if (damageFlashTimer <= 0f)
                {
                    damageFlashTimer = DamageFlashDuration;
                    healthFlashTimeLeft = HealthFlashDuration;
                }
            }
        }
        public void ResetDamageEffects()
        {
            damageFlashTimer = 0f;
            healthFlashTimeLeft = 0f;
        }
        public void AddStatBonus(PlayerStatType stat, float amount)
        {
            Stats.Add(stat, amount);
            RefreshStats();
        }

        private void RefreshStats()
        {
            Health.SetMaxHealth(Stats.MaxHealth);
            Health.SetRegenPerSecond(Stats.HealthRegen);

            Mana.SetMaxMana(Stats.MaxMana);
            Mana.SetRegenPerSecond(Stats.ManaRegen);
        }
        private void ChangeState(PlayerState newState)
        {
            if (CurrentState == newState)
                return;

            CurrentState = newState;
        }

        private void UpdateState(
            float deltaTime,
            bool isMoving)
        {
            if (CurrentState == PlayerState.Shoot)
            {
                shootStateTimer -= deltaTime;

                if (shootStateTimer > 0f)
                    return;
            }

            ChangeState(
                isMoving
                    ? PlayerState.Walk
                    : PlayerState.Idle);
        }

        public void EnterShootState()
        {
            shootStateTimer = ShootStateDuration;

            ChangeState(PlayerState.Shoot);
        }
    }
}
