using Microsoft.Xna.Framework;
using MonoGame.Extended;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics.Contacts;
using GameEngine.GameObjects;
using GameEngine.Graphics;
using GameEngine.Extensions;

namespace StopTheBoats.GameObjects
{
    public class Projectile : PhysicalObject
    {
        private readonly float damage;
        private readonly IGameObject owner;
        private readonly float maxVelocity;

        public Projectile(IGameContext context, World world, PhysicalObject owner, float damage, float maxVelocity)
            : base(context, world, new CircleShape(5f, 1f))
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
            renderer.World.DrawCircle(this.Position, 5f, 12, colour, 1.5f);

            if (AbstractObject.DebugInfo)
            {
                var envy = this.Context.Store.Fonts("Base", "envy12");
                renderer.World.DrawString(envy, $"damage: {this.Damage}", this.Position - new Vector2(0, -16), Color.White);
            }
            base.Draw(renderer);
        }

        public override bool HandleCollision(PhysicalObject other, Contact contact)
        {
            base.HandleCollision(other, contact);
            var asProjectile = other as Projectile;
            if (asProjectile == null && other != null)
            {
                this.IsAwaitingDeletion = true;
                this.Context.Store.Audio("StopTheBoats", "explosion1").Audio.Play(0.1f, 0, 0);
                var explosion = new AttachedObject(this.Context, this.Context.Store.Sprites("StopTheBoats", "explosion_sheet1"));
                other.AddChild(explosion);
                explosion.Position = this.Position;
                explosion.DeleteAfterAnimation = true;
                return false;
            }
            return false;
        }
    }
}
