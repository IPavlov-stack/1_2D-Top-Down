using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _1_2D_Top_Down
{
    public interface IGameOverlay
    {
        string Id { get; }
        bool BlocksInputBelow { get; }
        bool BlocksUpdateBelow { get; }
        void Enter();
        void Exit();
        void Update(GameTime gameTime);
        void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}
