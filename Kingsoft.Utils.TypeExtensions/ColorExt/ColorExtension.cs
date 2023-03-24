using System;
using System.Drawing;

namespace Kingsoft.Utils.TypeExtensions.ColorExt
{
    public static class ColorExtension
    {
        public static ConsoleColor GetClosestCColor(this Color self)
        {
            ConsoleColor ret = 0;
            double rr = self.R, gg = self.G, bb = self.B, delta = double.MaxValue;

            foreach (ConsoleColor cc in Enum.GetValues(typeof(ConsoleColor)))
            {
                var n = Enum.GetName(typeof(ConsoleColor), cc);
                var c = Color.FromName(
                    n == "DarkYellow" ? "Orange" : 
                    n == "DarkGray" ? "Gray" : 
                    n == "Gray" ? "DarkGray" :
                n);
                var t = Math.Pow(c.R - rr, 2.0) + Math.Pow(c.G - gg, 2.0) + Math.Pow(c.B - bb, 2.0);
                if (t == 0.0)
                    return cc;
                if (t < delta)
                {
                    delta = t;
                    ret = cc;
                }
            }
            return ret;
        }
    }
}
