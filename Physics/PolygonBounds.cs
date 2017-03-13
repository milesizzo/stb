using Microsoft.Xna.Framework;
using StopTheBoats.Common;
using System.Collections.Generic;
using System.Linq;

namespace StopTheBoats.Physics
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
                p1 = transformedPoints[i];
                if (i + 1 >= transformedPoints.Length)
                {
                    p2 = transformedPoints[0];
                }
                else
                {
                    p2 = transformedPoints[i + 1];
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
                float totalX = 0, totalY = 0;
                for (var i = 0; i < this.points.Count; i++)
                {
                    totalX += this.points[i].X;
                    totalY += this.points[i].Y;
                }
                return new Vector2(totalX / this.points.Count, totalY / this.points.Count);
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
