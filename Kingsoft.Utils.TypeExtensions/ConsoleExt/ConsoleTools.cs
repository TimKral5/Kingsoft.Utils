using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingsoft.Utils.TypeExtensions.ConsoleExt
{
    public static class ConsoleTools
    {
        private static int Width { get; set; }
        private static int Height { get; set; }
        private static bool IsSetup = false;

        private static void Setup()
        {
            if (IsSetup) return;
            IsSetup = true;

            ConsoleColorTool.Setup();
        }

        public static void HandleResize(Action handler = null, bool clearPrompt = false) => HandleResize((x, y) => handler(), clearPrompt);
        public static void HandleResize(Action<int, int> handler = null, bool clearPrompt = false)
        {
            int width = Console.WindowWidth,
                height = Console.WindowHeight;

            if (Width != width || Height != height)
            {
                Width = width;
                Height = height;

                try
                {
                    Console.SetBufferSize(width, height);
                    handler?.Invoke(width, height);
                    if (clearPrompt)
                    {
                        Console.Clear(); 
                        Console.WriteLine("\x1b[3J");
                    }
                }
                catch { }
            }
        }

        
        public static string SetConsoleColors(Color? fc = null, Color? bc = null)
        {
            Setup();
            return ConsoleColorTool.GetPrintable(fc, bc);
        }
    }
}
