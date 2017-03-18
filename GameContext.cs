using Microsoft.Xna.Framework;
using MonoGame.Extended;
using StopTheBoats.GameObjects;
using StopTheBoats.Graphics;
using System.Collections.Generic;
using System;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using System.Linq;

namespace StopTheBoats
{
    public class GameContext : IGameContext
    {
        private class ScheduledObject
        {
            public float TimeRemaining;
            public IGameObject Object;
        }

        private readonly List<IGameObject> objects = new List<IGameObject>();
        private readonly GameAssetStore assets;
        private readonly List<ScheduledObject> scheduled = new List<ScheduledObject>();

        public GameContext(GameAssetStore assets)
        {
            this.assets = assets;
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

        public GameAssetStore Assets { get { return this.assets; } }

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

    public static class WorldExtensions
    {
        private static void DrawShape(Renderer renderer, Shape shape, Transform transform, Color colour)
        {
            switch (shape.ShapeType)
            {
                case ShapeType.Circle:
                    var circle = shape as CircleShape;
                    var centre = MathUtils.Mul(ref transform, circle.Position);
                    renderer.Render.DrawCircle(centre, circle.Radius, 16, colour);
                    break;
                case ShapeType.Polygon:
                    var polygon = shape as PolygonShape;
                    var transformedPoints = polygon.Vertices.Select(v => MathUtils.Mul(ref transform, v));
                    renderer.Render.DrawPolygon(Vector2.Zero, transformedPoints.ToArray(), colour);
                    break;
                default:
                    throw new NotImplementedException($"Cannot draw shapes of Farseer type {shape.ShapeType}");
            }
        }

        public static void Draw(this World world, Renderer renderer)
        {
            foreach (var body in world.BodyList)
            {
                Transform transform;
                body.GetTransform(out transform);
                foreach (var fixture in body.FixtureList)
                {
                    DrawShape(renderer, fixture.Shape, transform, Color.White);
                }
                renderer.Render.DrawPoint(body.Position, Color.Yellow, size: 3f);
            }
        }
    }
}
