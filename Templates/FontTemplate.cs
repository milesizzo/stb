using Microsoft.Xna.Framework.Graphics;

namespace StopTheBoats.Templates
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
