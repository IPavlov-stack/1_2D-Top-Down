using System;
using Microsoft.Xna.Framework;

namespace _1_2D_Top_Down
{
    /// <summary>
    /// A stationary pursuit pattern that moves exclusively through discrete
    /// dashes aimed at the target's position when each dash begins.
    /// </summary>
    public sealed class DashOnlyBehavior :
        IEnemyBehavior,
        IEnemyHitReactionPolicy
    {
        private enum DashPhase
        {
            Waiting,
            Dashing
        }

        private readonly DashOnlyBehaviorDefinition definition;
        private readonly DashMotion dashMotion;

        private DashPhase phase = DashPhase.Waiting;
        private float cooldownRemaining;
        private float contactDamageTimer;

        public bool IsKnockbackImmune => phase == DashPhase.Dashing;

        public DashOnlyBehavior(DashOnlyBehaviorDefinition definition)
        {
            this.definition = definition ??
                throw new ArgumentNullException(nameof(definition));
            dashMotion = new DashMotion(definition.Dash);

            // Tier 1 can dash immediately after spawning. Each subsequent
            // cooldown starts when its dash starts, not when movement ends.
            cooldownRemaining = 0f;
            contactDamageTimer = definition.ContactDamageCooldown;
        }

        public void Update(
            Enemy enemy,
            GameTime gameTime,
            EnemyUpdateContext context)
        {
            float deltaTime =
                (float)gameTime.ElapsedGameTime.TotalSeconds;
            contactDamageTimer += deltaTime;
            cooldownRemaining = MathF.Max(
                0f,
                cooldownRemaining - deltaTime);

            if (phase == DashPhase.Dashing)
            {
                UpdateDash(enemy, gameTime, context, deltaTime);
                return;
            }

            UpdateWaiting(enemy, gameTime, context);
        }

        private void UpdateWaiting(
            Enemy enemy,
            GameTime gameTime,
            EnemyUpdateContext context)
        {
            enemy.Motor.Stop();
            enemy.ChangeState(EnemyState.Idle);
            enemy.SetAnimation(definition.IdleAnimation);
            enemy.UpdateAnimation(gameTime);
            TryApplyContactDamage(enemy, context);

            if (cooldownRemaining > 0f)
                return;

            Vector2 direction =
                context.Target.Hurtbox.Center.ToVector2() -
                enemy.Hurtbox.Center.ToVector2();
            BeginDash(enemy, direction);
        }

        private void UpdateDash(
            Enemy enemy,
            GameTime gameTime,
            EnemyUpdateContext context,
            float deltaTime)
        {
            enemy.ChangeState(EnemyState.Attacking);
            enemy.SetFacingDirection(dashMotion.Direction);

            bool intersectsPlayer =
                context.Target.Hurtbox.Intersects(
                    enemy.ContactHitbox);
            DashEndReason endReason = DashEndReason.None;

            if (!intersectsPlayer)
            {
                endReason = dashMotion.Update(
                    deltaTime,
                    delta => enemy.Motor.MoveDelta(
                        enemy,
                        delta,
                    context),
                    () => context.Target.Hurtbox.Intersects(
                        enemy.ContactHitbox));
                intersectsPlayer =
                    context.Target.Hurtbox.Intersects(
                        enemy.ContactHitbox);
            }
            else
            {
                dashMotion.Stop();
                endReason = DashEndReason.Interrupted;
            }

            if (intersectsPlayer)
                TryApplyContactDamage(enemy, context);

            if (endReason != DashEndReason.None)
            {
                FinishDash(enemy);
                return;
            }

            enemy.SetAnimation(definition.RunAnimation);
            enemy.UpdateAnimation(gameTime);
        }

        private void BeginDash(Enemy enemy, Vector2 direction)
        {
            enemy.Motor.Stop();
            dashMotion.Begin(direction);
            phase = DashPhase.Dashing;
            cooldownRemaining = definition.Dash.Cooldown;
            enemy.ChangeState(EnemyState.Attacking);
            enemy.SetFacingDirection(dashMotion.Direction);
            enemy.SetAnimation(definition.RunAnimation);
        }

        private void FinishDash(Enemy enemy)
        {
            dashMotion.Stop();
            enemy.Motor.Stop();
            phase = DashPhase.Waiting;
            enemy.ChangeState(EnemyState.Idle);
            enemy.SetAnimation(definition.IdleAnimation);
        }

        private void TryApplyContactDamage(
            Enemy enemy,
            EnemyUpdateContext context)
        {
            if (definition.ContactDamage <= 0 ||
                contactDamageTimer < definition.ContactDamageCooldown ||
                !context.Target.Hurtbox.Intersects(
                    enemy.ContactHitbox))
            {
                return;
            }

            context.RequestPlayerDamage(new CombatHit(
                definition.ContactDamage,
                DamageType.Physical,
                CombatFaction.Enemy,
                enemy.Definition.Id,
                enemy.ContactHitbox.Center.ToVector2(),
                definition.ContactKnockback));
            contactDamageTimer = 0f;
        }
    }
}
