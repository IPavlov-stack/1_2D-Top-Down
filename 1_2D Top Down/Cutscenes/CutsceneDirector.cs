using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace _1_2D_Top_Down
{
    public sealed class CutsceneDirector
    {
        private readonly CutsceneContext context;
        private readonly Queue<ICutsceneStep> steps = new();
        private ICutsceneStep currentStep;

        public CutsceneDirector(CutsceneContext context)
        {
            this.context = context;
        }

        public bool IsActive => currentStep != null || steps.Count > 0;

        public void Play(IEnumerable<ICutsceneStep> sequence)
        {
            Stop();

            foreach (ICutsceneStep step in sequence)
                steps.Enqueue(step);

            BeginNextStep();
        }

        public void Update(GameTime gameTime)
        {
            if (currentStep == null)
                return;

            if (!currentStep.Update(gameTime, context))
                return;

            currentStep.End(context);
            currentStep = null;
            BeginNextStep();
        }

        public void Stop()
        {
            currentStep?.End(context);
            currentStep = null;
            steps.Clear();
            context.OverlayManager.Remove(context.DialogueOverlay.Id);
        }

        private void BeginNextStep()
        {
            if (!steps.TryDequeue(out currentStep))
                return;

            currentStep.Begin(context);
        }
    }
}
