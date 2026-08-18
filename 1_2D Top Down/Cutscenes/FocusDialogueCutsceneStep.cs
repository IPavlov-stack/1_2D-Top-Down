using Microsoft.Xna.Framework;
using System;

namespace _1_2D_Top_Down
{
    public sealed class FocusDialogueCutsceneStep : ICutsceneStep
    {
        private readonly Func<Vector2> targetProvider;
        private readonly Func<Point> viewportSizeProvider;
        private readonly float zoom;
        private readonly string speaker;
        private readonly string text;
        private bool returningToFollow;

        public FocusDialogueCutsceneStep(
            Func<Vector2> targetProvider,
            Func<Point> viewportSizeProvider,
            float zoom,
            string speaker,
            string text)
        {
            this.targetProvider = targetProvider;
            this.viewportSizeProvider = viewportSizeProvider;
            this.zoom = zoom;
            this.speaker = speaker;
            this.text = text;
        }

        public void Begin(CutsceneContext context)
        {
            returningToFollow = false;
            context.CameraController.Play(
                new CameraShot(targetProvider, zoom, 0.65f, 3600f, 0.65f),
                viewportSizeProvider());
            context.DialogueOverlay.Show(speaker, text);
            context.OverlayManager.Push(context.DialogueOverlay);
        }

        public bool Update(GameTime gameTime, CutsceneContext context)
        {
            if (!returningToFollow &&
                !context.OverlayManager.Contains(context.DialogueOverlay.Id))
            {
                returningToFollow = true;
                context.CameraController.Play(
                    new CameraShot(targetProvider, zoom, 0.01f, 0f, 0.65f),
                    viewportSizeProvider());
            }

            return returningToFollow && !context.CameraController.IsShotActive;
        }

        public void End(CutsceneContext context)
        {
            context.OverlayManager.Remove(context.DialogueOverlay.Id);
        }
    }
}
