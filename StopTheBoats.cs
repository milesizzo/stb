using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace StopTheBoats
{
    public class RenderStore
    {
        public SpriteBatch Render;
        public readonly TemplateStore<FontTemplate> Fonts;
        public readonly SpriteStore Sprites;
        public readonly GraphicsDeviceManager Graphics;

        public RenderStore(Game game)
        {
            this.Sprites = new SpriteStore(game.Content);
            this.Fonts = new TemplateStore<FontTemplate>();
            this.Graphics = new GraphicsDeviceManager(game);
        }

        public void LoadContent()
        {
            this.Render = new SpriteBatch(this.Graphics.GraphicsDevice);
        }

        public void DrawString(string font, string text, Vector2 position, Color colour)
        {
            this.Render.DrawString(this.Fonts[font].Font, text, position, colour);
        }
    }

    public class GameStore
    {
        private readonly List<GameObject> objects = new List<GameObject>();

        public GameStore()
        {
            //
        }

        public int NumObjects { get { return this.objects.Count; } }

        public void AddObject(GameObject obj)
        {
            this.objects.Add(obj);
        }

        public void RemoveObject(GameObject obj)
        {
            this.objects.Remove(obj);
        }

        public void Update(GameTime gameTime)
        {
            foreach (var obj in this.objects)
            {
                obj.Update(gameTime);
            }
            this.objects.RemoveAll(o => o.IsAwaitingDeletion);
        }

        public void Draw(RenderStore render)
        {
            foreach (var obj in this.objects)
            {
                obj.Draw(render);
            }
        }
    }

    public class FontTemplate : Template
    {
        public SpriteFont Font;

        public FontTemplate(SpriteFont font)
        {
            this.Font = font;
        }
    }

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class StopTheBoats : Game
    {
        private Camera2D camera;
        private float zoomAmount;
        private float zoomTarget;
        private float zoomSource;
        //private float rotationAmount;
        private RenderStore render;
        private GameStore store = new GameStore();
        private Boat player;
        private readonly List<Boat> enemies = new List<Boat>();
        private Vector2 mouse;
        private TemplateStore<BoatTemplate> boats;
        private TemplateStore<WeaponTemplate> weapons;
        private bool spacePressed;
        private int lastScroll;

        public StopTheBoats()
        {
            this.Content.RootDirectory = "Content";
            this.render = new RenderStore(this);
            this.render.Graphics.PreferredBackBufferWidth = 2560;
            this.render.Graphics.PreferredBackBufferHeight = 1440;
            this.render.Graphics.ApplyChanges();

            this.boats = new TemplateStore<BoatTemplate>();
            this.weapons = new TemplateStore<WeaponTemplate>();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            this.camera = new Camera2D(this.GraphicsDevice);
            this.camera.Rotation = 0;
            this.camera.Zoom = 1;
            this.zoomTarget = this.zoomSource = this.camera.Zoom;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            this.render.LoadContent();

            // TODO: use this.Content to load your game content here
            this.render.Fonts.Add("envy", new FontTemplate(this.Content.Load<SpriteFont>("Envy")));

            var patrol_boat = this.render.Sprites.Load("patrol_boat");
            patrol_boat.Origin = new Vector2(19, 31);
            var small_boat = this.render.Sprites.Load("small_boat");
            small_boat.Origin = new Vector2(19, 27);
            var gun_single_barrel = this.render.Sprites.Load("gun_single_barrel");
            gun_single_barrel.Origin = new Vector2(11, 11);
            var rock1 = this.render.Sprites.Load("rock1");

            var patrolBoat = new BoatTemplate {
                Acceleration = 50.0f,
                SpriteTemplate = patrol_boat
            };
            patrolBoat.WeaponLocations.Add(new Vector2(99, 0));
            patrolBoat.WeaponLocations.Add(new Vector2(20, 0));
            this.boats.Add("patrol", patrolBoat);

            var smallBoat = new BoatTemplate
            {
                Acceleration = 75.0f,
                SpriteTemplate = small_boat
            };
            this.boats.Add("small", smallBoat);

            this.weapons.Add("single_barrel_gun", new WeaponTemplate
            {
                SpriteTemplate = gun_single_barrel,
                ProjectileVelocity = 1000f,
                FireRate = TimeSpan.FromSeconds(1),
            });

            this.player = new Boat(this.boats["patrol"]);
            this.player.Position = Vector2.Zero;
            this.player.AddWeapon(this.weapons["single_barrel_gun"]);
            this.player.AddWeapon(this.weapons["single_barrel_gun"]);

            this.store.AddObject(this.player);

            var random = new Random();
            this.store.AddObject(new GameElement(FrictionMedium.None)
            {
                Position = new Vector2(200, 200),
                SpriteTemplate = this.render.Sprites["rock1"],
                Rotation = MathHelper.ToRadians(random.Next(0, 360)),
            });
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (this.IsActive)
            {
                var mouse = Mouse.GetState();
                var keyboard = Keyboard.GetState();

                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboard.IsKeyDown(Keys.Escape))
                    Exit();
                if (keyboard.IsKeyDown(Keys.W))
                {
                    this.player.Accelerate(1f);
                }
                if (keyboard.IsKeyDown(Keys.S))
                {
                    this.player.Accelerate(-1f);
                }
                if (keyboard.IsKeyDown(Keys.A))
                {
                    this.player.Turn(-20 * (float)gameTime.ElapsedGameTime.TotalSeconds);
                }
                if (keyboard.IsKeyDown(Keys.D))
                {
                    this.player.Turn(20 * (float)gameTime.ElapsedGameTime.TotalSeconds);
                }
                if (keyboard.IsKeyDown(Keys.OemTilde))
                {
                    //this.zoomAmount = 0;
                    //this.rotationAmount = 0;
                }
                if (keyboard.IsKeyDown(Keys.Space) && !this.spacePressed)
                {
                    var random = new Random();
                    var enemy = new Boat(this.boats["small"]);
                    var topLeft = this.camera.ScreenToWorld(0, -100);
                    var topRight = this.camera.ScreenToWorld(this.GraphicsDevice.Viewport.Width, -100);
                    enemy.Position = Vector2.Lerp(topLeft, topRight, (float)random.NextDouble());
                    //enemy.Position = new Vector2((float)random.NextDouble() * 2560, -100);
                    enemy.Rotation = MathHelper.ToRadians(90 + random.Next(-30, 30));
                    this.enemies.Add(enemy);
                    this.store.AddObject(enemy);
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
                        var projectile = weapon.Fire(gameTime);
                        if (projectile != null)
                        {
                            this.store.AddObject(projectile);
                        }
                    }
                }
                if (mouse.ScrollWheelValue != this.lastScroll)
                {
                    var change = mouse.ScrollWheelValue - this.lastScroll;
                    this.zoomSource = this.camera.Zoom;
                    this.zoomTarget = Math.Max(1f, this.camera.Zoom + change / 200f);
                    this.zoomAmount = 0;
                    this.lastScroll = mouse.ScrollWheelValue;
                }
                this.mouse = this.camera.ScreenToWorld(mouse.X, mouse.Y);

                foreach (var weapon in this.player.Weapons)
                {
                    var world = weapon.World;
                    weapon.WorldRotation = (float)Math.Atan2(this.mouse.Y - world.Position.Y, this.mouse.X - world.Position.X);
                }
            }

            // TODO: Add your update logic here
            this.store.Update(gameTime);
            foreach (var enemy in this.enemies)
            {
                enemy.Accelerate(1f);
            }
            this.camera.LookAt(this.player.Position);

            this.camera.Zoom = MathHelper.SmoothStep(this.zoomSource, this.zoomTarget, this.zoomAmount);
            this.zoomAmount = this.zoomAmount + gameTime.GetElapsedSeconds() * 4;
            if (this.zoomAmount > 1f)
            {
                this.camera.Zoom = this.zoomTarget;
                this.zoomSource = this.zoomTarget;
                this.zoomAmount = 0;
            }

            //this.camera.Zoom = MathHelper.SmoothStep(6, 1, this.zoomAmount);
            //this.camera.Rotation = MathHelper.SmoothStep(0, MathHelper.Pi * 2, this.rotationAmount);

            //this.zoomAmount = Math.Min(1.0f, this.zoomAmount + gameTime.GetElapsedSeconds() / 2);
            //this.rotationAmount = Math.Min(1.0f, this.rotationAmount + gameTime.GetElapsedSeconds() / 2);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkSlateBlue);

            // TODO: Add your drawing code here
            this.render.Render.Begin(blendState: BlendState.NonPremultiplied, transformMatrix: this.camera.GetViewMatrix());
            this.store.Draw(this.render);

            foreach (var weapon in this.player.Weapons)
            {
                var lastPos = weapon.World.Position;
                var colour = Color.Black;
                for (var i = 0; i < 4; i++)
                {
                    var pos = Common.AtBearing(lastPos, weapon.World.Rotation, 128);
                    this.render.Render.DrawLine(lastPos, pos, colour);
                    colour.A -= 256 / 4;
                    lastPos = pos;
                }
            }
            //this.sprites["patrol_boat"].Draw(this.spriteBatch, this.player.Position, this.player.Bearing);
            //this.sprites["gun_single_barrel"].Draw(this.spriteBatch, this.player.Weapon.Position, this.player.Weapon.Bearing);
            //this.spriteBatch.DrawLine(this.player.Position, this.player.Position + new Vector2((float)Math.Cos(this.player.Bearing) * 32, (float)Math.Sin(this.player.Bearing) * 32), Color.Black);
            this.render.DrawString("envy", string.Format("#objects: {0}", this.store.NumObjects), this.camera.ScreenToWorld(10, 10), Color.White);
            this.render.DrawString("envy", string.Format("swv: {0}", Mouse.GetState().ScrollWheelValue), this.camera.ScreenToWorld(10, 24), Color.White);
            this.render.DrawString("envy", string.Format("zoom: {0}", this.camera.Zoom), this.camera.ScreenToWorld(10, 36), Color.White);
            this.render.DrawString("envy", string.Format("Speed: {0}", this.player.Velocity), this.player.Position - new Vector2(0, 100), Color.White);
            //this.spriteBatch.DrawPoint(this.mouse, Color.Yellow, size: 8);
            this.render.Render.DrawCircle(new CircleF(this.mouse, 8), 16, Color.IndianRed);
            this.render.Render.End();

            base.Draw(gameTime);
        }
    }
}
