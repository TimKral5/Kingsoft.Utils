using System;
using System.Numerics;
using System.Threading;
using Kingsoft.Utils.Programs.Engine;
using Newtonsoft.Json;
using System.IO;
using Kingsoft.Utils.Text;

namespace Kingsoft.Utils.Test
{
    internal class Program
    {
        static void Main(string[] args) => Engine.Invoke(new Program(), true);

        public void Awake()
        {
            WordProcessor processor = new WordProcessor();
            processor.AddWordListener("hello", data =>
            {
                Console.WriteLine("found 'hello'!");
            });

            processor.Decode("hello world");

            while (true) { }
        }
    }
}
