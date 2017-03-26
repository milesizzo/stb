using CommonLibrary.Serializing;
using GameEngine.Content;
using GameEngine.GameObjects;
using GameEngine.Serializing;
using GameEngine.Templates;
using Microsoft.Xna.Framework;
using StopTheBoats.Templates;
using System;
using System.IO;

namespace StopTheBoats
{
    public static class StopTheBoatsHelper
    {
        /*
        public static void LoadFonts(AssetStore assets)
        {
            assets.Fonts.GetOrAdd("envy12", (key) =>
            {
                return assets.Fonts.Load(key, "Envy12");
            });
            assets.Fonts.GetOrAdd("envy16", (key) =>
            {
                return assets.Fonts.Load(key, "Envy16");
            });
        }

        public static void LoadGameAssets(GameAssetStore assets)
        {
            // load audio
            assets.Audio.GetOrAdd("explosion1", (key) => assets.Audio.Load(key, "Audio/explosion1"));
            assets.Audio.GetOrAdd("explosion2", (key) => assets.Audio.Load(key, "Audio/explosion2"));
            assets.Audio.GetOrAdd("cannon1", (key) => assets.Audio.Load(key, "Audio/cannon1"));
            assets.Audio.GetOrAdd("ambient1", (key) => assets.Audio.Load(key, "Audio/ambient1"));
            assets.Audio.GetOrAdd("ambient2", (key) => assets.Audio.Load(key, "Audio/ambient2"));

            //LoadGameAssets(assets, "StopTheBoats.json");

            // load weapon templates
            assets.Objects.GetOrAdd("gun.single_barrel", (key) =>
            {
                return new WeaponTemplate(key)
                {
                    SpriteTemplate = assets.Sprites["gun_single_barrel"],
                    ProjectileVelocity = 5000f,
                    ProjectileMass = 100f,
                    FireRate = TimeSpan.FromSeconds(1),
                    Damage = 100f,
                };
            });

            // load boat templates
            var patrolBoat = assets.Objects.GetOrAdd("boat.patrol", (key) =>
            {
                var boat = new BoatTemplate(key)
                {
                    //Acceleration = 50.0f,
                    Acceleration = 100.0f,
                    SpriteTemplate = assets.Sprites["patrol_boat"],
                    MaxHealth = 1000f,
                    //EnginePosition = new Vector2(19, 31),
                    EnginePosition = new Vector2(-71, 0),
                    Mass = 5800000f,
                };
                boat.AddWeapon(assets.Objects.Get<WeaponTemplate>("gun.single_barrel"), new Vector2(99, 0));
                boat.AddWeapon(assets.Objects.Get<WeaponTemplate>("gun.single_barrel"), new Vector2(20, 0));
                //boat.WeaponLocations.Add(new Vector2(28, 0));
                //boat.WeaponLocations.Add(new Vector2(-52, 0));
                return boat;
            });
            var smallBoat = assets.Objects.GetOrAdd("boat.small", (key) =>
            {
                return new BoatTemplate(key)
                {
                    Acceleration = 75.0f,
                    SpriteTemplate = assets.Sprites["small_boat"],
                    MaxHealth = 200f,
                    Mass = 58000f,
                };
            });
        }
        */
    }
}
