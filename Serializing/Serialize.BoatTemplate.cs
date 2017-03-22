using GameEngine.Templates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Newtonsoft.Json.Linq;
using StopTheBoats.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StopTheBoats.Serializing
{
    public static partial class Serialize
    {
        public static void Write(ISerializer context, BoatTemplate boat)
        {
            context.Write("crew", boat.Crew);
            context.Write("passengers", boat.Passengers);
            context.Write("acceleration", boat.Acceleration);
            context.Write("health", boat.MaxHealth);
            context.Write("mass", boat.Mass);
            context.Write("engine", boat.EnginePosition, Write);
            context.Write("sprite", boat.SpriteTemplate, Write);
            context.WriteList("weapons", boat.Weapons, Write);
        }

        public static void Write(ISerializer context, BoatTemplate.WeaponPlacement placement)
        {
            context.Write("weapon", placement.Weapon, Write);
            context.Write("pos", placement.LocalPosition, Write);
        }

        public static void Read(ContentManager content, IDeserializer context, out BoatTemplate boat)
        {
            var crew = context.Read<int>("crew");
            var passengers = context.Read<int>("passengers");
            var acceleration = context.Read<float>("acceleration");
            var health = context.Read<float>("health");
            var mass = context.Read<float>("mass");
            var engine = context.Read<Vector2>("engine", Read);
            var sprite = context.Read<SpriteTemplate, ContentManager>("sprite", content, Read);
            var weapons = context.ReadList<BoatTemplate.WeaponPlacement, ContentManager>("weapons", content, Read);
            boat = new BoatTemplate
            {
                Crew = crew,
                Passengers = passengers,
                Acceleration = acceleration,
                MaxHealth = health,
                Mass = mass,
                EnginePosition = engine,
                SpriteTemplate = sprite,
            };
            boat.AddWeapons(weapons);
        }

        public static void Read(ContentManager content, IDeserializer context, out BoatTemplate.WeaponPlacement placement)
        {
            var weapon = context.Read<WeaponTemplate, ContentManager>("weapon", content, Read);
            var pos = context.Read<Vector2>("pos", Read);
            placement = new BoatTemplate.WeaponPlacement
            {
                Weapon = weapon,
                LocalPosition = pos,
            };
        }
    }
}
