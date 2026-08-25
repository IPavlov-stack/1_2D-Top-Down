using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _1_2D_Top_Down
{
    public sealed class WorldAreaEffect
    {
        private const float MinimumRepeatInterval = 0.05f;

        private readonly AreaEffectDefinition definition;
        private readonly Vector2 center;
        private readonly Vector2 tileSize;
        private readonly CombatFaction sourceFaction;
        private readonly string sourceId;
        private readonly Texture2D animationTexture;
        private readonly Texture2D startAnimationTexture;
        private readonly Texture2D cycleAnimationTexture;
        private readonly Texture2D finishAnimationTexture;

        private float phaseTimer;
        private float totalTimer;
        private float damageTimer;
        private bool hasHitPlayer;
        private bool activationDamageResolved;

        public WorldEffectPhase Phase { get; private set; }

        public Rectangle Bounds
        {
            get
            {
                Vector2 size = definition.Shape == AreaEffectShapeKind.Circle
                    ? new Vector2(
                        definition.RadiusInTiles * tileSize.X * 2f,
                        definition.RadiusInTiles * tileSize.Y * 2f)
                    : new Vector2(
                        definition.WidthInTiles * tileSize.X,
                        definition.HeightInTiles * tileSize.Y);

                return new Rectangle(
                    (int)MathF.Round(center.X - size.X / 2f),
                    (int)MathF.Round(center.Y - size.Y / 2f),
                    Math.Max(1, (int)MathF.Round(size.X)),
                    Math.Max(1, (int)MathF.Round(size.Y)));
            }
        }

        public bool IsFinished => Phase == WorldEffectPhase.Finished;

        public WorldAreaEffect(
            AreaEffectDefinition definition,
            Vector2 center,
            Vector2 tileSize,
            CombatFaction sourceFaction,
            string sourceId,
            Func<string, Texture2D> textureResolver = null)
        {
            this.definition = definition;
            this.center = center;
            this.tileSize = tileSize;
            this.sourceFaction = sourceFaction;
            this.sourceId = sourceId ?? string.Empty;
            animationTexture = ResolveTexture(
                definition.Animation,
                textureResolver);

            AreaEffectAnimationSequenceDefinition sequence =
                definition.AnimationSequence;
            startAnimationTexture = ResolveTexture(
                sequence?.Start,
                textureResolver);
            cycleAnimationTexture = ResolveTexture(
                sequence?.Cycle,
                textureResolver);
            finishAnimationTexture = ResolveTexture(
                sequence?.Finish,
                textureResolver);

            Phase = definition.TelegraphDuration > 0f
                ? WorldEffectPhase.Telegraph
                : WorldEffectPhase.Active;
        }

        public bool Update(GameTime gameTime, Player player)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            totalTimer += deltaTime;
            phaseTimer += deltaTime;

            if (Phase == WorldEffectPhase.Telegraph &&
                phaseTimer >= definition.TelegraphDuration)
            {
                Phase = WorldEffectPhase.Active;
                phaseTimer = 0f;
                damageTimer = 0f;
            }

            if (Phase == WorldEffectPhase.Finishing)
            {
                AreaEffectAnimationDefinition finish =
                    definition.AnimationSequence?.Finish;
                if (finish == null ||
                    phaseTimer >= GetAnimationDuration(finish))
                {
                    Phase = WorldEffectPhase.Finished;
                }

                return false;
            }

            if (Phase != WorldEffectPhase.Active)
                return false;

            bool canDamage = sourceFaction != CombatFaction.Player &&
                definition.Damage > 0 &&
                (!definition.HitOnce || !hasHitPlayer) &&
                (!definition.DamageOnlyOnActivation ||
                    !activationDamageResolved) &&
                damageTimer <= 0f;

            if (canDamage && Intersects(player.Hurtbox))
            {
                player.TakeHit(new CombatHit(
                    definition.Damage,
                    definition.DamageType,
                    sourceFaction,
                    sourceId,
                    center,
                    definition.Knockback));

                hasHitPlayer = true;
                damageTimer = MathF.Max(
                    MinimumRepeatInterval,
                    definition.DamageInterval);
            }

            // Resolve an impact-style attack once, immediately after its
            // telegraph. Entering the lingering animation later is harmless.
            if (definition.DamageOnlyOnActivation)
                activationDamageResolved = true;

            damageTimer = MathF.Max(0f, damageTimer - deltaTime);

            if (phaseTimer >= definition.ActiveDuration)
            {
                if (definition.AnimationSequence?.Finish != null &&
                    finishAnimationTexture != null)
                {
                    Phase = WorldEffectPhase.Finishing;
                    phaseTimer = 0f;
                }
                else
                {
                    Phase = WorldEffectPhase.Finished;
                }
            }

            return player.Health.IsDead;
        }

        public void Draw(
            SpriteBatch spriteBatch,
            Texture2D pixelTexture,
            Texture2D circleTexture)
        {
            if (IsFinished)
                return;

            if (Phase == WorldEffectPhase.Telegraph)
            {
                float opacity = 0.20f + 0.20f *
                    ((MathF.Sin(totalTimer * 14f) + 1f) / 2f);
                Texture2D telegraphTexture =
                    definition.Shape == AreaEffectShapeKind.Circle
                        ? circleTexture
                        : pixelTexture;

                spriteBatch.Draw(
                    telegraphTexture,
                    Bounds,
                    definition.TelegraphColor * opacity);
                return;
            }

            if (Phase == WorldEffectPhase.Finishing)
            {
                DrawAnimation(spriteBatch);
                return;
            }

            bool hasAnimation =
                definition.Animation != null && animationTexture != null ||
                definition.AnimationSequence != null &&
                cycleAnimationTexture != null;

            if (!hasAnimation || definition.ShowActiveIndicator)
                DrawActiveIndicator(spriteBatch, pixelTexture, circleTexture);

            DrawAnimation(spriteBatch);
        }

        private void DrawActiveIndicator(
            SpriteBatch spriteBatch,
            Texture2D pixelTexture,
            Texture2D circleTexture)
        {
            Texture2D activeTexture =
                definition.Shape == AreaEffectShapeKind.Circle
                    ? circleTexture
                    : pixelTexture;
            float opacity = 0.48f;
            if (definition.PulseActiveIndicator)
            {
                opacity = 0.20f + 0.24f *
                    ((MathF.Sin(totalTimer * 10f) + 1f) / 2f);
            }

            spriteBatch.Draw(
                activeTexture,
                Bounds,
                definition.ActiveColor * opacity);
        }

        private void DrawAnimation(SpriteBatch spriteBatch)
        {
            if (Phase == WorldEffectPhase.Finishing)
            {
                DrawAnimationClip(
                    spriteBatch,
                    definition.AnimationSequence?.Finish,
                    finishAnimationTexture,
                    phaseTimer,
                    loop: false);
                return;
            }

            AreaEffectAnimationSequenceDefinition sequence =
                definition.AnimationSequence;
            if (sequence != null)
            {
                float startDuration = GetAnimationDuration(sequence.Start);
                if (sequence.Start != null &&
                    startAnimationTexture != null &&
                    phaseTimer < startDuration)
                {
                    DrawAnimationClip(
                        spriteBatch,
                        sequence.Start,
                        startAnimationTexture,
                        phaseTimer,
                        loop: false);
                }
                else
                {
                    DrawAnimationClip(
                        spriteBatch,
                        sequence.Cycle,
                        cycleAnimationTexture,
                        MathF.Max(0f, phaseTimer - startDuration),
                        loop: true);
                }

                return;
            }

            DrawAnimationClip(
                spriteBatch,
                definition.Animation,
                animationTexture,
                phaseTimer,
                loop: definition.Animation?.Loop ?? false);
        }

        private void DrawAnimationClip(
            SpriteBatch spriteBatch,
            AreaEffectAnimationDefinition animation,
            Texture2D texture,
            float elapsed,
            bool loop)
        {
            if (animation == null || texture == null)
                return;

            int frameWidth = texture.Width / animation.SheetColumns;
            int frameHeight = texture.Height / animation.SheetRows;
            int rawFrame = (int)(elapsed / animation.FrameDuration);
            int frame = loop
                ? rawFrame % animation.FrameCount
                : Math.Min(animation.FrameCount - 1, rawFrame);

            Rectangle source = new(
                frame * frameWidth,
                animation.Row * frameHeight,
                frameWidth,
                frameHeight);

            Rectangle effectBounds = Bounds;
            int destinationWidth = Math.Max(
                1,
                (int)MathF.Round(
                    effectBounds.Width * animation.VisualScale));
            int destinationHeight = Math.Max(
                1,
                (int)MathF.Round(
                    effectBounds.Height * animation.VisualScale));
            Rectangle destination = new(
                effectBounds.Center.X - destinationWidth / 2 +
                    (int)MathF.Round(
                        animation.VisualOffsetInTiles.X * tileSize.X),
                effectBounds.Center.Y - destinationHeight / 2 +
                    (int)MathF.Round(
                        animation.VisualOffsetInTiles.Y * tileSize.Y),
                destinationWidth,
                destinationHeight);

            spriteBatch.Draw(
                texture,
                destination,
                source,
                Color.White);
        }

        private static Texture2D ResolveTexture(
            AreaEffectAnimationDefinition animation,
            Func<string, Texture2D> textureResolver)
        {
            return animation == null || textureResolver == null
                ? null
                : textureResolver(animation.TextureAsset);
        }

        private static float GetAnimationDuration(
            AreaEffectAnimationDefinition animation)
        {
            return animation == null
                ? 0f
                : animation.FrameCount * animation.FrameDuration;
        }

        private bool Intersects(Rectangle rectangle)
        {
            if (definition.Shape == AreaEffectShapeKind.Rectangle)
                return Bounds.Intersects(rectangle);

            float radiusX = definition.RadiusInTiles * tileSize.X;
            float radiusY = definition.RadiusInTiles * tileSize.Y;

            // Normalizing both axes also keeps circle effects correct when a
            // future map uses non-square tiles.
            float closestX = MathHelper.Clamp(
                center.X,
                rectangle.Left,
                rectangle.Right);
            float closestY = MathHelper.Clamp(
                center.Y,
                rectangle.Top,
                rectangle.Bottom);

            float normalizedX = (closestX - center.X) / radiusX;
            float normalizedY = (closestY - center.Y) / radiusY;
            return normalizedX * normalizedX + normalizedY * normalizedY <= 1f;
        }
    }
}
