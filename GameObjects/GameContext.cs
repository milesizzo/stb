using Microsoft.Xna.Framework;
using StopTheBoats.Content;
using StopTheBoats.Graphics;

namespace StopTheBoats.GameObjects
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
