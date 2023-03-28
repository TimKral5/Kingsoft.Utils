using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingsoft.Utils.DLL
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DLLDummy : Attribute
    {
        public string Namespace { get; private set; }
        public string Name { get; private set; }

        public DLLDummy() { Namespace = ""; Name = ""; }
        public DLLDummy(string ns, string name) { Namespace = ns; Name = name; }

        public DLLDummy(string name)
        {
            string[] fname = name.Split('.');
            Namespace = fname.Length > 0 ? fname[0] : "";
            for (int i = 1; i < fname.Length - 1; i++)
                Namespace += "." + fname[i];
            Name = fname[fname.Length - 1];
        }

        public string GetFullName() => Namespace + "." + Name;
    }
}
