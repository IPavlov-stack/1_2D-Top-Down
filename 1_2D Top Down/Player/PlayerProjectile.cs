using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _1_2D_Top_Down
{
    public class PlayerProjectile : Projectile
    {
        private const int ProjectileHitboxWidth = 40;
        private const int ProjectileHitboxHeight = 40;

        public override Rectangle Bounds
        {
            get
            {
                return new Rectangle(
                    (int)(
                        Position.X -
                        ProjectileHitboxWidth / 2f),

                    (int)(
                        Position.Y -
                        ProjectileHitboxHeight / 2f),

                    ProjectileHitboxWidth,
                    ProjectileHitboxHeight);
            }
        }

        public PlayerProjectile(
            Texture2D texture,
            Vector2 startPosition,
            Vector2 direction,
            float speed,
            int damage,
            float knockback)
            : base(
                texture,
                startPosition,
                direction,
                speed: speed,
                scale: 1.0f,
                frameCount: 3,
                frameRows: 1,
                frameDuration: 0.08f,
                faction: CombatFaction.Player,
                damage: damage,
                damageType: DamageType.Arcane,
                knockback: knockback,
                isReflectable: false,
                sourceId: "player")
        {
        }
    }
}
