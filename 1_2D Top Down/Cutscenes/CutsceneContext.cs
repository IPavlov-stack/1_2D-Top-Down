namespace _1_2D_Top_Down
{
    public sealed class CutsceneContext
    {
        public CutsceneContext(
            CameraController cameraController,
            OverlayManager overlayManager,
            DialogueOverlay dialogueOverlay)
        {
            CameraController = cameraController;
            OverlayManager = overlayManager;
            DialogueOverlay = dialogueOverlay;
        }

        public CameraController CameraController { get; }
        public OverlayManager OverlayManager { get; }
        public DialogueOverlay DialogueOverlay { get; }
    }
}
