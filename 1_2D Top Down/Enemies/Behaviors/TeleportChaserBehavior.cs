using System;
using Microsoft.Xna.Framework;

namespace _1_2D_Top_Down
{
    /// <summary>
    /// Distance-based walk/run pursuit with a readable fade teleport to the
    /// side opposite the enemy's current position relative to the player.
    /// </summary>
    public sealed class TeleportChaserBehavior : IEnemyBehavior
    {
        private enum TeleportPhase
        {
            Chasing,
            Attacking,
            FadingOut,
            FadingIn
        }

        private static readonly float[] CandidateAngles =
        {
            0f,
            MathHelper.PiOver4 * 0.65f,
            -MathHelper.PiOver4 * 0.65f,
            MathHelper.PiOver4 * 1.25f,
            -MathHelper.PiOver4 * 1.25f
        };

        private readonly TeleportChaserBehaviorDefinition definition;

        private TeleportPhase phase = TeleportPhase.Chasing;
        private float phaseTimer;
        private float teleportCooldownTimer;
        private float runLockoutTimer;
        private float contactDamageTimer;
        private bool attackDamageRequested;
        private Vector2 capturedPlayerToGhost = Vector2.UnitX;

        public TeleportChaserBehavior(
            TeleportChaserBehaviorDefinition definition)
        {
            this.definition = definition ??
                throw new ArgumentNullException(nameof(definition));

            // Tier 1 can use its ability as soon as it first reaches range.
            teleportCooldownTimer = definition.TeleportCooldown;
            contactDamageTimer = definition.ContactDamageCooldown;
        }

        public void Update(
            Enemy enemy,
            GameTime gameTime,
            EnemyUpdateContext context)
        {
            float deltaTime =
                (float)gameTime.ElapsedGameTime.TotalSeconds;

            teleportCooldownTimer += deltaTime;
            contactDamageTimer += deltaTime;
            runLockoutTimer = MathF.Max(
                0f,
                runLockoutTimer - deltaTime);

            switch (phase)
            {
                case TeleportPhase.Chasing:
                    UpdateChase(enemy, gameTime, context, deltaTime);
                    break;

                case TeleportPhase.Attacking:
                    UpdateAttack(enemy, gameTime, context, deltaTime);
                    break;

                case TeleportPhase.FadingOut:
                    UpdateFadeOut(enemy, gameTime, context, deltaTime);
                    break;

                case TeleportPhase.FadingIn:
                    UpdateFadeIn(enemy, gameTime, deltaTime);
                    break;
            }
        }

        private void UpdateChase(
            Enemy enemy,
            GameTime gameTime,
            EnemyUpdateContext context,
            float deltaTime)
        {
            Vector2 targetCenter =
                context.Target.Hurtbox.Center.ToVector2();
            Vector2 ghostCenter = enemy.Hurtbox.Center.ToVector2();
            Vector2 direction = targetCenter - ghostCenter;
            float distance = direction.Length();

            enemy.SetRenderOpacity(1f);
            enemy.SetFacingDirection(direction);

            if (distance <= definition.TeleportTriggerDistance &&
                teleportCooldownTimer >= definition.TeleportCooldown)
            {
                BeginTeleport(enemy, targetCenter, ghostCenter);
                return;
            }

            bool canRun = runLockoutTimer <= 0f;
            bool shouldRun =
                canRun && distance <= definition.RunTriggerDistance;
            float speed = shouldRun
                ? enemy.Definition.Locomotion.RunSpeed
                : enemy.Definition.Locomotion.WalkSpeed;

            enemy.ChangeState(EnemyState.Chasing);
            enemy.SetAnimation(shouldRun
                ? definition.RunAnimation
                : definition.WalkAnimation);

            if (context.Target.Hurtbox.Intersects(enemy.Hurtbox))
            {
                TryApplyContactDamage(enemy, context);
                BeginAttack(enemy);
                return;
            }

            if (direction != Vector2.Zero)
            {
                enemy.Motor.MoveSteered(
                    enemy,
                    direction,
                    speed,
                    shouldRun
                        ? EnemyMovementMode.Run
                        : EnemyMovementMode.Walk,
                    shouldRun
                        ? definition.RunSteering
                        : definition.WalkSteering,
                    deltaTime,
                    context,
                    () => context.Target.Hurtbox.Intersects(
                        enemy.Hurtbox));
            }
            else
            {
                enemy.Motor.MoveSteered(
                    enemy,
                    Vector2.Zero,
                    0f,
                    EnemyMovementMode.Walk,
                    definition.WalkSteering,
                    deltaTime,
                    context);
            }

            enemy.UpdateAnimation(gameTime);

            if (context.Target.Hurtbox.Intersects(enemy.Hurtbox))
            {
                TryApplyContactDamage(enemy, context);
                BeginAttack(enemy);
            }
        }

