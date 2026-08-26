using Microsoft.Xna.Framework;

namespace _1_2D_Top_Down
{
    /// <summary>
    /// Resolves enemy movement against the map one axis at a time. Behaviors
    /// choose direction and speed; the motor owns physical movement.
    /// </summary>
    public sealed class EnemyMotor
    {
        public Vector2 DesiredDirection { get; private set; }
        public EnemyMovementMode MovementMode { get; private set; }
        public bool IsMoving { get; private set; }

        public void Stop()
        {
            DesiredDirection = Vector2.Zero;
            IsMoving = false;
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
            DesiredDirection = direction;
            MovementMode = mode;
            IsMoving = true;
            MoveDelta(enemy, direction * speed * deltaTime, context);
        }

        public void MoveDelta(
            Enemy enemy,
            Vector2 delta,
            EnemyUpdateContext context)
        {
            TryMoveAxis(enemy, new Vector2(delta.X, 0f), context);
            TryMoveAxis(enemy, new Vector2(0f, delta.Y), context);
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

        private static void TryMoveAxis(
            Enemy enemy,
            Vector2 delta,
            EnemyUpdateContext context)
        {
            if (delta == Vector2.Zero)
                return;

            Vector2 previousPosition = enemy.Position;
            enemy.Position += delta;
            KeepInsideWorld(enemy, context.WorldBounds);

            if (context.IntersectsMapCollision(enemy.MovementBounds))
                enemy.Position = previousPosition;
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
