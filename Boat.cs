using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace StopTheBoats
{
    public class BoatTemplate : Template
    {
        public int Crew;
        public int Passengers;
        public float Acceleration;
        public SpriteTemplate SpriteTemplate;

        public List<Vector2> WeaponLocations = new List<Vector2>();
    }

    public class Boat : GameElement
    {
        public readonly BoatTemplate BoatTemplate;
        private readonly Weapon[] weapons;
        private Vector2 acceleration = Vector2.Zero;

        public Boat(BoatTemplate template) : base(FrictionMedium.Water)
        {
            this.BoatTemplate = template;
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
            this.acceleration = amount * new Vector2((float)Math.Cos(this.Rotation) * this.BoatTemplate.Acceleration, (float)Math.Sin(this.Rotation) * this.BoatTemplate.Acceleration);
        }

        public void Turn(float amountDegrees)
        {
            this.Rotation += MathHelper.ToRadians(amountDegrees);
        }

        public override void Update(GameTime gameTime)
        {
            this.Velocity += this.acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;
            base.Update(gameTime);

            this.acceleration = Vector2.Zero;
        }
    }
}
