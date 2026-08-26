using System;
using Microsoft.Xna.Framework;

namespace _1_2D_Top_Down
{
    public enum EnemySteeringCollisionResponse
    {
        /// <summary>Cancel all steering velocity on impact.</summary>
        Stop,

        /// <summary>Cancel only the blocked axis and keep sliding.</summary>
        Slide
    }

    /// <summary>
    /// Reusable tuning for velocity-based enemy motion. Speeds are expressed
    /// in world pixels and turn speed in degrees per second for readability.
    /// </summary>
    public sealed class EnemySteeringDefinition
    {
        public float Acceleration { get; }
        public float Deceleration { get; }
        public float MaxSpeed { get; }
        public float TurnSpeedDegrees { get; }
        public float TurnSpeedRadians { get; }
        public float MaximumMovementStep { get; }
        public EnemySteeringCollisionResponse CollisionResponse { get; }

        public EnemySteeringDefinition(
            float acceleration,
            float deceleration,
            float maxSpeed,
            float turnSpeedDegrees,
            float maximumMovementStep = 8f,
            EnemySteeringCollisionResponse collisionResponse =
                EnemySteeringCollisionResponse.Slide)
        {
            if (acceleration <= 0f)
                throw new ArgumentOutOfRangeException(nameof(acceleration));
            if (deceleration <= 0f)
                throw new ArgumentOutOfRangeException(nameof(deceleration));
            if (maxSpeed <= 0f)
                throw new ArgumentOutOfRangeException(nameof(maxSpeed));
            if (turnSpeedDegrees <= 0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(turnSpeedDegrees));
            }
            if (maximumMovementStep <= 0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maximumMovementStep));
            }

            Acceleration = acceleration;
            Deceleration = deceleration;
            MaxSpeed = maxSpeed;
            TurnSpeedDegrees = turnSpeedDegrees;
            TurnSpeedRadians = MathHelper.ToRadians(turnSpeedDegrees);
            MaximumMovementStep = maximumMovementStep;
            CollisionResponse = collisionResponse;
        }
    }
}
