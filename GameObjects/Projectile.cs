using MonoGame.Extended.Shapes;
using Microsoft.Xna.Framework;
using System.Linq;
using StopTheBoats.Graphics;
using PhysicsEngine;
using System;
using MonoGame.Extended;

namespace StopTheBoats.GameObjects
{
    public class Projectile : PhysicalObject
    {
        private readonly float damage;
        private readonly IGameObject owner;
        private readonly float maxVelocity;

        public Projectile(IPhysicsEngine physics, IGameObject owner, float damage, float maxVelocity) : base(physics, new PhysicsCircle(5f, 1f))
        {
            this.damage = damage;
            this.maxVelocity = maxVelocity;
            this.owner = owner;
        }

        public float Damage
        {
            get
            {
                return this.damage * this.LinearVelocity.Length() / this.maxVelocity;
            }
        }

        public IGameObject Owner { get { return this.owner; } }

        public override void Update(GameTime gameTime)
        {
            //base.Update(gameTime);
            var length = this.LinearVelocity.LengthSquared();
            if (length < 1000)
            {
                this.IsAwaitingDeletion = true;
            }
            base.Update(gameTime);
        }

        public override void Draw(Renderer renderer)
        {
            var length = this.LinearVelocity.LengthSquared();
            Color colour = new Color(0.5f, 0.5f, 0.5f);
            if (length < 500 * 500)
            {
                colour = new Color(0.5f, 0.5f, 0.5f, (length / (500 * 500)));
            }
            renderer.Render.DrawCircle(this.Position, 5f, 12, colour, 1.5f);
            /*if (GameObject.DebugInfo)
            {
                var world = this.World;
                var points = this.Bounds.Points.Select(p => world.TransformVector(p));
                renderer.Render.DrawPolygon(Vector2.Zero, points.ToArray(), this.physicsColour);
            }*/
            base.Draw(renderer);
        }

        /*
        public override void OnCollision(IActor<PolygonBounds> entity, CollisionResult<PolygonBounds> collision)
        {
            //base.OnCollision(entity, collision);
            var asProjectile = entity as Projectile;
            if (collision.Intersecting && entity != this.owner && (asProjectile == null || asProjectile.owner != this.owner))
            {
                // hit?
                this.AwaitingDeletion = true;
                this.Context.Assets.Audio["Audio/explosion1"].Audio.Play(0.1f, 0, 0);
                this.Context.AddObject(new GameElement(FrictionMedium.Air)
                {
                    Position = this.Position,
                    SpriteTemplate = this.Context.Assets.Sprites["explosion_sheet1"],
                    Bounds = null,
                    DeleteAfterAnimation = true,
                });
            }
        }
        */
    }
}
