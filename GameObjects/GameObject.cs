using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using StopTheBoats.Graphics;
using System.Linq;

namespace StopTheBoats.GameObjects
{
    public interface IGameObject
    {
        event EventHandler DestroyedEvent;

        void OnDestroyed();

        IGameObject Parent { get; set; }

        Vector2 Position { get; set; }

        Vector2 GetWorldPoint(Vector2 localPoint);

        Vector2 GetLocalPoint(Vector2 worldPoint);

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
        public static bool DebugInfo = false;
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

        public virtual Vector2 GetWorldPoint(Vector2 localPoint)
        {
            return this.Position + localPoint;
        }

        public virtual Vector2 GetLocalPoint(Vector2 worldPoint)
        {
            return worldPoint - this.Position;
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
}
