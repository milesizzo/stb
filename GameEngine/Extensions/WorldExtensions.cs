using System;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using GameEngine.Graphics;

namespace GameEngine.Extensions
{
    public static class WorldExtensions
    {
        private static void DrawShape(Renderer renderer, Shape shape, Transform transform, Color colour)
        {
            switch (shape.ShapeType)
            {
                case ShapeType.Circle:
                    var circle = shape as CircleShape;
                    var centre = MathUtils.Mul(ref transform, circle.Position);
                    renderer.World.DrawCircle(centre, circle.Radius, 16, colour);
                    break;
                case ShapeType.Polygon:
                    var polygon = shape as PolygonShape;
                    var transformedPoints = polygon.Vertices.Select(v => MathUtils.Mul(ref transform, v));
                    renderer.World.DrawPolygon(Vector2.Zero, transformedPoints.ToArray(), colour);
                    break;
                case ShapeType.Edge:
                    var edge = shape as EdgeShape;
                    renderer.World.DrawLine(MathUtils.Mul(ref transform, edge.Vertex1), MathUtils.Mul(ref transform, edge.Vertex2), colour);
                    break;
                default:
                    throw new NotImplementedException($"Cannot draw shapes of Farseer type {shape.ShapeType}");
            }
        }

        public static void Draw(this World world, Renderer renderer)
        {
            foreach (var body in world.BodyList)
            {
                Transform transform;
                body.GetTransform(out transform);
                foreach (var fixture in body.FixtureList)
                {
                    DrawShape(renderer, fixture.Shape, transform, Color.White);
                }
                renderer.World.DrawPoint(body.Position, Color.Yellow, size: 3f);
            }
        }
    }
}
