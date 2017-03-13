﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using StopTheBoats.Physics;
using StopTheBoats.Graphics;

namespace StopTheBoats.Templates
{
    public abstract class SpriteTemplate : ITemplate
    {
        public Vector2 Origin;
        public int FPS = 5;
        private PolygonBounds bounds;

        public abstract Texture2D Texture { get; set; }

        public abstract int NumberOfFrames { get; }

        public PolygonBounds DefaultBounds
        {
            get
            {
                return new PolygonBounds(
                    -this.Origin,
                    new Vector2(this.Texture.Width - this.Origin.X, -this.Origin.Y),
                    new Vector2(this.Texture.Width - this.Origin.X, this.Texture.Height - this.Origin.Y),
                    new Vector2(-this.Origin.X, this.Texture.Height - this.Origin.Y));
            }
        }

        public PolygonBounds Bounds
        {
            get
            {
                return this.bounds ?? this.DefaultBounds;
            }
            set
            {
                this.bounds = value;
            }
        }

        public virtual void DrawSprite(Renderer render, Vector2 position, Color colour, float rotation, Vector2 scale, SpriteEffects effects)
        {
            render.Render.Draw(this.Texture, position, null, colour, rotation, this.Origin, scale, effects, 0f);
        }

        public abstract void DrawSprite(Renderer render, int frame, Vector2 position, Color colour, float rotation, Vector2 scale, SpriteEffects effects);
    }

    public class AnimatedSpriteTemplate : SpriteTemplate
    {
        private readonly List<Texture2D> textures = new List<Texture2D>();

        public AnimatedSpriteTemplate(IEnumerable<Texture2D> textures)
        {
            float averageWidth = 0, averageHeight = 0;
            foreach (var texture in textures)
            {
                averageWidth += texture.Width;
                averageHeight += texture.Height;
                this.textures.Add(texture);
            }
            averageWidth /= this.textures.Count;
            averageHeight /= this.textures.Count;
            this.Origin = new Vector2(averageWidth / 2, averageHeight / 2);
        }

        public override Texture2D Texture
        {
            get { return this.textures.First(); }
            set
            {
                this.textures.Clear();
                this.textures.Add(value);
            }
        }

        public override int NumberOfFrames
        {
            get { return this.textures.Count; }
        }

        public void AddTexture(Texture2D texture)
        {
            this.textures.Add(texture);
        }

        public override void DrawSprite(Renderer render, int frame, Vector2 position, Color colour, float rotation, Vector2 scale, SpriteEffects effects)
        {
            render.Render.Draw(this.textures[frame], position, null, colour, rotation, this.Origin, scale, effects, 0f);
        }
    }

    public class AnimatedSpriteSheetTemplate : SpriteTemplate
    {
        private Texture2D texture;
        private readonly int width;
        private readonly int height;
        private readonly int gridWidth;
        private readonly int gridHeight;

        public AnimatedSpriteSheetTemplate(Texture2D texture, int width, int height)
        {
            this.width = width;
            this.height = height;
            this.texture = texture;
            this.gridWidth = this.texture.Width / this.width;
            this.gridHeight = this.texture.Height / this.height;
            this.Origin = new Vector2(this.width / 2, this.height / 2);
        }

        public override int NumberOfFrames
        {
            get
            {
                return (this.texture.Width / this.width) * (this.texture.Height / this.height);
            }
        }

        public override Texture2D Texture
        {
            get
            {
                return this.texture;
            }
            set
            {
                this.texture = value;
            }
        }

        public override void DrawSprite(Renderer render, int frame, Vector2 position, Color colour, float rotation, Vector2 scale, SpriteEffects effects)
        {
            var x = (frame % this.gridWidth) * this.width;
            var y = (frame / this.gridWidth) * this.height;
            var rect = new Rectangle(x, y, this.width, this.height);
            render.Render.Draw(this.texture, position, rect, colour, rotation, this.Origin, scale, effects, 0f);
        }
    }

    public class SingleSpriteTemplate : SpriteTemplate
    {
        private Texture2D texture;

        public SingleSpriteTemplate(Texture2D texture)
        {
            this.Origin = new Vector2(texture.Width / 2, texture.Height / 2);
            this.texture = texture;
        }

        public override Texture2D Texture
        {
            get { return this.texture; }
            set { this.texture = value; }
        }

        public override int NumberOfFrames
        {
            get
            {
                return 1;
            }
        }

        public override void DrawSprite(Renderer render, int frame, Vector2 position, Color colour, float rotation, Vector2 scale, SpriteEffects effects)
        {
            this.DrawSprite(render, position, colour, rotation, scale, effects);
        }
    }
}
