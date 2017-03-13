using Microsoft.Xna.Framework;
using StopTheBoats.Content;
using StopTheBoats.Graphics;
using StopTheBoats.Templates;

namespace StopTheBoats.Scenes
{
    public interface IScene : ITemplate
    {
        Camera Camera { get; }

        bool SceneEnded { get; }

        void SetUp(AssetStore assets);

        void Update(GameTime gameTime);

        void Draw(Renderer renderer);
    }
}
