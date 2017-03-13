using Microsoft.Xna.Framework.Content;
using StopTheBoats.Templates;

namespace StopTheBoats.Content
{
    public class AssetStore
    {
        public readonly FontStore Fonts;
        public readonly SpriteStore Sprites;

        public AssetStore(ContentManager content)
        {
            this.Sprites = new SpriteStore(content);
            this.Fonts = new FontStore(content);
        }

        public AssetStore(SpriteStore sprites, FontStore fonts)
        {
            this.Sprites = sprites;
            this.Fonts = fonts;
        }
    }
}
