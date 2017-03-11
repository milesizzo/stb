using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StopTheBoats
{
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

    public class Physics
    {
        public void ResolveCollision(Actor a, Actor b)
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
        }
    }

    public class AABB
    {
        public Vector2 Min;
        public Vector2 Max;

        public static bool IsColliding(AABB a, AABB b)
        {
            if (a.Max.X < b.Min.X || a.Min.X > b.Max.X) return false;
            if (a.Max.Y < b.Min.Y || a.Min.Y > b.Max.Y) return false;
            return true;
        }
    }

    public class Circle
    {
        public float Radius;
        public Vector2 Position;

        public static bool IsColliding(Circle a, Circle b)
        {
            var r = a.Radius + b.Radius;
            r *= r;
            return r < Math.Pow(a.Position.X + b.Position.X, 2) + Math.Pow(a.Position.Y + b.Position.Y, 2f);
        }
    }
}
