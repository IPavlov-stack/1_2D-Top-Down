using System;
using Microsoft.Xna.Framework;

namespace _1_2D_Top_Down
{
    /// <summary>
    /// Ranged caster behavior that starts an enemy attack animation and a
    /// world-space telegraph on the same frame. Damage is owned by the spawned
    /// area effect, keeping this behavior independent from Player and Game1.
    /// </summary>
    public sealed class TelegraphedAreaAttackBehavior : IEnemyBehavior
    {
        private readonly TelegraphedAreaAttackBehaviorDefinition definition;

        private float cooldownTimer;
        private float attackTimer;

        public TelegraphedAreaAttackBehavior(
            TelegraphedAreaAttackBehaviorDefinition definition)
        {
            this.definition = definition ??
                throw new ArgumentNullException(nameof(definition));
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

            enemy.SetFacingDirection(direction);

            if (enemy.CurrentState == EnemyState.Attacking)
            {
                UpdateAttack(enemy, gameTime, deltaTime);
                return;
            }

            float distance = direction.Length();
            if (distance > definition.AttackRange)
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

                enemy.SetAnimation(definition.MovementAnimation);
                enemy.UpdateAnimation(gameTime);
                return;
            }

            enemy.Motor.Stop();
            enemy.ChangeState(EnemyState.Idle);
            enemy.SetAnimation(definition.IdleAnimation);
            enemy.UpdateAnimation(gameTime);

            cooldownTimer += deltaTime;
            if (cooldownTimer < definition.AttackCooldown)
                return;

            BeginAttack(enemy, context);
        }

        private void BeginAttack(
            Enemy enemy,
            EnemyUpdateContext context)
        {
            cooldownTimer = 0f;
            attackTimer = 0f;
            enemy.Motor.Stop();
            enemy.ChangeState(EnemyState.Attacking);
            enemy.SetAnimation(
                definition.AttackAnimation ??
                definition.IdleAnimation);

            Vector2 targetCenter =
                context.Target.Hurtbox.Center.ToVector2();
            Vector2 snappedCenter = new(
                SnapToTileCenter(
                    targetCenter.X,
                    context.WorldTileSize.X),
                SnapToTileCenter(
                    targetCenter.Y,
                    context.WorldTileSize.Y));

            // The request is emitted on the exact frame on which the attack
            // animation starts. WorldAreaEffect delays damage until its
            // telegraph phase has completed.
            context.RequestAreaEffect(
                definition.AreaEffect,
                snappedCenter,
                enemy.Definition.Id);
        }

        private void UpdateAttack(
            Enemy enemy,
            GameTime gameTime,
            float deltaTime)
        {
            attackTimer += deltaTime;
            enemy.Motor.Stop();
            enemy.UpdateAnimation(
                gameTime,
                loop: definition.AttackAnimation == null);

            if (attackTimer < definition.AttackDuration)
                return;

            attackTimer = 0f;
            enemy.ChangeState(EnemyState.Idle);
            enemy.SetAnimation(definition.IdleAnimation);
        }

        private static float SnapToTileCenter(
            float coordinate,
            float tileSize)
        {
            if (tileSize <= 0f)
                return coordinate;

            return (MathF.Floor(coordinate / tileSize) + 0.5f) *
                tileSize;
        }
    }
}
