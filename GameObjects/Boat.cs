using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using CommonLibrary;
using GameEngine.GameObjects;
using GameEngine.Graphics;
using StopTheBoats.Templates;
using FarseerPhysics.Common;
using GameEngine.Extensions;
using Microsoft.Xna.Framework.Graphics;

namespace StopTheBoats.GameObjects
{
    public class Radar
    {
        private readonly Boat owner;
        private readonly float radiusSquared;
        private List<PhysicalObject> nearby = new List<PhysicalObject>();

        public Radar(Boat owner)
        {
            this.owner = owner;
            this.radiusSquared = (float)Math.Pow(this.owner.BoatTemplate.RadarRadius, 2);
        }

        private bool IsVisible(PhysicalObject obj)
        {
            return obj != this.owner &&
                !obj.IsAwaitingDeletion &&
                (obj is FixedObstacle || obj is Boat || obj is Projectile) &&
                Vector2.DistanceSquared(obj.Position, this.owner.Position) <= this.radiusSquared;
        }

        public void Update(GameTime gameTime)
        {
            this.nearby = this.owner.Context.Objects.OfType<PhysicalObject>().Where(this.IsVisible).ToList();
        }

        private void DrawRadarObject(Renderer renderer, PhysicalObject obj, ref Vector2 screenPosition, ref Vector2 radiusVector, float scale)
        {
            var location = screenPosition + (obj.Position - this.owner.Position) * scale + radiusVector;
            var objTransform = new Transform();
            objTransform.Set(location, obj.Rotation);
            var asSpriteObject = obj as SpriteObject;
            if (asSpriteObject == null)
            {
                renderer.Screen.DrawShape(obj.Shape, objTransform, Color.Red, scale: scale);
            }
            else
            {
                asSpriteObject.SpriteTemplate.DrawSprite(renderer.Screen, location, Color.Red, obj.Rotation, new Vector2(scale, scale), SpriteEffects.None);
            }
        }

        public void Draw(Renderer renderer, Vector2 screenPosition, float scale)
        {
            var offset = screenPosition;
            var radius = this.owner.BoatTemplate.RadarRadius;
            var radiusVector = new Vector2(radius, radius) * scale;
            foreach (var obj in this.nearby)
            {
                this.DrawRadarObject(renderer, obj, ref screenPosition, ref radiusVector, scale);
            }
            this.DrawRadarObject(renderer, this.owner, ref screenPosition, ref radiusVector, scale);
            renderer.Screen.DrawCircle(offset + radiusVector, radius * scale + 5, 64, Color.Black, 8f);
        }

        public IReadOnlyList<PhysicalObject> Nearby { get { return this.nearby; } }
    }

    public class Boat : SpriteObject
    {
        public readonly BoatTemplate BoatTemplate;
        private readonly Weapon[] weapons;
        private float health;
        private readonly Radar radar;

        public Boat(IGameContext context, World world, BoatTemplate template) : base(context, world, template.SpriteTemplate)
        {
            this.BoatTemplate = template;
            this.radar = new Radar(this);
            this.Mass = template.Mass;
            this.health = this.BoatTemplate.MaxHealth;
            this.weapons = new Weapon[this.BoatTemplate.Weapons.Count];
            foreach (var placement in this.BoatTemplate.Weapons)
            {
                this.AddWeapon(placement.Weapon);
            }
            this.Body.AngularDamping = this.Body.LinearDamping = PhysicalObject.WaterFriction;
            this.Fixture.Restitution = 0.01f;
        }

        public Radar Radar { get { return this.radar; } }

        public int ActiveWeapons
        {
            get { return this.weapons.Count(w => w != null); }
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
                    this.SetWeapon(slot, new Weapon(this.Context, this, weapon));
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
            weapon.LocalPosition = this.BoatTemplate.Weapons[slot].LocalPosition;
            this.weapons[slot] = weapon;
            this.AddChild(weapon);
        }

        public IEnumerable<Weapon> Weapons
        {
            get { return this.Children.OfType<Weapon>(); }
        }

        public void Accelerate(float amount)
        {
            var force = this.Body.GetWorldVector(new Vector2(amount * this.BoatTemplate.Acceleration, 0) * this.Mass);
            this.Body.ApplyForce(force, this.Body.WorldCenter);
        }

        public void Turn(GameTime gameTime, float scale)
        {
            this.Body.AngularVelocity += MathHelper.ToRadians(this.BoatTemplate.TurnSpeed * gameTime.GetElapsedSeconds() * scale);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            this.radar.Update(gameTime);
        }

        public override void Draw(Renderer renderer)
        {
            base.Draw(renderer);
            var origin = this.Position;
            renderer.World.DrawRectangle(origin + new Vector2(0, -64), new Vector2(this.SpriteTemplate.Texture.Width, 16), Color.Black);
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
            renderer.World.FillRectangle(origin + new Vector2(1, -63), new Vector2(width, 14), colour);
        }

        public float Health
        {
            get { return this.health; }
            set
            {
                this.health = value;
                if (this.health <= 0f)
                {
                    this.health = 0;
                    this.IsAwaitingDeletion = true;

                    //this.Context.Store.Audio("StopTheBoats", "explosion2").Audio.Play();

                    var random = new Random();
                    var assetName = random.Choice("explosion_sheet2", "explosion_sheet3");
                    var explosion = new SpriteObject(this.Context, this.Physics, this.Context.Store.Sprites("StopTheBoats", assetName))
                    {
                        Position = this.Position,
                        LinearVelocity = this.LinearVelocity,
                        DeleteAfterAnimation = true,
                    };
                    explosion.Body.AngularDamping = explosion.Body.LinearDamping = PhysicalObject.AirFriction;
                    explosion.Body.CollidesWith = Category.None;
                    this.Context.AddObject(explosion);
                }
            }
        }

        public override bool HandleCollision(PhysicalObject other, Contact contact)
        {
            base.HandleCollision(other, contact);
            var asProjectile = other as Projectile;
            if (asProjectile != null)
            {
                // we were hit by a projectile
                this.Health -= asProjectile.Damage;
                return false;
            }
            if (other != null)
            {
                this.Health -= (this.LinearVelocity - other.LinearVelocity).Length();
                this.Health -= Math.Abs(MathHelper.ToDegrees(this.AngularVelocity)) - Math.Abs(MathHelper.ToDegrees(other.AngularVelocity));
            }
            return true;
        }
    }
}
