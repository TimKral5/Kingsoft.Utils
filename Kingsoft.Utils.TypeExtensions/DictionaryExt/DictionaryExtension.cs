using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingsoft.Utils.TypeExtensions.DictionaryExt
{
    public static class DictionaryExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns>
        ///     Return Codes: <br/>
        ///     - 0: Added Value <br/>
        ///     - 1: Added List and Value <br/>
        ///     - 2: Value is allready stored <br/>
        /// </returns>
        public static byte Set<TKey, TValue>(this Dictionary<TKey, TValue[]> dict, TKey key, TValue value)
        {
            byte result = 0;
            if (dict.Get(key) == null)
            {
                dict.Add(key, new TValue[0]);
                result = 1;
            }
            if (!dict[key].Contains(value))
                dict[key].Append(value);
            else result = 2;
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns>
        ///     Return Codes: <br/>
        ///     - 0: Added <br/>
        ///     - 1: Updated
        /// </returns>
        public static byte Set<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (!dict.ContainsKey(key))
            {
                dict.Add(key, value);
                return 0;
            }

            dict[key] = value;
            return 1;
        }

        public static TValue Get<TKey, TValue>(this Dictionary<TKey, TValue[]> dict, TKey key, int index)
        {
            if (dict.ContainsKey(key))
                return dict[key][index];
            return default;
        }

        public static TValue Get<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
        {
            if (!dict.ContainsKey(key))
                return default;
            return dict[key];
        }
    }
}
