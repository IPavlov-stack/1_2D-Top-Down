using System;
using Microsoft.Xna.Framework;

namespace _1_2D_Top_Down
{
    public sealed class KeepDistanceRangedBehavior : IEnemyBehavior
    {
        private readonly float attackRange;
        private readonly float attackCooldown;
        private readonly EnemyAnimationDefinition idleAnimation;
        private readonly EnemyAnimationDefinition movementAnimation;
        private readonly EnemyAnimationDefinition attackAnimation;
        private readonly float projectileReleaseTime;
        private readonly float attackDuration;
        private readonly bool rotateDuringAttack;
        private readonly ProjectileSpec projectile;

        private float shootTimer;
        private float attackTimer;
        private bool projectileRequested;

        public KeepDistanceRangedBehavior(
            KeepDistanceRangedBehaviorDefinition definition)
        {
            attackRange = definition.AttackRange;
            attackCooldown = definition.AttackCooldown;
            idleAnimation = definition.IdleAnimation;
            movementAnimation = definition.MovementAnimation;
            attackAnimation = definition.AttackAnimation;
            projectileReleaseTime = definition.ReleaseTime;
            attackDuration = definition.AttackDuration;
            rotateDuringAttack = definition.RotateDuringAttack;
            projectile = definition.Projectile;
        }

        public void Update(
            Enemy enemy,
            GameTime gameTime,
            EnemyUpdateContext context)
        {
            float deltaTime =
                (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 direction =
                context.Target.Hurtbox.Center.ToVector2() -
                enemy.SpriteCenter;

            float distance = direction.Length();

            enemy.SetFacingDirection(direction);

            if (distance > attackRange &&
                enemy.CurrentState != EnemyState.Attacking)
            {
                enemy.ChangeState(EnemyState.Chasing);

                if (direction != Vector2.Zero)
                {
                    enemy.Motor.Move(
                        enemy,
                        direction,
                        enemy.Definition.MoveSpeed,
                        EnemyMovementMode.Walk,
                        deltaTime,
                        context);
                }

                enemy.SetAnimation(movementAnimation);
                enemy.UpdateAnimation(gameTime);
                return;
            }

            if (enemy.CurrentState != EnemyState.Attacking)
            {
                enemy.Motor.Stop();
                enemy.ChangeState(EnemyState.Idle);
                enemy.SetAnimation(idleAnimation);
                enemy.UpdateAnimation(gameTime);

                shootTimer += deltaTime;

                if (shootTimer >= attackCooldown)
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

                    enemy.SetAnimation(attackAnimation);
                }

                return;
            }

            attackTimer += deltaTime;
            enemy.Motor.Stop();
            enemy.UpdateAnimation(gameTime);

            if (!projectileRequested &&
                attackTimer >= projectileReleaseTime)
            {
                projectileRequested = true;

                if (direction != Vector2.Zero)
                    direction.Normalize();

                context.RequestProjectile(
                    projectile,
                    enemy.Definition.Id,
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
            enemy.SetAnimation(idleAnimation);
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
