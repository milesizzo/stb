using StopTheBoats.Content;

namespace StopTheBoats.GameObjects
{
    public interface IGameContext
    {
        void ScheduleObject(GameObject obj, float waitTime);

        void AddObject(GameObject obj);

        void RemoveObject(GameObject obj);

        AssetStore Assets { get; }
    }
}
