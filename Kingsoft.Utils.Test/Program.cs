using System;
using System.Numerics;
using System.Threading;
using Kingsoft.Utils.Programs.Engine;
using Newtonsoft.Json;
using System.IO;
using Kingsoft.Utils.Nuget.SpectreConsole;
using Spectre.Console;

namespace Kingsoft.Utils.Test
{
    internal class Program
    {
        static void Main(string[] args) => Engine.Invoke(new Program(), true);
        public static string PDir { get; set; }

        public void Awake()
        {
            var prompt = new ExplorerSelectionPrompt(backBtn: "[[...]]", frame: "[[{0}]]");
            prompt.SetPath("/hello/my/friend", (str, handler) => { handler.Resolve(); });
            prompt.SetPath("/hello/my/fellow/collegues", (str, handler) => { handler.Resolve(); });

            string res = AnsiConsole.Prompt(prompt);
            Console.WriteLine(res);
            while (true) { }
        }
    }
}
