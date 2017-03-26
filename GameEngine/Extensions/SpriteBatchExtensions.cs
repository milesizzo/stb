using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using GameEngine.Templates;

namespace GameEngine.Extensions
{
    public static class SpriteBatchExtensions
    {
        public static void DrawString(this SpriteBatch sb, FontTemplate font, string text, Vector2 position, Color colour)
        {
            sb.DrawString(font.Font, text, position, colour);
        }

        public static void DrawShape(this SpriteBatch sb, Shape shape, Transform transform, Color colour, float scale = 1f)
        {
            switch (shape.ShapeType)
            {
                case ShapeType.Circle:
                    var circle = shape as CircleShape;
                    var centre = MathUtils.Mul(ref transform, scale * circle.Position);
                    sb.DrawCircle(centre, circle.Radius * scale, 16, colour);
                    break;
                case ShapeType.Polygon:
                    var polygon = shape as PolygonShape;
                    var transformedPoints = polygon.Vertices.Select(v => MathUtils.Mul(ref transform, scale * v));
                    sb.DrawPolygon(Vector2.Zero, transformedPoints.ToArray(), colour);
                    break;
                case ShapeType.Edge:
                    var edge = shape as EdgeShape;
                    sb.DrawLine(MathUtils.Mul(ref transform, scale * edge.Vertex1), MathUtils.Mul(ref transform, scale * edge.Vertex2), colour);
                    break;
                default:
                    throw new NotImplementedException($"Cannot draw shapes of Farseer type {shape.ShapeType}");
            }
        }
    }
}
