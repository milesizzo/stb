using Microsoft.Xna.Framework;
using StopTheBoats.GameObjects;
using StopTheBoats.Graphics;
using StopTheBoats.Content;
using Microsoft.Xna.Framework.Graphics;

namespace StopTheBoats.Scenes
{
    public class GameScene<T> : IScene where T : IGameContext
    {
        protected readonly T Context;
        private readonly Camera camera;
        private bool sceneEnded;

        public GameScene(GraphicsDevice graphics, T context)
        {
            this.Context = context;
            this.camera = new Camera(graphics);
            this.sceneEnded = false;
        }

        public bool SceneEnded
        {
            get { return this.sceneEnded; }
            protected set { this.sceneEnded = value; }
        }

        public Camera Camera { get { return this.camera; } }

        public virtual void SetUp(AssetStore assets)
        {
            this.Context.Reset();
        }

        public virtual void Update(GameTime gameTime)
        {
            this.Context.Update(gameTime);
        }

        public virtual void Draw(Renderer renderer)
        {
            this.Context.Draw(renderer);
        }
    }
}
