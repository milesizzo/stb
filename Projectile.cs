using System;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using Microsoft.Xna.Framework;
using StopTheBoats.Physics;
using System.Collections.Generic;
using System.Linq;

namespace StopTheBoats
{
    public class Projectile : GameElement, IPhysicsObject<PolygonBounds>
    {
        private readonly float damage;
        private readonly GameObject owner;
        private readonly float maxVelocity;

        public Projectile(GameObject owner, float damage, float maxVelocity) : base(FrictionMedium.Air)
        {
            this.damage = damage;
            this.maxVelocity = maxVelocity;
            this.owner = owner;
            this.Bounds = new PolygonBounds(
                new Vector2(-2.5f, -2.5f),
                new Vector2(2.5f, -2.5f),
                new Vector2(2.5f, 2.5f),
                new Vector2(-2.5f, 2.5f));
        }

        public float Damage
        {
            get
            {
                return this.damage * this.Velocity.Length() / this.maxVelocity;
            }
        }

        public GameObject Owner { get { return this.owner; } }

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
            var world = this.World;
            var points = this.Bounds.Points.Select(p => world.TransformVector(p));
            render.Render.DrawPolygon(Vector2.Zero, points.ToArray(), this.physicsColour);
        }

        public override void OnCollision(List<CollisionResult<PolygonBounds>> collisions)
        {
            base.OnCollision(collisions);
            if (collisions == null) return;
            foreach (var collision in collisions)
            {
                var asProjectile = collision.Entity as Projectile;
                if (collision.Intersecting && collision.Entity != this.owner && (asProjectile == null || asProjectile.owner != this.owner))
                {
                    // hit?
                    this.AwaitingDeletion = true;
                }
            }
        }
    }
}
