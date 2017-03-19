using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using GameEngine.GameObjects;
using GameEngine.Graphics;
using StopTheBoats.GameObjects;

namespace StopTheBoats.Controllers
{
    public class ComputerBoatController : BoatController
    {
        public ComputerBoatController(Boat boat) : base(boat) { }

        public override void Control(IGameContext context, World physics, Camera camera, GameTime gameTime)
        {
            this.boat.Accelerate(1f);
        }
    }
}
