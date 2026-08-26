using System;
using Microsoft.Xna.Framework;

namespace _1_2D_Top_Down
{
    /// <summary>
    /// A committed sprint behavior with readable counterplay: the enemy first
    /// homes while accelerating, then loses steering at maximum speed.
    /// </summary>
    public sealed class ChargerBehavior : IEnemyBehavior
    {
        private const float MaximumSpeedDifference = 0.5f;

        private enum ChargePhase
        {
            Chasing,
            Accelerating,
            Locked,
            Recovering
        }

        private readonly ChargerBehaviorDefinition definition;

        private ChargePhase phase = ChargePhase.Chasing;
        private float cooldownTimer;
        private float lockedTimer;
        private float recoveryTimer;
        private float normalContactDamageTimer;
        private Vector2 lockedDirection = Vector2.UnitY;

        public ChargerBehavior(ChargerBehaviorDefinition definition)
        {
            this.definition = definition ??
                throw new ArgumentNullException(nameof(definition));

            // The first charge is ready as soon as the player enters range.
            cooldownTimer = definition.ChargeCooldown;
            normalContactDamageTimer =
                definition.NormalContactDamageCooldown;
        }

        public void Update(
            Enemy enemy,
            GameTime gameTime,
            EnemyUpdateContext context)
        {
            float deltaTime =
                (float)gameTime.ElapsedGameTime.TotalSeconds;
            normalContactDamageTimer += deltaTime;
            Vector2 targetDirection =
                context.Target.Hurtbox.Center.ToVector2() -
                enemy.Hurtbox.Center.ToVector2();

            switch (phase)
            {
                case ChargePhase.Chasing:
                    UpdateChase(
                        enemy,
                        gameTime,
                        context,
                        targetDirection,
                        deltaTime);
                    break;

                case ChargePhase.Accelerating:
                    UpdateAcceleration(
                        enemy,
                        gameTime,
                        context,
                        targetDirection,
                        deltaTime);
                    break;

                case ChargePhase.Locked:
                    UpdateLockedCharge(
                        enemy,
                        gameTime,
                        context,
                        deltaTime);
                    break;

                case ChargePhase.Recovering:
                    UpdateRecovery(
                        enemy,
                        gameTime,
                        context,
                        deltaTime);
                    break;
            }
        }

        private void UpdateChase(
            Enemy enemy,
            GameTime gameTime,
            EnemyUpdateContext context,
            Vector2 direction,
            float deltaTime)
        {
            cooldownTimer += deltaTime;
            enemy.ChangeState(EnemyState.Chasing);
            enemy.SetFacingDirection(direction);

            if (direction != Vector2.Zero)
            {
                enemy.Motor.MoveSteered(
                    enemy,
                    direction,
                    enemy.Definition.Locomotion.WalkSpeed,
                    EnemyMovementMode.Walk,
                    definition.ChaseSteering,
                    deltaTime,
                    context);
            }
            else
            {
                enemy.Motor.MoveSteered(
                    enemy,
                    Vector2.Zero,
                    0f,
                    EnemyMovementMode.Walk,
                    definition.ChaseSteering,
                    deltaTime,
                    context);
            }

            enemy.SetAnimation(definition.WalkAnimation);
            enemy.UpdateAnimation(gameTime);
            TryApplyNormalContactDamage(enemy, context);

            float distanceSquared = direction.LengthSquared();
            float minimumDistanceSquared =
                definition.MinimumChargeDistance *
                definition.MinimumChargeDistance;
            float triggerDistanceSquared =
                definition.TriggerDistance *
                definition.TriggerDistance;

            if (distanceSquared < minimumDistanceSquared ||
                distanceSquared > triggerDistanceSquared ||
                cooldownTimer < definition.ChargeCooldown)
            {
                return;
            }

            phase = ChargePhase.Accelerating;
            enemy.ChangeState(EnemyState.Attacking);
            enemy.SetAnimation(definition.RunAnimation);
        }

