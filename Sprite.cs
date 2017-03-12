using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using StopTheBoats.Physics;
using System.Collections.Generic;
using System.Linq;

namespace StopTheBoats
{
    public abstract class BaseSpriteTemplate : Template
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

        public virtual void DrawSprite(RenderStore render, Vector2 position, Color colour, float rotation, Vector2 scale, SpriteEffects effects)
        {
            render.Render.Draw(this.Texture, position, null, colour, rotation, this.Origin, scale, effects, 0f);
        }

        public abstract void DrawSprite(RenderStore render, int frame, Vector2 position, Color colour, float rotation, Vector2 scale, SpriteEffects effects);
    }

    public class AnimatedSpriteTemplate : BaseSpriteTemplate
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

        public override void DrawSprite(RenderStore render, int frame, Vector2 position, Color colour, float rotation, Vector2 scale, SpriteEffects effects)
        {
            render.Render.Draw(this.textures[frame], position, null, colour, rotation, this.Origin, scale, effects, 0f);
        }
    }

    public class AnimatedSpriteSheetTemplate : BaseSpriteTemplate
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

        public override void DrawSprite(RenderStore render, int frame, Vector2 position, Color colour, float rotation, Vector2 scale, SpriteEffects effects)
        {
            var x = (frame % this.gridWidth) * this.width;
            var y = (frame / this.gridWidth) * this.height;
            var rect = new Rectangle(x, y, this.width, this.height);
            render.Render.Draw(this.texture, position, rect, colour, rotation, this.Origin, scale, effects, 0f);
        }
    }

    public class SpriteTemplate : BaseSpriteTemplate
    {
        private Texture2D texture;

        public SpriteTemplate(Texture2D texture)
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

        public override void DrawSprite(RenderStore render, int frame, Vector2 position, Color colour, float rotation, Vector2 scale, SpriteEffects effects)
        {
            this.DrawSprite(render, position, colour, rotation, scale, effects);
        }
    }

    public class Sprite : GameObject, IPhysicsObject<PolygonBounds>
    {
        public bool DeleteAfterAnimation = false;
        private BaseSpriteTemplate spriteTemplate;
        private PolygonBounds bounds;
        protected Color physicsColour = Color.White;
        private float animFrame;

        public Sprite()
        {
        }

        public void DrawSprite(RenderStore render, Vector2 position, Color colour, float rotation, Vector2 scale, SpriteEffects effects)
        {
            this.SpriteTemplate.DrawSprite(render, (int)Math.Floor(this.animFrame), position, colour, rotation, scale, effects);
        }

        public override void Draw(GameContext context)
        {
            var world = this.World;
            this.DrawSprite(context.Render, world.Position, Color.White, world.Rotation, world.Scale, SpriteEffects.None);
            if (GameObject.DebugInfo && this.Bounds != null)
            {
                var points = this.Bounds.Points.Select(p => world.TransformVector(p));
                context.Render.Render.DrawPolygon(Vector2.Zero, points.ToArray(), this.physicsColour);
            }
            foreach (var child in this.Children)
            {
                child.Draw(context);
            }
            this.physicsColour = Color.White;
        }

        public override void Update(GameContext context, GameTime gameTime)
        {
            base.Update(context, gameTime);
            if (this.SpriteTemplate != null && this.SpriteTemplate.NumberOfFrames > 1)
            {
                this.animFrame += (float)gameTime.ElapsedGameTime.TotalSeconds * this.SpriteTemplate.FPS;
                while (this.animFrame > this.SpriteTemplate.NumberOfFrames)
                {
                    this.animFrame -= this.SpriteTemplate.NumberOfFrames;
                    if (this.DeleteAfterAnimation)
                    {
                        this.AwaitingDeletion = true;
                    }
                }
            }
        }

        public BaseSpriteTemplate SpriteTemplate
        {
            get { return this.spriteTemplate; }
            set
            {
                this.spriteTemplate = value;
                this.bounds = new PolygonBounds(value.Bounds.Points);
            }
        }

        public Transformation GetWorldTransform()
        {
            return this.World;
        }

        public PolygonBounds Bounds
        {
            get
            {
                return this.bounds;
            }
            set
            {
                this.bounds = value;
            }
        }

        public virtual void OnCollision(IPhysicsObject<PolygonBounds> entity, CollisionResult<PolygonBounds> collision)
        {
            if (collision.Intersecting)
            {
                this.physicsColour = Color.Red;
                //this.Position -= this.Velocity + collision.MinimumTranslationVector;
                //this.Velocity = -this.Velocity;
            }
        }
    }
}
