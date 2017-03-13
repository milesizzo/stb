using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StopTheBoats.Physics
{
    public class PolygonDetector : ICollisionDetector<PolygonBounds, PolygonBounds>
    {
        public CollisionResult<PolygonBounds> DetectCollision(GameTime gameTime, IPhysicsObject<PolygonBounds> target, IPhysicsObject<PolygonBounds> candidate)
        {
            var worldA = target.GetWorldTransform();
            var worldB = candidate.GetWorldTransform();
            /*Vector2[] pointsA, pointsB, edgesA, edgesB;
            target.Bounds.TransformPoints(worldA, out pointsA, out edgesA);
            candidate.Bounds.TransformPoints(worldB, out pointsB, out edgesB);*/
            var pointsA = target.Bounds.TransformedPoints;
            var edgesA = target.Bounds.TransformedEdges;
            var pointsB = candidate.Bounds.TransformedPoints;
            var edgesB = candidate.Bounds.TransformedEdges;

            var velocity = target.Velocity * gameTime.GetElapsedSeconds();
            var result = new CollisionResult<PolygonBounds>
            {
                Intersecting = true,
                WillIntersect = true,
            };

            var edgeCountA = edgesA.Length;
            var edgeCountB = edgesB.Length;
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

        private static void ProjectPolygon(Vector2 axis, Vector2[] points, ref float min, ref float max)
        {
            var d = Vector2.Dot(axis, points[0]);
            min = d;
            max = d;
            for (var i = 0; i < points.Length; i++)
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
}
