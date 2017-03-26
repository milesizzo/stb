using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using GameEngine.Templates;

namespace GameEngine.Content
{
    public class AudioStore : TemplateStore<AudioTemplate>
    {
        private readonly ContentManager content;

        public AudioStore(ContentManager content)
        {
            this.content = content;
        }

        public ContentManager Content { get { return this.content; } }

        public AudioTemplate Load(string name, string assetName)
        {
            return new AudioTemplate(name, this.content.Load<SoundEffect>(assetName));
        }
    }
}
