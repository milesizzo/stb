using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using GameEngine.Graphics;

namespace GameEngine.Templates
{
    public abstract class SpriteTemplate : ITemplate
    {
        public Vector2 Origin;
        public int FPS = 5;
        private Shape shape;
        private readonly string name;

        protected SpriteTemplate(string name)
        {
            this.name = name;
        }

        public string Name { get { return this.name; } }

        public abstract Texture2D Texture { get; set; }

        public abstract int NumberOfFrames { get; }

        public virtual int Width { get { return this.Texture.Width; } }

        public virtual int Height { get { return this.Texture.Height; } }

        public Shape DefaultShape
        {
            get
            {
                return new PolygonShape(new Vertices(new[]
                {
                    -this.Origin,
                    new Vector2(this.Width - this.Origin.X, -this.Origin.Y),
                    new Vector2(this.Width - this.Origin.X, this.Height - this.Origin.Y),
                    new Vector2(-this.Origin.X, this.Height - this.Origin.Y)
                }), 1f);
            }
        }

        public Shape Shape
        {
            get
            {
                return this.shape ?? this.DefaultShape;
            }
            set
            {
                this.shape = value;
            }
        }

        public virtual void DrawSprite(SpriteBatch sb, Vector2 position, Color colour, float rotation, Vector2 scale, SpriteEffects effects)
        {
            sb.Draw(this.Texture, position, null, colour, rotation, this.Origin, scale, effects, 0f);
        }

        public abstract void DrawSprite(SpriteBatch sb, int frame, Vector2 position, Color colour, float rotation, Vector2 scale, SpriteEffects effects);
    }

    public class AnimatedSpriteTemplate : SpriteTemplate
    {
        private readonly List<Texture2D> textures = new List<Texture2D>();

        public AnimatedSpriteTemplate(string name, IEnumerable<Texture2D> textures) : base(name)
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

        public IReadOnlyList<Texture2D> Textures
        {
            get { return this.textures.AsReadOnly(); }
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

        public override void DrawSprite(SpriteBatch sb, int frame, Vector2 position, Color colour, float rotation, Vector2 scale, SpriteEffects effects)
        {
            sb.Draw(this.textures[frame], position, null, colour, rotation, this.Origin, scale, effects, 0f);
        }
    }

    public class AnimatedSpriteSheetTemplate : SpriteTemplate
    {
        private Texture2D texture;
        private readonly int width;
        private readonly int height;
        private readonly int gridWidth;
        private readonly int gridHeight;
        private readonly int border;
        private readonly int numFrames;

        public AnimatedSpriteSheetTemplate(string name, Texture2D texture, int width, int height, int border = -1, int numFrames = -1) : base(name)
        {
            this.width = width;
            this.height = height;
            this.texture = texture;
            if (border < 0)
            {
                if (this.texture.Width % this.width != 0)
                {
                    // there is probably a border around each sprite
                    // TODO: detect borders of width > 1
                    this.border = 1;
                }
                else
                {
                    this.border = 0;
                }
            }
            else
            {
                this.border = border;
            }
            if (this.border == 0)
            {
                this.gridWidth = this.texture.Width / this.width;
                this.gridHeight = this.texture.Height / this.height;
            }
            else
            {
                this.gridWidth = (this.texture.Width % this.width) - 1;
                this.gridHeight = (this.texture.Height % this.height) - 1;
            }
            this.numFrames = numFrames == -1 ? this.gridWidth * this.gridHeight : numFrames;
            this.Origin = new Vector2(this.width / 2, this.height / 2);
        }

        public int Border { get { return this.border; } }

        public override int Width { get { return this.width; } }

        public override int Height { get { return this.height; } }

        public override int NumberOfFrames { get { return this.numFrames; } }

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

        public override void DrawSprite(SpriteBatch sb, int frame, Vector2 position, Color colour, float rotation, Vector2 scale, SpriteEffects effects)
        {
            var x = (frame % this.gridWidth) * (this.width + this.border) + this.border;
            var y = (frame / this.gridWidth) * (this.height + this.border) + this.border;
            var rect = new Rectangle(x, y, this.width, this.height);
            sb.Draw(this.texture, position, rect, colour, rotation, this.Origin, scale, effects, 0f);
        }
    }

    public class SingleSpriteTemplate : SpriteTemplate
    {
        private Texture2D texture;

        public SingleSpriteTemplate(string name, Texture2D texture) : base(name)
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

        public override void DrawSprite(SpriteBatch sb, int frame, Vector2 position, Color colour, float rotation, Vector2 scale, SpriteEffects effects)
        {
            this.DrawSprite(sb, position, colour, rotation, scale, effects);
        }
    }
}
