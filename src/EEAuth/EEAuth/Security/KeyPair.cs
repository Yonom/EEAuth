using System.Security.Cryptography;
using EEAuth.Helpers;

namespace EEAuth.Security
{
    internal class KeyPair
    {
        public KeyPair()
        {
        }

        public KeyPair(string publicKey, string privateKey)
        {
            PublicKey = publicKey;
            PrivateKey = privateKey;
        }

        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }

        public static KeyPair New()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var bytes = new byte[14];
                rng.GetBytes(bytes);
                return FromPublicPart(bytes);
            }
        }

        private static KeyPair FromPublicPart(byte[] publicKey)
        {
            return new KeyPair(publicKey.ToToken(), ToPrivate(publicKey));
        }

        public static KeyPair FromPublicPart(string publicKey)
        {
            return new KeyPair(publicKey, ToPrivate(publicKey.FromToken()));
        }

        private static string ToPrivate(byte[] publicKey)
        {
            return Key.Hmac.ComputeHash(publicKey).ToToken();
        }
    }
}