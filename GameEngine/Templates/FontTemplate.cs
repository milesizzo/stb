using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.Templates
{
    public class FontTemplate : ITemplate
    {
        public SpriteFont Font;

        public FontTemplate(SpriteFont font)
        {
            this.Font = font;
        }
    }
}
