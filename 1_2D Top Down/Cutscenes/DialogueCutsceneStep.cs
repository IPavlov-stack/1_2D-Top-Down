using Microsoft.Xna.Framework;

namespace _1_2D_Top_Down
{
    public sealed class DialogueCutsceneStep : ICutsceneStep
    {
        private readonly string speaker;
        private readonly string text;

        public DialogueCutsceneStep(string speaker, string text)
        {
            this.speaker = speaker;
            this.text = text;
        }

        public void Begin(CutsceneContext context)
        {
            context.DialogueOverlay.Show(speaker, text);
            context.OverlayManager.Push(context.DialogueOverlay);
        }

        public bool Update(GameTime gameTime, CutsceneContext context)
        {
            return !context.OverlayManager.Contains(context.DialogueOverlay.Id);
        }

        public void End(CutsceneContext context)
        {
            context.OverlayManager.Remove(context.DialogueOverlay.Id);
        }
    }
}
