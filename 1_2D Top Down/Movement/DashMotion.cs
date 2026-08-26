using System;
using Microsoft.Xna.Framework;

namespace _1_2D_Top_Down
{
    public enum DashEndReason
    {
        None,
        Completed,
        Blocked,
        Interrupted
    }

    public enum DashMotionPhase
    {
        Inactive,
        Burst,
        ResidualSlide
    }

    /// <summary>
    /// Direction-locked dash kinematics with a constant-speed powered phase
    /// and an ease-out slide. Collision remains owned by the caller, allowing
    /// this component to be reused by enemies and, later, by the player.
    /// </summary>
    public sealed class DashMotion
    {
        private readonly DashMovementDefinition definition;
        private float poweredDistance;
        private float slideElapsed;

        public Vector2 Direction { get; private set; } = Vector2.UnitY;
        public float RemainingDistance { get; private set; }
        public float CurrentSpeed { get; private set; }
        public bool IsActive { get; private set; }
        public DashMotionPhase Phase { get; private set; } =
            DashMotionPhase.Inactive;

        public DashMotion(DashMovementDefinition definition)
        {
            this.definition = definition ??
                throw new ArgumentNullException(nameof(definition));
        }

        public void Begin(Vector2 direction)
        {
            if (direction == Vector2.Zero)
                direction = Vector2.UnitY;
            else
                direction.Normalize();

            Direction = direction;
            RemainingDistance = definition.TotalDistance;
            poweredDistance = 0f;
            slideElapsed = 0f;
            CurrentSpeed = definition.InitialSpeed;
            IsActive = true;
            Phase = DashMotionPhase.Burst;
        }

        public DashEndReason Update(
            float deltaTime,
            Func<Vector2, bool> tryMove,
            Func<bool> interruptCondition = null)
        {
            if (!IsActive || deltaTime <= 0f)
                return DashEndReason.None;
            if (tryMove == null)
                throw new ArgumentNullException(nameof(tryMove));

            float remainingTime = deltaTime;
            while (remainingTime > 0.00001f && IsActive)
            {
                if (Phase == DashMotionPhase.Burst)
                {
                    float distanceLeft = MathF.Max(
                        0f,
                        definition.Distance - poweredDistance);
                    float phaseTimeLeft =
                        distanceLeft / definition.InitialSpeed;
                    float stepTime = MathF.Min(
                        remainingTime,
                        phaseTimeLeft);
                    float requestedDistance =
                        definition.InitialSpeed * stepTime;

                    DashEndReason movementResult = MoveDistance(
                        requestedDistance,
                        tryMove,
                        interruptCondition);
                    if (movementResult != DashEndReason.None)
                        return movementResult;

                    poweredDistance += requestedDistance;
                    remainingTime -= stepTime;

                    if (poweredDistance + 0.001f < definition.Distance)
                        continue;

                    if (definition.SlideDuration <= 0f)
                    {
                        Stop();
                        return DashEndReason.Completed;
                    }

                    Phase = DashMotionPhase.ResidualSlide;
                    CurrentSpeed = definition.InitialSpeed;
                    continue;
                }

                float previousElapsed = slideElapsed;
                slideElapsed = MathF.Min(
                    definition.SlideDuration,
                    slideElapsed + remainingTime);
                float requestedSlideDistance =
                    GetSlideDistance(slideElapsed) -
                    GetSlideDistance(previousElapsed);
                remainingTime -= slideElapsed - previousElapsed;

                DashEndReason slideResult = MoveDistance(
                    requestedSlideDistance,
                    tryMove,
                    interruptCondition);
                if (slideResult != DashEndReason.None)
                    return slideResult;

                float progress = MathHelper.Clamp(
                    slideElapsed / definition.SlideDuration,
                    0f,
                    1f);
                CurrentSpeed = definition.InitialSpeed * MathF.Pow(
                    1f - progress,
                    definition.SlideEasePower);

                if (slideElapsed + 0.00001f < definition.SlideDuration)
                    continue;

                Stop();
                return DashEndReason.Completed;
            }

            return DashEndReason.None;
        }

        private float GetSlideDistance(float elapsed)
        {
            float progress = MathHelper.Clamp(
                elapsed / definition.SlideDuration,
                0f,
                1f);
            return definition.SlideDistance *
                (1f - MathF.Pow(
                    1f - progress,
                    definition.SlideEasePower + 1f));
        }

        private DashEndReason MoveDistance(
            float requestedDistance,
            Func<Vector2, bool> tryMove,
            Func<bool> interruptCondition)
        {
            if (requestedDistance <= 0.0001f)
                return DashEndReason.None;

            int steps = Math.Max(
                1,
                (int)MathF.Ceiling(
                    requestedDistance /
                    definition.MaximumMovementStep));
            float stepDistance = requestedDistance / steps;

            for (int i = 0; i < steps; i++)
            {
                if (!tryMove(Direction * stepDistance))
                {
                    Stop();
                    return DashEndReason.Blocked;
                }

                RemainingDistance = MathF.Max(
                    0f,
                    RemainingDistance - stepDistance);

                if (interruptCondition != null && interruptCondition())
                {
                    Stop();
                    return DashEndReason.Interrupted;
                }
            }

            return DashEndReason.None;
        }

        public void Stop()
        {
            RemainingDistance = 0f;
            poweredDistance = 0f;
            slideElapsed = 0f;
            CurrentSpeed = 0f;
            IsActive = false;
            Phase = DashMotionPhase.Inactive;
        }
    }
}
