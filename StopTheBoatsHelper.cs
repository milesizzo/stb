using GameEngine.Content;
using GameEngine.GameObjects;
using Microsoft.Xna.Framework;
using StopTheBoats.Templates;
using System;

namespace StopTheBoats
{
    public static class StopTheBoatsHelper
    {
        public static void LoadFonts(AssetStore assets)
        {
            assets.Fonts.GetOrAdd("envy12", (key) =>
            {
                return assets.Fonts.Load("Envy12");
            });
            assets.Fonts.GetOrAdd("envy16", (key) =>
            {
                return assets.Fonts.Load("Envy16");
            });
        }

        public static void LoadGameAssets(GameAssetStore assets)
        {
            // load audio
            assets.Audio.GetOrAdd("Audio/explosion1", assets.Audio.Load);
            assets.Audio.GetOrAdd("Audio/explosion2", assets.Audio.Load);
            assets.Audio.GetOrAdd("Audio/cannon1", assets.Audio.Load);
            assets.Audio.GetOrAdd("Audio/ambient1", assets.Audio.Load);
            assets.Audio.GetOrAdd("Audio/ambient2", assets.Audio.Load);

            // load sprite templates
            var patrol_boat = assets.Sprites.GetOrAdd("patrol_boat", (key) =>
            {
                var obj = assets.Sprites.Load(key);
                obj.Origin = new Vector2(19, 31);
                return obj;
            });
            var small_boat = assets.Sprites.GetOrAdd("small_boat", (key) =>
            {
                var obj = assets.Sprites.Load(key);
                obj.Origin = new Vector2(19, 27);
                return obj;
            });
            var gun_single_barrel = assets.Sprites.GetOrAdd("gun_single_barrel", (key) =>
            {
                var obj = assets.Sprites.Load(key);
                obj.Origin = new Vector2(11, 11);
                return obj;
            });
            var rock1 = assets.Sprites.GetOrAdd("rock1", (key) =>
            {
                return assets.Sprites.Load(key);
            });
            var explosion1 = assets.Sprites.GetOrAdd("explosion_sheet1", (key) =>
            {
                var obj = assets.Sprites.Load(64, 64, key);
                obj.FPS = 30;
                return obj;
            });
            var explosion2 = assets.Sprites.GetOrAdd("explosion_sheet2", (key) =>
            {
                var obj = assets.Sprites.Load(100, 100, key);
                obj.FPS = 60;
                return obj;
            });
            var explosion3 = assets.Sprites.GetOrAdd("explosion_sheet3", (key) =>
            {
                var obj = assets.Sprites.Load(100, 100, key);
                obj.FPS = 50;
                return obj;
            });
            var whale = assets.Sprites.GetOrAdd("whale-swim", (key) =>
            {
                var obj = assets.Sprites.Load(62, 31, key, numFrames: 5);
                return obj;
            });

            // load boat templates
            var patrolBoat = assets.Objects.GetOrAdd("boat.patrol", (key) =>
            {
                var boat = new BoatTemplate
                {
                    //Acceleration = 50.0f,
                    Acceleration = 100.0f,
                    SpriteTemplate = patrol_boat,
                    MaxHealth = 1000f,
                    //EnginePosition = new Vector2(19, 31),
                    EnginePosition = new Vector2(-71, 0),
                    Mass = 5800000f,
                };
                boat.WeaponLocations.Add(new Vector2(99, 0));
                boat.WeaponLocations.Add(new Vector2(20, 0));
                //boat.WeaponLocations.Add(new Vector2(28, 0));
                //boat.WeaponLocations.Add(new Vector2(-52, 0));
                return boat;
            });
            var smallBoat = assets.Objects.GetOrAdd("boat.small", (key) =>
            {
                return new BoatTemplate
                {
                    Acceleration = 75.0f,
                    SpriteTemplate = small_boat,
                    MaxHealth = 200f,
                    Mass = 58000f,
                };
            });

            // load weapon templates
            assets.Objects.GetOrAdd("gun.single_barrel", (key) =>
            {
                return new WeaponTemplate
                {
                    SpriteTemplate = gun_single_barrel,
                    ProjectileVelocity = 5000f,
                    ProjectileMass = 100f,
                    FireRate = TimeSpan.FromSeconds(1),
                    Damage = 100f,
                };
            });
        }
    }
}
