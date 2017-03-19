using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using GameEngine.Templates;

namespace StopTheBoats.Json
{
    public static partial class Json
    {
        public static JObject Write(SpriteTemplate template)
        {
            var result = new JObject();
            result["type"] = template.GetType().Name;
            result["origin"] = Write(template.Origin);
            result["fps"] = template.FPS;
            result["shape"] = Write(template.Shape);

            var asSingle = template as SingleSpriteTemplate;
            var asAnimated = template as AnimatedSpriteTemplate;
            var asSheet = template as AnimatedSpriteSheetTemplate;

            if (asSingle != null)
            {
                Write(asSingle, result);
            }
            else if (asAnimated != null)
            {
                Write(asAnimated, result);
            }
            else if (asSheet != null)
            {
                Write(asSheet, result);
            }
            else
            {
                throw new InvalidOperationException("Unknown SpriteTemplate type");
            }
            return result;
        }

        /*
        public static void Read(JObject json, out SpriteTemplate template)
        {
            var type = Type.GetType(json.Value<string>("type"));
            if (type == typeof(SingleSpriteTemplate))
            {
                //
            }
        }
        */

        private static void Write(SingleSpriteTemplate template, JObject parent)
        {
            parent["texture"] = template.Texture.Name;
        }

        private static SingleSpriteTemplate Read(JObject parent)
        {
            var texture = parent.Value<string>("texture");
            return new SingleSpriteTemplate();
        }

        private static void Write(AnimatedSpriteTemplate template, JObject parent)
        {
            parent["textures"] = new JArray(template.Textures.Select(t => t.Name));
        }

        private static void Write(AnimatedSpriteSheetTemplate template, JObject parent)
        {
            parent["texture"] = template.Texture.Name;
            parent["width"] = template.Width;
            parent["height"] = template.Height;
            parent["border"] = template.Border;
            parent["frames"] = template.NumberOfFrames;
        }
    }
}
