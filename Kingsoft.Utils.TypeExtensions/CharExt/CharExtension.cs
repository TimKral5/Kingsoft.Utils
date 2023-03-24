using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingsoft.Utils.TypeExtensions.CharExt
{
    public static class CharExtension
    {
        public static char TreatAsUnicode(this string hex)
        {
            int code = int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
            string unicodeString = char.ConvertFromUtf32(code);
            return unicodeString[0];
        }
    }
}
