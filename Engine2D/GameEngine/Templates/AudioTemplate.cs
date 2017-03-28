using Microsoft.Xna.Framework.Audio;

namespace GameEngine.Templates
{
    public class AudioTemplate : ITemplate
    {
        private readonly string name;
        public readonly SoundEffect Audio;

        public AudioTemplate(string name, SoundEffect audio)
        {
            this.name = name;
            this.Audio = audio;
        }

        public string Name { get { return this.name; } }
    }
}
