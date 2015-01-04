using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace EEAuth.Security
{
    internal static class Sign
    {
        public static string CalcSig(KeyPair keyPair, Dictionary<string, string> parameters)
        {
            return Convert.ToBase64String(
                new HMACMD5(Encoding.UTF8.GetBytes(keyPair.PrivateKey)).ComputeHash(
                    Encoding.UTF8.GetBytes(
                        String.Concat(
                            parameters
                                .OrderBy(i => i.Key)
                                .Select(i => i.Key + "=" + i.Value)))));
        }
    }
}