using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using StopTheBoats.Templates;

namespace StopTheBoats.Content
{
    public class FontStore : TemplateStore<FontTemplate>
    {
        private ContentManager content;

        public FontStore(ContentManager content)
        {
            this.content = content;
        }

        public FontTemplate Load(string assetName)
        {
            return new FontTemplate(this.content.Load<SpriteFont>(assetName));
        }
    }
}
