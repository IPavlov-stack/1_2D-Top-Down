using _1_2D_Top_Down;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _1_2D_Top_Down
{

    public class EnemyProjectile : Projectile
    {
        private readonly float maxTravelDistance;
        private float travelledDistance;
        public bool HasReachedMaxTravelDistance => travelledDistance >= maxTravelDistance;
        public EnemyProjectile(
            Texture2D texture,
            Vector2 startPosition,
            Vector2 direction)
            : this(
                texture,
                startPosition,
                direction,
                new ProjectileSpec(
                    string.Empty,
                    damage: 15,
                    DamageType.Arcane,
                    CombatFaction.Enemy,
                    speed: 325f,
                    maxTravelDistance: 700f,
                    knockback: 0f,
                    isReflectable: true,
                    scale: 1.75f,
                    frameCount: 11,
                    frameRows: 1,
                    frameDuration: 0.04f),
                sourceId: string.Empty)
        {
        }

        public EnemyProjectile(
            Texture2D texture,
            Vector2 startPosition,
            Vector2 direction,
            ProjectileSpec spec,
            string sourceId)
            : base(
                texture,
                startPosition,
                direction,
                spec.Speed,
                spec.Scale,
                spec.FrameCount,
                spec.FrameRows,
                spec.FrameDuration,
                spec.Faction,
                spec.Damage,
                spec.DamageType,
                spec.Knockback,
                spec.IsReflectable,
                sourceId)
        {
            maxTravelDistance = spec.MaxTravelDistance;
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            float deltaTime =
                (float)gameTime.ElapsedGameTime.TotalSeconds;

            travelledDistance += speed * deltaTime;
        }
        public override Rectangle Bounds
        {
            get
            {
                int width = (int)(FrameWidth * scale * 0.5f);
                int height = (int)(FrameHeight * scale * 0.5f);

                return new Rectangle(
                    (int)(Position.X - width / 2f),
                    (int)(Position.Y - height / 2f),
                    width,
                    height);
            }
        }
    }
}
