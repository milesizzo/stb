using Microsoft.Xna.Framework.Content;
using GameEngine.Content;
using GameEngine.Templates;

namespace GameEngine.GameObjects
{
    public class GameAssetStore : AssetStore
    {
        private readonly GameObjectTemplateStore objects = new GameObjectTemplateStore();

        public GameAssetStore(ContentManager content) : base(content)
        {
        }

        public GameAssetStore(AssetStore other) : base(other.Sprites, other.Fonts, other.Audio)
        {
        }

        public GameObjectTemplateStore Objects { get { return this.objects; } }
    }
}
