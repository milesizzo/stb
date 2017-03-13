using Microsoft.Xna.Framework.Content;
using StopTheBoats.Templates;

namespace StopTheBoats.Content
{
    public class AssetStore
    {
        public readonly TemplateStore<FontTemplate> Fonts;
        public readonly SpriteStore Sprites;

        public AssetStore(ContentManager content)
        {
            this.Sprites = new SpriteStore(content);
            this.Fonts = new TemplateStore<FontTemplate>();
        }
    }
}
