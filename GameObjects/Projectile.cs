using Microsoft.Xna.Framework;
using MonoGame.Extended;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics.Contacts;
using GameEngine.GameObjects;
using GameEngine.Graphics;

namespace StopTheBoats.GameObjects
{
    public class Projectile : PhysicalObject
    {
        private readonly float damage;
        private readonly IGameObject owner;
        private readonly float maxVelocity;

        public Projectile(World world, PhysicalObject owner, float damage, float maxVelocity) : base(world, new CircleShape(5f, 1f))
        {
            this.damage = damage;
            this.maxVelocity = maxVelocity;
            this.owner = owner;
            this.Body.IgnoreCollisionWith(owner.Body);
            this.Body.AngularDamping = this.Body.LinearDamping = PhysicalObject.AirFriction;
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

        public override bool HandleCollision(PhysicalObject other, Contact contact)
        {
            var asProjectile = other as Projectile;
            if (asProjectile == null)
            {
                this.IsAwaitingDeletion = true;
                this.Context.Assets.Audio["Audio/explosion1"].Audio.Play(0.1f, 0, 0);
                var explosion = new AttachedObject(this.Context.Assets.Sprites["explosion_sheet1"]);
                other.AddChild(explosion);
                explosion.Position = this.Position;
                explosion.DeleteAfterAnimation = true;
                return false;
            }
            return false;
        }
    }
}
