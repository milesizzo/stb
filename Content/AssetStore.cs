using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using StopTheBoats.Templates;

namespace StopTheBoats.Content
{
    public class AssetStore
    {
        public readonly FontStore Fonts;
        public readonly SpriteStore Sprites;
        public readonly AudioStore Audio;

        public AssetStore(ContentManager content)
        {
            this.Sprites = new SpriteStore(content);
            this.Fonts = new FontStore(content);
            this.Audio = new AudioStore(content);
        }

        public AssetStore(SpriteStore sprites, FontStore fonts, AudioStore audio)
        {
            this.Sprites = sprites;
            this.Fonts = fonts;
            this.Audio = audio;
        }
    }
}
