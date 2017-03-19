using System.Collections.Generic;
using Microsoft.Xna.Framework;
using GameEngine.Templates;

namespace StopTheBoats.Templates
{
    public class BoatTemplate : IGameObjectTemplate
    {
        public int Crew;
        public int Passengers;
        public float Acceleration;
        public float MaxHealth;
        public float Mass;
        public Vector2 EnginePosition;
        public SingleSpriteTemplate SpriteTemplate;

        public List<Vector2> WeaponLocations = new List<Vector2>();
    }
}
