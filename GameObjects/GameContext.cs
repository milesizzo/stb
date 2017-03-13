using Microsoft.Xna.Framework;
using StopTheBoats.Content;
using StopTheBoats.Graphics;

namespace StopTheBoats.GameObjects
{
    public interface IGameContext
    {
        void ScheduleObject(GameObject obj, float waitTime);

        void AddObject(GameObject obj);

        void RemoveObject(GameObject obj);

        void Reset();

        AssetStore Assets { get; }

        void Update(GameTime gameTime);

        void Draw(Renderer renderer);
    }
}
