using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StopTheBoats.Templates
{
    /*
    public static partial class SpriteTemplateSerializer
    {
        private const byte SingleSprite = 0x0001;
        private const byte AnimatedSprite = 0x0002;
        private const byte SpriteSheet = 0x0003;

        public static void Save(string filename, SpriteTemplate sprite)
        {
            using (var writer = new BinaryWriter(File.Open(filename, FileMode.Create)))
            {
                Save(writer, sprite);
            }
        }

        public static void Save(BinaryWriter writer, SpriteTemplate sprite)
        {
            var asSingle = sprite as SingleSpriteTemplate;
            var asAnimated = sprite as AnimatedSpriteTemplate;
            var asSheet = sprite as AnimatedSpriteSheetTemplate;
            if (asSingle != null)
            {
                writer.Write(SingleSprite);
                Write(writer, sprite);
                Write(writer, asSingle);
            }
            else if (asAnimated != null)
            {
                writer.Write(AnimatedSprite);
                Write(writer, sprite);
                Write(writer, asAnimated);
            }
            else if (asSheet != null)
            {
                writer.Write(SpriteSheet);
                Write(writer, sprite);
                Write(writer, asSheet);
            }
            else
            {
                throw new InvalidDataException("Unknown sprite template");
            }
        }

        public static void Write(BinaryWriter writer, SpriteTemplate sprite)
        {
            Write(writer, sprite.Bounds);
            Write(writer, sprite.Origin);
        }

        public static void Write(BinaryWriter writer, SingleSpriteTemplate sprite)
        {
            writer.Write(sprite.Texture.Name);
        }

        public static void Write(BinaryWriter writer, AnimatedSpriteTemplate sprite)
        {
            writer.Write(sprite.Textures.Count);
            foreach (var texture in sprite.Textures)
            {
                writer.Write(texture.Name);
            }
            writer.Write(sprite.FPS);
        }

        public static void Write(BinaryWriter writer, AnimatedSpriteSheetTemplate sprite)
        {
            writer.Write(sprite.Texture.Name);
            writer.Write(sprite.FPS);
            writer.Write(sprite.Width);
            writer.Write(sprite.Height);
            writer.Write(sprite.Border);
            writer.Write(sprite.NumberOfFrames);
        }

        public static void Write(BinaryWriter writer, Vector2 vector)
        {
            writer.Write(vector.X);
            writer.Write(vector.Y);
        }

        public static void Write(BinaryWriter writer, PolygonBounds bounds)
        {
            writer.Write(bounds.Points.Count);
            foreach (var point in bounds.Points)
            {
                Write(writer, point);
            }
        }
    }
    */
}
