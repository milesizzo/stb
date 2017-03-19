using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using FarseerPhysics.Dynamics;
using CommonLibrary;
using GameEngine.Templates;
using GameEngine.Graphics;
using GameEngine.Extensions;

namespace GameEngine.GameObjects
{
    public class Sprite
    {
        public const int InfiniteCycles = -1;
        private readonly SpriteTemplate template;
        private float animFrame;
        private int animCyclesTotal;
        private int animCyclesCount;

        public Sprite(SpriteTemplate template, int animCycles = InfiniteCycles)
        {
            this.template = template;
            this.animFrame = 0;
            this.animCyclesTotal = animCycles;
            this.animCyclesCount = 0;
        }

        public void Draw(Renderer renderer, Vector2 position, Color colour, float rotation, Vector2 scale, SpriteEffects effects)
        {
            if (!this.IsComplete)
            {
                this.template.DrawSprite(renderer, (int)Math.Floor(this.animFrame), position, colour, rotation, scale, effects);
            }
        }

        public void Update(GameTime gameTime)
        {
            if (this.template != null && this.template.NumberOfFrames > 1)
            {
                this.animFrame += (float)gameTime.ElapsedGameTime.TotalSeconds * this.template.FPS;
                while (this.animFrame > this.template.NumberOfFrames)
                {
                    this.animFrame -= this.template.NumberOfFrames;
                    this.animCyclesCount++;
                }
            }
        }

        public int TotalCycles
        {
            get { return this.animCyclesTotal; }
            set { this.animCyclesTotal = value; }
        }

        public SpriteTemplate Template
        {
            get { return this.template; }
        }

        public bool IsComplete
        {
            get
            {
                if (this.animCyclesTotal == InfiniteCycles) return false;
                if (this.animCyclesCount < this.animCyclesTotal) return false;
                return true;
            }
        }
    }

    public class SpriteObject : PhysicalObject
    {
        private Sprite sprite;

        public SpriteObject(World world, SpriteTemplate sprite) : base(world, sprite.Shape)
        {
            this.sprite = new Sprite(sprite);
        }

        public bool DeleteAfterAnimation
        {
            get { return this.sprite.TotalCycles != Sprite.InfiniteCycles; }
            set { this.sprite.TotalCycles = value ? 1 : 0; }
        }

        public override void Draw(Renderer renderer)
        {
            this.sprite.Draw(renderer, this.Position, Color.White, this.Rotation, Vector2.One, SpriteEffects.None);
            if (AbstractObject.DebugInfo)
            {
                var loc = this.Position - this.SpriteTemplate.Origin;
                var font = this.Context.Assets.Fonts["envy12"];
                //renderer.DrawString(this.Context.Assets.Fonts["envy12"], string.Format("velocity: {0}", this.LinearVelocity), origin + new Vector2(0, -96), Color.White);
                renderer.World.DrawString(font, $"rotation: {this.Rotation}", loc + new Vector2(0, -96), Color.White);
                renderer.World.DrawString(font, $"velocity: {this.LinearVelocity}", loc + new Vector2(0, -96 + 12), Color.White);
                renderer.World.DrawString(font, $"position: {this.Position}", loc + new Vector2(0, -96 + 24), Color.White);
                renderer.World.DrawString(font, $"collided: {this.NumCollisions}", loc + new Vector2(0, -96 + 36), Color.White);
            }
            base.Draw(renderer);
        }

        public override void Update(GameTime gameTime)
        {
            this.sprite.Update(gameTime);
            if (this.sprite.IsComplete && this.DeleteAfterAnimation)
            {
                this.IsAwaitingDeletion = true;
            }
            base.Update(gameTime);
        }

        public SpriteTemplate SpriteTemplate
        {
            get { return this.sprite.Template; }
        }

        /*
        public virtual void OnCollision(IActor<PolygonBounds> entity, CollisionResult<PolygonBounds> collision)
        {
            if (collision.WillIntersect && !collision.Intersecting)
            {
                this.physicsColour = Color.Yellow;
                //this.Position -= collision.MinimumTranslationVector;
                //this.Velocity = -this.Velocity;
            }
            else if (collision.Intersecting)
            {
                this.physicsColour = Color.Red;
                //this.Position -= this.Velocity + collision.MinimumTranslationVector;
                //this.Velocity = -this.Velocity;
            }
        }
        */
    }
}
