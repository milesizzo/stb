using CommonLibrary;
using FarseerPhysics.Dynamics;
using GameEngine.GameObjects;
using GameEngine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using StopTheBoats.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StopTheBoats
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

    public class HumanBoatController : BoatController
    {
        public HumanBoatController(Boat boat) : base(boat)
        {
            //
        }

        public override void Control(IGameContext context, World physics, Camera camera, GameTime gameTime)
        {
            var keyboard = Keyboard.GetState();
            var mouse = Mouse.GetState();

            if (keyboard.IsKeyDown(Keys.W))
            {
                this.boat.Accelerate(1f);
            }
            if (keyboard.IsKeyDown(Keys.S))
            {
                this.boat.Accelerate(-1f);
            }
            if (keyboard.IsKeyDown(Keys.A))
            {
                this.boat.Turn(-20f * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            if (keyboard.IsKeyDown(Keys.D))
            {
                this.boat.Turn(+20f * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            if (mouse.LeftButton == ButtonState.Pressed)
            {
                foreach (var weapon in this.boat.Weapons)
                {
                    var projectile = weapon.Fire(physics, gameTime);
                    if (projectile != null)
                    {
                        context.AddObject(projectile);
                    }
                }
            }
            var mouseWorld = camera.ScreenToWorld(mouse.X, mouse.Y);
            foreach (var weapon in this.boat.Weapons)
            {
                weapon.Rotation = (float)Math.Atan2(mouseWorld.Y - weapon.Position.Y, mouseWorld.X - weapon.Position.X);
            }
        }

        public override void Draw(Renderer renderer)
        {
            foreach (var weapon in this.boat.Weapons)
            {
                var lastPos = weapon.Position;
                var colour = Color.Black;
                for (var i = 0; i < 4; i++)
                {
                    var pos = lastPos.AtBearing(weapon.Rotation, 128);
                    renderer.World.DrawLine(lastPos, pos, colour);
                    colour.A -= 256 / 4;
                    lastPos = pos;
                }
            }
        }
    }

    public class ComputerBoatController : BoatController
    {
        public ComputerBoatController(Boat boat) : base(boat) { }

        public override void Control(IGameContext context, World physics, Camera camera, GameTime gameTime)
        {
            this.boat.Accelerate(1f);
        }
    }

    public class BoatControllerCollection
    {
        private readonly List<BoatController> controllers = new List<BoatController>();
        private readonly IGameContext context;
        private readonly Camera camera;
        private readonly World physics;
        private int Focus = 0;

        public BoatControllerCollection(IGameContext context, Camera camera, World physics)
        {
            this.context = context;
            this.camera = camera;
            this.physics = physics;
        }

        public void Update(GameTime gameTime)
        {
            foreach (var controller in this.controllers)
            {
                controller.Control(this.context, this.physics, this.camera, gameTime);
            }
            this.camera.LookAt(this.controllers[this.Focus].Boat.Position);
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
