using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using GameEngine.GameObjects;
using GameEngine.Templates;
using GameEngine.Graphics;
using StopTheBoats.Templates;

namespace StopTheBoats.GameObjects
{
    public class AttachedObject : AbstractObject
    {
        private readonly Sprite sprite;
        private Vector2 localPosition;
        private float rotation;

        public AttachedObject(IGameContext context, ISpriteTemplate spriteTemplate) : base(context)
        {
            this.sprite = new Sprite(spriteTemplate);
        }

        public Vector2 LocalPosition
        {
            get { return this.localPosition; }
            set { this.localPosition = value; }
        }

        public override Vector2 Position
        {
            get { return this.Parent.GetWorldPoint(this.localPosition); }
            set { this.localPosition = this.Parent.GetLocalPoint(value); }
        }

        public float Rotation
        {
            get { return this.rotation; }
            set { this.rotation = MathHelper.WrapAngle(value); }
        }

        public bool DeleteAfterAnimation
        {
            get { return this.sprite.TotalCycles != Sprite.InfiniteCycles; }
            set { this.sprite.TotalCycles = value ? 1 : 0; }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            this.sprite.Update(gameTime);
            if (this.sprite.IsComplete && this.DeleteAfterAnimation)
            {
                this.IsAwaitingDeletion = true;
            }
        }

        public override void Draw(Renderer renderer, GameTime gameTime)
        {
            base.Draw(renderer, gameTime);
            this.sprite.Draw(renderer, this.Position, Color.White, this.Rotation, Vector2.One, SpriteEffects.None);
        }
    }

    public class Weapon : AttachedObject
    {
        public readonly WeaponTemplate WeaponTemplate;
        private readonly Boat boat;
        private TimeSpan lastFire;

        public Weapon(IGameContext context, Boat boat, WeaponTemplate template) : base(context, template.SpriteTemplate)
        {
            this.WeaponTemplate = template;
            this.boat = boat;
            this.lastFire = TimeSpan.Zero;
        }

        public Projectile Fire(World physics, GameTime gameTime)
        {
            if (gameTime.TotalGameTime < this.lastFire + this.WeaponTemplate.FireRate)
            {
                // not enough time has passed - we can't fire our weapon yet
                return null;
            }
            //this.Context.Store.Audio("StopTheBoats", "cannon1").Audio.Play(0.1f, 0, 0);
            this.lastFire = gameTime.TotalGameTime;
            var velocity = this.WeaponTemplate.ProjectileVelocity;
            var projectile = new Projectile(this.Context, physics, this.boat, this.WeaponTemplate.Damage, velocity);
            projectile.Position = this.Position;
            projectile.LinearVelocity = new Vector2((float)(velocity * Math.Cos(this.Rotation)), (float)(velocity * Math.Sin(this.Rotation)));
            return projectile;
        }
    }
}
