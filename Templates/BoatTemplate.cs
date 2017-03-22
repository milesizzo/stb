using System.Collections.Generic;
using Microsoft.Xna.Framework;
using GameEngine.Templates;

namespace StopTheBoats.Templates
{
    public class BoatTemplate : IGameObjectTemplate
    {
        public class WeaponPlacement
        {
            public WeaponTemplate Weapon;
            public Vector2 LocalPosition;
        }

        public int Crew;
        public int Passengers;
        public float Acceleration;
        public float MaxHealth;
        public float Mass;
        public Vector2 EnginePosition;
        public SpriteTemplate SpriteTemplate;
        public List<WeaponPlacement> Weapons = new List<WeaponPlacement>();

        public void AddWeapon(WeaponTemplate weapon, Vector2 localPosition)
        {
            this.Weapons.Add(new WeaponPlacement { Weapon = weapon, LocalPosition = localPosition });
        }

        public void AddWeapon(WeaponPlacement placement)
        {
            this.Weapons.Add(placement);
        }

        public void AddWeapons(IEnumerable<WeaponPlacement> placements)
        {
            this.Weapons.AddRange(placements);
        }
    }
}
