using MonoGame.Extended.Shapes;
using Microsoft.Xna.Framework;
using System.Linq;
using StopTheBoats.Physics;
using StopTheBoats.Graphics;

namespace StopTheBoats.GameObjects
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

        public override void Draw(Renderer renderer)
        {
            var length = this.Velocity.LengthSquared();
            Color colour = new Color(0.5f, 0.5f, 0.5f);
            if (length < 500 * 500)
            {
                colour = new Color(0.5f, 0.5f, 0.5f, (length / (500 * 500)));
            }
            renderer.Render.DrawCircle(this.Position, 5f, 12, colour, 1.5f);
            if (GameObject.DebugInfo)
            {
                var world = this.World;
                var points = this.Bounds.Points.Select(p => world.TransformVector(p));
                renderer.Render.DrawPolygon(Vector2.Zero, points.ToArray(), this.physicsColour);
            }
        }

        public override void OnCollision(IPhysicsObject<PolygonBounds> entity, CollisionResult<PolygonBounds> collision)
        {
            //base.OnCollision(entity, collision);
            var asProjectile = entity as Projectile;
            if (collision.Intersecting && entity != this.owner && (asProjectile == null || asProjectile.owner != this.owner))
            {
                // hit?
                this.AwaitingDeletion = true;
                this.Context.AddObject(new GameElement(FrictionMedium.Air)
                {
                    Position = this.Position,
                    SpriteTemplate = this.Context.Assets.Sprites["explosion_sheet1"],
                    Bounds = null,
                    DeleteAfterAnimation = true,
                });
            }
        }
    }
}
