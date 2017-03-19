using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using GameEngine.GameObjects;
using GameEngine.Graphics;
using StopTheBoats.GameObjects;
using System.Linq;

namespace StopTheBoats.Controllers
{
    public class BoatControllerCollection
    {
        private readonly List<BoatController> controllers = new List<BoatController>();
        private readonly IGameContext context;
        private readonly Camera camera;
        private readonly World physics;
        private readonly List<int> focus = new List<int>();

        public BoatControllerCollection(IGameContext context, Camera camera, World physics)
        {
            this.context = context;
            this.camera = camera;
            this.physics = physics;
        }

        public void FocusOn(params Boat[] boats)
        {
            this.focus.Clear();
            foreach (var boat in boats)
            {
                var index = this.controllers.FindIndex(c => c.Boat == boat);
                if (index == -1)
                {
                    throw new InvalidOperationException("Boat not found");
                }
                this.focus.Add(index);
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (var controller in this.controllers)
            {
                controller.Control(this.context, this.physics, this.camera, gameTime);
            }
            var position = Vector2.Zero;
            if (this.focus.Any())
            {
                foreach (var index in this.focus)
                {
                    position += this.controllers[index].Boat.Position;
                }
                position /= this.focus.Count;
            }
            else
            {
                position = this.controllers.First().Boat.Position;
            }
            this.camera.LookAt(position);
        }

        public void Add(BoatController controller)
        {
            this.controllers.Add(controller);
        }

        public void Draw(Renderer renderer)
        {
            foreach (var controller in this.controllers)
            {
                controller.Draw(renderer);
            }
        }
    }
}
