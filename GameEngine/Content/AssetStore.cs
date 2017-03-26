using System.Linq;
using Microsoft.Xna.Framework.Content;
using GameEngine.Templates;
using CommonLibrary.Serializing;
using GameEngine.Serializing;

namespace GameEngine.Content
{
    public class AssetStore : ITemplate
    {
        public readonly ContentManager Content;
        public readonly FontStore Fonts;
        public readonly SpriteStore Sprites;
        public readonly AudioStore Audio;
        private readonly string name;

        public AssetStore(string name, ContentManager content)
        {
            this.name = name;
            this.Content = content;
            this.Sprites = new SpriteStore(content);
            this.Fonts = new FontStore(content);
            this.Audio = new AudioStore(content);
        }

        public string Name { get { return this.name; } }

        #region Serialization

        internal virtual void Write(ISerializer context)
        {
            context.WriteList("sprites", this.Sprites.Templates.ToList(), GameEngineSerialize.Write);
            context.WriteList("audio", this.Audio.Templates.ToList(), GameEngineSerialize.Write);
            context.WriteList("fonts", this.Fonts.Templates.ToList(), GameEngineSerialize.Write);
        }

        internal virtual void Read(IDeserializer context)
        {
            var sprites = context.ReadList<SpriteTemplate, ContentManager>("sprites", this.Content, GameEngineSerialize.Read);
            this.Sprites.AddOrReplace(sprites);

            var audio = context.ReadList<AudioTemplate, ContentManager>("audio", this.Content, GameEngineSerialize.Read);
            this.Audio.AddOrReplace(audio);

            var fonts = context.ReadList<FontTemplate, ContentManager>("fonts", this.Content, GameEngineSerialize.Read);
            this.Fonts.AddOrReplace(fonts);
        }

        #endregion
    }
}
