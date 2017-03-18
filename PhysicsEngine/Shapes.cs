using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace PhysicsEngine
{
    public abstract class PhysicsShape
    {
        private readonly float density;

        public PhysicsShape(float density)
        {
            this.density = density;
        }

        public abstract PhysicsShape Offset(Vector2 amount);

        public float Density { get { return this.density; } }
    }

    public class PhysicsCircle : PhysicsShape
    {
        private readonly float radius;
        private readonly Vector2 origin;

        public PhysicsCircle(Vector2 origin, float radius, float density) : base(density)
        {
            this.origin = origin;
            this.radius = radius;
        }

        public PhysicsCircle(float radius, float density) : this(Vector2.Zero, radius, density) { }

        public float Radius { get { return this.radius; } }

        public Vector2 Origin { get { return this.origin; } }

        public override PhysicsShape Offset(Vector2 amount)
        {
            return new PhysicsCircle(this.origin + amount, this.radius, this.Density);
        }
    }

    public class PhysicsPolygon : PhysicsShape
    {
        private readonly List<Vector2> vertices = new List<Vector2>();

        public PhysicsPolygon(float density) : base(density) { }

        public PhysicsPolygon(IEnumerable<Vector2> vertices, float density) : base(density)
        {
            this.vertices.AddRange(vertices);
        }

        public override PhysicsShape Offset(Vector2 amount)
        {
            return new PhysicsPolygon(this.vertices.Select(v => v + amount), this.Density);
        }

        public IReadOnlyList<Vector2> Vertices { get { return this.vertices.AsReadOnly(); } }
    }
}
