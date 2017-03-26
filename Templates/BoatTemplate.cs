using System.Collections.Generic;
using Microsoft.Xna.Framework;
using GameEngine.Templates;
using CommonLibrary.Serializing;
using GameEngine.GameObjects;

namespace StopTheBoats.Templates
{
    public class BoatTemplate : GameObjectTemplate
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

        public BoatTemplate(string name) : base(name)
        {
        }

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

        #region Serialization

        public override void Write(ISerializer context)
        {
            base.Write(context);
            context.Write("crew", this.Crew);
            context.Write("passengers", this.Passengers);
            context.Write("acceleration", this.Acceleration);
            context.Write("health", this.MaxHealth);
            context.Write("mass", this.Mass);
            context.Write("engine", this.EnginePosition, CommonSerialize.Write);
            context.Write("sprite", this.SpriteTemplate.Name);
            context.WriteList("weapons", this.Weapons, (ctx, placement) =>
            {
                ctx.Write("weapon", placement.Weapon.Name);
                ctx.Write("pos", placement.LocalPosition, CommonSerialize.Write);
            });
        }

        public override void Read(GameAssetStore store, IDeserializer context)
        {
            base.Read(store, context);
            this.Crew = context.Read<int>("crew");
            this.Passengers = context.Read<int>("passengers");
            this.Acceleration = context.Read<float>("acceleration");
            this.MaxHealth = context.Read<float>("health");
            this.Mass = context.Read<float>("mass");
            this.EnginePosition = context.Read<Vector2>("engine", CommonSerialize.Read);
            var spriteName = context.Read<string>("sprite");
            this.SpriteTemplate = store.Sprites[spriteName];
            var weapons = context.ReadList("weapons", (ctx) =>
            {
                var name = ctx.Read<string>("weapon");
                var pos = ctx.Read<Vector2>("pos", CommonSerialize.Read);
                return new WeaponPlacement
                {
                    Weapon = store.Objects.Get<WeaponTemplate>(name),
                    LocalPosition = pos,
                };
            });
            this.AddWeapons(weapons);
        }

        #endregion
    }
}
