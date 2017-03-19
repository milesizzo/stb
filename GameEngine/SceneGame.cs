using GameEngine.Graphics;
using GameEngine.Scenes;
using GameEngine.Templates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace GameEngine
{
    public class SceneGame : Game
    {
        private IScene currentScene;
        private TemplateStore<IScene> scenes;
        private Renderer renderer;
        private double frameRate;

        public SceneGame()
        {
            this.renderer = new Renderer(this);
            this.scenes = new TemplateStore<IScene>();
        }
        
        protected IScene CurrentScene
        {
            get { return this.currentScene; }
        }

        protected TemplateStore<IScene> Scenes
        {
            get { return this.scenes; }
        }

        protected double FPS
        {
            get { return this.frameRate; }
        }

        protected void SetCurrentScene(string name)
        {
            this.currentScene = this.Scenes[name];
            this.currentScene.SetUp();
        }

        protected override void Update(GameTime gameTime)
        {
            if (this.currentScene != null)
            {
                if (this.IsActive)
                {
                    this.currentScene.Update(gameTime);
                }
            }
            base.Update(gameTime);
        }

        protected sealed override void Draw(GameTime gameTime)
        {
            if (gameTime.GetElapsedSeconds() > 0)
            {
                this.frameRate = 1 / gameTime.GetElapsedSeconds();
            }

            if (this.currentScene != null)
            {
                this.renderer.Screen.Begin(blendState: BlendState.NonPremultiplied);
                this.renderer.World.Begin(blendState: BlendState.NonPremultiplied, transformMatrix: this.currentScene.Camera.GetViewMatrix());
                try
                {
                    this.currentScene.Draw(this.renderer);
                    this.Draw(this.renderer);
                }
                finally
                {
                    // end causes the graphics to be drawn - we want screen to happen last (overlay)
                    this.renderer.World.End();
                    this.renderer.Screen.End();
                }
            }
            base.Draw(gameTime);
        }

        protected virtual void Draw(Renderer renderer)
        {
            // do nothing
        }
    }
}
