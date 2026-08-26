using System;
using Microsoft.Xna.Framework;

namespace _1_2D_Top_Down
{
    /// <summary>
    /// Resolves enemy movement against the map one axis at a time. Behaviors
    /// choose direction and speed; the motor owns physical movement.
    /// </summary>
    public sealed class EnemyMotor
    {
        private const float VelocityEpsilon = 0.01f;

        public Vector2 DesiredDirection { get; private set; }
        public Vector2 Velocity { get; private set; }
        public EnemyMovementMode MovementMode { get; private set; }
        public bool IsMoving { get; private set; }
        public float Speed => Velocity.Length();

        public void Stop()
        {
            DesiredDirection = Vector2.Zero;
            Velocity = Vector2.Zero;
            IsMoving = false;
        }

        public void ResetSteering()
        {
            Velocity = Vector2.Zero;
        }

        public void Move(
            Enemy enemy,
            Vector2 direction,
            float speed,
            EnemyMovementMode mode,
            float deltaTime,
            EnemyUpdateContext context)
        {
            if (direction == Vector2.Zero || speed <= 0f || deltaTime <= 0f)
            {
                Stop();
                return;
            }

            direction.Normalize();
            ResetSteering();
            DesiredDirection = direction;
            MovementMode = mode;
            IsMoving = true;
            MoveDelta(enemy, direction * speed * deltaTime, context);
        }

        /// <summary>
        /// Moves using persistent velocity. Direction, speed and stopping are
        /// approached gradually according to the supplied steering tuning.
        /// </summary>
        public bool MoveSteered(
            Enemy enemy,
            Vector2 desiredDirection,
            float targetSpeed,
            EnemyMovementMode mode,
            EnemySteeringDefinition steering,
            float deltaTime,
            EnemyUpdateContext context,
            Func<bool> stopCondition = null)
        {
            if (steering == null)
                throw new ArgumentNullException(nameof(steering));
            if (deltaTime <= 0f)
                return false;

            bool hasDesiredDirection =
                desiredDirection.LengthSquared() >
                VelocityEpsilon * VelocityEpsilon;
            if (hasDesiredDirection)
                desiredDirection.Normalize();
            else
                desiredDirection = Vector2.Zero;

            DesiredDirection = desiredDirection;
            MovementMode = mode;
            targetSpeed = MathHelper.Clamp(
                targetSpeed,
                0f,
                steering.MaxSpeed);

            float currentSpeed = Velocity.Length();
            Vector2 currentDirection = currentSpeed > VelocityEpsilon
                ? Velocity / currentSpeed
                : desiredDirection;

            if (!hasDesiredDirection || targetSpeed <= 0f)
            {
                currentSpeed = MoveTowards(
                    currentSpeed,
                    0f,
                    steering.Deceleration * deltaTime);
            }
            else
            {
                currentDirection = currentSpeed <= VelocityEpsilon
                    ? desiredDirection
                    : RotateTowards(
                        currentDirection,
                        desiredDirection,
                        steering.TurnSpeedRadians * deltaTime);

                float speedChange = targetSpeed >= currentSpeed
                    ? steering.Acceleration
                    : steering.Deceleration;
                currentSpeed = MoveTowards(
                    currentSpeed,
                    targetSpeed,
                    speedChange * deltaTime);
            }

            if (currentSpeed <= VelocityEpsilon ||
                currentDirection == Vector2.Zero)
            {
                Velocity = Vector2.Zero;
                IsMoving = false;
                return false;
            }

            Velocity = currentDirection * currentSpeed;
            IsMoving = true;
            return MoveSteeringVelocity(
                enemy,
                steering,
                deltaTime,
                context,
                stopCondition);
        }

        public bool MoveDelta(
            Enemy enemy,
            Vector2 delta,
            EnemyUpdateContext context)
        {
            bool movedX = TryMoveAxis(
                enemy,
                new Vector2(delta.X, 0f),
                context);
            bool movedY = TryMoveAxis(
                enemy,
                new Vector2(0f, delta.Y),
                context);
            return movedX && movedY;
        }

