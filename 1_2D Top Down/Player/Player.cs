using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace _1_2D_Top_Down
{
    public class Player : IYSortedWorldDrawable
    {
        // Combat
        private const float BasicAttackRange = 60f;
        private const float BasicAttackArcDegrees = 90f;
        private const float BasicAttackReleaseProgress = 0.45f;
        private const float AttackLungeMinimumAngleDegrees = 5f;
        private const float AttackLungeMaximumAngleDegrees = 18f;
        private float attackElapsed;
        private float attackDuration;
        private Vector2 attackDirection = Vector2.UnitY;
        private bool hasReleasedAttack;
        private bool hasPendingMeleeAttack;
        private static readonly DashMovementDefinition AttackLungeDefinition =
            new(
                initialSpeed: 500f,
                distance: 6f,
                cooldown: 0f,
                slideDuration: 0.06f,
                slideEasePower: 2f);
        private readonly DashMotion attackLungeMotion;
        public PlayerState CurrentState { get; private set; } = PlayerState.Idle;

        private const float KnockbackDeceleration = 9f;
        private readonly Knockback knockback = new(KnockbackDeceleration);

        private static readonly DashMovementDefinition DashDefinition = new(
            initialSpeed: 850f,
            distance: 170f,
            cooldown: 1.0f,
            slideDuration: 0.12f,
            slideEasePower: 1.5f);
        private readonly DashMotion dashMotion;
        private Vector2 lastMovementDirection = Vector2.UnitY;
        private float dashCooldownRemaining;
        private bool wasDashKeyDown;

        public bool IsDashing => dashMotion.IsActive;
        public bool IsAttackLunging => attackLungeMotion.IsActive;
        public float DashCooldownRemaining => dashCooldownRemaining;
        public Vector2 FacingDirection => facingDirection;

        public float MoveSpeed => Stats.MoveSpeed;
        private readonly Func<string, Texture2D> textureResolver;
        private Texture2D texture;
        private PlayerAnimationDefinition currentAnimation;
        private Vector2 facingDirection = Vector2.UnitY;
        private int animationRow;

        private int currentFrame;
        private float animationTimer;
        private int FrameWidth =>
            texture.Width / currentAnimation.SheetColumns;
        private int FrameHeight =>
            texture.Height / currentAnimation.SheetRows;
        private int CurrentAnimationFrameCount =>
            currentAnimation.GetFrameCount(animationRow);

        public PlayerVisualDefinition Visuals { get; private set; }
        public Texture2D ShadowTexture { get; private set; }

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
                int spriteWidth = (int)(FrameWidth * Visuals.Scale);
                int spriteHeight = (int)(FrameHeight * Visuals.Scale);

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
                return CreateFootAnchoredBounds(
                    Visuals.MovementHitboxWidth,
                    Visuals.MovementHitboxHeight);
            }
        }

        public Rectangle Hurtbox
        {
            get
            {
                return CreateFootAnchoredBounds(
                    Visuals.HurtboxWidth,
                    Visuals.HurtboxHeight);
            }
        }

        public Vector2 FootPosition => Position + new Vector2(
            Visuals.FootAnchorX * Visuals.Scale,
            Visuals.FootAnchorY * Visuals.Scale);

        private Rectangle CreateFootAnchoredBounds(
            float sourceWidth,
            float sourceHeight)
        {
            Vector2 foot = FootPosition;
            int width = Math.Max(
                1,
                (int)MathF.Round(sourceWidth * Visuals.Scale));
            int height = Math.Max(
                1,
                (int)MathF.Round(sourceHeight * Visuals.Scale));

            return new Rectangle(
                (int)MathF.Round(foot.X) - width / 2,
                (int)MathF.Round(foot.Y) - height,
                width,
                height);
        }

        // Temporary compatibility alias for systems that still need the
        // players combat body rather than its movement footprint.
        public Rectangle Bounds => Hurtbox;
        public int SortY => MovementBounds.Bottom;
        public Vector2 Center => Hurtbox.Center.ToVector2();

        public Player(
            PlayerVisualDefinition visuals,
            Func<string, Texture2D> textureResolver,
            Vector2 startPosition,
            PlayerProfile profile)
        {
            this.textureResolver = textureResolver ??
                throw new ArgumentNullException(nameof(textureResolver));
            Position = startPosition;
            Profile = profile ?? throw new ArgumentNullException(nameof(profile));

            Stats = new PlayerStats();
            Health = new Health(Stats.MaxHealth, Stats.HealthRegen);
            Mana = new Mana(Stats.MaxMana, Stats.ManaRegen);
            dashMotion = new DashMotion(DashDefinition);
            attackLungeMotion = new DashMotion(AttackLungeDefinition);
            wasDashKeyDown = IsDashKeyDown(Keyboard.GetState());
            SetVisuals(visuals);
        }

        public void SetVisuals(PlayerVisualDefinition visuals)
        {
            Visuals = visuals ??
                throw new ArgumentNullException(nameof(visuals));
            ShadowTexture = textureResolver(Visuals.ShadowTextureAsset);
            currentAnimation = null;
            SetAnimation(Visuals.Idle);
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
            dashCooldownRemaining = MathF.Max(
                0f,
                dashCooldownRemaining - deltaTime);
            healthFlashTimeLeft = MathF.Max( 0f, healthFlashTimeLeft - (float)gameTime.ElapsedGameTime.TotalSeconds);
            Health.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            if (damageFlashTimer > 0f)
            {
                damageFlashTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            damageFlashTimer = MathF.Max( 0f, damageFlashTimer - (float)gameTime.ElapsedGameTime.TotalSeconds);
            KeyboardState keyboard = Keyboard.GetState();
            bool dashKeyDown = IsDashKeyDown(keyboard);
            bool dashPressed = dashKeyDown && !wasDashKeyDown;
            wasDashKeyDown = dashKeyDown;

            bool isAttacking = CurrentState == PlayerState.Attacking;

            if (!canMove && attackLungeMotion.IsActive)
                attackLungeMotion.Stop();

            if (canMove && isAttacking && attackLungeMotion.IsActive)
            {
                attackLungeMotion.Update(
                    deltaTime,
                    movement => TryMoveDash(
                        movement,
                        arena,
                        intersectsCollision));
            }

            bool canControlMovement = canMove && !isAttacking;

            if (!canControlMovement && dashMotion.IsActive)
                dashMotion.Stop();

            if (canControlMovement && dashMotion.IsActive)
            {
                isMoving = true;
                dashMotion.Update(
                    deltaTime,
                    movement => TryMoveDash(
                        movement,
                        arena,
                        intersectsCollision));
            }
            else if (canControlMovement)
            {
                Vector2 direction = GetMovementDirection(keyboard);

                if (direction != Vector2.Zero)
                {
                    direction.Normalize();
                    lastMovementDirection = direction;
                    SetFacingDirection(direction);
                }

                if (dashPressed && dashCooldownRemaining <= 0f)
                {
                    BeginDash();
                    isMoving = true;
                    dashMotion.Update(
                        deltaTime,
                        movement => TryMoveDash(
                            movement,
                            arena,
                            intersectsCollision));
                }
                else
                {
                    isMoving = direction != Vector2.Zero;

                    float movementDistance = Stats.MoveSpeed * deltaTime;
                    // Each axis is tried independently. If X is blocked but Y
                    // is clear, the player still moves along the obstacle.
                    TryMoveHorizontally(
                        direction.X * movementDistance,
                        arena,
                        intersectsCollision);
                    TryMoveVertically(
                        direction.Y * movementDistance,
                        arena,
                        intersectsCollision);
                }

            }

            ApplyKnockbackMovement(
                gameTime,
                arena,
                intersectsCollision);

            Mana.Update(gameTime);
            UpdateState(deltaTime, isMoving, dashMotion.IsActive);
            UpdateVisualAnimation(deltaTime);
        }

        private static bool IsDashKeyDown(KeyboardState keyboard) =>
            keyboard.IsKeyDown(Keys.LeftShift) ||
            keyboard.IsKeyDown(Keys.RightShift);

        private static Vector2 GetMovementDirection(
            KeyboardState keyboard)
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

            return direction;
        }

        private void BeginDash()
        {
            dashMotion.Begin(lastMovementDirection);
            dashCooldownRemaining = DashDefinition.Cooldown;
            SetFacingDirection(dashMotion.Direction);
            ChangeState(PlayerState.Dash);
        }

        private void UpdateVisualAnimation(float deltaTime)
        {
            PlayerAnimationDefinition targetAnimation = CurrentState switch
            {
                PlayerState.Walk => Visuals.Walk,
                PlayerState.Dash => Visuals.Run,
                PlayerState.Attacking => Visuals.Attack,
                _ => Visuals.Idle
            };
            SetAnimation(targetAnimation);

            float animationSpeed = CurrentState == PlayerState.Attacking
                ? Stats.AttackSpeed
                : 1f;
            animationTimer += deltaTime * animationSpeed;

            if (animationTimer < currentAnimation.FrameDuration)
                return;

            animationTimer -= currentAnimation.FrameDuration;
            currentFrame++;

            if (currentFrame < CurrentAnimationFrameCount)
                return;

            currentFrame = currentAnimation.Loop
                ? 0
                : CurrentAnimationFrameCount - 1;
        }

        private void SetFacingDirection(Vector2 direction)
        {
            if (direction == Vector2.Zero)
                return;

            facingDirection = direction;
        }

        private void SetAnimation(PlayerAnimationDefinition animation)
        {
            int targetRow = ResolveDirectionalRow(facingDirection);
            bool animationChanged =
                !ReferenceEquals(currentAnimation, animation);
            bool rowChanged = animationRow != targetRow;

            if (animationChanged)
            {
                currentAnimation = animation;
                texture = textureResolver(animation.TextureAsset);
            }

            if (!animationChanged && !rowChanged)
                return;

            animationRow = targetRow;
            currentFrame = 0;
            animationTimer = 0f;
        }

        private static int ResolveDirectionalRow(Vector2 direction)
        {
            // The swordsman sheets use: front/down, left, right, back/up.
            if (MathF.Abs(direction.X) > MathF.Abs(direction.Y))
                return direction.X < 0f ? 1 : 2;

            return direction.Y < 0f ? 3 : 0;
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
                animationRow * FrameHeight,
                FrameWidth,
                FrameHeight);

            spriteBatch.Draw(
                texture,
                Position,
                sourceRectangle,
                Color.White,
                0f,
                Vector2.Zero,
                Visuals.Scale,
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

        private bool TryMoveDash(
            Vector2 movement,
            Rectangle arena,
            Func<Rectangle, bool> intersectsCollision)
        {
            Vector2 previousPosition = Position;
            Position += movement;
            Rectangle movementBounds = MovementBounds;

            bool outsideArena =
                movementBounds.Left < arena.Left ||
                movementBounds.Right > arena.Right ||
                movementBounds.Top < arena.Top ||
                movementBounds.Bottom > arena.Bottom;

            if (outsideArena || intersectsCollision(movementBounds))
            {
                Position = previousPosition;
                return false;
            }

            return true;
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

        public void TakeHit(CombatHit hit)
        {
            TakeDamage(hit.Damage);

            if (hit.Knockback <= 0f)
                return;

            Vector2 direction = Center - hit.HitPosition;
            if (direction == Vector2.Zero)
            {
                // An area effect can be centered exactly on the player. A
                // deterministic fallback still gives that impact a visible
                // physical response.
                direction = Vector2.UnitY;
            }

            knockback.Apply(direction, hit.Knockback);
        }

        private void ApplyKnockbackMovement(
            GameTime gameTime,
            Rectangle arena,
            Func<Rectangle, bool> intersectsCollision)
        {
            Vector2 movement = knockback.Update(gameTime);
            TryMoveHorizontally(
                movement.X,
                arena,
                intersectsCollision);
            TryMoveVertically(
                movement.Y,
                arena,
                intersectsCollision);
        }
        public void ResetDamageEffects()
        {
            damageFlashTimer = 0f;
            healthFlashTimeLeft = 0f;
        }

        public void ResetMovementAbilities()
        {
            dashMotion.Stop();
            attackLungeMotion.Stop();
            dashCooldownRemaining = 0f;
            lastMovementDirection = Vector2.UnitY;
            facingDirection = Vector2.UnitY;
            attackDirection = Vector2.UnitY;
            attackElapsed = 0f;
            attackDuration = 0f;
            hasReleasedAttack = false;
            hasPendingMeleeAttack = false;
            wasDashKeyDown = IsDashKeyDown(Keyboard.GetState());
            knockback.Clear();
            ChangeState(PlayerState.Idle);
            SetAnimation(Visuals.Idle);
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
            bool isMoving,
            bool isDashing)
        {
            if (isDashing)
            {
                ChangeState(PlayerState.Dash);
                return;
            }

            if (CurrentState == PlayerState.Attacking)
            {
                attackElapsed += deltaTime;

                if (!hasReleasedAttack &&
                    attackElapsed >=
                        attackDuration * BasicAttackReleaseProgress)
                {
                    hasReleasedAttack = true;
                    hasPendingMeleeAttack = true;
                }

                if (attackElapsed < attackDuration)
                    return;

                attackLungeMotion.Stop();
            }

            ChangeState(
                isMoving
                    ? PlayerState.Walk
                    : PlayerState.Idle);
        }

        public bool TryBeginMeleeAttack(Vector2 direction)
        {
            if (CurrentState == PlayerState.Attacking || IsDashing)
                return false;

            if (direction == Vector2.Zero)
                direction = facingDirection;

            direction.Normalize();
            attackDirection = direction;
            SetFacingDirection(direction);
            attackElapsed = 0f;
            attackDuration =
                Visuals.Attack.GetFrameCount(
                    ResolveDirectionalRow(direction)) *
                Visuals.Attack.FrameDuration /
                Stats.AttackSpeed;
            hasReleasedAttack = false;
            hasPendingMeleeAttack = false;
            attackLungeMotion.Begin(
                GetRandomAttackLungeDirection(direction));

            ChangeState(PlayerState.Attacking);
            SetAnimation(Visuals.Attack);
            return true;
        }

        private static Vector2 GetRandomAttackLungeDirection(
            Vector2 attackDirection)
        {
            float angleMagnitude = MathHelper.Lerp(
                AttackLungeMinimumAngleDegrees,
                AttackLungeMaximumAngleDegrees,
                Random.Shared.NextSingle());
            float angleSign = Random.Shared.Next(2) == 0 ? -1f : 1f;
            float angleRadians = MathHelper.ToRadians(
                angleMagnitude * angleSign);

            return Vector2.Transform(
                attackDirection,
                Matrix.CreateRotationZ(angleRadians));
        }

        public bool TryConsumeMeleeAttack(out MeleeAttack attack)
        {
            if (!hasPendingMeleeAttack)
            {
                attack = default;
                return false;
            }

            hasPendingMeleeAttack = false;
            Vector2 origin = Hurtbox.Center.ToVector2();
            attack = new MeleeAttack(
                origin,
                attackDirection,
                BasicAttackRange,
                BasicAttackArcDegrees,
                new CombatHit(
                    Stats.Damage,
                    DamageType.Physical,
                    CombatFaction.Player,
                    "player_melee",
                    origin,
                    Stats.Knockback));

            return true;
        }
    }
}
