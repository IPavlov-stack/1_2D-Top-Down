using System;
using Microsoft.Xna.Framework;

namespace _1_2D_Top_Down
{
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
            Color activeColor)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("An effect id is required.", nameof(id));
            if (telegraphDuration < 0f)
                throw new ArgumentOutOfRangeException(nameof(telegraphDuration));
            if (activeDuration <= 0f)
                throw new ArgumentOutOfRangeException(nameof(activeDuration));
            if (damage < 0)
                throw new ArgumentOutOfRangeException(nameof(damage));

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
            Color activeColor = default)
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
                activeColor);
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
            Color activeColor = default)
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
                activeColor);
        }
    }
}
