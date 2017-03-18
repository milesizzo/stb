using Microsoft.Xna.Framework;

namespace PhysicsEngine
{
    // this is a point-mass (eg. it has no shape)
    public interface IBody
    {
        Vector2 Position { get; set; }

        float Rotation { get; set; }

        Vector2 LinearVelocity { get; set; }

        float AngularVelocity { get; set; }

        float Mass { get; set; }

        bool Enabled { get; set; }

        IFixture ReplaceFixture(IFixture old, PhysicsShape replacement);

        IFixture AddFixture(PhysicsShape shape);

        void RemoveFixture(IFixture fixture);

        void ApplyImpulse(Vector2 impulse, Vector2 point);

        void ApplyForce(Vector2 force, Vector2 worldPoint);

        Vector2 LocalToWorld(Vector2 local);

        Vector2 RotateLocal(Vector2 local);
    }
}
