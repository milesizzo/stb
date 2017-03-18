using Microsoft.Xna.Framework;
using System;
using StopTheBoats.Templates;
using PhysicsEngine;

namespace StopTheBoats.GameObjects
{
    public class Weapon : Sprite
    {
        public readonly WeaponTemplate WeaponTemplate;
        private TimeSpan lastFire;

        public Weapon(IPhysicsEngine physics, WeaponTemplate template) : base(physics, template.SpriteTemplate)
        {
            this.WeaponTemplate = template;
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
            var projectile = new Projectile(this.Physics, this, this.WeaponTemplate.Damage, velocity);
            projectile.Position = this.Position;
            projectile.LinearVelocity = new Vector2((float)(velocity * Math.Cos(this.Rotation)), (float)(velocity * Math.Sin(this.Rotation)));
            return projectile;
        }
    }
}
