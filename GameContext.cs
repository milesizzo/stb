using Microsoft.Xna.Framework;
using MonoGame.Extended;
using StopTheBoats.GameObjects;
using StopTheBoats.Graphics;
using PhysicsEngine;
using System.Collections.Generic;

namespace StopTheBoats
{
    public class GameContext : IGameContext
    {
        private class ScheduledObject
        {
            public float TimeRemaining;
            public GameObject Object;
        }

        private readonly List<GameObject> objects = new List<GameObject>();
        private readonly Physics<PolygonBounds> physics;
        private readonly GameAssetStore assets;
        private readonly List<ScheduledObject> scheduled = new List<ScheduledObject>();

        public GameContext(GameAssetStore assets)
        {
            this.physics = new Physics<PolygonBounds>(new PolygonDetector());
            this.assets = assets;
        }

        public void Reset()
        {
            this.physics.Reset();
            foreach (var obj in this.objects)
            {
                obj.Context = null;
            }
            this.objects.Clear();
            this.scheduled.Clear();
        }

        public GameAssetStore Assets { get { return this.assets; } }

        public int NumObjects { get { return this.objects.Count; } }

        public void ScheduleObject(GameObject obj, float waitTime)
        {
            this.scheduled.Add(new ScheduledObject
            {
                TimeRemaining = waitTime,
                Object = obj,
            });
        }

        public void AddObject(GameObject obj)
        {
            obj.Context = this;
            this.objects.Add(obj);
            var asPhysics = obj as IPhysicsObject<PolygonBounds>;
            if (asPhysics != null)
            {
                this.physics.AddActor(asPhysics);
            }
        }

        private void RemovePhysics(GameObject obj)
        {
            var asPhysics = obj as IPhysicsObject<PolygonBounds>;
            if (asPhysics != null)
            {
                this.physics.RemoveActor(asPhysics);
            }
        }

        public void RemoveObject(GameObject obj)
        {
            obj.Context = null;
            this.objects.Remove(obj);
            this.RemovePhysics(obj);
        }

        public void Update(GameTime gameTime)
        {
            this.physics.DetectCollisions(gameTime);
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
            foreach (var obj in this.objects)
            {
                obj.Update(gameTime);
                if (obj.IsAwaitingDeletion)
                {
                    this.RemovePhysics(obj);
                }
            }
            this.objects.RemoveAll(o => o.IsAwaitingDeletion);
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
