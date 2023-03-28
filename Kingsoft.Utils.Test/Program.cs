using System;
using System.Numerics;
using System.Threading;
using Kingsoft.Utils.Programs.Engine;
using Newtonsoft.Json;
using System.IO;
using Kingsoft.Utils.Text;
using Kingsoft.Utils.Security.Cryptography;
using Kingsoft.Utils.Http.Client;
using Kingsoft.Utils.DLL;

namespace Kingsoft.Utils.Test
{
    internal class Program
    {
        static void Main(string[] args) => Engine.Invoke(new Program(), true);
        public static string PDir { get; set; }

        public void Awake()
        {
            DLLTool.Load(PDir + "/Kingsoft.Utils.TypeExtensions.dll");
            while (true) { }
        }
    }
}
