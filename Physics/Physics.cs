using Microsoft.Xna.Framework;
using System.Collections.Generic;
using StopTheBoats.Common;

namespace StopTheBoats.Physics
{
    public interface IPhysicsObject<T> where T : IBounds
    {
        Vector2 Velocity { get; set; }

        T Bounds { get; }

        Transformation GetWorldTransform();

        void OnCollision(IPhysicsObject<T> entity, CollisionResult<T> collision);
    }

    public interface IBounds
    {
    }

    public class CollisionResult<T> where T : IBounds
    {
        public bool WillIntersect;
        public bool Intersecting;
        public Vector2 MinimumTranslationVector;
    }

    public interface ICollisionDetector<A, B> where A : IBounds where B : IBounds
    {
        CollisionResult<B> DetectCollision(GameTime gameTime, IPhysicsObject<A> target, IPhysicsObject<B> candidate);
    }

    /*public class Actor
    {
        private float mass;
        private float invMass;

        public float Restitution { get; set; }
        public Vector2 Velocity { get; set; }

        public float Mass
        {
            get { return this.mass; }
            set
            {
                this.mass = value;
                this.invMass = this.mass == 0 ? 0 : 1 / value;
            }
        }

        public float InverseMass
        {
            get { return this.invMass; }
        }
    }*/

    public class Physics<T> where T : IBounds
    {
        /*public void ResolveCollision(Actor a, Actor b)
        {
            var relativeVelocity = b.Velocity - a.Velocity;
            var normal = new Vector2(relativeVelocity.X, -relativeVelocity.Y);
            var velocityAlongNormal = Vector2.Dot(relativeVelocity, normal);
            if (velocityAlongNormal > 0)
            {
                return;
            }
            var e = Math.Min(a.Restitution, b.Restitution);
            var j = -(1 + e) * velocityAlongNormal;
            j /= a.InverseMass + b.InverseMass;

            var impulse = j * normal;
            a.Velocity -= a.InverseMass * impulse;
            b.Velocity += b.InverseMass * impulse;
        }*/

        private readonly ICollisionDetector<T, T> detector;
        private readonly List<IPhysicsObject<T>> actors = new List<IPhysicsObject<T>>();

        public Physics(ICollisionDetector<T, T> detector)
        {
            this.detector = detector;
        }

        public void Reset()
        {
            // clear everything and start again
            this.actors.Clear();
        }

        public void AddActor(IPhysicsObject<T> actor)
        {
            this.actors.Add(actor);
        }

        public void RemoveActor(IPhysicsObject<T> actor)
        {
            this.actors.Remove(actor);
        }

        public void DetectCollisions(GameTime gameTime)
        {
            if (this.actors.Count < 2) return;

            // this bit is dodgy! figure out a better way of doing it
            foreach (var actor in this.actors)
            {
                if (actor.Bounds == null) continue;
                (actor.Bounds as PolygonBounds).TransformPoints(actor.GetWorldTransform());
            }

            for (var i = 0; i < this.actors.Count - 1; i++)
            {
                var target = this.actors[i];
                if (target.Bounds == null) continue; // no physics information
                for (var j = i+1; j < this.actors.Count; j++)
                {
                    var candidate = this.actors[j];
                    if (candidate.Bounds == null) continue; // no physics information
                    var collision = this.detector.DetectCollision(gameTime, target, candidate);
                    if (collision != null)
                    {
                        target.OnCollision(candidate, collision);
                        candidate.OnCollision(target, collision);
                    }
                }
            }
            /*
            foreach (var target in this.actors)
            {
                if (target.Bounds == null) continue;
                foreach (var candidate in this.actors)
                {
                    if (candidate.Bounds == null) continue;
                    if (candidate != target)
                    {
                        var collision = this.detector.DetectCollision(target, candidate);
                        if (collision != null)
                        {
                            target.OnCollision(candidate, collision);
                            candidate.OnCollision(target, collision);
                        }
                    }
                }
            }
            */
        }
    }
}
