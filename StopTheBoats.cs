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
using StopTheBoats.Physics;
using StopTheBoats.Templates;
using StopTheBoats.GameObjects;
using StopTheBoats.Common;
using StopTheBoats.Graphics;
using StopTheBoats.Content;
using StopTheBoats.Scenes;

namespace StopTheBoats
{
    public class GameContext : IGameContext
    {
        private class ScheduledObject
        {
            public float TimeRemaining;
            public GameObject Object;
        }

        private readonly List<GameObject> objects = new List<GameObject>();
        private readonly Physics<PolygonBounds> physics;
        private readonly GameAssetStore assets;
        private readonly List<ScheduledObject> scheduled = new List<ScheduledObject>();

        public GameContext(GameAssetStore assets)
        {
            this.physics = new Physics<PolygonBounds>(new PolygonDetector());
            this.assets = assets;
        }

        public void Reset()
        {
            this.physics.Reset();
            foreach (var obj in this.objects)
            {
                obj.Context = null;
            }
            this.objects.Clear();
            this.scheduled.Clear();
        }

        public GameAssetStore Assets { get { return this.assets; } }

        public int NumObjects { get { return this.objects.Count; } }

        public void ScheduleObject(GameObject obj, float waitTime)
        {
            this.scheduled.Add(new ScheduledObject
            {
                TimeRemaining = waitTime,
                Object = obj,
            });
        }

        public void AddObject(GameObject obj)
        {
            obj.Context = this;
            this.objects.Add(obj);
            var asPhysics = obj as IPhysicsObject<PolygonBounds>;
            if (asPhysics != null)
            {
                this.physics.AddActor(asPhysics);
            }
        }

        private void RemovePhysics(GameObject obj)
        {
            var asPhysics = obj as IPhysicsObject<PolygonBounds>;
            if (asPhysics != null)
            {
                this.physics.RemoveActor(asPhysics);
            }
        }

        public void RemoveObject(GameObject obj)
        {
            obj.Context = null;
            this.objects.Remove(obj);
            this.RemovePhysics(obj);
        }

