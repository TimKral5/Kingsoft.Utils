using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Kingsoft.Utils.DLL
{
    public static class DLLTool
    {
        public static void Load(string file)
        {
            var asm = Assembly.LoadFile(file);
            var t = asm.GetTypes();
            t.All(el => { Console.WriteLine(el.FullName); return true; });
        }
    }
}
