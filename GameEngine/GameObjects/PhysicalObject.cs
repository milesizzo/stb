using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.GameObjects
{
    public abstract class PhysicalObject : AbstractObject
    {
        public static float AirFriction = 0.4f;
        public static float WaterFriction = 0.5f;

        private Fixture fixture;
        private readonly Shape shape;
        private readonly World world;
        public int NumCollisions;

        protected PhysicalObject(IGameContext context, World world, Shape shape) : base(context)
        {
            this.world = world;
            var body = new Body(world, bodyType: BodyType.Dynamic);
            this.Fixture = body.CreateFixture(shape);
            this.DestroyedEvent += (sender, e) =>
            {
                var parentAsPhysics = this.Parent as PhysicalObject;
                if (this.Parent == null)
                {
                    this.world.RemoveBody(this.Body);
                }
                else if (parentAsPhysics != null)
                {
                    parentAsPhysics.Body.DestroyFixture(this.Fixture);
                }
            };
        }

        public World Physics { get { return this.world; } }

        public override void AddChild(IGameObject obj)
        {
            base.AddChild(obj);
            var asPhysics = obj as PhysicalObject;
            if (asPhysics != null)
            {
                this.world.RemoveBody(asPhysics.Body);
                asPhysics.Fixture = this.Body.CreateFixture(asPhysics.Shape);
            }
        }

        public Fixture Fixture
        {
            get { return this.fixture; }
            set
            {
                this.fixture = value;
                this.fixture.UserData = this;
                this.fixture.OnCollision += OnFarseerCollision;
            }
        }

        public Body Body
        {
            get { return this.Fixture.Body; }
        }

        public float Mass
        {
            get { return this.Body.Mass; }
            set { this.Body.Mass = value; }
        }

        public override Vector2 Position
        {
            get { return this.Body.Position; }
            set { this.Body.Position = value; }
        }

        public Vector2 LinearVelocity
        {
            get { return this.Body.LinearVelocity; }
            set { this.Body.LinearVelocity = value; }
        }

        public float Rotation
        {
            get { return this.Body.Rotation; }
            set { this.Body.Rotation = value; }
        }

        public float AngularVelocity
        {
            get { return this.Body.AngularVelocity; }
            set { this.Body.AngularVelocity = value; }
        }

        public override Vector2 GetLocalPoint(Vector2 worldPoint)
        {
            return this.Body.GetLocalPoint(worldPoint);
        }

        public override Vector2 GetWorldPoint(Vector2 localPoint)
        {
            return this.Body.GetWorldPoint(localPoint);
        }

        public Shape Shape { get { return this.Fixture.Shape; } }

        public virtual bool HandleCollision(PhysicalObject other, Contact contact)
        {
            this.NumCollisions++;
            return true;
        }

        private static bool OnFarseerCollision(Fixture first, Fixture second, Contact contact)
        {
            var firstObj = first.UserData as PhysicalObject;
            var secondObj = second.UserData as PhysicalObject;
            /*if (firstObj == null || secondObj == null)
            {
                //throw new InvalidOperationException("Expected fixtures to be attached to a game object!");
                return true;
            }*/
            return firstObj.HandleCollision(secondObj, contact);
        }
    }
}
