using Microsoft.Xna.Framework;
using System;

namespace StopTheBoats
{
    public static class Common
    {
        public static Vector2 AtBearing(Vector2 pos, float bearing, float distance)
        {
            return pos + new Vector2((float)Math.Cos(bearing) * distance, (float)Math.Sin(bearing) * distance);
        }

        public static float Rad2Deg(float rad)
        {
            return (float)(rad * 180 / Math.PI);
        }

        public static float Deg2Rad(float deg)
        {
            return (float)(deg * Math.PI / 180);
        }
    }
}
