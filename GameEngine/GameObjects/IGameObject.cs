using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using GameEngine.Graphics;

namespace GameEngine.GameObjects
{
    public interface IGameObject
    {
        event EventHandler DestroyedEvent;

        void OnDestroyed();

        IGameObject Parent { get; set; }

        Vector2 Position { get; set; }

        Vector2 GetWorldPoint(Vector2 localPoint);

        Vector2 GetLocalPoint(Vector2 worldPoint);

        IGameContext Context { get; set; }

        bool IsAwaitingDeletion { get; }

        void Update(GameTime gameTime);

        void Draw(Renderer renderer);

        void AddChild(IGameObject obj);

        void RemoveChild(IGameObject obj);

        IEnumerable<IGameObject> Children { get; }
    }
}
