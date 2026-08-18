using Microsoft.Xna.Framework;
using System;

namespace _1_2D_Top_Down
{
    public sealed class CameraController
    {
        private readonly Camera2D camera;
        private readonly Func<Vector2> followTargetProvider;
        private Vector2 previousFollowTarget;
        private Vector2 shotStartFocus;
        private float shotStartZoom;
        private float shotElapsed;
        private CameraShot activeShot;
        private CameraMode followMode = CameraMode.FollowPlayer;
        private Vector2 staticFocus;
        private float staticZoom = 1.2f;

        public CameraController(Camera2D camera, Func<Vector2> followTargetProvider)
        {
            this.camera = camera;
            this.followTargetProvider = followTargetProvider;
        }

        public float FollowZoom { get; set; } = 1.2f;
        public float LookAheadDistance { get; set; } = 70f;
        public float FollowSharpness { get; set; } = 8f;
        public bool IsShotActive => activeShot != null;
        public Rectangle WorldBounds { get; set; }
        public CameraMode Mode => activeShot != null ? CameraMode.FocusTarget : followMode;

        public void FollowPlayer(bool useLookAhead = false)
        {
            activeShot = null;
            followMode = useLookAhead
                ? CameraMode.FollowWithLookAhead
                : CameraMode.FollowPlayer;
        }

        public void HoldStatic(Vector2 focus, float zoom)
        {
            activeShot = null;
            staticFocus = focus;
            staticZoom = zoom;
            followMode = CameraMode.Static;
        }

        public void SnapToFollow(Point viewportSize)
        {
            activeShot = null;
            followMode = CameraMode.FollowPlayer;
            Vector2 target = followTargetProvider();
            previousFollowTarget = target;
            camera.SetZoom(FollowZoom);
            camera.SetPosition(ClampToWorld(ToTopLeft(target, FollowZoom, viewportSize), FollowZoom, viewportSize));
        }

        public void Play(CameraShot shot, Point viewportSize)
        {
            activeShot = shot;
            shotElapsed = 0f;
            shotStartZoom = camera.Zoom;
            shotStartFocus = GetCurrentFocus(viewportSize);
        }

        public void Update(GameTime gameTime, Point viewportSize)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 followTarget = followTargetProvider();
            Vector2 movement = followTarget - previousFollowTarget;
            previousFollowTarget = followTarget;

            Vector2 lookAhead = followMode != CameraMode.FollowWithLookAhead || movement == Vector2.Zero
                ? Vector2.Zero
                : Vector2.Normalize(movement) * LookAheadDistance;

            Vector2 followFocus = followTarget + lookAhead;

            if (activeShot != null)
            {
                UpdateShot(deltaTime, followFocus, viewportSize);
                return;
            }


            if (followMode == CameraMode.Static)
            {
                SmoothToward(staticFocus, staticZoom, deltaTime, viewportSize);
                return;
            }

            SmoothToward(followFocus, FollowZoom, deltaTime, viewportSize);
        }

        private void UpdateShot(float deltaTime, Vector2 followFocus, Point viewportSize)
        {
            shotElapsed += deltaTime;
            float blendInEnd = activeShot.BlendInDuration;
            float holdEnd = blendInEnd + activeShot.HoldDuration;
            Vector2 shotFocus = activeShot.TargetProvider();
            Vector2 focus;
            float zoom;

            if (shotElapsed < blendInEnd)
            {
                float amount = EaseInOut(shotElapsed / blendInEnd);
                focus = Vector2.Lerp(shotStartFocus, shotFocus, amount);
                zoom = MathHelper.Lerp(shotStartZoom, activeShot.Zoom, amount);
            }
            else if (shotElapsed < holdEnd)
            {
                focus = shotFocus;
                zoom = activeShot.Zoom;
            }
            else
            {
                float amount = EaseInOut((shotElapsed - holdEnd) / activeShot.BlendOutDuration);
                focus = Vector2.Lerp(shotFocus, followFocus, amount);
                zoom = MathHelper.Lerp(activeShot.Zoom, FollowZoom, amount);
            }

            camera.SetZoom(zoom);
            camera.SetPosition(ClampToWorld(ToTopLeft(focus, zoom, viewportSize), zoom, viewportSize));

            if (shotElapsed >= activeShot.TotalDuration)
                activeShot = null;
        }

        private void SmoothToward(Vector2 focus, float zoom, float deltaTime, Point viewportSize)
        {
            float amount = 1f - MathF.Exp(-FollowSharpness * deltaTime);
            float nextZoom = MathHelper.Lerp(camera.Zoom, zoom, amount);
            Vector2 destination = ToTopLeft(focus, nextZoom, viewportSize);
            camera.SetZoom(nextZoom);
            camera.SetPosition(ClampToWorld(
                Vector2.Lerp(camera.Position, destination, amount),
                nextZoom,
                viewportSize));
        }

        private Vector2 GetCurrentFocus(Point viewportSize)
        {
            return camera.Position + new Vector2(
                viewportSize.X / (2f * camera.Zoom),
                viewportSize.Y / (2f * camera.Zoom));
        }

        private static Vector2 ToTopLeft(Vector2 focus, float zoom, Point viewportSize)
        {
            return focus - new Vector2(
                viewportSize.X / (2f * zoom),
                viewportSize.Y / (2f * zoom));
        }

        private Vector2 ClampToWorld(Vector2 position, float zoom, Point viewportSize)
        {
            if (WorldBounds.Width <= 0 || WorldBounds.Height <= 0)
                return position;

            float visibleWidth = viewportSize.X / zoom;
            float visibleHeight = viewportSize.Y / zoom;
            float maxX = MathF.Max(WorldBounds.Left, WorldBounds.Right - visibleWidth);
            float maxY = MathF.Max(WorldBounds.Top, WorldBounds.Bottom - visibleHeight);

            return new Vector2(
                MathHelper.Clamp(position.X, WorldBounds.Left, maxX),
                MathHelper.Clamp(position.Y, WorldBounds.Top, maxY));
        }

        private static float EaseInOut(float amount)
        {
            amount = MathHelper.Clamp(amount, 0f, 1f);
            return amount * amount * (3f - 2f * amount);
        }
    }
}
