using Microsoft.Xna.Framework;
using GameEngine.Content;
using GameEngine.Graphics;
using GameEngine.Templates;

namespace GameEngine.Scenes
{
    public interface IScene : ITemplate
    {
        Camera Camera { get; }

        bool SceneEnded { get; }

        void SetUp();

        void Update(GameTime gameTime);

        void Draw(Renderer renderer);
    }
}
