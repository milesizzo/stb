using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using GameEngine.Templates;
using GameEngine.Graphics;
using GameEngine.Extensions;

namespace GameEngine.GameObjects
{
    public class SpriteObject : PhysicalObject
    {
        private Sprite sprite;

        public SpriteObject(IGameContext context, World world, SpriteTemplate sprite) : base(context, world, sprite.Shape)
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
    }
}
