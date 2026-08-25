using System;
using Microsoft.Xna.Framework;

namespace _1_2D_Top_Down
{
    public sealed class ChaseContactBehavior : IEnemyBehavior
    {
        private readonly float contactDamageCooldown;
        private readonly float attackStateDuration;

        private float contactDamageTimer;
        private float attackStateTimer;

        public ChaseContactBehavior(
            float contactDamageCooldown,
            float attackStateDuration)
        {
            this.contactDamageCooldown = contactDamageCooldown;
            this.attackStateDuration = attackStateDuration;
            contactDamageTimer = contactDamageCooldown;
        }

        public void Update(
            Enemy enemy,
            GameTime gameTime,
            EnemyUpdateContext context)
        {
            float deltaTime =
                (float)gameTime.ElapsedGameTime.TotalSeconds;

            contactDamageTimer += deltaTime;
            attackStateTimer = MathF.Max(0f, attackStateTimer - deltaTime);

            enemy.ChangeState(
                attackStateTimer > 0f
                    ? EnemyState.Attacking
                    : EnemyState.Chasing);

            Vector2 direction =
                context.Target.Hurtbox.Center.ToVector2() -
                enemy.Bounds.Center.ToVector2();

            if (direction != Vector2.Zero)
            {
                direction.Normalize();
                enemy.Position +=
                    direction * enemy.Definition.MoveSpeed * deltaTime;
            }

            enemy.UpdateAnimation(gameTime);

            if (contactDamageTimer < contactDamageCooldown ||
                !context.Target.Hurtbox.Intersects(enemy.Bounds))
            {
                return;
            }

            context.RequestPlayerDamage(enemy.Definition.ContactDamage);
            contactDamageTimer = 0f;
            attackStateTimer = attackStateDuration;
            enemy.ChangeState(EnemyState.Attacking);
        }
    }
}
