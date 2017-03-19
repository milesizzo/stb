using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using GameEngine.Templates;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

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

        public static void Read(ContentManager content, JObject json, out SpriteTemplate template)
        {
            var type = Type.GetType(json.Value<string>("type"));
            if (type == typeof(SingleSpriteTemplate))
            {
                SingleSpriteTemplate sprite;
                Read(content, json, out sprite);
                template = sprite;
            }
            else if (type == typeof(AnimatedSpriteTemplate))
            {
                AnimatedSpriteTemplate sprite;
                Read(content, json, out sprite);
                template = sprite;
            }
            else if (type == typeof(AnimatedSpriteSheetTemplate))
            {
                AnimatedSpriteSheetTemplate sprite;
                Read(content, json, out sprite);
                template = sprite;
            }
            else
            {
                throw new InvalidOperationException($"Unknown sprite template type: {json["type"]}");
            }
        }

        private static void Write(SingleSpriteTemplate template, JObject parent)
        {
            parent["texture"] = template.Texture.Name;
        }

        private static void Read(ContentManager content, JObject json, out SingleSpriteTemplate template)
        {
            var assetName = json.Value<string>("texture");
            var texture = content.Load<Texture2D>(assetName);
            template = new SingleSpriteTemplate(texture);
        }

        private static void Write(AnimatedSpriteTemplate template, JObject parent)
        {
            parent["textures"] = new JArray(template.Textures.Select(t => t.Name));
        }

        private static void Read(ContentManager content, JObject json, out AnimatedSpriteTemplate template)
        {
            var assets = json.Value<JArray>("textures");
            var textures = new List<Texture2D>();
            foreach (var asset in assets)
            {
                var texture = content.Load<Texture2D>(asset.Value<string>());
                textures.Add(texture);
            }
            template = new AnimatedSpriteTemplate(textures);
        }

        private static void Write(AnimatedSpriteSheetTemplate template, JObject parent)
        {
            parent["texture"] = template.Texture.Name;
            parent["width"] = template.Width;
            parent["height"] = template.Height;
            parent["border"] = template.Border;
            parent["frames"] = template.NumberOfFrames;
        }

        private static void Read(ContentManager content, JObject json, out AnimatedSpriteSheetTemplate template)
        {
            var assetName = json.Value<string>("texture");
            var texture = content.Load<Texture2D>(assetName);
            var width = json.Value<int>("width");
            var height = json.Value<int>("height");
            var border = json.Value<int>("border");
            var frames = json.Value<int>("frames");
            template = new AnimatedSpriteSheetTemplate(texture, width, height, border, frames);
        }
    }
}
