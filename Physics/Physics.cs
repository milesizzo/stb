using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StopTheBoats.Physics
{
    public interface IPhysicsObject<T> where T : IBounds
    {
        Vector2 Velocity { get; set; }

        T Bounds { get; }

        Transformation GetWorldTransform();

        void OnCollision(List<CollisionResult<T>> collisions);
    }

    public interface IBounds
    {
    }

    public class CollisionResult<T> where T : IBounds
    {
        public IPhysicsObject<T> Entity;
        public bool WillIntersect;
        public bool Intersecting;
        public Vector2 MinimumTranslationVector;
    }

    public class PolygonBounds : IBounds
    {
        private readonly List<Vector2> points = new List<Vector2>();
        //private readonly List<Vector2> edges = new List<Vector2>();

        public PolygonBounds(IEnumerable<Vector2> points)
        {
            this.points.AddRange(points);
            //this.BuildEdges();
        }

        public PolygonBounds(params Vector2[] points) : this(points.AsEnumerable())
        {
        }

        public static List<Vector2> BuildEdges(List<Vector2> points)
        {
            Vector2 p1, p2;
            var edges = new List<Vector2>();
            for (var i = 0; i < points.Count; i++)
            {
                p1 = points[i];
                if (i + 1 >= points.Count)
                {
                    p2 = points[0];
                }
                else
                {
                    p2 = points[i + 1];
                }
                edges.Add(p2 - p1);
            }
            return edges;
        }

        /*public IReadOnlyList<Vector2> Edges
        {
            get { return this.edges.AsReadOnly(); }
        }*/

        public IReadOnlyList<Vector2> Points
        {
            get { return this.points.AsReadOnly(); }
        }

        public Vector2 Centre
        {
            get
            {
                float totalX = 0, totalY = 0;
                for (var i = 0; i < this.points.Count; i++)
                {
                    totalX += this.points[i].X;
                    totalY += this.points[i].Y;
                }
                return new Vector2(totalX / this.points.Count, totalY / this.points.Count);
            }
        }

        public List<Vector2> TransformPoints(Transformation transform)
        {
            return this.points.Select(p => transform.TransformVector(p)).ToList();
        }

        /*public List<Vector2> TransformEdges(Transformation transform)
        {
            return this.edges.Select(p => transform.TransformVector(p)).ToList();
        }*/

        /*public void Offset(Vector2 v)
        {
            this.Offset(v.X, v.Y);
        }

        public void Offset(float x, float y)
        {
            for (var i = 0; i < this.points.Count; i++)
            {
                var p = this.points[i];
                this.points[i] = new Vector2(p.X + x, p.Y + y);
            }
        }*/
    }

    public interface ICollisionDetector<A, B> where A : IBounds where B : IBounds
    {
        CollisionResult<B> DetectCollision(IPhysicsObject<A> target, IPhysicsObject<B> candidate);
    }

    public class PolygonCollidor : ICollisionDetector<PolygonBounds, PolygonBounds>
    {
        public CollisionResult<PolygonBounds> DetectCollision(IPhysicsObject<PolygonBounds> target, IPhysicsObject<PolygonBounds> candidate)
        {
            var worldA = target.GetWorldTransform();
            var worldB = candidate.GetWorldTransform();
            //var polygonA = target.Bounds;
            //var polygonB = candidate.Bounds;
            var pointsA = target.Bounds.TransformPoints(worldA);
            var pointsB = candidate.Bounds.TransformPoints(worldB);
            //var edgesA = target.Bounds.TransformEdges(worldA);
            //var edgesB = candidate.Bounds.TransformEdges(worldB);
            var edgesA = PolygonBounds.BuildEdges(pointsA);
            var edgesB = PolygonBounds.BuildEdges(pointsB);

            var velocity = target.Velocity;
            var result = new CollisionResult<PolygonBounds>
            {
                Entity = candidate,
                Intersecting = true,
                WillIntersect = true,
            };

            var edgeCountA = edgesA.Count;
            var edgeCountB = edgesB.Count;
            float minIntervalDistance = float.PositiveInfinity;
            var translationAxis = new Vector2();
            Vector2 edge;

            // Loop through all the edges of both polygons
            for (var edgeIndex = 0; edgeIndex < edgeCountA + edgeCountB; edgeIndex++)
            {
                if (edgeIndex < edgeCountA)
                {
                    edge = edgesA[edgeIndex];
                }
                else
                {
                    edge = edgesB[edgeIndex - edgeCountA];
                }

                // ===== 1. Find if the polygons are currently intersecting =====

                // Find the axis perpendicular to the current edge
                var axis = new Vector2(-edge.Y, edge.X);
                axis.Normalize();

                // Find the projection of the polygon on the current axis
                float minA = 0; float minB = 0; float maxA = 0; float maxB = 0;
                ProjectPolygon(axis, pointsA, ref minA, ref maxA);
                ProjectPolygon(axis, pointsB, ref minB, ref maxB);

                // Check if the polygon projections are currentlty intersecting
                if (IntervalDistance(minA, maxA, minB, maxB) > 0)
                {
                    result.Intersecting = false;
                }

                // ===== 2. Now find if the polygons *will* intersect =====

                // Project the velocity on the current axis
                var velocityProjection = Vector2.Dot(axis, velocity);

                // Get the projection of polygon A during the movement
                if (velocityProjection < 0)
                {
                    minA += velocityProjection;
                }
                else
                {
                    maxA += velocityProjection;
                }

                // Do the same test as above for the new projection
                var intervalDistance = IntervalDistance(minA, maxA, minB, maxB);
                if (intervalDistance > 0)
                {
                    result.WillIntersect = false;
                }

                // If the polygons are not intersecting and won't intersect, exit the loop
                if (!result.Intersecting && !result.WillIntersect)
                {
                    break;
                }

                // Check if the current interval distance is the minimum one. If so store
                // the interval distance and the current distance.
                // This will be used to calculate the minimum translation vector
                intervalDistance = Math.Abs(intervalDistance);
                if (intervalDistance < minIntervalDistance)
                {
                    minIntervalDistance = intervalDistance;
                    translationAxis = axis;
                    var d = target.Bounds.Centre - candidate.Bounds.Centre;
                    if (Vector2.Dot(d, translationAxis) < 0) translationAxis = -translationAxis;
                }
            }

            // The minimum translation vector can be used to push the polygons appart.
            // First moves the polygons by their velocity
            // then move polygonA by MinimumTranslationVector.
            if (result.WillIntersect)
            {
                result.MinimumTranslationVector = translationAxis * minIntervalDistance;
            }

            return result;
        }

        private static float IntervalDistance(float minA, float maxA, float minB, float maxB)
        {
            if (minA < minB)
            {
                return minB - maxA;
            }
            return minA - maxB;
        }

        private static void ProjectPolygon(Vector2 axis, List<Vector2> points, ref float min, ref float max)
        {
            var d = Vector2.Dot(axis, points[0]);
            min = d;
            max = d;
            for (var i = 0; i < points.Count; i++)
            {
                d = Vector2.Dot(points[i], axis);
                if (d < min)
                {
                    min = d;
                }
                else if (d > max)
                {
                    max = d;
                }
            }
        }
    }

    public class Actor
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
    }

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

        public void AddActor(IPhysicsObject<T> actor)
        {
            this.actors.Add(actor);
        }

        public void RemoveActor(IPhysicsObject<T> actor)
        {
            this.actors.Remove(actor);
        }

        public void DetectCollisions()
        {
            foreach (var target in this.actors)
            {
                if (target.Bounds == null) continue;
                List<CollisionResult<T>> collisions = null;
                foreach (var candidate in this.actors)
                {
                    if (candidate.Bounds == null) continue;
                    if (candidate != target)
                    {
                        var collision = this.detector.DetectCollision(target, candidate);
                        if (collision.Intersecting || collision.WillIntersect)
                        {
                            if (collisions == null) collisions = new List<CollisionResult<T>>();
                            collisions.Add(collision);
                        }
                    }
                }
                target.OnCollision(collisions);
            }
        }
    }
}
