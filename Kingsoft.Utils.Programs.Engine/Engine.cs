using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kingsoft.Utils.Programs.Engine
{
    internal struct EngineExecutionContext
    {
        public MethodInfo Awake { get; set; }
        public MethodInfo Load { get; set; }
        public MethodInfo Update { get; set; }
        public MethodInfo Update30 { get; set; }
        public MethodInfo Update60 { get; set; }

        public bool isConsole { get; set; }
        public MethodInfo ResizeEvent { get; set; }
    }

    internal class CA_Data
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class Engine
    {
        public static void Invoke(object obj, bool useFields = false)
        {
            Type type = obj.GetType();
            PropertyInfo[] properties = type.GetProperties();
            MethodInfo[] methods = type.GetMethods();
            EventInfo[] events = type.GetEvents();
            type.GetEvents();

            EngineExecutionContext ctx = new EngineExecutionContext()
            {
                Awake = null,
                Load = null,
                Update = null,
                Update30 = null,
                Update60 = null,
            };

            if (useFields)
                properties.All(prop =>
                {
                    if (prop.Name == "Dir")
                        prop.SetValue(obj, AppDomain.CurrentDomain.BaseDirectory);
                    else if (prop.Name == "PDir")
                        prop.SetValue(obj, Environment.CurrentDirectory);
                    else if (prop.Name == "Appdata")
                        prop.SetValue(obj, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                    else if (prop.Name == "C_Appdata")
                        prop.SetValue(obj, Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));
                    else if (prop.Name == "L_Appdata")
                        prop.SetValue(obj, Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
                    return true;
                });

            methods.All(fun =>
            {
                if (fun.Name == "Awake")
                    ctx.Awake = fun;
                else if (fun.Name == "Load")
                    ctx.Load = fun;
                else if (fun.Name == "Update")
                    ctx.Update = fun;
                else if (fun.Name == "Update30")
                    ctx.Update30 = fun;
                else if (fun.Name == "Update60")
                    ctx.Update60 = fun;
                return true;
            });

            Run(obj, ctx);
        }

        private static void Run(object obj, EngineExecutionContext ctx)
        {

            if (ctx.Awake != null)
                ctx.Awake.Invoke(obj, new object[0]);
            if (ctx.Load != null)
                ctx.Load.Invoke(obj, new object[0]);

            if (ctx.Update != null)
            {
                Thread thread = new Thread(new ThreadStart(() =>
                {
                    while (true) ctx.Update.Invoke(obj, new object[0]);
                }));
                thread.Start();
            }

            if (ctx.Update30 != null)
            {
                Thread thread = new Thread(new ThreadStart(() =>
                {
                    async void fun() => ctx.Update30.Invoke(obj, new object[0]);
                    while (true)
                    {
                        fun();
                        Thread.Sleep(100 / 3);
                    }
                }));
                thread.Start();
            }

            if (ctx.Update60 != null)
            {
                Thread thread = new Thread(new ThreadStart(() =>
                {
                    async void fun() => ctx.Update60.Invoke(obj, new object[0]);
                    while (true)
                    {
                        fun();
                        Thread.Sleep(100 / 6);
                    }
                }));
                thread.Start();
            }

        }
    }
}
