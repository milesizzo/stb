using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StopTheBoats
{
    public static class KeyboardHelper
    {
        private static Dictionary<Keys, bool> keyHeld = new Dictionary<Keys, bool>();

        public static bool KeyPressed(Keys key)
        {
            if (Keyboard.GetState().IsKeyDown(key))
            {
                if (!keyHeld.ContainsKey(key))
                {
                    keyHeld[key] = true;
                    return true;
                }
            }
            else
            {
                keyHeld.Remove(key);
            }
            return false;
        }
    }
}
