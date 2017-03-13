using Microsoft.Xna.Framework;
using StopTheBoats.GameObjects;
using StopTheBoats.Templates;
using System;

namespace StopTheBoats
{
    public static class StopTheBoatsHelper
    {
        public static void LoadGameAssets(GameAssetStore assets)
        {
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

            // load boat templates
            var patrolBoat = assets.Objects.GetOrAdd("boat.patrol", (key) =>
            {
                var boat = new BoatTemplate
                {
                    Acceleration = 50.0f,
                    SpriteTemplate = patrol_boat,
                    MaxHealth = 1000f,
                };
                boat.WeaponLocations.Add(new Vector2(99, 0));
                boat.WeaponLocations.Add(new Vector2(20, 0));
                return boat;
            });
            var smallBoat = assets.Objects.GetOrAdd("boat.small", (key) =>
            {
                return new BoatTemplate
                {
                    Acceleration = 75.0f,
                    SpriteTemplate = small_boat,
                    MaxHealth = 200f,
                };
            });

            // load weapon templates
            assets.Objects.GetOrAdd("gun.single_barrel", (key) =>
            {
                return new WeaponTemplate
                {
                    SpriteTemplate = gun_single_barrel,
                    ProjectileVelocity = 1000f,
                    FireRate = TimeSpan.FromSeconds(1),
                    Damage = 100f,
                };
            });
        }
    }
}
