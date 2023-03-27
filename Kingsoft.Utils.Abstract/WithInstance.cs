using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingsoft.Utils.Abstract
{
    public abstract class WithInstance<T>
        where T : new()
    {
#pragma warning disable IDE0044
        private static T _Instance;
#pragma warning restore IDE0044
        public static T Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new T();
                return _Instance;
            }
        }
    }
}