        private void UpdateAcceleration(
            Enemy enemy,
            GameTime gameTime,
            EnemyUpdateContext context,
            Vector2 direction,
            float deltaTime)
        {
            enemy.ChangeState(EnemyState.Attacking);
            enemy.SetFacingDirection(direction);
            enemy.SetAnimation(definition.RunAnimation);

            bool hitPlayer = context.Target.Hurtbox.Intersects(
                enemy.Hurtbox);
            bool blocked = false;
            if (!hitPlayer)
            {
                blocked = enemy.Motor.MoveSteered(
                    enemy,
                    direction,
                    enemy.Definition.Locomotion.RunSpeed,
                    EnemyMovementMode.Run,
                    definition.ChargeSteering,
                    deltaTime,
                    context,
                    () => context.Target.Hurtbox.Intersects(
                        enemy.Hurtbox));
                hitPlayer = context.Target.Hurtbox.Intersects(
                    enemy.Hurtbox);
            }
            enemy.UpdateAnimation(gameTime);

            if (hitPlayer)
            {
                HitPlayer(enemy, context);
                BeginRecovery(enemy);
                return;
            }

            if (blocked)
            {
                BeginRecovery(enemy);
                return;
            }

            float maximumChargeSpeed = MathF.Min(
                enemy.Definition.Locomotion.RunSpeed,
                definition.ChargeSteering.MaxSpeed);
            if (enemy.Motor.Speed + MaximumSpeedDifference <
                maximumChargeSpeed)
            {
                return;
            }

            lockedDirection = enemy.Motor.Velocity;
            if (lockedDirection == Vector2.Zero)
                lockedDirection = Vector2.UnitY;
            else
                lockedDirection.Normalize();

            phase = ChargePhase.Locked;
            lockedTimer = 0f;
        }

        private void UpdateLockedCharge(
            Enemy enemy,
            GameTime gameTime,
            EnemyUpdateContext context,
            float deltaTime)
        {
            lockedTimer += deltaTime;
            enemy.ChangeState(EnemyState.Attacking);
            enemy.SetFacingDirection(lockedDirection);
            enemy.SetAnimation(definition.RunAnimation);

            bool hitPlayer = context.Target.Hurtbox.Intersects(
                enemy.Hurtbox);
            bool blocked = false;
            if (!hitPlayer)
            {
                blocked = enemy.Motor.MoveSteered(
                    enemy,
                    lockedDirection,
                    enemy.Definition.Locomotion.RunSpeed,
                    EnemyMovementMode.Run,
                    definition.ChargeSteering,
                    deltaTime,
                    context,
                    () => context.Target.Hurtbox.Intersects(
                        enemy.Hurtbox));
                hitPlayer = context.Target.Hurtbox.Intersects(
                    enemy.Hurtbox);
            }
            enemy.UpdateAnimation(gameTime);

            if (hitPlayer)
            {
                HitPlayer(enemy, context);
                BeginRecovery(enemy);
                return;
            }

            if (blocked ||
                lockedTimer >= definition.LockedChargeDuration)
            {
                BeginRecovery(enemy);
            }
        }

        private void UpdateRecovery(
            Enemy enemy,
            GameTime gameTime,
            EnemyUpdateContext context,
            float deltaTime)
        {
            recoveryTimer += deltaTime;
            enemy.Motor.MoveSteered(
                enemy,
                Vector2.Zero,
                0f,
                EnemyMovementMode.Run,
                definition.ChargeSteering,
                deltaTime,
                context);
            enemy.ChangeState(EnemyState.Idle);
            enemy.SetAnimation(enemy.Motor.IsMoving
                ? definition.RunAnimation
                : definition.IdleAnimation);
            enemy.UpdateAnimation(gameTime);
            TryApplyNormalContactDamage(enemy, context);

            if (recoveryTimer < definition.RecoveryDuration)
                return;

            phase = ChargePhase.Chasing;
            cooldownTimer = 0f;
            lockedTimer = 0f;
        }

        private void HitPlayer(
            Enemy enemy,
            EnemyUpdateContext context)
        {
            context.RequestPlayerDamage(new CombatHit(
                definition.ContactDamage,
                DamageType.Physical,
                CombatFaction.Enemy,
                enemy.Definition.Id,
                enemy.Hurtbox.Center.ToVector2(),
                definition.ContactKnockback));

            // Prevent the following recovery frame from immediately adding a
            // second, weaker contact hit on top of the charge impact.
            normalContactDamageTimer = 0f;
        }

        private void TryApplyNormalContactDamage(
            Enemy enemy,
            EnemyUpdateContext context)
        {
            if (definition.NormalContactDamage <= 0 ||
                normalContactDamageTimer <
                    definition.NormalContactDamageCooldown ||
                !context.Target.Hurtbox.Intersects(enemy.Hurtbox))
            {
                return;
            }

            context.RequestPlayerDamage(new CombatHit(
                definition.NormalContactDamage,
                DamageType.Physical,
                CombatFaction.Enemy,
                enemy.Definition.Id,
                enemy.Hurtbox.Center.ToVector2()));
            normalContactDamageTimer = 0f;
        }

        private void BeginRecovery(Enemy enemy)
        {
            phase = ChargePhase.Recovering;
            recoveryTimer = 0f;
            enemy.ChangeState(EnemyState.Idle);
            enemy.SetAnimation(enemy.Motor.IsMoving
                ? definition.RunAnimation
                : definition.IdleAnimation);
        }
    }
}
