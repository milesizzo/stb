using FarseerPhysics.Collision.Shapes;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StopTheBoats.Templates;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StopTheBoats.Json
{
    public static partial class Json
    {
        /*
        public static void Write(ITemplate template, string filename)
        {
            var serializer = new JsonSerializer();
            using (var writer = new StreamWriter(filename))
            using (var json = new JsonTextWriter(writer))
            {
                serializer.Formatting = Formatting.Indented;
                serializer.Serialize(json, Write(template));
            }
        }
        */
    }
}
