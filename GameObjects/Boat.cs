using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes;
using StopTheBoats.Templates;
using StopTheBoats.Graphics;
using PhysicsEngine;
using CommonLibrary;

namespace StopTheBoats.GameObjects
{
    public class Boat : GameElement
    {
        public readonly BoatTemplate BoatTemplate;
        private readonly Weapon[] weapons;
        private Vector2 acceleration = Vector2.Zero;
        private float health;

        public Boat(BoatTemplate template) : base(FrictionMedium.Water)
        {
            this.BoatTemplate = template;
            this.health = this.BoatTemplate.MaxHealth;
            this.SpriteTemplate = this.BoatTemplate.SpriteTemplate;
            this.weapons = new Weapon[this.BoatTemplate.WeaponLocations.Count];
        }

        public int WeaponSlots
        {
            get { return this.weapons.Length; }
        }

        public int AddWeapon(WeaponTemplate weapon)
        {
            if (this.weapons.Length == 0)
            {
                throw new InvalidOperationException("No weapon slots on this boat.");
            }
            var slot = 0;
            while (slot < this.weapons.Length)
            {
                if (this.weapons[slot] == null)
                {
                    this.SetWeapon(slot, new Weapon(weapon));
                    return slot;
                }
                slot++;
            }
            throw new InvalidOperationException("No available weapon slots on this boat.");
        }

        public void SetWeapon(int slot, Weapon weapon)
        {
            if (slot < 0 || slot >= this.weapons.Length)
            {
                throw new InvalidOperationException("Invalid weapon slot value.");
            }
            var existing = this.weapons[slot];
            if (existing != null)
            {
                this.RemoveChild(existing);
                this.weapons[slot] = null;
            }
            weapon.Position = this.BoatTemplate.WeaponLocations[slot];
            this.weapons[slot] = weapon;
            this.AddChild(weapon);
        }

        public IEnumerable<Weapon> Weapons
        {
            get { return this.Children.OfType<Weapon>(); }
        }

        public void Accelerate(float amount)
        {
            this.acceleration = amount * new Vector2((float)Math.Cos(this.Angle) * this.BoatTemplate.Acceleration, (float)Math.Sin(this.Angle) * this.BoatTemplate.Acceleration);
        }

        public void Turn(float amountDegrees)
        {
            this.Angle += MathHelper.ToRadians(amountDegrees);
        }

        public override void Update(GameTime gameTime)
        {
            this.Velocity += this.acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;
            base.Update(gameTime);

            this.acceleration = Vector2.Zero;
        }

        public override void Draw(Renderer renderer)
        {
            base.Draw(renderer);
            var origin = -this.SpriteTemplate.Origin + this.Position;
            renderer.Render.DrawRectangle(origin + new Vector2(0, -64), new Vector2(this.SpriteTemplate.Texture.Width, 16), Color.Black);
            var colour = Color.LightGreen;
            if (this.health < this.BoatTemplate.MaxHealth / 3)
            {
                colour = Color.Red;
            }
            else if (this.health < 2 * this.BoatTemplate.MaxHealth / 3)
            {
                colour = Color.Yellow;
            }
            var width = (this.SpriteTemplate.Texture.Width - 2) * health / this.BoatTemplate.MaxHealth;
            renderer.Render.FillRectangle(origin + new Vector2(1, -63), new Vector2(width, 14), colour);
            if (GameObject.DebugInfo)
            {
                renderer.DrawString(this.Context.Assets.Fonts["envy12"], string.Format("velocity: {0}", this.Velocity), origin + new Vector2(0, -96), Color.White);
            }
        }

        public override void OnCollision(IPhysicsObject<PolygonBounds> entity, CollisionResult<PolygonBounds> collision)
        {
            base.OnCollision(entity, collision);
            var asProjectile = entity as Projectile;
            if (asProjectile != null && asProjectile.Owner != this && collision.Intersecting)
            {
                // we were hit
                this.health = this.health - asProjectile.Damage;
                if (this.health <= 0f)
                {
                    this.health = 0;
                    this.AwaitingDeletion = true;

                    this.Context.Assets.Audio["Audio/explosion2"].Audio.Play();

                    var random = new Random();
                    var assetName = random.Choice("explosion_sheet2", "explosion_sheet3");
                    this.Context.AddObject(new GameElement(FrictionMedium.Air)
                    {
                        Position = this.Position,
                        Velocity = this.Velocity,
                        SpriteTemplate = this.Context.Assets.Sprites[assetName],
                        Bounds = null,
                        DeleteAfterAnimation = true,
                    });
                }
            }
        }
    }
}
