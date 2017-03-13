using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StopTheBoats.Templates;

namespace StopTheBoats.Graphics
{
    public class Renderer
    {
        private SpriteBatch render;
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
        }

        public SpriteBatch Render { get { return this.render; } }

        public void DrawString(FontTemplate font, string text, Vector2 position, Color colour)
        {
            this.Render.DrawString(font.Font, text, position, colour);
        }
    }
}
