using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using StopTheBoats.Physics;
using System.Collections.Generic;
using System.Linq;

namespace StopTheBoats
{
    public class SpriteTemplate : Template
    {
        public Texture2D Texture;
        public Vector2 Origin;
        private PolygonBounds bounds;

        public SpriteTemplate(Texture2D texture)
        {
            this.Origin = new Vector2(texture.Width / 2, texture.Height / 2);
            this.Texture = texture;
        }

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
    }

    public class Sprite : GameObject, IPhysicsObject<PolygonBounds>
    {
        private SpriteTemplate spriteTemplate;
        private PolygonBounds bounds;
        protected Color physicsColour = Color.White;

        public Sprite()
        {
        }

        public override void Draw(RenderStore render)
        {
            var world = this.World;
            render.Render.Draw(this.SpriteTemplate.Texture, world.Position, null, Color.White, world.Rotation, this.SpriteTemplate.Origin, world.Scale, SpriteEffects.None, 0f);
            var points = this.Bounds.Points.Select(p => world.TransformVector(p));
            render.Render.DrawPolygon(Vector2.Zero, points.ToArray(), this.physicsColour);
            foreach (var child in this.Children)
            {
                child.Draw(render);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public SpriteTemplate SpriteTemplate
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
            protected set
            {
                this.bounds = value;
            }
        }

        public virtual void OnCollision(List<CollisionResult<PolygonBounds>> collisions)
        {
            if (collisions == null)
            {
                this.physicsColour = Color.White;
                return;
            }
            foreach (var collision in collisions)
            {
                if (collision.Intersecting)
                {
                    this.physicsColour = Color.Red;
                    //this.Position -= this.Velocity + collision.MinimumTranslationVector;
                    //this.Velocity = -this.Velocity;
                }
                else if (collision.WillIntersect)
                {
                    this.physicsColour = Color.Yellow;
                }
            }
        }
    }
}
