using System;
using Microsoft.Xna.Framework;

namespace _1_2D_Top_Down
{
    /// <summary>
    /// Optional sprite-sheet presentation for an area effect. Its timeline
    /// starts when the telegraph finishes and the effect becomes active.
    /// </summary>
    public sealed class AreaEffectAnimationDefinition
    {
        public string TextureAsset { get; }
        public int SheetColumns { get; }
        public int SheetRows { get; }
        public int Row { get; }
        public int FrameCount { get; }
        public float FrameDuration { get; }
        public float VisualScale { get; }
        public bool Loop { get; }
        public Vector2 VisualOffsetInTiles { get; }

        public AreaEffectAnimationDefinition(
            string textureAsset,
            int sheetColumns,
            int sheetRows,
            int frameCount,
            float frameDuration,
            int row = 0,
            float visualScale = 1f,
            bool loop = false,
            Vector2 visualOffsetInTiles = default)
        {
            if (string.IsNullOrWhiteSpace(textureAsset))
                throw new ArgumentException(
                    "An animation texture asset is required.",
                    nameof(textureAsset));
            if (sheetColumns <= 0)
                throw new ArgumentOutOfRangeException(nameof(sheetColumns));
            if (sheetRows <= 0)
                throw new ArgumentOutOfRangeException(nameof(sheetRows));
            if (frameCount <= 0 || frameCount > sheetColumns)
                throw new ArgumentOutOfRangeException(nameof(frameCount));
            if (frameDuration <= 0f)
                throw new ArgumentOutOfRangeException(nameof(frameDuration));
            if (row < 0 || row >= sheetRows)
                throw new ArgumentOutOfRangeException(nameof(row));
            if (visualScale <= 0f)
                throw new ArgumentOutOfRangeException(nameof(visualScale));

            TextureAsset = textureAsset;
            SheetColumns = sheetColumns;
            SheetRows = sheetRows;
            FrameCount = frameCount;
            FrameDuration = frameDuration;
            Row = row;
            VisualScale = visualScale;
            Loop = loop;
            VisualOffsetInTiles = visualOffsetInTiles;
        }
    }

    /// <summary>
    /// Three-part presentation for persistent effects: Start plays once,
    /// Cycle loops while the effect is dangerous, and Finish plays once after
    /// damage has stopped.
    /// </summary>
    public sealed class AreaEffectAnimationSequenceDefinition
    {
        public AreaEffectAnimationDefinition Start { get; }
        public AreaEffectAnimationDefinition Cycle { get; }
        public AreaEffectAnimationDefinition Finish { get; }

        public AreaEffectAnimationSequenceDefinition(
            AreaEffectAnimationDefinition start,
            AreaEffectAnimationDefinition cycle,
            AreaEffectAnimationDefinition finish)
        {
            Cycle = cycle ?? throw new ArgumentNullException(nameof(cycle));
            Start = start;
            Finish = finish;
        }
    }

    /// <summary>
    /// Reusable, data-only description of a telegraphed or persistent area.
    /// Dimensions are expressed in map tiles and are converted at spawn time.
    /// </summary>
    public sealed class AreaEffectDefinition
    {
        public string Id { get; }
        public AreaEffectShapeKind Shape { get; }
        public float WidthInTiles { get; }
        public float HeightInTiles { get; }
        public float RadiusInTiles { get; }
        public float TelegraphDuration { get; }
        public float ActiveDuration { get; }
        public float DamageInterval { get; }
        public int Damage { get; }
        public DamageType DamageType { get; }
        public float Knockback { get; }
        public bool HitOnce { get; }
        public Color TelegraphColor { get; }
        public Color ActiveColor { get; }
        public AreaEffectAnimationDefinition Animation { get; }
        public AreaEffectAnimationSequenceDefinition AnimationSequence { get; }
        public bool DamageOnlyOnActivation { get; }
        public bool ShowActiveIndicator { get; }
        public bool PulseActiveIndicator { get; }

