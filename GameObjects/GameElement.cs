using Microsoft.Xna.Framework;

namespace StopTheBoats.GameObjects
{
    public enum FrictionMedium
    {
        Air,
        Water,
        None
    }

    public class GameElement : Sprite
    {
        private const float FrictionAir = 0.8f;
        private const float FrictionWater = 0.5f;
        private const float VelocityEpsilon = 1e-10f * 1e-10f;
        protected readonly FrictionMedium Medium;

        public GameElement(FrictionMedium medium)
        {
            this.Medium = medium;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            float frictionCoeff;
            switch (this.Medium)
            {
                case FrictionMedium.Air:
                    frictionCoeff = 0.8f;
                    break;
                case FrictionMedium.Water:
                    frictionCoeff = 0.5f;
                    break;
                default:
                    frictionCoeff = 1.0f;
                    break;
            }

            // apply friction
            var friction = -this.Velocity * frictionCoeff;
            this.Velocity += friction * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (this.Velocity.LengthSquared() < VelocityEpsilon)
            {
                this.Velocity = Vector2.Zero;
            }
        }
    }
}
