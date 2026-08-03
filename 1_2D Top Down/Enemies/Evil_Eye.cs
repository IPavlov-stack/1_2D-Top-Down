using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace _1_2D_Top_Down
{
    public class Evil_Eye : Enemy
    {
        private const float Speed = 100f;
        private const float AttackRange = 600f;
        private const float AttackSpeed = 1.0f;
        private const int EyeMaxHealth = 2;

        //animations
        private const int FlyingRow = 0;
        private const int AttackRow = 1;
        private const int DeathRow = 2;

        private float shootTimer;
        private float attackTimer;

        private bool projectileFired;

        public bool IsDeathAnimationFinished { get; private set; }
        public bool IsDead => CurrentState == EnemyState.Dead;
        public Evil_Eye(Texture2D texture, Vector2 startPosition)
            : base(
                texture,
                startPosition,
                frameCount: 6,
                frameRows: 3,
                frameDuration: 0.15f,
                scale: 0.5f,
                maxHealth: EyeMaxHealth)

        {

            SetAnimation(FlyingRow, 4);
            ChangeState(EnemyState.Idle);
        }

        public void Die()
        {
            if (IsDead)
                return;

            ChangeState(EnemyState.Dead);
            SetAnimation(DeathRow, 4);
        }
        public EnemyProjectile? Update(
            GameTime gameTime,
            Player player,
            Texture2D projectileTexture)
        {
            if (IsDead)
            {
                IsDeathAnimationFinished =
                    UpdateAnimation(
                        gameTime,
                        loop: false);

                return null;
            }

            UpdateKnockback(gameTime);

            float deltaTime =
                (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 direction =
                player.Bounds.Center.ToVector2() -
                SpriteCenter;

            float distance = direction.Length();

            // Врагът преследва играча, докато
            // влезе в обсега за атака.
            if (distance > AttackRange &&
                CurrentState != EnemyState.Attacking)
            {
                ChangeState(EnemyState.Chasing);

                if (direction != Vector2.Zero)
                {
                    direction.Normalize();

                    Position +=
                                       direction * Speed * deltaTime;
                }

                SetAnimation(FlyingRow, 4);
                UpdateAnimation(gameTime);

                return null;
            }

            // В обсег е, но още не атакува.
            if (CurrentState != EnemyState.Attacking)
            {
                ChangeState(EnemyState.Idle);

                SetAnimation(FlyingRow, 4);
                UpdateAnimation(gameTime);

                shootTimer += deltaTime;

                if (shootTimer >= AttackSpeed)
                {
                    shootTimer = 0f;
                    attackTimer = 0f;
                    projectileFired = false;

                    ChangeState(EnemyState.Attacking);

                    if (direction != Vector2.Zero)
                    {
                        direction.Normalize();
                        SetAttackRotation(direction);
                    }

                    SetAnimation(AttackRow, 6);
                }

                return null;
            }

            // Attack state.
            attackTimer += deltaTime;
            UpdateAnimation(gameTime);

            // Projectile се създава около третия кадър.
            if (!projectileFired &&
                attackTimer >= 0.30f)
            {
                projectileFired = true;

                if (direction != Vector2.Zero)
                {
                    direction.Normalize();
                }

                return new EnemyProjectile(
                    projectileTexture,
                    SpriteCenter,
                    direction);
            }

            // 6 кадъра × 0.15 секунди = 0.9 секунди.
            if (attackTimer >= 0.9f)
            {
                attackTimer = 0f;
                rotation = 0f;

                ChangeState(EnemyState.Idle);
                SetAnimation(FlyingRow, 4);
            }

            return null;
        }
        private void SetAttackRotation(Vector2 direction)
        {
            if (MathF.Abs(direction.X) > MathF.Abs(direction.Y))
            {
                // Надясно / наляво
                rotation = direction.X >= 0
                    ? 0f
                    : MathF.PI;
            }
            else
            {
                // Надолу / нагоре
                rotation = direction.Y >= 0
                    ? MathF.PI / 2f
                    : -MathF.PI / 2f;
            }
        }
    }
}