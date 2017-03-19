using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using GameEngine.GameObjects;
using GameEngine.Graphics;
using StopTheBoats.GameObjects;

namespace StopTheBoats.Controllers
{
    public abstract class BoatController
    {
        protected readonly Boat boat;

        protected BoatController(Boat boat)
        {
            this.boat = boat;
        }

        public abstract void Control(IGameContext context, World physics, Camera camera, GameTime gameTime);

        public virtual void Draw(Renderer renderer) { }

        public Boat Boat { get { return this.boat; } }
    }
}
