using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace _1_2D_Top_Down
{
    /// <summary>
    /// A static rectangular or polygonal obstacle used by the map collision grid.
    /// Polygons may be convex or concave, but must not self-intersect.
    /// </summary>
    public sealed class StaticCollisionShape
    {
        private const float Epsilon = 0.0001f;
        private readonly IReadOnlyList<Vector2> vertices;

        public Rectangle Bounds { get; }
        public bool IsPolygon => vertices != null;
        public IReadOnlyList<Vector2> Vertices => vertices ?? Array.Empty<Vector2>();

        private StaticCollisionShape(Rectangle bounds, IReadOnlyList<Vector2> vertices)
        {
            Bounds = bounds;
            this.vertices = vertices;
        }

        public static StaticCollisionShape FromRectangle(Rectangle rectangle) =>
            new(rectangle, null);

        public static StaticCollisionShape FromPolygon(IEnumerable<Vector2> points)
        {
            Vector2[] polygon = points?.ToArray()
                ?? throw new ArgumentNullException(nameof(points));
            if (polygon.Length < 3)
                throw new ArgumentException("A collision polygon needs at least three points.", nameof(points));

            float minX = polygon.Min(point => point.X);
            float minY = polygon.Min(point => point.Y);
            float maxX = polygon.Max(point => point.X);
            float maxY = polygon.Max(point => point.Y);
            Rectangle bounds = new(
                (int)MathF.Floor(minX),
                (int)MathF.Floor(minY),
                Math.Max(1, (int)MathF.Ceiling(maxX) - (int)MathF.Floor(minX)),
                Math.Max(1, (int)MathF.Ceiling(maxY) - (int)MathF.Floor(minY)));

            return new StaticCollisionShape(bounds, polygon);
        }

        public bool Intersects(Rectangle rectangle)
        {
            if (!Bounds.Intersects(rectangle))
                return false;
            if (!IsPolygon)
                return true;

            foreach (Vector2 vertex in vertices)
            {
                if (ContainsInclusive(rectangle, vertex))
                    return true;
            }

            Vector2[] corners =
            {
                new(rectangle.Left, rectangle.Top),
                new(rectangle.Right, rectangle.Top),
                new(rectangle.Right, rectangle.Bottom),
                new(rectangle.Left, rectangle.Bottom)
            };

            foreach (Vector2 corner in corners)
            {
                if (ContainsPoint(corner))
                    return true;
            }

            for (int polygonIndex = 0; polygonIndex < vertices.Count; polygonIndex++)
            {
                Vector2 polygonStart = vertices[polygonIndex];
                Vector2 polygonEnd = vertices[(polygonIndex + 1) % vertices.Count];
                for (int rectangleIndex = 0; rectangleIndex < corners.Length; rectangleIndex++)
                {
                    Vector2 rectangleStart = corners[rectangleIndex];
                    Vector2 rectangleEnd = corners[(rectangleIndex + 1) % corners.Length];
                    if (SegmentsIntersect(
                        polygonStart,
                        polygonEnd,
                        rectangleStart,
                        rectangleEnd))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool ContainsPoint(Vector2 point)
        {
            bool inside = false;
            for (int current = 0, previous = vertices.Count - 1;
                 current < vertices.Count;
                 previous = current++)
            {
                Vector2 a = vertices[previous];
                Vector2 b = vertices[current];
                if (PointOnSegment(point, a, b))
                    return true;

                bool crosses = (a.Y > point.Y) != (b.Y > point.Y) &&
                    point.X < (b.X - a.X) * (point.Y - a.Y) / (b.Y - a.Y) + a.X;
                if (crosses)
                    inside = !inside;
            }

            return inside;
        }

        private static bool ContainsInclusive(Rectangle rectangle, Vector2 point) =>
            point.X >= rectangle.Left && point.X <= rectangle.Right &&
            point.Y >= rectangle.Top && point.Y <= rectangle.Bottom;

        private static bool SegmentsIntersect(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        {
            float abC = Cross(b - a, c - a);
            float abD = Cross(b - a, d - a);
            float cdA = Cross(d - c, a - c);
            float cdB = Cross(d - c, b - c);

            if (((abC > Epsilon && abD < -Epsilon) || (abC < -Epsilon && abD > Epsilon)) &&
                ((cdA > Epsilon && cdB < -Epsilon) || (cdA < -Epsilon && cdB > Epsilon)))
            {
                return true;
            }

            return MathF.Abs(abC) <= Epsilon && PointOnSegment(c, a, b) ||
                   MathF.Abs(abD) <= Epsilon && PointOnSegment(d, a, b) ||
                   MathF.Abs(cdA) <= Epsilon && PointOnSegment(a, c, d) ||
                   MathF.Abs(cdB) <= Epsilon && PointOnSegment(b, c, d);
        }

        private static bool PointOnSegment(Vector2 point, Vector2 start, Vector2 end) =>
            MathF.Abs(Cross(end - start, point - start)) <= Epsilon &&
            point.X >= MathF.Min(start.X, end.X) - Epsilon &&
            point.X <= MathF.Max(start.X, end.X) + Epsilon &&
            point.Y >= MathF.Min(start.Y, end.Y) - Epsilon &&
            point.Y <= MathF.Max(start.Y, end.Y) + Epsilon;

        private static float Cross(Vector2 left, Vector2 right) =>
            left.X * right.Y - left.Y * right.X;
    }
}
