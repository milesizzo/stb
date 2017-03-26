using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using GameEngine.Graphics;
using GameEngine.Content;

namespace GameEngine.GameObjects
{
    public class GameContext : IGameContext
    {
        private class ScheduledObject
        {
            public float TimeRemaining;
            public IGameObject Object;
        }

        private readonly List<IGameObject> objects = new List<IGameObject>();
        private readonly Store store;
        private readonly List<ScheduledObject> scheduled = new List<ScheduledObject>();

        public GameContext(Store store)
        {
            this.store = store;
        }

        public void Reset()
        {
            foreach (var obj in this.objects)
            {
                obj.Context = null;
            }
            this.objects.Clear();
            this.scheduled.Clear();
        }

        public Store Store { get { return this.store; } }

        public int NumObjects { get { return this.objects.Count; } }

        public void ScheduleObject(IGameObject obj, float waitTime)
        {
            this.scheduled.Add(new ScheduledObject
            {
                TimeRemaining = waitTime,
                Object = obj,
            });
        }

        public void AddObject(IGameObject obj)
        {
            if (obj.Parent != null)
            {
                throw new InvalidOperationException("You cannot add a child object");
            }
            obj.Context = this;
            this.objects.Add(obj);
        }

        public void RemoveObject(IGameObject obj)
        {
            this.RemoveObject(this.objects.IndexOf(obj));
        }

        private void RemoveObject(int index)
        {
            var obj = this.objects[index];
            if (obj.Parent != null)
            {
                throw new InvalidOperationException("You cannot remove a child object");
            }
            obj.Context = null;
            this.objects.RemoveAt(index);
            obj.OnDestroyed();
        }

        public void Update(GameTime gameTime)
        {
            var i = 0;
            while (i < this.scheduled.Count)
            {
                var schedule = this.scheduled[i];
                schedule.TimeRemaining -= gameTime.GetElapsedSeconds();
                if (schedule.TimeRemaining < 0)
                {
                    this.AddObject(schedule.Object);
                    this.scheduled.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }

            i = 0;
            while (i < this.objects.Count)
            {
                var obj = this.objects[i];
                if (obj.IsAwaitingDeletion)
                {
                    this.RemoveObject(i);
                }
                else
                {
                    obj.Update(gameTime);
                    i++;
                }
            }
        }

        public void Draw(Renderer renderer)
        {
            foreach (var obj in this.objects)
            {
                obj.Draw(renderer);
            }
        }
    }
}
