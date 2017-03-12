using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StopTheBoats
{
    public static class RandomExtensions
    {
        public static T Choice<T>(this Random random, params T[] objs)
        {
            return objs[random.Next(0, objs.Length)];
        }
    }
}
