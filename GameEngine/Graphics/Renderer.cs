using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.Graphics
{
    public class Renderer
    {
        private SpriteBatch render;
        private SpriteBatch screen;
        public readonly GraphicsDeviceManager Graphics;

        public Renderer(Game game)
        {
            this.Graphics = new GraphicsDeviceManager(game);

            // TODO: figure out how to make this configurable
            this.Graphics.PreferredBackBufferWidth = 2560;
            this.Graphics.PreferredBackBufferHeight = 1440;
            //this.Graphics.IsFullScreen = true;
            this.Graphics.ApplyChanges();

            // NOTE! we can only create the SpriteBatch after the changes have been applied!
            this.render = new SpriteBatch(this.Graphics.GraphicsDevice);
            this.screen = new SpriteBatch(this.Graphics.GraphicsDevice);
        }

        public SpriteBatch Render { get { return this.render; } }

        public SpriteBatch Screen { get { return this.screen; } }
    }
}
