using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace PhysicsEngine.Farseer
{
    public class FarseerBody : IBody
    {
        private readonly Body body;
        private readonly List<FarseerFixture> fixtures = new List<FarseerFixture>();

        public FarseerBody(Body body)
        {
            this.body = body;
            this.body.OnCollision += Body_OnCollision;
        }

        private bool Body_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            throw new System.NotImplementedException();
        }

        public Body Internal { get { return this.body; } }

        public Vector2 Position
        {
            get { return this.body.Position; }
            set { this.body.Position = value; }
        }

        public float Rotation
        {
            get { return this.body.Rotation; }
            set { this.body.Rotation = value; }
        }

        public Vector2 LinearVelocity
        {
            get { return this.body.LinearVelocity; }
            set { this.body.LinearVelocity = value; }
        }

        public float AngularVelocity
        {
            get { return this.body.AngularVelocity; }
            set { this.body.AngularVelocity = value; }
        }

        public float Mass
        {
            get { return this.body.Mass; }
            set { this.body.Mass = value; }
        }

        public bool Enabled
        {
            get { return this.body.Enabled; }
            set { this.body.Enabled = value; }
        }

        private Shape ShapeToFarseer(PhysicsShape shape)
        {
            var asCircle = shape as PhysicsCircle;
            if (asCircle != null)
            {
                var circle = new CircleShape(asCircle.Radius, asCircle.Density);
                circle.Position = asCircle.Origin;
                return circle;
            }
            var asPolygon = shape as PhysicsPolygon;
            if (asPolygon != null)
            {
                return new PolygonShape(new Vertices(asPolygon.Vertices), asPolygon.Density);
            }
            throw new InvalidOperationException($"No Farseer equivalent of shape: {shape.GetType().Name}");
        }

        public void RemoveFixture(IFixture fixture)
        {
            var asFarseer = fixture as FarseerFixture;
            if (asFarseer == null)
            {
                throw new InvalidOperationException($"Cannot operate on fixtures of type {fixture.GetType().Name}");
            }
            fixture.Body = null;
            this.body.DestroyFixture(asFarseer.Internal);
        }

        public IFixture ReplaceFixture(IFixture old, PhysicsShape replacement)
        {
            this.RemoveFixture(old);
            return this.AddFixture(replacement);
        }

        public IFixture AddFixture(PhysicsShape shape)
        {
            var farseerFixture = this.body.CreateFixture(this.ShapeToFarseer(shape));
            var fixture = new FarseerFixture(this, farseerFixture, shape);
            this.fixtures.Add(fixture);
            return fixture;
        }

        public void ApplyImpulse(Vector2 impulse, Vector2 point)
        {
            this.body.ApplyLinearImpulse(impulse, point);
        }

        public void ApplyForce(Vector2 force, Vector2 worldPoint)
        {
            this.body.ApplyForce(ref force, ref worldPoint);
        }

        public Vector2 LocalToWorld(Vector2 local)
        {
            return this.body.GetWorldPoint(ref local);
        }

        public Vector2 RotateLocal(Vector2 local)
        {
            return this.body.GetWorldVector(ref local);
        }
    }
}
