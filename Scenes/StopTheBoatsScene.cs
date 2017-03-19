using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using FarseerPhysics.Dynamics;
using CommonLibrary;
using GameEngine.Scenes;
using GameEngine.GameObjects;
using GameEngine.Graphics;
using GameEngine.Extensions;
using StopTheBoats.GameObjects;
using StopTheBoats.Templates;

namespace StopTheBoats.Scenes
{
    public class StopTheBoatsScene : GameScene<GameContext>
    {
        private float zoomAmount;
        private float zoomTarget;
        private float zoomSource;
        private Boat player;
        private readonly List<Boat> enemies = new List<Boat>();
        private bool spacePressed = false;
        private int lastScroll = 0;
        private Vector2 mouse;
        private World physics;

        public StopTheBoatsScene(GraphicsDevice graphics, GameAssetStore assets) : base(graphics, new GameContext(assets))
        {
            this.physics = new World(Vector2.Zero);
            this.Camera.Rotation = 0;
            this.Camera.Zoom = 1;
            this.zoomTarget = this.zoomSource = this.Camera.Zoom;
        }

        public override void SetUp()
        {
            base.SetUp();

            this.physics.Clear();

            StopTheBoatsHelper.LoadGameAssets(this.Assets);

            // set up and add player
            this.player = new Boat(this.physics, this.Assets.Objects.Get<BoatTemplate>("boat.patrol"));
            this.Context.AddObject(this.player);

            this.player.Position = Vector2.Zero;
            this.player.AddWeapon(this.Assets.Objects.Get<WeaponTemplate>("gun.single_barrel"));
            this.player.AddWeapon(this.Assets.Objects.Get<WeaponTemplate>("gun.single_barrel"));

            // set up and add a random rock
            var random = new Random();
            var rock = new SpriteObject(this.physics, this.Context.Assets.Sprites["rock1"])
            {
                Position = new Vector2(200, 200),
                Rotation = MathHelper.ToRadians(random.Next(0, 360)),
            };
            rock.Body.BodyType = BodyType.Static;
            rock.Fixture.Restitution = 0.001f;
            this.Context.AddObject(rock);

            this.Assets.Audio["Audio/ambient2"].Audio.Play(0.1f, 0, 0);
            this.Assets.Audio["Audio/ambient1"].Audio.Play(0.02f, 0, -0.8f);
        }