        public void Update(GameTime gameTime)
        {
            this.physics.DetectCollisions();
            var i = 0;
            while (i < this.scheduled.Count)
            {
                var schedule = this.scheduled[i];
                schedule.TimeRemaining -= gameTime.GetElapsedSeconds();
                if (schedule.TimeRemaining < 0)
                {
                    this.AddObject(schedule.Object);
                    this.scheduled.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
            foreach (var obj in this.objects)
            {
                obj.Update(gameTime);
                if (obj.IsAwaitingDeletion)
                {
                    this.RemovePhysics(obj);
                }
            }
            this.objects.RemoveAll(o => o.IsAwaitingDeletion);
        }

        public void Draw(Renderer renderer)
        {
            foreach (var obj in this.objects)
            {
                obj.Draw(renderer);
            }
        }
    }

    public static class StopTheBoatsHelper
    {
        public static void LoadGameAssets(GameAssetStore assets)
        {
            // load sprite templates
            var patrol_boat = assets.Sprites.GetOrAdd("patrol_boat", (key) =>
            {
                var obj = assets.Sprites.Load(key);
                obj.Origin = new Vector2(19, 31);
                return obj;
            });
            var small_boat = assets.Sprites.GetOrAdd("small_boat", (key) =>
            {
                var obj = assets.Sprites.Load(key);
                obj.Origin = new Vector2(19, 27);
                return obj;
            });
            var gun_single_barrel = assets.Sprites.GetOrAdd("gun_single_barrel", (key) =>
            {
                var obj = assets.Sprites.Load(key);
                obj.Origin = new Vector2(11, 11);
                return obj;
            });
            var rock1 = assets.Sprites.GetOrAdd("rock1", (key) =>
            {
                return assets.Sprites.Load(key);
            });
            var explosion1 = assets.Sprites.GetOrAdd("explosion_sheet1", (key) =>
            {
                var obj = assets.Sprites.Load(64, 64, key);
                obj.FPS = 30;
                return obj;
            });
            var explosion2 = assets.Sprites.GetOrAdd("explosion_sheet2", (key) =>
            {
                var obj = assets.Sprites.Load(100, 100, key);
                obj.FPS = 60;
                return obj;
            });
            var explosion3 = assets.Sprites.GetOrAdd("explosion_sheet3", (key) =>
            {
                var obj = assets.Sprites.Load(100, 100, key);
                obj.FPS = 50;
                return obj;
            });

            // load boat templates
            var patrolBoat = assets.Objects.GetOrAdd("boat.patrol", (key) =>
            {
                var boat = new BoatTemplate
                {
                    Acceleration = 50.0f,
                    SpriteTemplate = patrol_boat,
                    MaxHealth = 1000f,
                };
                boat.WeaponLocations.Add(new Vector2(99, 0));
                boat.WeaponLocations.Add(new Vector2(20, 0));
                return boat;
            });
            var smallBoat = assets.Objects.GetOrAdd("boat.small", (key) =>
            {
                return new BoatTemplate
                {
                    Acceleration = 75.0f,
                    SpriteTemplate = small_boat,
                    MaxHealth = 200f,
                };
            });

            // load weapon templates
            assets.Objects.GetOrAdd("gun.single_barrel", (key) =>
            {
                return new WeaponTemplate
                {
                    SpriteTemplate = gun_single_barrel,
                    ProjectileVelocity = 1000f,
                    FireRate = TimeSpan.FromSeconds(1),
                    Damage = 100f,
                };
            });
        }
    }

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

        public StopTheBoatsScene(GraphicsDevice graphics, GameAssetStore assets) : base(graphics, new GameContext(assets))
        {
            this.Camera.Rotation = 0;
            this.Camera.Zoom = 1;
            this.zoomTarget = this.zoomSource = this.Camera.Zoom;
        }

        public override void SetUp()
        {
            base.SetUp();

            StopTheBoatsHelper.LoadGameAssets(this.Assets);

            // set up and add player
            this.player = new Boat(this.Assets.Objects.Get<BoatTemplate>("boat.patrol"));
            this.player.Position = Vector2.Zero;
            this.player.AddWeapon(this.Assets.Objects.Get<WeaponTemplate>("gun.single_barrel"));
            this.player.AddWeapon(this.Assets.Objects.Get<WeaponTemplate>("gun.single_barrel"));
            this.Context.AddObject(this.player);

            // set up and add a random rock
            var random = new Random();
            this.Context.AddObject(new GameElement(FrictionMedium.None)
            {
                Position = new Vector2(200, 200),
                SpriteTemplate = this.Context.Assets.Sprites["rock1"],
                Rotation = MathHelper.ToRadians(random.Next(0, 360)),
            });
        }

        public override void Update(GameTime gameTime)
        {
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
            if (keyboard.IsKeyDown(Keys.A))
            {
                this.player.Turn(-20 * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            if (keyboard.IsKeyDown(Keys.D))
            {
                this.player.Turn(20 * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            if (keyboard.IsKeyDown(Keys.Space) && !this.spacePressed)
            {
                var random = new Random();
                var enemy = new Boat(this.Assets.Objects.Get<BoatTemplate>("boat.small"));
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
                    var projectile = weapon.Fire(gameTime);
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
                var world = weapon.World;
                weapon.WorldRotation = (float)Math.Atan2(this.mouse.Y - world.Position.Y, this.mouse.X - world.Position.X);
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
                var lastPos = weapon.World.Position;
                var colour = Color.Black;
                for (var i = 0; i < 4; i++)
                {
                    var pos = lastPos.AtBearing(weapon.World.Rotation, 128);
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

            renderer.Render.DrawCircle(new CircleF(this.mouse, 8), 16, Color.IndianRed);
        }
    }

    public class ObjectEditorScene : GameAssetScene
    {
        private IGameObjectTemplate current;
        private SpriteTemplate cursor;

        public ObjectEditorScene(GraphicsDevice graphics, GameAssetStore assets) : base(graphics, assets)
        {
            //
        }

        public IGameObjectTemplate Current
        {
            get { return this.current; }
            set { this.current = value; }
        }

        public override void SetUp()
        {
            StopTheBoatsHelper.LoadGameAssets(this.Assets);
            this.cursor = this.Assets.Sprites.GetOrAdd("editor_cursor", (key) =>
            {
                var sprite = this.Assets.Sprites.Load("editor_cursor");
                sprite.Origin = Vector2.Zero;
                return sprite;
            });

            this.Current = this.Assets.Objects["boat.patrol"];
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(Renderer renderer)
        {
            this.Camera.Clear(Color.DarkSlateBlue);
            var mouse = Mouse.GetState().Position;
            this.cursor.DrawSprite(renderer, new Vector2(mouse.X, mouse.Y), Color.White, 0, Vector2.One, SpriteEffects.None);
            if (this.Current != null)
            {
            }
            //renderer.Render.DrawPoint(new Vector2(mouse.X, mouse.Y), Color.White);
            //renderer.Render.DrawCircle(new Vector2(mouse.X, mouse.Y), 9, 16, Color.Gray);
        }
    }

    public class PolygonBoundsEditorScene : GameAssetScene
    {
        private SpriteTemplate current;
        private List<Vector2> points;

        private SpriteTemplate cursor;

        public PolygonBoundsEditorScene(GraphicsDevice graphics, GameAssetStore assets) : base(graphics, assets)
        {
            this.Camera.Zoom = 2;
        }

        public SpriteTemplate Current
        {
            get { return this.current; }
            set
            {
                this.current = value;
                this.points = new List<Vector2>(this.current.Bounds.Points);
            }
        }

        public override void SetUp()
        {
            StopTheBoatsHelper.LoadGameAssets(this.Assets);
            this.cursor = this.Assets.Sprites.GetOrAdd("editor_cursor", (key) =>
            {
                var sprite = this.Assets.Sprites.Load("editor_cursor");
                sprite.Origin = Vector2.Zero;
                return sprite;
            });

            this.Current = this.Assets.Sprites["patrol_boat"];
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(Renderer renderer)
        {
            this.Camera.Clear(Color.DarkSlateBlue);
            var mouse = Mouse.GetState().Position;
            this.cursor.DrawSprite(renderer, new Vector2(mouse.X, mouse.Y), Color.White, 0, Vector2.One, SpriteEffects.None);
            if (this.Current != null)
            {
                var position = new Vector2(this.Camera.Viewport.Width / 2, this.Camera.Viewport.Height / 2);
                this.Current.DrawSprite(renderer, position, Color.White, 0, Vector2.One, SpriteEffects.None);

                renderer.Render.DrawPoint(position + this.Current.Origin, Color.White, size: 3);
                renderer.Render.DrawPolygon(position, this.points.ToArray(), Color.Yellow);
            }
            //renderer.Render.DrawPoint(new Vector2(mouse.X, mouse.Y), Color.White);
            //renderer.Render.DrawCircle(new Vector2(mouse.X, mouse.Y), 9, 16, Color.Gray);
        }
    }

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class StopTheBoats : Game
    {
        private IScene currentScene;
        private TemplateStore<IScene> scenes;
        private GameAssetStore assets;
        private Renderer renderer;
        private int frameCounter;
        private double frameRate;
        private Dictionary<Keys, bool> keyHeld = new Dictionary<Keys, bool>();

        public StopTheBoats()
        {
            this.Content.RootDirectory = "Content";
            this.renderer = new Renderer(this);

            this.assets = new GameAssetStore(this.Content);
            this.scenes = new TemplateStore<IScene>();
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
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            //this.renderer.LoadContent();

            // TODO: use this.Content to load your game content here
            this.assets.Fonts.Add("envy12", new FontTemplate(this.Content.Load<SpriteFont>("Envy12")));
            this.assets.Fonts.Add("envy16", new FontTemplate(this.Content.Load<SpriteFont>("Envy16")));

            this.scenes.GetOrAdd("game", (key) =>
            {
                return new StopTheBoatsScene(this.GraphicsDevice, this.assets);
            });
            this.scenes.GetOrAdd("editor.bounds", (key) =>
            {
                return new PolygonBoundsEditorScene(this.GraphicsDevice, this.assets);
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

        private bool KeyPressed(Keys key)
        {
            if (Keyboard.GetState().IsKeyDown(key))
            {
                if (!this.keyHeld.ContainsKey(key))
                {
                    this.keyHeld[key] = true;
                    return true;
                }
            }
            else
            {
                this.keyHeld.Remove(key);
            }
            return false;
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (this.KeyPressed(Keys.F1))
            {
                this.currentScene = this.scenes["game"];
                this.currentScene.SetUp();
            }
            else if (this.KeyPressed(Keys.F2))
            {
                this.currentScene = this.scenes["editor.bounds"];
                this.currentScene.SetUp();
            }

            if (this.currentScene != null)
            {
                if (this.IsActive)
                {
                    this.currentScene.Update(gameTime);
                }

                if (this.currentScene.SceneEnded)
                {
                    this.Exit();
                }
            }

            if (gameTime.GetElapsedSeconds() > 0)
            {
                this.frameRate = this.frameCounter / gameTime.GetElapsedSeconds();
            }
            this.frameCounter = 0;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (this.currentScene != null)
            {
                this.renderer.Render.Begin(blendState: BlendState.NonPremultiplied, transformMatrix: this.currentScene.Camera.GetViewMatrix());
                try
                {
                    this.currentScene.Draw(this.renderer);

                    var envy16 = this.assets.Fonts["envy16"];
                    this.renderer.DrawString(envy16, string.Format("FPS: {0:0.0}", this.frameRate), this.currentScene.Camera.ScreenToWorld(1024, 10), Color.White);
                }
                finally
                {
                    this.renderer.Render.End();
                }
            }
            base.Draw(gameTime);

            this.frameCounter++;
        }
    }
}
