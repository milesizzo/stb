using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace StopTheBoats
{
    public class SpriteStore : TemplateStore<SpriteTemplate>
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
    }
}
