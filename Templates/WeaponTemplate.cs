using System;

namespace StopTheBoats.Templates
{
    public class WeaponTemplate : ITemplate
    {
        public float ProjectileVelocity;
        public TimeSpan FireRate;
        public SingleSpriteTemplate SpriteTemplate;
        public float Damage;
    }
}
