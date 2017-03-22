using System;
using GameEngine.Templates;

namespace StopTheBoats.Templates
{
    public class WeaponTemplate : IGameObjectTemplate
    {
        public float ProjectileVelocity;
        public float ProjectileMass;
        public TimeSpan FireRate;
        public SpriteTemplate SpriteTemplate;
        public float Damage;
    }
}
