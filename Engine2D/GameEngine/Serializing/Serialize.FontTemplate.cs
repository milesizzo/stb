using CommonLibrary.Serializing;
using GameEngine.Templates;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.Serializing
{
    public static partial class GameEngineSerialize
    {
        public static void Write(ISerializer context, FontTemplate template)
        {
            context.Write("name", template.Name);
            context.Write("asset", template.AssetName);
        }

        public static void Read(ContentManager content, IDeserializer context, out FontTemplate template)
        {
            var name = context.Read<string>("name");
            var texture = context.Read<string>("asset");
            var font = content.Load<SpriteFont>(texture);
            template = new FontTemplate(name, texture, font);
        }
    }
}
