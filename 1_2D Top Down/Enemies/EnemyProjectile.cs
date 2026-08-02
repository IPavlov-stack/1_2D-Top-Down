using _1_2D_Top_Down;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _1_2D_Top_Down
{

    public class EnemyProjectile : Projectile
    {
        private const float MaxTravelDistance = 700f;
        private float travelledDistance;
        public bool HasReachedMaxTravelDistance =>  travelledDistance >= MaxTravelDistance;
        public EnemyProjectile(
            Texture2D texture,
            Vector2 startPosition,
            Vector2 direction)
            : base(
                texture,
                startPosition,
                direction,
                speed: 325f,
                scale: 1.75f,
                frameCount: 11,
                frameRows: 1,
                frameDuration: 0.04f)
        {

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