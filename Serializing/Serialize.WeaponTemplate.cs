using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StopTheBoats.Templates;
using Microsoft.Xna.Framework.Content;
using GameEngine.Templates;

namespace StopTheBoats.Serializing
{
    public static partial class Serialize
    {
        public static void Write(ISerializer context, WeaponTemplate template)
        {
            context.Write("projectile_velocity", template.ProjectileVelocity);
            context.Write("projectile_mass", template.ProjectileMass);
            context.Write("fire_rate", template.FireRate);
            context.Write("sprite", template.SpriteTemplate, Write);
            context.Write("damage", template.Damage);
        }

        public static void Read(ContentManager content, IDeserializer context, out WeaponTemplate template)
        {
            var velocity = context.Read<float>("projectile_velocity");
            var mass = context.Read<float>("projectile_mass");
            var rate = context.Read<TimeSpan>("fire_rate");
            var sprite = context.Read<SpriteTemplate, ContentManager>("sprite", content, Read);
            var damage = context.Read<float>("damage");
            template = new WeaponTemplate
            {
                ProjectileVelocity = velocity,
                ProjectileMass = mass,
                FireRate = rate,
                SpriteTemplate = sprite,
                Damage = damage,
            };
        }
    }
}
