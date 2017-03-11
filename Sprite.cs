using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;

namespace StopTheBoats
{
    public class SpriteTemplate : Template
    {
        public Texture2D Texture;
        public Vector2 Origin;

        public SpriteTemplate(Texture2D texture)
        {
            this.Origin = new Vector2(texture.Width / 2, texture.Height / 2);
            this.Texture = texture;
        }
    }

    public class Sprite : GameObject
    {
        public SpriteTemplate SpriteTemplate;

        public override RectangleF BoundingBox
        {
            get
            {
                return new RectangleF(this.Position - this.SpriteTemplate.Origin, new Vector2(this.SpriteTemplate.Texture.Width, this.SpriteTemplate.Texture.Height));
            }
        }

        public override void Draw(RenderStore render)
        {
            var world = this.World;
            render.Render.Draw(this.SpriteTemplate.Texture, world.Position, null, Color.White, world.Rotation, this.SpriteTemplate.Origin, world.Scale, SpriteEffects.None, 0f);
            foreach (var child in this.Children)
            {
                child.Draw(render);
            }
        }

        public Sprite()
        {
        }
    }
}
