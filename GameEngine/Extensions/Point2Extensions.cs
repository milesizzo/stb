using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace GameEngine.Extensions
{
    public static class Point2Extensions
    {
        public static Vector2 ToVector(this Point2 point)
        {
            return new Vector2(point.X, point.Y);
        }
    }
}
