using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingsoft.Utils.Basic
{
    public static class ErrorHandler
    {
        public static T OnError<T>(Func<T> func, T _default, Action<Exception> onError = null)
        {
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex);
                return _default;
            }
        }

        public static bool Try(Action func, Action<Exception> onError = null) => 
            OnError(() => { func(); return true; }, false, onError);
    }
}
