using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StopTheBoats.Templates
{
    /*
    public static partial class SpriteTemplateSerializer
    {
        public static SpriteTemplate Load(ContentManager content, string filename)
        {
            using (var reader = new BinaryReader(File.Open(filename, FileMode.Open)))
            {
                return Load(content, reader);
            }
        }

        public static SpriteTemplate Load(ContentManager content, BinaryReader reader)
        {
            var spriteType = reader.ReadByte();
            var bounds = ReadBounds(reader);
            var origin = ReadVector(reader);
            SpriteTemplate sprite;
            switch (spriteType)
            {
                case SingleSprite:
                    sprite = LoadSingleSprite(content, reader);
                    break;
                case AnimatedSprite:
                    sprite = LoadAnimatedSprite(content, reader);
                    break;
                case SpriteSheet:
                    sprite = LoadSpriteSheet(content, reader);
                    break;
                default:
                    throw new InvalidOperationException("Unknown sprite template type");
            }
            sprite.Bounds = bounds;
            sprite.Origin = origin;
            return sprite;
        }

        public static Vector2 ReadVector(BinaryReader reader)
        {
            var x = reader.ReadSingle();
            var y = reader.ReadSingle();
            return new Vector2(x, y);
        }

        public static PolygonBounds ReadBounds(BinaryReader reader)
        {
            var numPoints = reader.ReadInt32();
            var points = new List<Vector2>();
            while (numPoints > 0)
            {
                points.Add(ReadVector(reader));
                numPoints--;
            }
            return new PolygonBounds(points);
        }

        public static SingleSpriteTemplate LoadSingleSprite(ContentManager content, BinaryReader reader)
        {
            var name = reader.ReadString();
            return new SingleSpriteTemplate(content.Load<Texture2D>(name));
        }

        public static AnimatedSpriteTemplate LoadAnimatedSprite(ContentManager content, BinaryReader reader)
        {
            var numTextures = reader.ReadInt32();
            var names = new List<string>();
            while (numTextures > 0)
            {
                names.Add(reader.ReadString());
                numTextures--;
            }
            var fps = reader.ReadInt32();
            var sprite = new AnimatedSpriteTemplate(names.Select(n => content.Load<Texture2D>(n)));
            sprite.FPS = fps;
            return sprite;
        }

        public static AnimatedSpriteSheetTemplate LoadSpriteSheet(ContentManager content, BinaryReader reader)
        {
            var name = reader.ReadString();
            var fps = reader.ReadInt32();
            var width = reader.ReadInt32();
            var height = reader.ReadInt32();
            var border = reader.ReadInt32();
            var numFrames = reader.ReadInt32();
            var sprite = new AnimatedSpriteSheetTemplate(content.Load<Texture2D>(name), width, height, border, numFrames);
            sprite.FPS = fps;
            return sprite;
        }
    }
    */
}
