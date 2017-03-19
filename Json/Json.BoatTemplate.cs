using Newtonsoft.Json.Linq;
using StopTheBoats.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StopTheBoats.Json
{
    public static partial class Json
    {
        public static JObject Write(BoatTemplate boat)
        {
            var result = new JObject();
            result["crew"] = boat.Crew;
            result["passengers"] = boat.Passengers;
            result["acceleration"] = boat.Acceleration;
            result["health"] = boat.MaxHealth;
            result["mass"] = boat.Mass;
            result["engine"] = Write(boat.EnginePosition);
            result["sprite"] = Write(boat.SpriteTemplate);
            result["weapon_slots"] = new JArray(boat.WeaponLocations.Select(l => Write(l)));
            return result;
        }
    }
}
