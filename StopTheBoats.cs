using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using GameEngine;
using GameEngine.GameObjects;
using GameEngine.Graphics;
using GameEngine.Helpers;
using GameEngine.Extensions;
using StopTheBoats.Scenes;

namespace StopTheBoats
{
    public class StopTheBoats : SceneGame
    {
        public StopTheBoats()
        {
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
            this.Store.LoadFromJson("Content\\Base.json");

            this.Scenes.GetOrAdd("StopTheBoats", (key) =>
            {
                return new StopTheBoatsScene(key, this.GraphicsDevice, this.Store);
            });
            this.Scenes.GetOrAdd("BoundsEditor", (key) =>
            {
                return new PolygonBoundsEditor(key, this.GraphicsDevice, this.Store);
            });
            this.Scenes.GetOrAdd("MainMenu", (key) =>
            {
                return new UIScene(key, this.GraphicsDevice, this.Store);
            });
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            this.Store.SaveAllToJson("Content");
        }

        protected override void Update(GameTime gameTime)
        {
            if (KeyboardHelper.KeyPressed(Keys.F1))
            {
                this.SetCurrentScene("StopTheBoats");
            }
            else if (KeyboardHelper.KeyPressed(Keys.F2))
            {
                this.SetCurrentScene("BoundsEditor");
            }
            else if (KeyboardHelper.KeyPressed(Keys.F3))
            {
                this.SetCurrentScene("MainMenu");
            }
            else if (KeyboardHelper.KeyPressed(Keys.F12))
            {
                AbstractObject.DebugInfo = !AbstractObject.DebugInfo;
            }

            if (this.CurrentScene != null && this.CurrentScene.SceneEnded)
            {
                this.Exit();
            }

            base.Update(gameTime);
        }

        protected override void Draw(Renderer renderer)
        {
            var envy16 = this.Store["Base"].Fonts["envy16"];
            renderer.Screen.DrawString(envy16, string.Format("FPS: {0:0.0}", this.FPS), new Vector2(1024, 10), Color.White);
            base.Draw(renderer);
        }
    }
}
