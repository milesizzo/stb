using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using PhysicsEngine;
using CommonLibrary;
using System.Collections.Generic;
using System.Linq;
using System;
using PhysicsEngine.Farseer;

namespace PhysicsEngineTest
{
    public class PhysicalObject
    {
        private readonly IBody body;

        public PhysicalObject(IBody body)
        {
            this.body = body;
        }

        public IBody Body { get { return this.body; } }

        public Vector2 Position
        {
            get { return this.body.Position; }
            set { this.body.Position = value; }
        }

        public float Rotation
        {
            get { return this.body.Rotation; }
            set { this.body.Rotation = value; }
        }
    }
    
    public class PhysicsTest : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private IPhysicsEngine physics;
        private bool spaceDown;

        public PhysicsTest()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 2560;
            graphics.PreferredBackBufferHeight = 1440;
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            this.physics = new FarseerEngine();
            //this.physics.WorldDamping = 0.5f;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            IBody body;
            this.physics.CreateBody(new PhysicsCircle(64f, 1f), out body);
            body.Position = new Vector2(795, 0);
            body.LinearVelocity = new Vector2(0, 100);

            IBody target;
            this.physics.CreateBody(new PhysicsPolygon(new[]
            {
                new Vector2(-16, -16),
                new Vector2(16, -16),
                new Vector2(16, 16),
                new Vector2(-16, 16),
            }, 1f), out target);
            target.Position = new Vector2(800, 800);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Space) && !this.spaceDown)
            {
                this.spaceDown = true;
            }
            else if (!Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                this.spaceDown = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.OemTilde))
            {
            }

            var mouse = Mouse.GetState();
            //this.body.Position = new Vector2(mouse.X, mouse.Y);

            // TODO: Add your update logic here
            //this.simulation.Update(gameTime);
            //_world.Step(Math.Min((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f, (1f / 30f)));
            this.physics.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            this.spriteBatch.Begin();

            this.physics.Draw(this.spriteBatch);
            this.spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
