using Microsoft.Xna.Framework;
using GameEngine.Content;
using GameEngine.Graphics;

namespace GameEngine.GameObjects
{
    public interface IGameContext
    {
        void ScheduleObject(IGameObject obj, float waitTime);

        void AddObject(IGameObject obj);

        void RemoveObject(IGameObject obj);

        void Reset();

        GameAssetStore Assets { get; }

        void Update(GameTime gameTime);

        void Draw(Renderer renderer);
    }
}
