using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using PhysicsEngine;
using System.Linq;
using StopTheBoats.Templates;
using StopTheBoats.Graphics;
using CommonLibrary;

namespace StopTheBoats.GameObjects
{
    public class Sprite : PhysicalObject
    {
        public bool DeleteAfterAnimation = false;
        private SpriteTemplate spriteTemplate;
        private float animFrame;

        public Sprite(IPhysicsEngine physics, SpriteTemplate sprite) : base(physics, sprite.Shape)
        {
            this.spriteTemplate = sprite;
        }

        public void DrawSprite(Renderer render, Vector2 position, Color colour, float rotation, Vector2 scale, SpriteEffects effects)
        {
            this.SpriteTemplate.DrawSprite(render, (int)Math.Floor(this.animFrame), position, colour, rotation, scale, effects);
        }

        public override void Draw(Renderer renderer)
        {
            this.DrawSprite(renderer, this.Position, Color.White, this.Rotation, Vector2.One, SpriteEffects.None);
            if (GameObject.DebugInfo)
            {
                var loc = this.Position - this.spriteTemplate.Origin;
                var font = this.Context.Assets.Fonts["envy12"];
                //renderer.DrawString(this.Context.Assets.Fonts["envy12"], string.Format("velocity: {0}", this.LinearVelocity), origin + new Vector2(0, -96), Color.White);
                renderer.DrawString(font, $"rotation: {this.Rotation}", loc + new Vector2(0, -96), Color.White);
                renderer.DrawString(font, $"velocity: {this.LinearVelocity}", loc + new Vector2(0, -96 + 12), Color.White);
                renderer.DrawString(font, $"position: {this.Position}", loc + new Vector2(0, -96 + 24), Color.White);
            }
            base.Draw(renderer);
        }

        public override void Update(GameTime gameTime)
        {
            if (this.SpriteTemplate != null && this.SpriteTemplate.NumberOfFrames > 1)
            {
                this.animFrame += (float)gameTime.ElapsedGameTime.TotalSeconds * this.SpriteTemplate.FPS;
                while (this.animFrame > this.SpriteTemplate.NumberOfFrames)
                {
                    this.animFrame -= this.SpriteTemplate.NumberOfFrames;
                    if (this.DeleteAfterAnimation)
                    {
                        this.IsAwaitingDeletion = true;
                    }
                }
            }
            base.Update(gameTime);
        }

        public SpriteTemplate SpriteTemplate
        {
            get { return this.spriteTemplate; }
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