        public bool TryTeleport(
            Enemy enemy,
            Vector2 destinationCenter,
            EnemyUpdateContext context)
        {
            Vector2 previousPosition = enemy.Position;
            enemy.Position +=
                destinationCenter -
                enemy.MovementBounds.Center.ToVector2();
            KeepInsideWorld(enemy, context.WorldBounds);

            if (context.IntersectsMapCollision(enemy.MovementBounds))
            {
                enemy.Position = previousPosition;
                return false;
            }

            Stop();
            return true;
        }

        private bool MoveSteeringVelocity(
            Enemy enemy,
            EnemySteeringDefinition steering,
            float deltaTime,
            EnemyUpdateContext context,
            Func<bool> stopCondition)
        {
            float totalDistance = Velocity.Length() * deltaTime;
            int steps = Math.Max(
                1,
                (int)MathF.Ceiling(
                    totalDistance / steering.MaximumMovementStep));
            float stepTime = deltaTime / steps;

            for (int i = 0; i < steps; i++)
            {
                Vector2 step = Velocity * stepTime;
                bool movedX = TryMoveAxis(
                    enemy,
                    new Vector2(step.X, 0f),
                    context);
                bool movedY = TryMoveAxis(
                    enemy,
                    new Vector2(0f, step.Y),
                    context);

                // Fast attacks can inspect every movement substep, avoiding
                // tunnelling through the player at low frame rates.
                if (stopCondition != null && stopCondition())
                    return false;

                if (movedX && movedY)
                    continue;

                if (steering.CollisionResponse ==
                    EnemySteeringCollisionResponse.Stop)
                {
                    Velocity = Vector2.Zero;
                    IsMoving = false;
                    return true;
                }

                if (!movedX)
                    Velocity = new Vector2(0f, Velocity.Y);
                if (!movedY)
                    Velocity = new Vector2(Velocity.X, 0f);

                if (Velocity.LengthSquared() <=
                    VelocityEpsilon * VelocityEpsilon)
                {
                    Velocity = Vector2.Zero;
                    IsMoving = false;
                    return true;
                }
            }

            return false;
        }

        private static bool TryMoveAxis(
            Enemy enemy,
            Vector2 delta,
            EnemyUpdateContext context)
        {
            if (delta == Vector2.Zero)
                return true;

            Vector2 previousPosition = enemy.Position;
            enemy.Position += delta;
            KeepInsideWorld(enemy, context.WorldBounds);

            if (context.IntersectsMapCollision(enemy.MovementBounds))
            {
                enemy.Position = previousPosition;
                return false;
            }

            Vector2 actualDelta = enemy.Position - previousPosition;
            return Vector2.DistanceSquared(actualDelta, delta) <= 0.01f;
        }

        private static float MoveTowards(
            float current,
            float target,
            float maximumDelta)
        {
            if (MathF.Abs(target - current) <= maximumDelta)
                return target;

            return current + MathF.Sign(target - current) * maximumDelta;
        }

        private static Vector2 RotateTowards(
            Vector2 currentDirection,
            Vector2 desiredDirection,
            float maximumRadians)
        {
            float currentAngle = MathF.Atan2(
                currentDirection.Y,
                currentDirection.X);
            float desiredAngle = MathF.Atan2(
                desiredDirection.Y,
                desiredDirection.X);
            float difference = MathHelper.WrapAngle(
                desiredAngle - currentAngle);
            float rotation = MathHelper.Clamp(
                difference,
                -maximumRadians,
                maximumRadians);
            float resultAngle = currentAngle + rotation;

            return new Vector2(
                MathF.Cos(resultAngle),
                MathF.Sin(resultAngle));
        }

        private static void KeepInsideWorld(Enemy enemy, Rectangle worldBounds)
        {
            Rectangle bounds = enemy.MovementBounds;
            if (bounds.Left < worldBounds.Left)
                enemy.Position.X += worldBounds.Left - bounds.Left;
            else if (bounds.Right > worldBounds.Right)
                enemy.Position.X -= bounds.Right - worldBounds.Right;

            bounds = enemy.MovementBounds;
            if (bounds.Top < worldBounds.Top)
                enemy.Position.Y += worldBounds.Top - bounds.Top;
            else if (bounds.Bottom > worldBounds.Bottom)
                enemy.Position.Y -= bounds.Bottom - worldBounds.Bottom;
        }
    }
}
