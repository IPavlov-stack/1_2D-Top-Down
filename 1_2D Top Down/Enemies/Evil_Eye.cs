using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace _1_2D_Top_Down
{
    public class Evil_Eye : Enemy
    {
        private const float Speed = 100f;
        private const float PreferredDistance = 650f;
        private const float ShootCooldown = 1.0f;

        private const int FlyingRow = 0;
        private const int AttackRow = 1;
        private const int DeathRow = 2;

        private float shootTimer;
        private float attackTimer;

        private bool isAttacking;
        private bool projectileFired;
        private bool isDead;

        public bool IsDeathAnimationFinished { get; private set; }
        public bool IsDead => isDead;

        public Evil_Eye(Texture2D texture, Vector2 startPosition)
            : base(
                texture,
                startPosition,
                frameCount: 6,
                frameRows: 3,
                frameDuration: 0.15f,
                scale: 0.5f)

        {
            SetAnimation(FlyingRow, 4);
        }

        public void Die()
        {
            if (isDead)
            {
                return;
            }

            isDead = true;
            SetAnimation(DeathRow, 4);
        }

        public EnemyProjectile? Update(
            GameTime gameTime,
            Player player,
            Texture2D projectileTexture)
        {
            if (isDead)
            {
                IsDeathAnimationFinished = UpdateAnimation(
                    gameTime,
                    loop: false);

                return null;
            }

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 direction = player.Bounds.Center.ToVector2() - SpriteCenter;
            float distance = direction.Length();

            if (distance > PreferredDistance && !isAttacking)
            {
                if (direction != Vector2.Zero)
                {
                    direction.Normalize();
                    Position += direction * Speed * deltaTime;
                }

                SetAnimation(FlyingRow, 4);
                UpdateAnimation(gameTime);

                return null;
            }

            if (!isAttacking)
            {
                SetAnimation(FlyingRow, 4);
                UpdateAnimation(gameTime);

                shootTimer += deltaTime;

                if (shootTimer >= ShootCooldown)
                {
                    shootTimer = 0f;
                    attackTimer = 0f;
                    projectileFired = false;
                    isAttacking = true;

                    if (direction != Vector2.Zero)
                    {
                        direction.Normalize();
                        SetAttackRotation(direction);
                    }
                    SetAnimation(AttackRow, 6);
                    if (attackTimer >= 0.9f)
                    {
                        isAttacking = false;
                        rotation = 0f;

                        SetAnimation(FlyingRow, 4);
                    }
                }

                return null;
            }


            // Attack animation: projectile-ът излиза около третия кадър.
            attackTimer += deltaTime;
            UpdateAnimation(gameTime);

            if (!projectileFired && attackTimer >= 0.30f)
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

            // 6 кадъра × 0.15 s = 0.9 s
            if (attackTimer >= 0.9f)
            {
                isAttacking = false;
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