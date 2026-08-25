using Microsoft.Xna.Framework.Graphics;

namespace _1_2D_Top_Down
{
    public interface IYSortedWorldDrawable
    {
        int SortY { get; }

        void Draw(SpriteBatch spriteBatch);
    }
}
