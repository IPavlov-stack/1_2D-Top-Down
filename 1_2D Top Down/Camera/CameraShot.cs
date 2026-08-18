using Microsoft.Xna.Framework;
using System;

namespace _1_2D_Top_Down
{
    public sealed class CameraShot
    {
        public CameraShot(
            Func<Vector2> targetProvider,
            float zoom,
            float blendInDuration,
            float holdDuration,
            float blendOutDuration)
        {
            TargetProvider = targetProvider ?? throw new ArgumentNullException(nameof(targetProvider));
            Zoom = zoom;
            BlendInDuration = MathF.Max(0.01f, blendInDuration);
            HoldDuration = MathF.Max(0f, holdDuration);
            BlendOutDuration = MathF.Max(0.01f, blendOutDuration);
        }

        public Func<Vector2> TargetProvider { get; }
        public float Zoom { get; }
        public float BlendInDuration { get; }
        public float HoldDuration { get; }
        public float BlendOutDuration { get; }
        public float TotalDuration => BlendInDuration + HoldDuration + BlendOutDuration;
    }
}
