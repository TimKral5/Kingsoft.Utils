using System;
using System.Drawing;

namespace Kingsoft.Utils.TypeExtensions.ColorExt
{
    public static class ConsoleColorExtension
    {
        public static Color ToColor(this ConsoleColor color)
        {
            string n = Enum.GetName(typeof(ConsoleColor), color);
            return Color.FromName(n == "DarkYellow" ? "Orange" : n);
        }
    }
}
