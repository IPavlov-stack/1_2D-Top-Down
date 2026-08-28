using Microsoft.Xna.Framework;
using System;

namespace _1_2D_Top_Down
{
    /// <summary>
    /// Immutable snapshot of one melee swing. The attack is represented by a
    /// forward-facing arc and can therefore hit wide targets without reaching
    /// enemies standing behind the attacker.
    /// </summary>
    public readonly struct MeleeAttack
    {
        public Vector2 Origin { get; }
        public Vector2 Direction { get; }
        public float Range { get; }
        public float ArcDegrees { get; }
        public CombatHit Hit { get; }

        public Rectangle BroadphaseBounds
        {
            get
            {
                int radius = Math.Max(1, (int)MathF.Ceiling(Range));
                return new Rectangle(
                    (int)MathF.Floor(Origin.X) - radius,
                    (int)MathF.Floor(Origin.Y) - radius,
                    radius * 2,
                    radius * 2);
            }
        }

        public MeleeAttack(
            Vector2 origin,
            Vector2 direction,
            float range,
            float arcDegrees,
            CombatHit hit)
        {
            if (direction == Vector2.Zero)
                throw new ArgumentException(
                    "A melee attack requires a direction.",
                    nameof(direction));

            direction.Normalize();
            Origin = origin;
            Direction = direction;
            Range = MathF.Max(0f, range);
            ArcDegrees = MathHelper.Clamp(arcDegrees, 0f, 360f);
            Hit = hit;
        }

        public bool Intersects(Rectangle target)
        {
            float closestX = MathHelper.Clamp(
                Origin.X,
                target.Left,
                target.Right);
            float closestY = MathHelper.Clamp(
                Origin.Y,
                target.Top,
                target.Bottom);
            Vector2 closestOffset =
                new Vector2(closestX, closestY) - Origin;

            if (closestOffset.LengthSquared() > Range * Range)
                return false;

            Vector2 targetOffset = target.Center.ToVector2() - Origin;

            // Overlapping targets are always inside the swing.
            if (targetOffset == Vector2.Zero || target.Contains(Origin))
                return true;

            targetOffset.Normalize();
            float halfArcRadians = MathHelper.ToRadians(ArcDegrees * 0.5f);
            float minimumDot = MathF.Cos(halfArcRadians);
            return Vector2.Dot(Direction, targetOffset) >= minimumDot;
        }
    }
}
