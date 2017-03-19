using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameEngine.Templates;

namespace GameEngine.Extensions
{
    public static class SpriteBatchExtensions
    {
        public static void DrawString(this SpriteBatch sb, FontTemplate font, string text, Vector2 position, Color colour)
        {
            sb.DrawString(font.Font, text, position, colour);
        }
    }
}
