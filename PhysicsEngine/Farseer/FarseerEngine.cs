using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Linq;

namespace PhysicsEngine.Farseer
{
    public class FarseerEngine : PhysicsEngine<FarseerBody, FarseerFixture>
    {
        private const float InvFrameRate = 1f / 30f;
        private readonly World world;
        private float damping;

        public FarseerEngine(Vector2 gravity)
        {
            this.world = new World(gravity);
            this.damping = 0;
        }

        public FarseerEngine() : this(Vector2.Zero) { }

        public override float WorldDamping
        {
            get { return this.damping; }
            set { this.damping = value; }
        }

        public override void Update(GameTime gameTime)
        {
            this.world.Step((float)Math.Min(gameTime.ElapsedGameTime.TotalSeconds, InvFrameRate));
        }

        private void DrawShape(SpriteBatch sb, Shape shape, Transform transform, Color colour)
        {
            switch (shape.ShapeType)
            {
                case ShapeType.Circle:
                    var circle = shape as CircleShape;
                    var centre = MathUtils.Mul(ref transform, circle.Position);
                    sb.DrawCircle(centre, circle.Radius, 16, colour);
                    break;
                case ShapeType.Polygon:
                    var polygon = shape as PolygonShape;
                    var transformedPoints = polygon.Vertices.Select(v => MathUtils.Mul(ref transform, v));
                    sb.DrawPolygon(Vector2.Zero, transformedPoints.ToArray(), colour);
                    break;
                default:
                    throw new NotImplementedException($"Cannot draw shapes of Farseer type {shape.ShapeType}");
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            foreach (var body in this.world.BodyList)
            {
                Transform transform;
                body.GetTransform(out transform);
                foreach (var fixture in body.FixtureList)
                {
                    this.DrawShape(sb, fixture.Shape, transform, Color.White);
                }
                sb.DrawPoint(body.Position, Color.Yellow, size: 3f);
            }
        }

        public override void CreateBody(PhysicsShape shape, out FarseerBody body)
        {
            FarseerFixture fixture;
            this.CreateFixture(shape, out fixture);
            body = fixture.Body as FarseerBody;
        }
        
        public override void RemoveBody(FarseerBody body)
        {
            this.world.RemoveBody(body.Internal);
        }

        public override void CreateFixture(PhysicsShape shape, out FarseerFixture fixture)
        {
            var farseerBody = new Body(this.world, bodyType: BodyType.Dynamic);
            farseerBody.AngularDamping = this.damping;
            farseerBody.LinearDamping = this.damping;

            var body = new FarseerBody(farseerBody);
            fixture = body.AddFixture(shape) as FarseerFixture;
        }

        public override void CreateFixture(FarseerBody parent, PhysicsShape shape, out FarseerFixture fixture)
        {
            throw new NotImplementedException();
        }

        public override void Reset()
        {
            this.world.Clear();
        }
    }
}
