using System;
using Microsoft.Xna.Framework;

namespace _1_2D_Top_Down
{
    public sealed class ChaseContactBehavior : IEnemyBehavior
    {
        private readonly int contactDamage;
        private readonly float contactDamageCooldown;
        private readonly float attackStateDuration;
        private readonly float damageReleaseTime;
        private readonly EnemyAnimationDefinition movementAnimation;
        private readonly EnemyAnimationDefinition attackAnimation;

        private float contactDamageTimer;
        private float attackTimer;
        private bool isAttacking;
        private bool attackDamageRequested;

        public ChaseContactBehavior(ChaseContactBehaviorDefinition definition)
        {
            contactDamage = definition.ContactDamage;
            contactDamageCooldown = definition.DamageCooldown;
            attackStateDuration = definition.AttackStateDuration;
            damageReleaseTime = definition.DamageReleaseTime;
            movementAnimation = definition.MovementAnimation;
            attackAnimation = definition.AttackAnimation;
            contactDamageTimer = definition.DamageCooldown;
        }

        public void Update(
            Enemy enemy,
            GameTime gameTime,
            EnemyUpdateContext context)
        {
            float deltaTime =
                (float)gameTime.ElapsedGameTime.TotalSeconds;

            contactDamageTimer += deltaTime;

            Vector2 direction =
                context.Target.Hurtbox.Center.ToVector2() -
                enemy.Bounds.Center.ToVector2();

            enemy.SetFacingDirection(direction);

            if (isAttacking)
            {
                attackTimer += deltaTime;
                enemy.Motor.Stop();
                enemy.ChangeState(EnemyState.Attacking);
                enemy.SetAnimation(attackAnimation);
                enemy.UpdateAnimation(gameTime, loop: false);

                if (!attackDamageRequested &&
                    attackTimer >= damageReleaseTime)
                {
                    attackDamageRequested = true;

                    if (context.Target.Hurtbox.Intersects(
                        enemy.ContactHitbox))
                    {
                        context.RequestPlayerDamage(new CombatHit(
                            contactDamage,
                            DamageType.Physical,
                            CombatFaction.Enemy,
                            enemy.Definition.Id,
                            enemy.ContactHitbox.Center.ToVector2()));
                        contactDamageTimer = 0f;
                    }
                }

                if (attackTimer >= attackStateDuration)
                {
                    isAttacking = false;
                    enemy.ChangeState(EnemyState.Chasing);
                }

                return;
            }

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
            else
                enemy.Motor.Stop();

            enemy.SetAnimation(movementAnimation);
            enemy.UpdateAnimation(gameTime);

            if (contactDamageTimer < contactDamageCooldown ||
                !context.Target.Hurtbox.Intersects(
                    enemy.ContactHitbox))
            {
                return;
            }

            isAttacking = true;
            attackTimer = 0f;
            attackDamageRequested = false;
            enemy.ChangeState(EnemyState.Attacking);
            enemy.SetAnimation(attackAnimation);
        }
    }
}