        private void UpdateAttack(
            Enemy enemy,
            GameTime gameTime,
            EnemyUpdateContext context,
            float deltaTime)
        {
            phaseTimer += deltaTime;
            enemy.Motor.Stop();
            enemy.ChangeState(EnemyState.Attacking);
            enemy.SetAnimation(definition.AttackAnimation);
            enemy.UpdateAnimation(gameTime, loop: false);

            // Remaining on top of the ghost is dangerous even before or
            // after the dedicated attack release frame.
            TryApplyContactDamage(enemy, context);

            if (!attackDamageRequested &&
                phaseTimer >= definition.DamageReleaseTime)
            {
                attackDamageRequested = true;
                TryApplyContactDamage(enemy, context);
            }

            if (phaseTimer < definition.AttackDuration)
                return;

            phase = TeleportPhase.Chasing;
            phaseTimer = 0f;
        }

        private void UpdateFadeOut(
            Enemy enemy,
            GameTime gameTime,
            EnemyUpdateContext context,
            float deltaTime)
        {
            phaseTimer += deltaTime;
            enemy.Motor.Stop();
            enemy.ChangeState(EnemyState.Teleporting);
            enemy.SetAnimation(definition.IdleAnimation);
            enemy.UpdateAnimation(gameTime);

            float progress = MathHelper.Clamp(
                phaseTimer / definition.FadeOutDuration,
                0f,
                1f);
            enemy.SetRenderOpacity(
                MathHelper.SmoothStep(1f, 0f, progress));

            if (progress < 1f)
                return;

            TryTeleportBehind(enemy, context);
            enemy.SetRenderOpacity(0f);
            phase = TeleportPhase.FadingIn;
            phaseTimer = 0f;
            runLockoutTimer = definition.PostTeleportRunLockout;
            teleportCooldownTimer = 0f;
        }

        private void UpdateFadeIn(
            Enemy enemy,
            GameTime gameTime,
            float deltaTime)
        {
            phaseTimer += deltaTime;
            enemy.Motor.Stop();
            enemy.ChangeState(EnemyState.Teleporting);
            enemy.SetAnimation(definition.IdleAnimation);
            enemy.UpdateAnimation(gameTime);

            float progress = MathHelper.Clamp(
                phaseTimer / definition.FadeInDuration,
                0f,
                1f);
            enemy.SetRenderOpacity(
                MathHelper.SmoothStep(0f, 1f, progress));

            if (progress < 1f)
                return;

            enemy.SetRenderOpacity(1f);
            phase = TeleportPhase.Chasing;
            phaseTimer = 0f;
        }

        private void BeginTeleport(
            Enemy enemy,
            Vector2 playerCenter,
            Vector2 ghostCenter)
        {
            capturedPlayerToGhost = ghostCenter - playerCenter;
            if (capturedPlayerToGhost == Vector2.Zero)
                capturedPlayerToGhost = Vector2.UnitX;
            else
                capturedPlayerToGhost.Normalize();

            phase = TeleportPhase.FadingOut;
            phaseTimer = 0f;
            enemy.Motor.Stop();
            enemy.ChangeState(EnemyState.Teleporting);
            enemy.SetAnimation(definition.IdleAnimation);
        }

        private void BeginAttack(Enemy enemy)
        {
            phase = TeleportPhase.Attacking;
            phaseTimer = 0f;
            attackDamageRequested = false;
            enemy.Motor.Stop();
            enemy.ChangeState(EnemyState.Attacking);
            enemy.SetAnimation(definition.AttackAnimation);
        }

        private void TryApplyContactDamage(
            Enemy enemy,
            EnemyUpdateContext context)
        {
            if (definition.ContactDamage <= 0 ||
                contactDamageTimer < definition.ContactDamageCooldown ||
                !context.Target.Hurtbox.Intersects(enemy.Hurtbox))
            {
                return;
            }

            context.RequestPlayerDamage(new CombatHit(
                definition.ContactDamage,
                DamageType.Physical,
                CombatFaction.Enemy,
                enemy.Definition.Id,
                enemy.Hurtbox.Center.ToVector2(),
                definition.ContactKnockback));
            contactDamageTimer = 0f;
        }

        private void TryTeleportBehind(
            Enemy enemy,
            EnemyUpdateContext context)
        {
            Vector2 playerCenter =
                context.Target.Hurtbox.Center.ToVector2();
            float[] distanceScales = { 1f, 0.80f, 0.62f };

            foreach (float distanceScale in distanceScales)
            {
                foreach (float angle in CandidateAngles)
                {
                    Vector2 sideDirection = Rotate(
                        capturedPlayerToGhost,
                        angle);
                    Vector2 destination = playerCenter -
                        sideDirection *
                        definition.TeleportExitDistance * distanceScale;

                    if (enemy.Motor.TryTeleport(
                        enemy,
                        destination,
                        context))
                    {
                        enemy.SetFacingDirection(
                            playerCenter -
                            enemy.Hurtbox.Center.ToVector2());
                        return;
                    }
                }
            }
        }

        private static Vector2 Rotate(Vector2 vector, float angle)
        {
            float cosine = MathF.Cos(angle);
            float sine = MathF.Sin(angle);
            return new Vector2(
                vector.X * cosine - vector.Y * sine,
                vector.X * sine + vector.Y * cosine);
        }
    }
}
