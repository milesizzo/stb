using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameEngine.GameObjects;
using GameEngine.Graphics;
using GameEngine.Content;
using Microsoft.Xna.Framework.Content;

namespace GameEngine.Scenes
{
    // any scene that requires game assets
    public abstract class GameAssetScene : IScene
    {
        private readonly Store store;
        private readonly Camera camera;
        private readonly string name;
        private bool sceneEnded;

        protected GameAssetScene(string name, GraphicsDevice graphics, ContentManager content) : this(name, graphics, new Store(content))
        {
        }

        protected GameAssetScene(string name, GraphicsDevice graphics, Store store)
        {
            this.name = name;
            this.store = store;
            this.camera = new Camera(graphics);
            this.sceneEnded = false;
        }

        public Store Store { get { return this.store; } }
        
        public string Name { get { return this.name; } }

        public Camera Camera { get { return this.camera; } }

        //protected GameAssetStore Assets { get { return this.assets; } }

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
        private T context;

        protected GameScene(string name, GraphicsDevice graphics, ContentManager content) : base(name, graphics, content)
        {
        }

        protected GameScene(string name, GraphicsDevice graphics, Store store) : base(name, graphics, store)
        {
        }

        protected T Context { get { return this.context; } }

        public override void SetUp()
        {
            this.context = this.CreateContext();
        }

        protected abstract T CreateContext();

        public override void Update(GameTime gameTime)
        {
            this.context.Update(gameTime);
        }

        public override void Draw(Renderer renderer)
        {
            this.context.Draw(renderer);
        }
    }
}
