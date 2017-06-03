using System;
using CommonLibrary.Serializing;
using GameEngine.GameObjects;
using GameEngine.Templates;

namespace StopTheBoats.Templates
{
    public class WeaponTemplate : GameObjectTemplate
    {
        public float ProjectileVelocity;
        public float ProjectileMass;
        public TimeSpan FireRate;
        public ISpriteTemplate SpriteTemplate;
        public float Damage;

        public WeaponTemplate(string name) : base(name)
        {
        }

        public override void Write(ISerializer context)
        {
            base.Write(context);
            context.Write("projectile_velocity", this.ProjectileVelocity);
            context.Write("projectile_mass", this.ProjectileMass);
            context.Write("fire_rate", this.FireRate.TotalSeconds);
            context.Write("damage", this.Damage);
            context.Write("sprite", this.SpriteTemplate.Name);
        }

        public override void Read(GameAssetStore store, IDeserializer context)
        {
            base.Read(store, context);
            this.ProjectileVelocity = context.Read<float>("projectile_velocity");
            this.ProjectileMass = context.Read<float>("projectile_mass");
            this.FireRate = TimeSpan.FromSeconds(context.Read<double>("fire_rate"));
            this.Damage = context.Read<float>("damage");
            var spriteName = context.Read<string>("sprite");
            this.SpriteTemplate = store.Sprites[spriteName];
        }
    }
}
