using FarseerPhysics.Collision.Shapes;
using Microsoft.Xna.Framework;
using System;

namespace GameEngine.Extensions
{
    public static class ShapesExtensions
    {
        public static Shape Offset(this Shape shape, Vector2 offset)
        {
            switch (shape.ShapeType)
            {
                case ShapeType.Circle:
                    var circle = shape as CircleShape;
                    return new CircleShape(circle.Radius, circle.Density)
                    {
                        Position = offset
                    };
                case ShapeType.Polygon:
                    var polygon = shape as PolygonShape;
                    return polygon;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
