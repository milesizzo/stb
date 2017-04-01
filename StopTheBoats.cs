using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using GameEngine;
using GameEngine.GameObjects;
using GameEngine.Graphics;
using GameEngine.Helpers;
using GameEngine.Extensions;
using StopTheBoats.Scenes;
using GameEngine.Scenes;

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
            this.Scenes.GetOrAdd("MainMenu", (key) =>
            {
                var scene = new MainMenuScene(key, this.GraphicsDevice, this.Store);
                scene.SceneEnd += this.OnSceneEnd;
                return scene;
            });
            this.Scenes.GetOrAdd("StopTheBoats", (key) =>
            {
                var scene = new StopTheBoatsScene(key, this.GraphicsDevice, this.Store);
                scene.SceneEnd += this.OnSceneEnd;
                return scene;
            });
            this.Scenes.GetOrAdd("BoundsEditor", (key) =>
            {
                var scene = new PolygonBoundsEditor(key, this.GraphicsDevice, this.Store);
                scene.SceneEnd += this.OnSceneEnd;
                return scene;
            });
            this.SetCurrentScene("MainMenu");
        }

        private void OnSceneEnd(IScene scene)
        {
            var asMenu = scene as MainMenuScene;
            if (asMenu != null)
            {
                switch (asMenu.SelectedItem)
                {
                    case MenuItem.PlayGame:
                        this.SetCurrentScene("StopTheBoats");
                        break;
                    case MenuItem.Editor:
                        this.SetCurrentScene("BoundsEditor");
                        break;
                    case MenuItem.Quit:
                        this.Exit();
                        break;
                }
            }
            else
            {
                this.SetCurrentScene("MainMenu");
            }
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
