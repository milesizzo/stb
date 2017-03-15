using Microsoft.Xna.Framework.Audio;

namespace StopTheBoats.Templates
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
