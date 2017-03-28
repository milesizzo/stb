using CommonLibrary.Serializing;
using GameEngine.Templates;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.Serializing
{
    public static partial class GameEngineSerialize
    {
        public static void Write(ISerializer context, AudioTemplate template)
        {
            context.Write("name", template.Name);
            context.Write("asset", template.Audio.Name);
        }

        public static void Read(ContentManager content, IDeserializer context, out AudioTemplate template)
        {
            var name = context.Read<string>("name");
            var assetName = context.Read<string>("asset");
            var sound = content.Load<SoundEffect>(assetName);
            template = new AudioTemplate(name, sound);
        }
    }
}
