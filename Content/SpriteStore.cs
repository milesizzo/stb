using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using StopTheBoats.Templates;

namespace StopTheBoats.Content
{
    public class SpriteStore : TemplateStore<SpriteTemplate>
    {
        private ContentManager content;

        public SpriteStore(ContentManager content)
        {
            this.content = content;
        }

        public SingleSpriteTemplate Load(string assetName)
        {
            var texture = this.content.Load<Texture2D>(assetName);
            var obj = new SingleSpriteTemplate(texture);
            return obj;
        }

        public AnimatedSpriteTemplate Load(string name, params string[] assetNames)
        {
            var obj = new AnimatedSpriteTemplate(assetNames.Select(a => this.content.Load<Texture2D>(a)));
            return obj;
        }

        public AnimatedSpriteSheetTemplate Load(int width, int height, string assetName)
        {
            var texture = this.content.Load<Texture2D>(assetName);
            var obj = new AnimatedSpriteSheetTemplate(texture, width, height);
            return obj;
        }
    }
}
