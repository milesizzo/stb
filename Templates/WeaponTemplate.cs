using System;

namespace StopTheBoats.Templates
{
    public class WeaponTemplate : IGameObjectTemplate
    {
        public float ProjectileVelocity;
        public float ProjectileMass;
        public TimeSpan FireRate;
        public SingleSpriteTemplate SpriteTemplate;
        public float Damage;
    }
}
