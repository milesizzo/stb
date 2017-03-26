using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using FarseerPhysics.Dynamics;
using GameEngine.Scenes;
using GameEngine.GameObjects;
using GameEngine.Graphics;
using GameEngine.Extensions;
using StopTheBoats.GameObjects;
using StopTheBoats.Templates;
using FarseerPhysics.Collision.Shapes;
using StopTheBoats.Controllers;
using Microsoft.Xna.Framework.Content;
using GameEngine.Content;

namespace StopTheBoats.Scenes
{
    public class StopTheBoatsScene : GameScene<StbGameContext>
    {
        private float zoomAmount;
        private float zoomTarget;
        private float zoomSource;
        private BoatControllerCollection boats;
        private bool spacePressed = false;
        private int lastScroll = 0;
        private World physics;
        private RectangleF boundaries;

        public StopTheBoatsScene(string name, GraphicsDevice graphics, Store store) : base(name, graphics, store)
        {
            this.physics = new World(Vector2.Zero);
            this.Camera.Rotation = 0;
            this.Camera.Zoom = 1;
            this.zoomTarget = this.zoomSource = this.Camera.Zoom;
        }

        private void SetBoundaries(RectangleF rect)
        {
            var edges = new List<EdgeShape>();
            edges.Add(new EdgeShape(new Vector2(rect.Left, rect.Top), new Vector2(rect.Left, rect.Bottom)));
            edges.Add(new EdgeShape(new Vector2(rect.Right, rect.Top), new Vector2(rect.Right, rect.Bottom)));
            edges.Add(new EdgeShape(new Vector2(rect.Left, rect.Top), new Vector2(rect.Right, rect.Top)));
            edges.Add(new EdgeShape(new Vector2(rect.Left, rect.Bottom), new Vector2(rect.Right, rect.Bottom)));
            foreach (var edge in edges)
            {
                var body = new Body(this.physics, bodyType: BodyType.Static);
                body.CreateFixture(edge);
            }
            this.boundaries = rect;
        }

        private GameAssetStore StopTheBoatsAssets
        {
            get { return this.Store.Get<GameAssetStore>("StopTheBoats"); }
        }

        protected override StbGameContext CreateContext()
        {
            return new StbGameContext(this.Store);
        }

        public override void SetUp()
        {
            this.Store.LoadFromJson("Content\\StopTheBoats.json");

            base.SetUp();

            this.physics.Clear();

            this.boats = new BoatControllerCollection(this.Context, this.Camera, this.physics);

            // set up and add player
            var player = new Boat(this.Context, this.physics, this.StopTheBoatsAssets.Objects.Get<BoatTemplate>("boat.patrol"));
            this.boats.Add(new HumanBoatController(player, HumanBoatActionMap.Default));
            this.Context.AddObject(player);

            /*var player2 = new Boat(this.Context, this.physics, this.StopTheBoatsAssets.Objects.Get<BoatTemplate>("boat.small"));
            player2.Position = new Vector2(200, 0);
            this.boats.Add(new HumanBoatController(player2, HumanBoatActionMap.GamePad));
            this.Context.AddObject(player2);

            this.boats.FocusOn(player, player2);*/
            this.boats.FocusOn(player);

            // set up and add a random rock
            var random = new Random();
            var rock = new FixedObstacle(this.Context, this.physics, this.StopTheBoatsAssets.Sprites["rock1"])
            {
                Position = new Vector2(200, 200),
                Rotation = MathHelper.ToRadians(random.Next(0, 360)),
            };
            this.Context.AddObject(rock);

            var shore = new SpriteObject(this.Context, this.physics, this.StopTheBoatsAssets.Sprites["shore"])
            {
                Position = new Vector2(-2048, 800),
            };
            shore.Body.BodyType = BodyType.Static;
            shore.Fixture.Restitution = 0.001f;
            this.Context.AddObject(shore);

            this.SetBoundaries(new RectangleF(-2048, -1024, 4096, 1024 + 800 + shore.SpriteTemplate.Height));

            this.StopTheBoatsAssets.Audio["ambient2"].Audio.Play(0.1f, 0, 0);
            this.StopTheBoatsAssets.Audio["ambient1"].Audio.Play(0.02f, 0, -0.8f);
        }

