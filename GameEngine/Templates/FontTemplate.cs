using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.Templates
{
    public class FontTemplate : ITemplate
    {
        private readonly string name;
        public readonly string AssetName;
        public readonly SpriteFont Font;

        public FontTemplate(string name, string assetName, SpriteFont font)
        {
            this.name = name;
            this.AssetName = assetName;
            this.Font = font;
        }

        public string Name { get { return this.name; } }
    }
}
