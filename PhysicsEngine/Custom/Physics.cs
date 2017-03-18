using Microsoft.Xna.Framework;
using System.Collections.Generic;
using CommonLibrary;
using MonoGame.Extended;
using System.Linq;

namespace CustomPhysicsEngine
{
    public class ForceOnActor
    {
        public Vector2 Force;
        public Vector2 ContactPoint;
    }

    public interface IActor<T> where T : IBounds
    {
        Vector2 Position { get; set; }

        Vector2 Velocity { get; set; }

        float Angle { get; set; }

        float AngularVelocity { get; set; }

        float Mass { get; }

        T Bounds { get; }

        Transformation GetWorldTransform();

        void OnCollision(IActor<T> entity, CollisionResult<T> collision);
    }

    public interface INewActor<T> : IActor<T> where T : IBounds
    {
        IEnumerable<ForceOnActor> Force { get; }
    }

    public interface IBounds
    {
        Vector2 Centre { get; }

        float MomentOfInertia { get; }
    }

    public class CollisionResult<T> where T : IBounds
    {
        public bool WillIntersect;
        public bool Intersecting;
        public Vector2 MinimumTranslationVector;
    }

    public interface ICollisionDetector<A, B> where A : IBounds where B : IBounds
    {
        CollisionResult<B> DetectCollision(GameTime gameTime, IActor<A> target, IActor<B> candidate);
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
        private readonly List<IActor<T>> actors = new List<IActor<T>>();

        public Physics(ICollisionDetector<T, T> detector)
        {
            this.detector = detector;
        }

        public IReadOnlyList<IActor<T>> Actors
        {
            get { return this.actors.AsReadOnly(); }
        }

        public void Reset()
        {
            // clear everything and start again
            this.actors.Clear();
        }

        public void AddActor(IActor<T> actor)
        {
            this.actors.Add(actor);
        }

        public void RemoveActor(IActor<T> actor)
        {
            this.actors.Remove(actor);
        }

        public float CalculateTorque(IActor<T> actor, Vector2 contactPoint, Vector2 force)
        {
            var r = contactPoint;
            return r.X * force.X - r.Y * force.Y;
        }

        public float CalculateAngularAcceleration(IActor<T> actor, Vector2 contactPoint, Vector2 force)
        {
            // calculates angular acceleration due to a force
            return this.CalculateTorque(actor, contactPoint, force) / actor.Bounds.MomentOfInertia;
        }

        public void Update(GameTime gameTime)
        {
            var dt = gameTime.GetElapsedSeconds();
            foreach (var actor in this.actors.OfType<INewActor<T>>())
            {
                var linearAcceleration = Vector2.Zero;
                var angularAcceleration = 0f;

                // this bit is dodgy! figure out a better way of doing it
                (actor.Bounds as PolygonBounds).TransformPoints(actor.GetWorldTransform());
                /*
                foreach (var other in this.actors)
                {
                    if (actor == other) continue;
                    var collision = this.detector.DetectCollision(gameTime, actor, other);
                    if (collision.WillIntersect)
                    {
                        var force = collision.MinimumTranslationVector;
                        // F = ma, a = F/m
                        //this.CalculateAngularAcceleration(actor, )
                    }
                }
                */
                foreach (var force in actor.Force)
                {
                    var f = actor.Position - actor.GetWorldTransform().TransformVector(force.Force);
                    f.Y = -f.Y;
                    linearAcceleration += f / actor.Mass;
                    f.Y = -f.Y;
                    angularAcceleration += this.CalculateAngularAcceleration(actor, force.ContactPoint, f);
                }

                actor.Velocity += linearAcceleration * dt;
                actor.Position += actor.Velocity * dt;
                actor.AngularVelocity += angularAcceleration * dt;
                actor.Angle += actor.AngularVelocity * dt;
            }
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
                        if (collision.WillIntersect)
                        {
                            //
                        }
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
