using System;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using Microsoft.Xna.Framework;

namespace StopTheBoats
{
    public class Projectile : GameElement
    {
        public Projectile() : base(FrictionMedium.Air)
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            var length = this.Velocity.LengthSquared();
            if (length < 1000)
            {
                this.AwaitingDeletion = true;
            }
        }

        public override void Draw(RenderStore render)
        {
            var length = this.Velocity.LengthSquared();
            Color colour = new Color(0.5f, 0.5f, 0.5f);
            if (length < 500 * 500)
            {
                colour = new Color(0.5f, 0.5f, 0.5f, (length / (500 * 500)));
            }
            render.Render.DrawCircle(this.Position, 5f, 12, colour, 1.5f);
        }
    }
}
