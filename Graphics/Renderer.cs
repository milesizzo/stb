using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StopTheBoats.Templates;

namespace StopTheBoats.Graphics
{
    public class Renderer
    {
        public SpriteBatch Render;
        public readonly GraphicsDeviceManager Graphics;

        public Renderer(Game game)
        {
            this.Graphics = new GraphicsDeviceManager(game);
        }

        public void LoadContent()
        {
            this.Render = new SpriteBatch(this.Graphics.GraphicsDevice);
        }

        public void DrawString(FontTemplate font, string text, Vector2 position, Color colour)
        {
            this.Render.DrawString(font.Font, text, position, colour);
        }
    }
}
