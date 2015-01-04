using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace EEAuth.Security
{
    public static class Key
    {
        public static HMAC HMAC; // Removed
        public static X509Certificate2 Cert; // Removed
    }
}