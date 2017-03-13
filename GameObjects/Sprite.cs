using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using StopTheBoats.Physics;
using System.Linq;
using StopTheBoats.Templates;
using StopTheBoats.Common;

namespace StopTheBoats.GameObjects
{
    public class Sprite : GameObject, IPhysicsObject<PolygonBounds>
    {
        public bool DeleteAfterAnimation = false;
        private SpriteTemplate spriteTemplate;
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
