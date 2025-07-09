using System;
using System.Security.Cryptography;
using System.Text;

namespace Client.Crypto
{
    public static class RSAHelper
    {
        public static byte[] EncryptWithPublicKey(string publicKeyXml, byte[] data)
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.FromXmlString(publicKeyXml);
                return rsa.Encrypt(data, false); // false = PKCS#1 v1.5
            }
        }

        public static string ExportPublicKey(RSACryptoServiceProvider rsa)
        {
            return rsa.ToXmlString(false); // Solo clave pública
        }

        public static string ExportPrivateKey(RSACryptoServiceProvider rsa)
        {
            return rsa.ToXmlString(true); // Clave privada completa
        }

        public static RSACryptoServiceProvider CreateNewKeys(out string publicKeyXml, out string privateKeyXml)
        {
            var rsa = new RSACryptoServiceProvider(2048);
            publicKeyXml = rsa.ToXmlString(false);
            privateKeyXml = rsa.ToXmlString(true);
            return rsa;
        }
    }
}
