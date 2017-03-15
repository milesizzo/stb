using Microsoft.Xna.Framework;
using System;
using StopTheBoats.Templates;

namespace StopTheBoats.GameObjects
{
    public class Weapon : GameElement
    {
        public readonly WeaponTemplate WeaponTemplate;
        private TimeSpan lastFire;

        public Weapon(WeaponTemplate template) : base(FrictionMedium.None)
        {
            this.WeaponTemplate = template;
            this.SpriteTemplate = this.WeaponTemplate.SpriteTemplate;
            this.lastFire = TimeSpan.Zero;
        }

        public Projectile Fire(GameTime gameTime)
        {
            if (gameTime.TotalGameTime < this.lastFire + this.WeaponTemplate.FireRate)
            {
                // not enough time has passed - we can't fire our weapon yet
                return null;
            }
            this.Context.Assets.Audio["Audio/cannon1"].Audio.Play(0.1f, 0, 0);
            this.lastFire = gameTime.TotalGameTime;
            var velocity = this.WeaponTemplate.ProjectileVelocity;
            var projectile = new Projectile(this.Parent, this.WeaponTemplate.Damage, velocity);
            var world = this.World;
            projectile.Position = world.Position;
            projectile.Velocity = new Vector2((float)(velocity * Math.Cos(world.Rotation)), (float)(velocity * Math.Sin(world.Rotation)));
            //projectile.Rotation = world.Rotation;
            return projectile;
        }
    }
}
