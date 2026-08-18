using System;
using Microsoft.Xna.Framework;

namespace _1_2D_Top_Down
{
    public sealed class KeepDistanceRangedBehavior : IEnemyBehavior
    {
        private readonly int movementAnimationRow;
        private readonly int movementFrameCount;
        private readonly int attackAnimationRow;
        private readonly int attackFrameCount;
        private readonly float projectileReleaseTime;
        private readonly float attackDuration;
        private readonly bool rotateDuringAttack;

        private float shootTimer;
        private float attackTimer;
        private bool projectileRequested;

        public KeepDistanceRangedBehavior(
            int movementAnimationRow,
            int movementFrameCount,
            int attackAnimationRow,
            int attackFrameCount,
            float projectileReleaseTime,
            float attackDuration,
            bool rotateDuringAttack = true)
        {
            this.movementAnimationRow = movementAnimationRow;
            this.movementFrameCount = movementFrameCount;
            this.attackAnimationRow = attackAnimationRow;
            this.attackFrameCount = attackFrameCount;
            this.projectileReleaseTime = projectileReleaseTime;
            this.attackDuration = attackDuration;
            this.rotateDuringAttack = rotateDuringAttack;
        }

        public void Update(
            Enemy enemy,
            GameTime gameTime,
            EnemyUpdateContext context)
        {
            float deltaTime =
                (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 direction =
                context.Target.Bounds.Center.ToVector2() -
                enemy.SpriteCenter;

            float distance = direction.Length();

            if (distance > enemy.Definition.AttackRange &&
                enemy.CurrentState != EnemyState.Attacking)
            {
                enemy.ChangeState(EnemyState.Chasing);

                if (direction != Vector2.Zero)
                {
                    direction.Normalize();
                    enemy.Position +=
                        direction * enemy.Definition.MoveSpeed * deltaTime;
                }

                enemy.SetAnimation(movementAnimationRow, movementFrameCount);
                enemy.UpdateAnimation(gameTime);
                return;
            }

            if (enemy.CurrentState != EnemyState.Attacking)
            {
                enemy.ChangeState(EnemyState.Idle);
                enemy.SetAnimation(movementAnimationRow, movementFrameCount);
                enemy.UpdateAnimation(gameTime);

                shootTimer += deltaTime;

                if (shootTimer >= enemy.Definition.AttackCooldownSeconds)
                {
                    shootTimer = 0f;
                    attackTimer = 0f;
                    projectileRequested = false;
                    enemy.ChangeState(EnemyState.Attacking);

                    if (rotateDuringAttack && direction != Vector2.Zero)
                    {
                        direction.Normalize();
                        enemy.SetRotation(GetAttackRotation(direction));
                    }

                    enemy.SetAnimation(attackAnimationRow, attackFrameCount);
                }

                return;
            }

            attackTimer += deltaTime;
            enemy.UpdateAnimation(gameTime);

            if (!projectileRequested &&
                attackTimer >= projectileReleaseTime)
            {
                projectileRequested = true;

                if (direction != Vector2.Zero)
                    direction.Normalize();

                context.RequestProjectile(
                    enemy.Definition.ProjectileTextureAsset!,
                    enemy.SpriteCenter,
                    direction);
            }

            if (attackTimer < attackDuration)
                return;

            attackTimer = 0f;
            if (rotateDuringAttack)
            {
                enemy.SetRotation(0f);
            }
            enemy.ChangeState(EnemyState.Idle);
            enemy.SetAnimation(movementAnimationRow, movementFrameCount);
        }

        private static float GetAttackRotation(Vector2 direction)
        {
            if (MathF.Abs(direction.X) > MathF.Abs(direction.Y))
                return direction.X >= 0f ? 0f : MathF.PI;

            return direction.Y >= 0f
                ? MathF.PI / 2f
                : -MathF.PI / 2f;
        }
    }
}
