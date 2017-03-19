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
        public static JObject Write(Vector2 vector)
        {
            var result = new JObject();
            result["x"] = vector.X;
            result["y"] = vector.Y;
            return result;
        }
		
		public static void Read(JObject json, out Vector2 vector)
        {
            vector = new Vector2(json.Value<float>("x"), json.Value<float>("y"));
        }
    }
}
