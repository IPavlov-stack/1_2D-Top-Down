using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace _1_2D_Top_Down
{
    public sealed class WorldRenderQueue
    {
        private readonly List<Entry> entries = new();
        private int nextSequence;

        public void Clear()
        {
            entries.Clear();
            nextSequence = 0;
        }

        public void Add(IYSortedWorldDrawable drawable)
        {
            entries.Add(new Entry(drawable, nextSequence++));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            entries.Sort(static (left, right) =>
            {
                int depthComparison = left.Drawable.SortY.CompareTo(
                    right.Drawable.SortY);

                return depthComparison != 0
                    ? depthComparison
                    : left.Sequence.CompareTo(right.Sequence);
            });

            foreach (Entry entry in entries)
                entry.Drawable.Draw(spriteBatch);
        }

        private readonly struct Entry
        {
            public IYSortedWorldDrawable Drawable { get; }
            public int Sequence { get; }

            public Entry(IYSortedWorldDrawable drawable, int sequence)
            {
                Drawable = drawable;
                Sequence = sequence;
            }
        }
    }
}
