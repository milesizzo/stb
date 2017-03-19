using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StopTheBoats.Templates;

namespace StopTheBoats.Json
{
    public static partial class Json
    {
        public static JObject Write(WeaponTemplate template)
        {
            var result = new JObject();
            result["projectile_velocity"] = template.ProjectileVelocity;
            result["projectile_mass"] = template.ProjectileMass;
            result["fire_rate"] = template.FireRate;
            result["sprite"] = Write(template.SpriteTemplate);
            result["damage"] = template.Damage;
            return result;
        }
    }
}
