using System;
using System.Numerics;
using System.Threading;
using Kingsoft.Utils.Programs.Engine;
using YamlDotNet.Serialization;
using Newtonsoft.Json;
using System.IO;

namespace Kingsoft.Utils.Test
{
    internal class Program
    {
        static void Main(string[] args) => Engine.Invoke(new Program(), true);

        public void Awake()
        {
            Deserializer deserializer = new Deserializer();
            object obj = deserializer.Deserialize<object>(File.ReadAllText(@"C:\Users\TimKral\Desktop\Projects\PeopleDB\person.yaml"));
            Console.WriteLine(JsonConvert.SerializeObject(obj, Formatting.Indented));
            while (true)
            {

            }
        }
    }

    internal class Person
    {
        [YamlMember(Alias = "name")]
        public string Name { get; set; }
        [YamlMember(Alias = "name")]
        public string Name { get; set; }
    }
}
