using Microsoft.Xna.Framework.Graphics;

namespace _1_2D_Top_Down
{
    public interface IGameplayPanel
    {
        string Id { get; }
        bool BlocksGameplayInput { get; }

        void Open();
        void Close();
        bool HandleInput(GameplayUiInput input);
        void Draw(SpriteBatch spriteBatch);
    }
}
