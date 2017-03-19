﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using GameEngine.Graphics;

namespace GameEngine.GameObjects
{
    public abstract class AbstractObject : IGameObject
    {
        public static bool DebugInfo = false;
        private readonly List<IGameObject> children = new List<IGameObject>();
        private IGameObject parent;
        private IGameContext context;

        protected AbstractObject(IGameContext context)
        {
            this.parent = null;
            this.context = context;
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
