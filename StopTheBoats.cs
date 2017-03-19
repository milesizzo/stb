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
using GameEngine.GameObjects;
using GameEngine.Scenes;
using GameEngine.Templates;
using GameEngine.Graphics;
using StopTheBoats.Scenes;
using GameEngine.Helpers;

namespace StopTheBoats
{
    /*public class ObjectEditorScene : GameAssetScene
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
    }*/

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
            StopTheBoatsHelper.LoadFonts(this.assets);

            this.scenes.GetOrAdd("game", (key) =>
            {
                return new StopTheBoatsScene(this.GraphicsDevice, this.assets);
            });
            this.scenes.GetOrAdd("editor.bounds", (key) =>
            {
                return new PolygonBoundsEditor(this.GraphicsDevice, this.assets);
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
            if (KeyboardHelper.KeyPressed(Keys.F1))
            {
                this.currentScene = this.scenes["game"];
                this.currentScene.SetUp();
            }
            else if (KeyboardHelper.KeyPressed(Keys.F2))
            {
                this.currentScene = this.scenes["editor.bounds"];
                this.currentScene.SetUp();
            }
            else if (KeyboardHelper.KeyPressed(Keys.F12))
            {
                AbstractObject.DebugInfo = !AbstractObject.DebugInfo;
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
