using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using GameEngine.GameObjects;
using GameEngine.Graphics;
using StopTheBoats.GameObjects;
using MonoGame.Extended;
using FarseerPhysics.Common;
using System.Collections.Generic;

namespace StopTheBoats.Controllers
{
    public class ComputerBoatController : BoatController
    {
        private const float FuturePrediction = 2; // in secs
        private readonly List<Vector2> collisions = new List<Vector2>();

        public ComputerBoatController(Boat boat) : base(boat) { }

        public override void Control(IGameContext context, World physics, Camera camera, GameTime gameTime)
        {
            var accelerate = 0.5f;
            var future = this.boat.Position + this.boat.LinearVelocity * FuturePrediction;

            this.collisions.Clear();
            if (this.boat.Position != future)
            {
                foreach (var fixture in physics.RayCast(this.boat.Position, future))
                {
                    if (fixture.UserData == this.boat || fixture.UserData == null)
                        continue;
                    if (fixture.CollidesWith == Category.None)
                        continue;
                    var obj = fixture.UserData as IGameObject;
                    this.collisions.Add(obj.Position);
                    this.boat.Turn(gameTime, MathHelper.Clamp(1f * (1 / Vector2.Distance(this.boat.Position, obj.Position)), -1f, 1f));
                }
            }

            foreach (var obj in this.boat.Radar.Nearby)
            {
                var asBoat = obj as Boat;
                if (asBoat != null && asBoat.ActiveWeapons > 0)
                {
                    // avoid boats with weapons
                    accelerate = 1f;
                }
            }
            this.boat.Accelerate(accelerate);
        }

        public override void Draw(Renderer renderer)
        {
            base.Draw(renderer);
            //var future = this.boat.Position + this.boat.LinearVelocity * FuturePrediction;
            /*Transform transform;
            this.boat.Body.GetTransform(out transform);
            transform.Set(this.boat.Position + this.boat.LinearVelocity * FuturePrediction, this.boat.Rotation + this.boat.AngularVelocity * FuturePrediction);
            renderer.World.DrawLine(this.boat.Position, transform.p, Color.Yellow);

            foreach (var collision in this.collisions)
            {
                renderer.World.DrawLine(this.boat.Position, collision, Color.Red);
            }*/
        }
    }
}
