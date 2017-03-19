using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using GameEngine.Templates;

namespace GameEngine.Content
{
    public class AudioStore : TemplateStore<AudioTemplate>
    {
        private ContentManager content;

        public AudioStore(ContentManager content)
        {
            this.content = content;
        }

        public AudioTemplate Load(string assetName)
        {
            return new AudioTemplate(this.content.Load<SoundEffect>(assetName));
        }
    }
}
