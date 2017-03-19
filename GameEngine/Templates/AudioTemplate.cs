using Microsoft.Xna.Framework.Audio;

namespace GameEngine.Templates
{
    public class AudioTemplate : ITemplate
    {
        public SoundEffect Audio;

        public AudioTemplate(SoundEffect audio)
        {
            this.Audio = audio;
        }
    }
}
