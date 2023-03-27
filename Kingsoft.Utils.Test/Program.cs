using System;
using System.Numerics;
using System.Threading;
using Kingsoft.Utils.Programs.Engine;
using Newtonsoft.Json;
using System.IO;
using Kingsoft.Utils.Text;
using Kingsoft.Utils.Security.Cryptography;
using Kingsoft.Utils.Http.Client;

namespace Kingsoft.Utils.Test
{
    internal class Program
    {
        static void Main(string[] args) => Engine.Invoke(new Program(), true);

        public void Awake()
        {
            string key = Guid.NewGuid().ToString();
            HttpRequest req = new HttpRequest();
            req.Open("GET", "http://localhost:3005/register", false);
            req.Send(key);
            Console.WriteLine(StringEncription.Instance.Decrypt(req.ResponseText, key));
            while (true) { }
        }
    }
}
