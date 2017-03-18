using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using MonoGame.Extended;
using StopTheBoats.Graphics;
using CommonLibrary;
using PhysicsEngine;
using System.Linq;

namespace StopTheBoats.GameObjects
{
    public interface IGameObject
    {
        event EventHandler DestroyedEvent;

        void OnDestroyed();

        IGameObject Parent { get; set; }

        Vector2 Position { get; set; }

        //Vector2 LinearVelocity { get; set; }

        //float Rotation { get; set; }

        //float AngularVelocity { get; set; }

        IGameContext Context { get; set; }

        bool IsAwaitingDeletion { get; }

        void Update(GameTime gameTime);

        void Draw(Renderer renderer);

        void AddChild(IGameObject obj);

        void RemoveChild(IGameObject obj);

        IEnumerable<IGameObject> Children { get; }
    }

    public abstract class AbstractObject : IGameObject
    {
        private readonly List<IGameObject> children = new List<IGameObject>();
        private IGameObject parent;
        private IGameContext context;

        protected AbstractObject()
        {
            this.parent = null;
            this.context = null;
            this.IsAwaitingDeletion = false;
            this.DestroyedEvent += (sender, e) =>
            {
                // destroy children too
                while (this.children.Any())
                {
                    this.RemoveChild(this.children.First());
                }
            };
        }

        public void OnDestroyed()
        {
            this.DestroyedEvent(this, EventArgs.Empty);
        }

        public event EventHandler DestroyedEvent;

        public IGameObject Parent
        {
            get { return this.parent; }
            set { this.parent = value; }
        }

        public abstract Vector2 Position { get; set; }

        public IGameContext Context
        {
            get { return this.context; }
            set { this.context = value; }
        }

        public bool IsAwaitingDeletion { get; protected set; }

        public IEnumerable<IGameObject> Children
        {
            get { return this.children; }
        }

        public virtual void AddChild(IGameObject obj)
        {
            obj.Parent = this;
            obj.Context = this.Context;
            this.children.Add(obj);
        }

        public void RemoveChild(IGameObject obj)
        {
            obj.Parent = null;
            obj.Context = null;
            this.children.Remove(obj);
            obj.OnDestroyed();
        }

        public virtual void Update(GameTime gameTime)
        {
            foreach (var child in this.children)
            {
                child.Update(gameTime);
            }
        }

        public virtual void Draw(Renderer renderer)
        {
            foreach (var child in this.children)
            {
                child.Draw(renderer);
            }
        }
    }

    public abstract class PhysicalObject : AbstractObject
    {
        private IFixture fixture;
        private readonly PhysicsShape shape;
        private readonly IPhysicsEngine physics;

        protected PhysicalObject(IPhysicsEngine physics, PhysicsShape shape)
        {
            this.physics = physics;
            IFixture fixture;
            this.physics.CreateFixture(shape, out fixture);
            this.fixture = fixture;
            /*IFixture fixture;
            physics.CreateFixture(parent.Body, shape, out fixture);
            this.fixture = fixture;*/
            this.shape = shape;
            this.DestroyedEvent += (sender, e) =>
            {
                var parentAsPhysics = this.Parent as PhysicalObject;
                if (this.Parent == null)
                {
                    this.physics.RemoveBody(this.Body);
                }
                else if (parentAsPhysics != null)
                {
                    parentAsPhysics.Body.RemoveFixture(this.Fixture);
                }
            };
        }

        public IPhysicsEngine Physics { get { return this.physics; } }

        public override void AddChild(IGameObject obj)
        {
            base.AddChild(obj);
            var asPhysics = obj as PhysicalObject;
            if (asPhysics != null)
            {
                this.physics.RemoveBody(asPhysics.Body);
                asPhysics.Fixture = this.Body.AddFixture(asPhysics.Shape);
            }
        }

        public IFixture Fixture
        {
            get { return this.fixture; }
            set { this.fixture = value; }
        }

        public IBody Body
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

        public PhysicsShape Shape { get { return this.shape; } }
    }

    public abstract class GameObject : IMovable
    {
        public static bool DebugInfo = false;
        private GameObject parent;
        private readonly List<GameObject> children = new List<GameObject>();
        private Transformation local;
        private float angularVelocity = 0f;
        private Vector2 velocity = Vector2.Zero;
        protected bool AwaitingDeletion = false;
        private IGameContext context = null;

        public bool IsAwaitingDeletion { get { return this.AwaitingDeletion; } }

        public IGameContext Context
        {
            get
            {
                if (this.context != null) return this.context;
                if (this.parent != null)
                {
                    var parent = this.parent;
                    while (parent != null)
                    {
                        if (parent.context != null) return parent.context;
                        parent = parent.parent;
                    }
                }
                return null;
            }
            set
            {
                this.context = value;
            }
        }

        public virtual Vector2 Position
        {
            get { return this.local.Position; }
            set { this.local.Position = value; }
        }

        public virtual float Angle
        {
            get { return this.local.Rotation; }
            set { this.local.Rotation = value; }
        }

        public float AngularVelocity
        {
            get { return this.angularVelocity; }
            set { this.angularVelocity = value; }
        }

        public float WorldRotation
        {
            get { return this.World.Rotation; }
            set
            {
                this.local.Rotation = value;
                this.local.Rotation -= this.parent.Angle;
            }
        }

        protected GameObject Parent { get { return this.parent; } }

        public Vector2 Velocity
        {
            get { return this.velocity; }
            set { this.velocity = value; }
        }

        public IList<GameObject> Children
        {
            get { return this.children; }
        }

        public GameObject()
        {
            this.local = new Transformation();
        }

        public virtual void Update(GameTime gameTime)
        {
            this.Position += this.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            this.children.RemoveAll(o => o.IsAwaitingDeletion);
            // TODO: call Update on child objects?
        }

        public void AddChild(GameObject obj)
        {
            if (obj.parent != null)
            {
                throw new InvalidOperationException("GameObject already has a parent");
            }
            obj.parent = this;
            obj.Context = this.Context;
            this.children.Add(obj);
        }

        public void RemoveChild(GameObject obj)
        {
            if (obj.parent != this)
            {
                throw new InvalidOperationException("GameObject parent does not match this");
            }
            this.children.Remove(obj);
            obj.parent = null;
            obj.Context = null;
        }

        /*
        public static void DecomposeMatrix(ref Matrix matrix, out Vector2 position, out float rotation, out Vector2 scale)
        {
            Vector3 position3, scale3;
            Quaternion rotationQ;
            matrix.Decompose(out scale3, out rotationQ, out position3);
            var direction = Vector2.Transform(Vector2.UnitX, rotationQ);
            rotation = (float)Math.Atan2(direction.Y, direction.X);
            position = new Vector2(position3.X, position3.Y);
            scale = new Vector2(scale3.X, scale3.Y);
        }
        */

        public Transformation Local
        {
            get { return this.local; }
        }

        public Transformation World
        {
            get
            {
                return this.parent == null ? this.local : Transformation.Compose(this.parent.World, this.local);
            }
        }

        public abstract void Draw(Renderer renderer);
    }
}
