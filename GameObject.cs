using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Shapes;

namespace StopTheBoats
{
    public abstract class GameObject : IMovable, ICollidable, IActorTarget
    {
        private GameObject parent;
        private readonly List<GameObject> children = new List<GameObject>();
        private Transformation local;
        private Vector2 velocity = Vector2.Zero;
        protected bool AwaitingDeletion = false;

        public bool IsAwaitingDeletion { get { return this.AwaitingDeletion; } }

        public abstract RectangleF BoundingBox { get; }

        public Vector2 Position
        {
            get { return this.local.Position; }
            set { this.local.Position = value; }
        }

        public float Rotation
        {
            get { return this.local.Rotation; }
            set { this.local.Rotation = value; }
        }

        public float WorldRotation
        {
            get { return this.World.Rotation; }
            set
            {
                this.local.Rotation = value;
                this.local.Rotation -= this.parent.Rotation;
            }
        }

        public Vector2 Velocity
        {
            get { return this.velocity; }
            set { this.velocity = value; }
        }

        public void OnCollision(CollisionInfo collisionInfo)
        {
            //
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

        public Transformation World
        {
            get
            {
                return this.parent == null ? this.local : Transformation.Compose(this.parent.World, this.local);
            }
        }

        public abstract void Draw(RenderStore render);
    }
}