        public override void Update(GameTime gameTime)
        {
            this.physics.Step((float)Math.Min(gameTime.ElapsedGameTime.TotalSeconds, 1f / 30f));

            var mouse = Mouse.GetState();
            var keyboard = Keyboard.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboard.IsKeyDown(Keys.Escape))
            {
                this.SceneEnded = true;
            }
            if (keyboard.IsKeyDown(Keys.W))
            {
                this.player.Accelerate(1f);
            }
            if (keyboard.IsKeyDown(Keys.S))
            {
                this.player.Accelerate(-1f);
            }
            if (keyboard.IsKeyDown(Keys.Q))
            {
                this.player.Rotation += MathHelper.ToRadians(-20 * gameTime.GetElapsedSeconds());
            }
            if (keyboard.IsKeyDown(Keys.E))
            {
                this.player.Rotation += MathHelper.ToRadians(+20 * gameTime.GetElapsedSeconds());
            }
            if (keyboard.IsKeyDown(Keys.A))
            {
                this.player.Turn(-20f * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            if (keyboard.IsKeyDown(Keys.D))
            {
                this.player.Turn(+20f * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            if (keyboard.IsKeyDown(Keys.Space) && !this.spacePressed)
            {
                var random = new Random();
                var enemy = new Boat(this.physics, this.Assets.Objects.Get<BoatTemplate>("boat.small"));
                var topLeft = this.Camera.ScreenToWorld(0, -100);
                var topRight = this.Camera.ScreenToWorld(this.Camera.Viewport.Width, -100);
                enemy.Position = Vector2.Lerp(topLeft, topRight, (float)random.NextDouble());
                //enemy.Position = new Vector2((float)random.NextDouble() * 2560, -100);
                enemy.Rotation = MathHelper.ToRadians(90 + random.Next(-30, 30));
                this.enemies.Add(enemy);
                this.Context.AddObject(enemy);
                this.spacePressed = true;
            }
            if (!keyboard.IsKeyDown(Keys.Space))
            {
                this.spacePressed = false;
            }
            if (mouse.LeftButton == ButtonState.Pressed)
            {
                foreach (var weapon in this.player.Weapons)
                {
                    var projectile = weapon.Fire(this.physics, gameTime);
                    if (projectile != null)
                    {
                        this.Context.AddObject(projectile);
                    }
                }
            }
            if (mouse.ScrollWheelValue != this.lastScroll)
            {
                var change = mouse.ScrollWheelValue - this.lastScroll;
                this.zoomSource = this.Camera.Zoom;
                this.zoomTarget = Math.Max(1f, this.Camera.Zoom + change / 200f);
                this.zoomAmount = 0;
                this.lastScroll = mouse.ScrollWheelValue;
            }
            this.mouse = this.Camera.ScreenToWorld(mouse.X, mouse.Y);

            foreach (var weapon in this.player.Weapons)
            {
                weapon.Rotation = (float)Math.Atan2(this.mouse.Y - weapon.Position.Y, this.mouse.X - weapon.Position.X);
            }

            base.Update(gameTime);

            foreach (var enemy in this.enemies)
            {
                enemy.Accelerate(1f);
            }
            this.Camera.LookAt(this.player.Position);

            this.Camera.Zoom = MathHelper.SmoothStep(this.zoomSource, this.zoomTarget, this.zoomAmount);
            this.zoomAmount = this.zoomAmount + gameTime.GetElapsedSeconds() * 4;
            if (this.zoomAmount > 1f)
            {
                this.Camera.Zoom = this.zoomTarget;
                this.zoomSource = this.zoomTarget;
                this.zoomAmount = 0;
            }
        }

        public override void Draw(Renderer renderer)
        {
            this.Camera.Clear(Color.DarkSlateBlue);

            base.Draw(renderer);

            foreach (var weapon in this.player.Weapons)
            {
                var lastPos = weapon.Position;
                var colour = Color.Black;
                for (var i = 0; i < 4; i++)
                {
                    var pos = lastPos.AtBearing(weapon.Rotation, 128);
                    renderer.Render.DrawLine(lastPos, pos, colour);
                    colour.A -= 256 / 4;
                    lastPos = pos;
                }
            }
            //this.sprites["patrol_boat"].Draw(this.spriteBatch, this.player.Position, this.player.Bearing);
            //this.sprites["gun_single_barrel"].Draw(this.spriteBatch, this.player.Weapon.Position, this.player.Weapon.Bearing);
            //this.spriteBatch.DrawLine(this.player.Position, this.player.Position + new Vector2((float)Math.Cos(this.player.Bearing) * 32, (float)Math.Sin(this.player.Bearing) * 32), Color.Black);
            var envy12 = this.Context.Assets.Fonts["envy12"];
            var envy16 = this.Context.Assets.Fonts["envy16"];
            renderer.DrawString(envy12, string.Format("#objects: {0}", this.Context.NumObjects), this.Camera.ScreenToWorld(10, 10), Color.White);
            renderer.DrawString(envy12, string.Format("swv: {0}", Mouse.GetState().ScrollWheelValue), this.Camera.ScreenToWorld(10, 24), Color.White);
            renderer.DrawString(envy12, string.Format("zoom: {0}", this.Camera.Zoom), this.Camera.ScreenToWorld(10, 36), Color.White);

            if (AbstractObject.DebugInfo)
            {
                this.physics.Draw(renderer);
            }

            renderer.Render.DrawCircle(new CircleF(this.mouse, 8), 16, Color.IndianRed);
        }
    }
}
