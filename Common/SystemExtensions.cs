using Microsoft.Xna.Framework;
using System;

namespace StopTheBoats.Common
{
    public static class RandomExtensions
    {
        public static T Choice<T>(this Random random, params T[] objs)
        {
            return objs[random.Next(0, objs.Length)];
        }
    }

    public static class Vector2Extensions
    {
        public static Vector2 AtBearing(this Vector2 pos, float bearing, float distance)
        {
            return pos + new Vector2((float)Math.Cos(bearing) * distance, (float)Math.Sin(bearing) * distance);
        }
    }
}
