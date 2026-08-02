using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace _1_2D_Top_Down
{
    public sealed class StaticCollisionGrid
    {
        private readonly int cellSize;

        private readonly Dictionary<Point, List<Rectangle>> cells = new();

        public StaticCollisionGrid(int cellSize)
        {
            this.cellSize = cellSize;
        }

        // Извиква се само веднъж, след като картата и collision layers са заредени.
        public void Build(IEnumerable<Rectangle> collisionRectangles)
        {
            cells.Clear();

            foreach (Rectangle rectangle in collisionRectangles)
            {
                int startCellX = rectangle.Left / cellSize;
                int endCellX = (rectangle.Right - 1) / cellSize;

                int startCellY = rectangle.Top / cellSize;
                int endCellY = (rectangle.Bottom - 1) / cellSize;

                // Голям obstacle може да заема повече от една клетка.
                for (int y = startCellY; y <= endCellY; y++)
                {
                    for (int x = startCellX; x <= endCellX; x++)
                    {
                        Point cellPosition = new Point(x, y);

                        if (!cells.TryGetValue(
                                cellPosition,
                                out List<Rectangle>? cell))
                        {
                            cell = new List<Rectangle>();
                            cells.Add(cellPosition, cell);
                        }

                        cell.Add(rectangle);
                    }
                }
            }
        }

        public bool Intersects(Rectangle bounds)
        {
            int startCellX = bounds.Left / cellSize;
            int endCellX = (bounds.Right - 1) / cellSize;

            int startCellY = bounds.Top / cellSize;
            int endCellY = (bounds.Bottom - 1) / cellSize;

            for (int y = startCellY; y <= endCellY; y++)
            {
                for (int x = startCellX; x <= endCellX; x++)
                {
                    if (!cells.TryGetValue(
                            new Point(x, y),
                            out List<Rectangle>? cell))
                    {
                        continue;
                    }

                    for (int i = 0; i < cell.Count; i++)
                    {
                        if (bounds.Intersects(cell[i]))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}