        public override void Update(GameTime gameTime)
        {
            this.physics.Step((float)Math.Min(gameTime.ElapsedGameTime.TotalSeconds, 1f / 30f));

            var mouse = Mouse.GetState();
            var keyboard = Keyboard.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboard.IsKeyDown(Keys.Escape))
            {
                // quit
                this.SceneEnded = true;
            }
            if (keyboard.IsKeyDown(Keys.Space) && !this.spacePressed)
            {
                // spawn a computer-controlled boat
                var random = new Random();
                var topLeft = this.Camera.ScreenToWorld(0, -100);
                var topRight = this.Camera.ScreenToWorld(this.Camera.Viewport.Width, -100);
                var enemy = new Boat(this.Context, this.physics, this.StopTheBoatsAssets.Objects.Get<BoatTemplate>("boat.small"))
                {
                    Position = Vector2.Lerp(topLeft, topRight, (float)random.NextDouble()),
                    Rotation = MathHelper.ToRadians(90 + random.Next(-30, 30)),
                };
                this.boats.Add(new ComputerBoatController(enemy));
                this.Context.AddObject(enemy);
                this.spacePressed = true;
            }
            if (!keyboard.IsKeyDown(Keys.Space))
            {
                this.spacePressed = false;
            }
            if (mouse.ScrollWheelValue != this.lastScroll)
            {
                // scroll the mouse to zoom in/out
                var change = mouse.ScrollWheelValue - this.lastScroll;
                this.zoomSource = this.Camera.Zoom;
                this.zoomTarget = Math.Max(1f, this.Camera.Zoom + change / 200f);
                this.zoomAmount = 0;
                this.lastScroll = mouse.ScrollWheelValue;
            }

            // update the base scene (eg. game object context)
            base.Update(gameTime);

            // update the boat controllers
            this.boats.Update(gameTime);

            // update the camera zoom
            this.Camera.Zoom = MathHelper.SmoothStep(this.zoomSource, this.zoomTarget, this.zoomAmount);
            this.zoomAmount = this.zoomAmount + gameTime.GetElapsedSeconds() * 4;
            if (this.zoomAmount > 1f)
            {
                this.Camera.Zoom = this.zoomTarget;
                this.zoomSource = this.zoomTarget;
                this.zoomAmount = 0;
            }

            // ensure the camera doesn't scroll past the boundaries
            if (this.Camera.BoundingRectangle.Left < this.boundaries.Left)
            {
                var offset = new Vector2(this.boundaries.Left - this.Camera.BoundingRectangle.Left, 0f);
                this.Camera.Position += offset;
            }
            if (this.Camera.BoundingRectangle.Right > this.boundaries.Right)
            {
                var offset = new Vector2(this.boundaries.Right - this.Camera.BoundingRectangle.Right, 0f);
                this.Camera.Position += offset;
            }
            if (this.Camera.BoundingRectangle.Top < this.boundaries.Top)
            {
                var offset = new Vector2(0f, this.boundaries.Top - this.Camera.BoundingRectangle.Top);
                this.Camera.Position += offset;
            }
            if (this.Camera.BoundingRectangle.Bottom > this.boundaries.Bottom)
            {
                var offset = new Vector2(0f, this.boundaries.Bottom - this.Camera.BoundingRectangle.Bottom);
                this.Camera.Position += offset;
            }
        }

        public override void Draw(Renderer renderer)
        {
            this.Camera.Clear(Color.DarkBlue);

            base.Draw(renderer);

            // draw any boat controller specific objects (eg. mouse overlay for human player)
            this.boats.Draw(renderer);

            var envy12 = this.Store["Base"].Fonts["envy12"];
            var envy16 = this.Store["Base"].Fonts["envy16"];
            renderer.Screen.DrawString(envy12, string.Format("#objects: {0}", this.Context.NumObjects), new Vector2(10, 10), Color.White);
            renderer.Screen.DrawString(envy12, string.Format("swv: {0}", Mouse.GetState().ScrollWheelValue), new Vector2(10, 24), Color.White);
            renderer.Screen.DrawString(envy12, string.Format("zoom: {0}", this.Camera.Zoom), new Vector2(10, 36), Color.White);

            if (AbstractObject.DebugInfo)
            {
                this.physics.Draw(renderer);
            }

            var mouse = this.Camera.ScreenToWorld(Mouse.GetState().X, Mouse.GetState().Y);
            renderer.World.DrawCircle(new CircleF(mouse, 8), 16, Color.IndianRed);
        }
    }
}