        private AreaEffectDefinition(
            string id,
            AreaEffectShapeKind shape,
            float widthInTiles,
            float heightInTiles,
            float radiusInTiles,
            float telegraphDuration,
            float activeDuration,
            float damageInterval,
            int damage,
            DamageType damageType,
            float knockback,
            bool hitOnce,
            Color telegraphColor,
            Color activeColor,
            AreaEffectAnimationDefinition animation,
            bool damageOnlyOnActivation,
            AreaEffectAnimationSequenceDefinition animationSequence,
            bool showActiveIndicator,
            bool pulseActiveIndicator)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("An effect id is required.", nameof(id));
            if (telegraphDuration < 0f)
                throw new ArgumentOutOfRangeException(nameof(telegraphDuration));
            if (activeDuration <= 0f)
                throw new ArgumentOutOfRangeException(nameof(activeDuration));
            if (damage < 0)
                throw new ArgumentOutOfRangeException(nameof(damage));
            if (animation != null && animationSequence != null)
            {
                throw new ArgumentException(
                    "Use either a single animation or an animation sequence.");
            }

            Id = id;
            Shape = shape;
            WidthInTiles = widthInTiles;
            HeightInTiles = heightInTiles;
            RadiusInTiles = radiusInTiles;
            TelegraphDuration = telegraphDuration;
            ActiveDuration = activeDuration;
            DamageInterval = MathF.Max(0f, damageInterval);
            Damage = damage;
            DamageType = damageType;
            Knockback = MathF.Max(0f, knockback);
            HitOnce = hitOnce;
            TelegraphColor = telegraphColor == default
                ? Color.Red
                : telegraphColor;
            ActiveColor = activeColor == default
                ? Color.OrangeRed
                : activeColor;
            Animation = animation;
            AnimationSequence = animationSequence;
            DamageOnlyOnActivation = damageOnlyOnActivation;
            ShowActiveIndicator = showActiveIndicator;
            PulseActiveIndicator = pulseActiveIndicator;
        }

        public static AreaEffectDefinition Rectangle(
            string id,
            float widthInTiles,
            float heightInTiles,
            float telegraphDuration,
            float activeDuration,
            int damage,
            DamageType damageType,
            float damageInterval = 0f,
            bool hitOnce = true,
            float knockback = 0f,
            Color telegraphColor = default,
            Color activeColor = default,
            AreaEffectAnimationDefinition animation = null,
            bool damageOnlyOnActivation = false,
            AreaEffectAnimationSequenceDefinition animationSequence = null,
            bool showActiveIndicator = false,
            bool pulseActiveIndicator = false)
        {
            if (widthInTiles <= 0f)
                throw new ArgumentOutOfRangeException(nameof(widthInTiles));
            if (heightInTiles <= 0f)
                throw new ArgumentOutOfRangeException(nameof(heightInTiles));

            return new AreaEffectDefinition(
                id,
                AreaEffectShapeKind.Rectangle,
                widthInTiles,
                heightInTiles,
                radiusInTiles: 0f,
                telegraphDuration,
                activeDuration,
                damageInterval,
                damage,
                damageType,
                knockback,
                hitOnce,
                telegraphColor,
                activeColor,
                animation,
                damageOnlyOnActivation,
                animationSequence,
                showActiveIndicator,
                pulseActiveIndicator);
        }

        public static AreaEffectDefinition Circle(
            string id,
            float radiusInTiles,
            float telegraphDuration,
            float activeDuration,
            int damage,
            DamageType damageType,
            float damageInterval = 0f,
            bool hitOnce = true,
            float knockback = 0f,
            Color telegraphColor = default,
            Color activeColor = default,
            AreaEffectAnimationDefinition animation = null,
            bool damageOnlyOnActivation = false,
            AreaEffectAnimationSequenceDefinition animationSequence = null,
            bool showActiveIndicator = false,
            bool pulseActiveIndicator = false)
        {
            if (radiusInTiles <= 0f)
                throw new ArgumentOutOfRangeException(nameof(radiusInTiles));

            return new AreaEffectDefinition(
                id,
                AreaEffectShapeKind.Circle,
                widthInTiles: 0f,
                heightInTiles: 0f,
                radiusInTiles,
                telegraphDuration,
                activeDuration,
                damageInterval,
                damage,
                damageType,
                knockback,
                hitOnce,
                telegraphColor,
                activeColor,
                animation,
                damageOnlyOnActivation,
                animationSequence,
                showActiveIndicator,
                pulseActiveIndicator);
        }
    }
}
