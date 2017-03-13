using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using MonoGame.Extended;
using StopTheBoats.Common;
using StopTheBoats.Graphics;

namespace StopTheBoats.GameObjects
{
    public abstract class GameObject : IMovable
    {
        public static bool DebugInfo = true;
        private GameObject parent;
        private readonly List<GameObject> children = new List<GameObject>();
        private Transformation local;
        private Vector2 velocity = Vector2.Zero;
        protected bool AwaitingDeletion = false;
        public IGameContext Context;

        public bool IsAwaitingDeletion { get { return this.AwaitingDeletion; } }

        public virtual Vector2 Position
        {
            get { return this.local.Position; }
            set { this.local.Position = value; }
        }

        public virtual float Rotation
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
