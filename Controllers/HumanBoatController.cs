using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using FarseerPhysics.Dynamics;
using CommonLibrary;
using GameEngine.GameObjects;
using GameEngine.Graphics;
using StopTheBoats.GameObjects;
using System.Collections.Generic;

namespace StopTheBoats.Controllers
{
    public interface IHumanAction
    {
        float IsHeld { get; }

        bool IsTapped { get; }
    }

    public interface IHumanWorldSelect
    {
        Vector2 GetPosition(Camera camera);
    }

    public class HumanMouseWorldSelect : IHumanWorldSelect
    {
        public Vector2 GetPosition(Camera camera)
        {
            var mouse = Mouse.GetState();
            return camera.ScreenToWorld(mouse.X, mouse.Y);
        }
    }

    public class HumanKeyboardAction : IHumanAction
    {
        private readonly Keys key;
        private bool keyDown;

        public HumanKeyboardAction(Keys key)
        {
            this.key = key;
            this.keyDown = false;
        }

        private bool KeyDown
        {
            get { return Keyboard.GetState().IsKeyDown(this.key); }
        }

        public float IsHeld
        {
            get { return this.KeyDown ? 1f : 0f; }
        }

        public bool IsTapped
        {
            get
            {
                if (this.KeyDown)
                {
                    if (!this.keyDown)
                    {
                        this.keyDown = true;
                        return true;
                    }
                }
                else
                {
                    this.keyDown = false;
                }
                return false;
            }
        }
    }

    public class HumanGamepadAction : IHumanAction
    {
        private readonly Buttons button;
        private bool buttonDown;

        public HumanGamepadAction(Buttons button)
        {
            this.button = button;
        }

        public float IsHeld
        {
            get
            {
                var pad = GamePad.GetState(0);
                return pad.IsButtonDown(this.button) ? 1f : 0f;
            }
        }

        public bool IsTapped
        {
            get
            {
                var pad = GamePad.GetState(0);
                if (pad.IsButtonDown(this.button))
                {
                    if (!this.buttonDown)
                    {
                        this.buttonDown = true;
                        return true;
                    }
                }
                else
                {
                    this.buttonDown = false;
                }
                return false;
            }
        }
    }

    public class HumanMouseAction : IHumanAction
    {
        public enum Buttons
        {
            Left,
            Right,
            Middle,
        }
        private readonly Buttons button;
        private bool buttonDown;

        public HumanMouseAction(Buttons button)
        {
            this.button = button;
        }

        private bool ButtonDown
        {
            get
            {
                var mouse = Mouse.GetState();
                switch (this.button)
                {
                    case Buttons.Left:
                        return mouse.LeftButton == ButtonState.Pressed;
                    case Buttons.Right:
                        return mouse.RightButton == ButtonState.Pressed;
                    case Buttons.Middle:
                        return mouse.MiddleButton == ButtonState.Pressed;
                }
                return false;
            }
        }

        public float IsHeld
        {
            get { return this.ButtonDown ? 1f : 0f; }
        }

        public bool IsTapped
        {
            get
            {
                if (this.ButtonDown)
                {
                    if (!this.buttonDown)
                    {
                        this.buttonDown = true;
                        return true;
                    }
                }
                else
                {
                    this.buttonDown = false;
                }
                return false;
            }
        }
    }

    public class HumanBoatActionMap
    {
        public enum Actions
        {
            Accelerate,
            Decelerate,
            TurnLeft,
            TurnRight,
            Fire
        }

        private readonly Dictionary<Actions, IHumanAction> actions = new Dictionary<Actions, IHumanAction>();
        private IHumanWorldSelect worldSelect;

        public float IsHeld(Actions action)
        {
            var result = this[action]?.IsHeld;
            return result ?? 0f;
        }

        public bool IsTapped(Actions action)
        {
            var result = this[action]?.IsTapped;
            return result ?? false;
        }
        
        public IHumanWorldSelect WorldSelect
        {
            get { return this.worldSelect; }
            set { this.worldSelect = value; }
        }

        public IHumanAction this[Actions action]
        {
            get
            {
                IHumanAction result;
                if (this.actions.TryGetValue(action, out result))
                {
                    return result;
                }
                return null;
            }
            set
            {
                this.actions[action] = value;
            }
        }

        public static HumanBoatActionMap Default
        {
            get
            {
                var result = new HumanBoatActionMap();
                result.WorldSelect = new HumanMouseWorldSelect();
                result[Actions.Accelerate] = new HumanKeyboardAction(Keys.W);
                result[Actions.Decelerate] = new HumanKeyboardAction(Keys.S);
                result[Actions.TurnLeft] = new HumanKeyboardAction(Keys.A);
                result[Actions.TurnRight] = new HumanKeyboardAction(Keys.D);
                result[Actions.Fire] = new HumanMouseAction(HumanMouseAction.Buttons.Left);
                return result;
            }
        }

        public static HumanBoatActionMap Alternate
        {
            get
            {
                var result = new HumanBoatActionMap();
                result.WorldSelect = new HumanMouseWorldSelect();
                result[Actions.Accelerate] = new HumanKeyboardAction(Keys.Up);
                result[Actions.Decelerate] = new HumanKeyboardAction(Keys.Down);
                result[Actions.TurnLeft] = new HumanKeyboardAction(Keys.Left);
                result[Actions.TurnRight] = new HumanKeyboardAction(Keys.Right);
                result[Actions.Fire] = new HumanMouseAction(HumanMouseAction.Buttons.Left);
                return result;
            }
        }

        public static HumanBoatActionMap GamePad
        {
            get
            {
                var result = new HumanBoatActionMap();
                result.WorldSelect = new HumanMouseWorldSelect();
                result[Actions.Accelerate] = new HumanGamepadAction(Buttons.DPadUp);
                result[Actions.Decelerate] = new HumanGamepadAction(Buttons.DPadDown);
                result[Actions.TurnLeft] = new HumanGamepadAction(Buttons.DPadLeft);
                result[Actions.TurnRight] = new HumanGamepadAction(Buttons.DPadRight);
                result[Actions.Fire] = new HumanMouseAction(HumanMouseAction.Buttons.Left);
                return result;
            }
        }
    }

    public class HumanBoatController : BoatController
    {
        private readonly HumanBoatActionMap mapping;
        private Vector2 worldPosition;

        public HumanBoatController(Boat boat, HumanBoatActionMap mapping) : base(boat)
        {
            this.mapping = mapping;
        }

        public override void Control(IGameContext context, World physics, Camera camera, GameTime gameTime)
        {
            this.boat.Accelerate(+this.mapping.IsHeld(HumanBoatActionMap.Actions.Accelerate));
            this.boat.Accelerate(-this.mapping.IsHeld(HumanBoatActionMap.Actions.Decelerate));
            this.boat.Turn(gameTime, -this.mapping.IsHeld(HumanBoatActionMap.Actions.TurnLeft));
            this.boat.Turn(gameTime, +this.mapping.IsHeld(HumanBoatActionMap.Actions.TurnRight));
            if (this.mapping.IsTapped(HumanBoatActionMap.Actions.Fire))
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
            this.worldPosition = this.mapping.WorldSelect.GetPosition(camera);
            foreach (var weapon in this.boat.Weapons)
            {
                weapon.Rotation = (float)Math.Atan2(this.worldPosition.Y - weapon.Position.Y, this.worldPosition.X - weapon.Position.X);
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
            this.boat.Radar.Draw(renderer, new Vector2(24, 24), 0.1f);

            renderer.World.DrawCircle(new CircleF(this.worldPosition, 8), 16, Color.IndianRed);
        }
    }
}
