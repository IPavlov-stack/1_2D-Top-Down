using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace _1_2D_Top_Down
{
    public class SpatialGrid<T> where T : Enemy
    {
        private readonly int cellSize;

        private readonly Dictionary<Point, List<T>> cells = new();

        public SpatialGrid(int cellSize)
        {
            this.cellSize = cellSize;
        }

        public void Rebuild(IReadOnlyList<T> items)
        {
            foreach (List<T> cell in cells.Values)
            {
                cell.Clear();
            }

            for (int i = 0; i < items.Count; i++)
            {
                T item = items[i];

                Point cellPosition = GetCell(item.Bounds.Center);

                if (!cells.TryGetValue(
                        cellPosition,
                        out List<T>? cell))
                {
                    cell = new List<T>();
                    cells.Add(cellPosition, cell);
                }

                cell.Add(item);
            }
        }

        public void QueryNearby(
            Rectangle bounds,
            List<T> results)
        {
            results.Clear();

            Point centerCell = GetCell(bounds.Center);

            // Проверяваме текущата клетка и осемте съседни.
            for (int y = centerCell.Y - 1; y <= centerCell.Y + 1; y++)
            {
                for (int x = centerCell.X - 1; x <= centerCell.X + 1; x++)
                {
                    if (!cells.TryGetValue(
                            new Point(x, y),
                            out List<T>? cell))
                    {
                        continue;
                    }

                    for (int i = 0; i < cell.Count; i++)
                    {
                        results.Add(cell[i]);
                    }
                }
            }
        }

        private Point GetCell(Point position)
        {
            return new Point(
                position.X / cellSize,
                position.Y / cellSize);
        }
    }
}