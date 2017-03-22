using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StopTheBoats.Serializing
{
    public static partial class Serialize
    {
        public static void Write(ISerializer context, Vector2 vector)
        {
            context.Write("x", vector.X);
            context.Write("y", vector.Y);
        }

        public static void Read(IDeserializer context, out Vector2 vector)
        {
            var x = context.Read<float>("x");
            var y = context.Read<float>("y");
            vector = new Vector2(x, y);
        }
    }
}
