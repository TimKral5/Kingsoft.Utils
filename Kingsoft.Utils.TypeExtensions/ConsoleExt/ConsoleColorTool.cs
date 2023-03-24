using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Kingsoft.Utils.TypeExtensions.ConsoleExt
{
    internal class ConsoleColorTool
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleMode(IntPtr hConsoleHandle, int mode);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetConsoleMode(IntPtr handle, out int mode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetStdHandle(int handle);

        public static void Setup()
        {
            var handle = GetStdHandle(-11);
            int mode;
            GetConsoleMode(handle, out mode);
            SetConsoleMode(handle, mode | 0x4);
        }

        public static string GetPrintable(Color? fc = null, Color? bc = null)
            => (fc != null ? "\x1b[38;2;" + fc.Value.R + ";" + fc.Value.G + ";" + fc.Value.B + "m" : "") + (bc != null ? "\x1b[48;2;" + bc.Value.R + ";" + bc.Value.G + ";" + bc.Value.B + "m" : "");
    }
}
