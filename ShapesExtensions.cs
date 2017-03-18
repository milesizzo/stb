using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StopTheBoats
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
