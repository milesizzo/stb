using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StopTheBoats.Json
{
    public static partial class Json
    {
        public static JObject Write(Shape shape)
        {
            var result = new JObject();
            result["type"] = shape.GetType().Name;
            result["density"] = shape.Density;
            switch (shape.ShapeType)
            {
                case ShapeType.Circle:
                    var circle = shape as CircleShape;
                    result["radius"] = circle.Radius;
                    result["pos"] = Write(circle.Position);
                    break;
                case ShapeType.Polygon:
                    var polygon = shape as PolygonShape;
                    var array = new JArray();
                    foreach (var vertex in polygon.Vertices)
                    {
                        array.Add(Write(vertex));
                    }
                    result["vertices"] = array;
                    break;
                default:
                    throw new InvalidOperationException("Can only serialize circles and polygons");
            }
            return result;
        }

        public static void Read(JObject json, out Shape shape)
        {
            var type = Type.GetType(json.Value<string>("type"));
            var density = json.Value<float>("density");
            if (type == typeof(CircleShape))
            {
                // it's a circle
                var radius = json.Value<float>("radius");
                Vector2 pos;
                Read(json["pos"].Value<JObject>(), out pos);
                shape = new CircleShape(radius, density)
                {
                    Position = pos,
                };
            }
            else if (type == typeof(PolygonShape))
            {
                // it's a polygon
                var vertices = new Vertices();
                foreach (var vertex in json["vertices"])
                {
                    Vector2 point;
                    Read(vertex.Value<JObject>(), out point);
                    vertices.Add(point);
                }
                shape = new PolygonShape(vertices, density);
            }
            else
            {
                throw new InvalidOperationException($"Unknown Shape type: {json["type"]}");
            }
        }
    }
}
