using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using CommonLibrary;

namespace CustomPhysicsEngine
{
    public class PolygonBounds : IBounds
    {
        private readonly List<Vector2> points = new List<Vector2>();
        private Vector2[] transformedPoints;
        private Vector2[] edges;
        //private readonly List<Vector2> edges = new List<Vector2>();

        public PolygonBounds(IEnumerable<Vector2> points)
        {
            this.points.AddRange(points);
            this.transformedPoints = new Vector2[this.points.Count];
            this.edges = new Vector2[this.points.Count];
            //this.BuildEdges();
        }

        public PolygonBounds(params Vector2[] points) : this(points.AsEnumerable())
        {
        }

        private Vector2[] BuildEdges()
        {
            Vector2 p1, p2;
            for (var i = 0; i < this.transformedPoints.Length; i++)
            {
                p1 = this.transformedPoints[i];
                if (i + 1 >= this.transformedPoints.Length)
                {
                    p2 = this.transformedPoints[0];
                }
                else
                {
                    p2 = this.transformedPoints[i + 1];
                }
                this.edges[i] = p2 - p1;
            }
            return this.edges;
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
                return CalculateCentre(this.transformedPoints);
            }
        }

        public float Mass { get; set; }

        public static float CalculateMomentOfInertia(Vector2[] points, float mass)
        {
            // stolen from https://www.gamedev.net/topic/342822-moment-of-inertia-of-a-polygon-2d/
            var denom = 0f;
            var numer = 0f;
            for (int i = 0, j = points.Length - 1; i < points.Length; j = i, i++)
            {
                var p1 = points[j];
                var p2 = points[i];
                var a = Math.Abs(p1.X * p2.Y - p1.Y * p2.X); // cross product
                var b = Vector2.Dot(p2, p2) + Vector2.Dot(p2, p1) + Vector2.Dot(p1, p1);
                denom += a * b;
                numer += a;
            }
            return (mass / 6f) * (denom / numer);
        }

        public static Vector2 CalculateCentre(Vector2[] points)
        {
            float totalX = 0, totalY = 0;
            for (var i = 0; i < points.Length; i++)
            {
                totalX += points[i].X;
                totalY += points[i].Y;
            }
            return new Vector2(totalX / points.Length, totalY / points.Length);
        }

        public float MomentOfInertia
        {
            get
            {
                return CalculateMomentOfInertia(this.points.ToArray(), this.Mass);
            }
        }

        /*public List<Vector2> TransformPoints(Transformation transform)
        {
            return this.points.Select(p => transform.TransformVector(p)).ToList();
        }*/
        public void TransformPoints(Transformation transform)
        {
            for (var i = 0; i < this.points.Count; i++)
            {
                this.transformedPoints[i] = transform.TransformVector(this.points[i]);
            }
            //points = this.transformedPoints;
            this.BuildEdges();
            //edges = this.edges;
        }

        public Vector2[] TransformedPoints { get { return this.transformedPoints; } }

        public Vector2[] TransformedEdges { get { return this.edges; } }

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
}
