using Microsoft.Xna.Framework;
using System;

namespace _1_2D_Top_Down
{
    public sealed class CameraShotCutsceneStep : ICutsceneStep
    {
        private readonly CameraShot shot;
        private readonly Func<Point> viewportSizeProvider;

        public CameraShotCutsceneStep(CameraShot shot, Func<Point> viewportSizeProvider)
        {
            this.shot = shot;
            this.viewportSizeProvider = viewportSizeProvider;
        }

        public void Begin(CutsceneContext context)
        {
            context.CameraController.Play(shot, viewportSizeProvider());
        }

        public bool Update(GameTime gameTime, CutsceneContext context)
        {
            return !context.CameraController.IsShotActive;
        }

        public void End(CutsceneContext context) { }
    }
}
