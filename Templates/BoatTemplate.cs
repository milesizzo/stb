using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StopTheBoats.Templates
{
    public class BoatTemplate : ITemplate
    {
        public int Crew;
        public int Passengers;
        public float Acceleration;
        public float MaxHealth;
        public SingleSpriteTemplate SpriteTemplate;

        public List<Vector2> WeaponLocations = new List<Vector2>();
    }
}
