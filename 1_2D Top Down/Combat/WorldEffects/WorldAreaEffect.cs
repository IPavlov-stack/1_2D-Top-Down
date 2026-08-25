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

        private float phaseTimer;
        private float totalTimer;
        private float damageTimer;
        private bool hasHitPlayer;

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
            string sourceId)
        {
            this.definition = definition;
            this.center = center;
            this.tileSize = tileSize;
            this.sourceFaction = sourceFaction;
            this.sourceId = sourceId ?? string.Empty;

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

            if (Phase != WorldEffectPhase.Active)
                return false;

            bool canDamage = sourceFaction != CombatFaction.Player &&
                definition.Damage > 0 &&
                (!definition.HitOnce || !hasHitPlayer) &&
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

            damageTimer = MathF.Max(0f, damageTimer - deltaTime);

            if (phaseTimer >= definition.ActiveDuration)
                Phase = WorldEffectPhase.Finished;

            return player.Health.IsDead;
        }

        public void Draw(
            SpriteBatch spriteBatch,
            Texture2D pixelTexture,
            Texture2D circleTexture)
        {
            if (IsFinished)
                return;

            float opacity = Phase == WorldEffectPhase.Telegraph
                ? 0.20f + 0.20f *
                    ((MathF.Sin(totalTimer * 14f) + 1f) / 2f)
                : 0.48f;

            Color color = (Phase == WorldEffectPhase.Telegraph
                ? definition.TelegraphColor
                : definition.ActiveColor) * opacity;

            Texture2D texture = definition.Shape == AreaEffectShapeKind.Circle
                ? circleTexture
                : pixelTexture;

            spriteBatch.Draw(texture, Bounds, color);
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
