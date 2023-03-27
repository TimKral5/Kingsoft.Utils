using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingsoft.Utils.Security.Cryptography
{
    public interface ICrypticConverter<TDec, TKey, TEnc>
    {
        TEnc Encrypt(TDec obj, TKey key);
        TDec Decrypt(TEnc obj, TKey key);
    }
}
