using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace StopTheBoats
{
    public class SpriteStore : TemplateStore<BaseSpriteTemplate>
    {
        private ContentManager content;

        public SpriteStore(ContentManager content)
        {
            this.content = content;
        }

        public SpriteTemplate Load(string assetName)
        {
            var texture = this.content.Load<Texture2D>(assetName);
            var obj = new SpriteTemplate(texture);
            this.Add(assetName, obj);
            return obj;
        }

        public AnimatedSpriteTemplate Load(string name, params string[] assetNames)
        {
            var obj = new AnimatedSpriteTemplate(assetNames.Select(a => this.content.Load<Texture2D>(a)));
            this.Add(name, obj);
            return obj;
        }

        public AnimatedSpriteSheetTemplate Load(int width, int height, string assetName)
        {
            var texture = this.content.Load<Texture2D>(assetName);
            var obj = new AnimatedSpriteSheetTemplate(texture, width, height);
            this.Add(assetName, obj);
            return obj;
        }
    }
}
