using Microsoft.Xna.Framework;

namespace _1_2D_Top_Down
{
    public class Knockback
    {
        private Vector2 velocity;
        private readonly float deceleration;

        public Knockback(float deceleration)
        {
            this.deceleration = deceleration;
        }

        public void Apply(Vector2 direction, float force)
        {
            if (direction == Vector2.Zero)
                return;

            direction.Normalize();
            velocity = direction * force;
        }

        public Vector2 Update(GameTime gameTime)
        {
            float deltaTime =
                (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 movement = velocity * deltaTime;

            velocity = Vector2.Lerp(
                velocity,
                Vector2.Zero,
                MathHelper.Clamp(deceleration * deltaTime, 0f, 1f));

            return movement;
        }
    }
}