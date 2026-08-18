using Microsoft.Xna.Framework;

namespace _1_2D_Top_Down
{
    public interface ICutsceneStep
    {
        void Begin(CutsceneContext context);
        bool Update(GameTime gameTime, CutsceneContext context);
        void End(CutsceneContext context);
    }
}
