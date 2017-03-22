using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using GameEngine.Templates;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Collision.Shapes;

namespace StopTheBoats.Serializing
{
    public static partial class Serialize
    {
        public static void Write(ISerializer context, SpriteTemplate template)
        {
            context.Write("type", template.GetType().AssemblyQualifiedName);
            context.Write("origin", template.Origin, Write);
            context.Write("shape", template.Shape, Write);

            var asSingle = template as SingleSpriteTemplate;
            var asAnimated = template as AnimatedSpriteTemplate;
            var asSheet = template as AnimatedSpriteSheetTemplate;

            if (asSingle != null)
            {
                context.Write("texture", template.Texture.Name);
            }
            else if (asAnimated != null)
            {
                context.Write("fps", template.FPS);
                context.WriteList("textures", asAnimated.Textures.Select(t => t.Name).ToList());
            }
            else if (asSheet != null)
            {
                context.Write("fps", template.FPS);
                context.Write("texture", asSheet.Texture.Name);
                context.Write("width", asSheet.Width);
                context.Write("height", asSheet.Height);
                context.Write("border", asSheet.Border);
                context.Write("frames", asSheet.NumberOfFrames);
            }
            else
            {
                throw new InvalidOperationException("Unknown SpriteTemplate type");
            }
        }

        public static void Read(ContentManager content, IDeserializer context, out SpriteTemplate template)
        {
            var typeName = context.Read<string>("type");
            var type = Type.GetType(typeName);
            var origin = context.Read<Vector2>("origin", Read);
            var shape = context.Read<Shape>("shape", Read);
            if (type == typeof(SingleSpriteTemplate))
            {
                var assetName = context.Read<string>("texture");
                template = new SingleSpriteTemplate(content.Load<Texture2D>(assetName));
            }
            else if (type == typeof(AnimatedSpriteTemplate))
            {
                var fps = context.Read<int>("fps");
                var textures = context.ReadList<string>("textures").Select(name => content.Load<Texture2D>(name));
                template = new AnimatedSpriteTemplate(textures)
                {
                    FPS = fps,
                };
            }
            else if (type == typeof(AnimatedSpriteSheetTemplate))
            {
                var fps = context.Read<int>("fps");
                var texture = content.Load<Texture2D>(context.Read<string>("texture"));
                var width = context.Read<int>("width"); 
                var height = context.Read<int>("height"); 
                var border = context.Read<int>("border"); 
                var frames = context.Read<int>("frames");
                template = new AnimatedSpriteSheetTemplate(texture, width, height, border, frames)
                {
                    FPS = fps,
                };
            }
            else
            {
                throw new InvalidOperationException($"Unknown sprite template type: {typeName}");
            }
            template.Shape = shape;
            template.Origin = origin;
        }
    }
}
