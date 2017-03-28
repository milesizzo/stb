﻿using CommonLibrary.Serializing;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.Serializing
{
    public static partial class FarseerSerialize
    {
        public static void Write(ISerializer context, Shape shape)
        {
            context.Write("type", shape.GetType().AssemblyQualifiedName);
            context.Write("density", shape.Density);
            switch (shape.ShapeType)
            {
                case ShapeType.Circle:
                    var circle = shape as CircleShape;
                    context.Write("radius", circle.Radius);
                    context.Write("position", circle.Position, CommonSerialize.Write);
                    break;
                case ShapeType.Polygon:
                    var polygon = shape as PolygonShape;
                    context.WriteList("vertices", polygon.Vertices, CommonSerialize.Write);
                    break;
                default:
                    throw new InvalidOperationException("Can only serialize circles and polygons");
            }
        }

        public static void Read(IDeserializer context, out Shape shape)
        {
            var typeName = context.Read<string>("type");
            var type = Type.GetType(typeName);
            var density = context.Read<float>("density");
            if (type == typeof(CircleShape))
            {
                // it's a circle
                var radius = context.Read<float>("radius");
                var pos = context.Read<Vector2>("position", CommonSerialize.Read);
                shape = new CircleShape(radius, density)
                {
                    Position = pos,
                };
            }
            else if (type == typeof(PolygonShape))
            {
                // it's a polygon
                var vertices = new Vertices();
                foreach (var vector in context.ReadList<Vector2>("vertices", CommonSerialize.Read))
                {
                    vertices.Add(vector);
                }
                shape = new PolygonShape(vertices, density);
            }
            else
            {
                throw new InvalidOperationException($"Unknown Shape type: {typeName}");
            }
        }
    }
}
