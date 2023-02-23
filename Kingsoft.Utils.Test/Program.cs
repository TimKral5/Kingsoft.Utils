using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingsoft.Utils.Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Http.Server.HttpListener server = new Http.Server.HttpListener();

            server.Get("/api/login", (_args, self, WriteRes, req) =>
            {
                var body = Http.Server.HttpListener.Utils.GetFPostBody(req);
                Console.WriteLine(body);
                byte[] data = Encoding.UTF8.GetBytes("hello world!!!");
                WriteRes(self, (data, Encoding.UTF8, "text/plain", data.LongLength), null);
                return data;
            });

            server.RunServer(3000);
        }
    }
}
