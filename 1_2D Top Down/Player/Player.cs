using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace _1_2D_Top_Down
{
    public class Player
    {
        public Texture2D texture;

        private const int FrameCount = 4;
        private const float FrameDuration = 0.15f;

        private int currentFrame;
        private float animationTimer;
        private int FrameWidth => texture.Width / FrameCount;
        private int FrameHeight => texture.Height;

        private const float Scale = 1.3f;
        private const float PlayerMoveSpeed = 400f;

        public Vector2 Position;
        public Vector2 playerPosition = new Vector2(400, 500);

        public Health Health { get; }
        private const float InvulnerabilityDuration = 0.75f;
        private float invulnerabilityTimer;



        public Rectangle Bounds
        {
            get
            {
                int spriteWidth = (int)(FrameWidth * Scale);
                int spriteHeight = (int)(FrameHeight * Scale);

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

        public Player(Texture2D texture, Vector2 startPosition)
        {
            this.texture = texture;
            Position = startPosition;
            Health = new Health(100);
        }

        public void Update(
            GameTime gameTime,
            Rectangle arena,
            IReadOnlyList<Rectangle> collisionRectangles)
        {
            if (invulnerabilityTimer > 0f)
            {
                invulnerabilityTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            KeyboardState keyboard = Keyboard.GetState();
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

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

            float movementDistance = PlayerMoveSpeed * deltaTime;

            // Each axis is tried independently. If X is blocked but Y is
            // clear, the player still moves along the obstacle instead of
            // getting stuck against its corner.
            TryMoveHorizontally(direction.X * movementDistance, arena, collisionRectangles);
            TryMoveVertically(direction.Y * movementDistance, arena, collisionRectangles);

            animationTimer += deltaTime;

            if (animationTimer >= FrameDuration)
            {
                currentFrame++;
                animationTimer = 0f;

                if (currentFrame >= FrameCount)
                    currentFrame = 0;
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            //=== while player is invulnerable after taking damage: effect
            if (invulnerabilityTimer > 0f &&
                (int)(invulnerabilityTimer * 12f) % 2 == 0)
            {
                return;
            }
            //===
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
           IReadOnlyList<Rectangle> collisionRectangles)
        {
            float previousX = Position.X;
            Position.X += distance;
            KeepInsideArena(arena);

            if (IntersectsCollision(collisionRectangles))
                Position.X = previousX;
        }

        private void TryMoveVertically(
            float distance,
            Rectangle arena,
            IReadOnlyList<Rectangle> collisionRectangles)
        {
            float previousY = Position.Y;
            Position.Y += distance;
            KeepInsideArena(arena);

            if (IntersectsCollision(collisionRectangles))
                Position.Y = previousY;
        }

        private void KeepInsideArena(Rectangle arena)
        {
            float playerWidth = FrameWidth * Scale;
            float playerHeight = FrameHeight * Scale;

            Position.X = Math.Clamp(Position.X, arena.Left, arena.Right - playerWidth);
            Position.Y = Math.Clamp(Position.Y, arena.Top, arena.Bottom - playerHeight);
        }

        private bool IntersectsCollision(IReadOnlyList<Rectangle> collisionRectangles)
        {
            foreach (Rectangle collisionRectangle in collisionRectangles)
            {
                if (Bounds.Intersects(collisionRectangle))
                    return true;
            }

            return false;
        }
        public void TakeDamage(int damage)
        {
            if (invulnerabilityTimer > 0f || Health.IsDead)
                return;

            Health.TakeDamage(damage);
            invulnerabilityTimer = InvulnerabilityDuration;
        }
    }
}
