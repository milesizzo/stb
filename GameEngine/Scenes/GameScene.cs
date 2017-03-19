using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameEngine.GameObjects;
using GameEngine.Graphics;

namespace GameEngine.Scenes
{
    // any scene that requires game assets
    public abstract class GameAssetScene : IScene
    {
        private readonly GameAssetStore assets;
        private readonly Camera camera;
        private bool sceneEnded;

        public GameAssetScene(GraphicsDevice graphics, GameAssetStore assets)
        {
            this.assets = assets;
            this.camera = new Camera(graphics);
            this.sceneEnded = false;
        }

        public Camera Camera { get { return this.camera; } }

        protected GameAssetStore Assets { get { return this.assets; } }

        public bool SceneEnded
        {
            get { return this.sceneEnded; }
            protected set { this.sceneEnded = value; }
        }

        public abstract void SetUp();

        public abstract void Update(GameTime gameTime);

        public abstract void Draw(Renderer renderer);
    }

    // a game asset scene that also includes a game context
    public abstract class GameScene<T> : GameAssetScene where T : IGameContext
    {
        protected readonly T Context;

        public GameScene(GraphicsDevice graphics, T context) : base(graphics, context.Assets)
        {
            this.Context = context;
        }

        public override void SetUp()
        {
            this.Context.Reset();
        }

        public override void Update(GameTime gameTime)
        {
            this.Context.Update(gameTime);
        }

        public override void Draw(Renderer renderer)
        {
            this.Context.Draw(renderer);
        }
    }
}
