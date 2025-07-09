using System;
using System.Security.Cryptography;
using System.Text;
using Server.Log;

namespace Server.Crypto
{
    public static class RSAHelper
    {
        private static RSACryptoServiceProvider rsa;
        public static string publicKey { get; private set; }
        public static string privateKey { get; private set; }


        private static Logger _logger = Logger.getInstance();


        static RSAHelper()
        {
            rsa = new RSACryptoServiceProvider(2048);
            publicKey = rsa.ToXmlString(false);  // Solo clave pública
            privateKey = rsa.ToXmlString(true);  // Clave completa
        }

        public static string GetPublicKey() => publicKey;

        public static byte[] EncryptWithPublicKey(string xmlPublicKey, byte[] data)
        {
            using var rsaPublic = new RSACryptoServiceProvider();
            rsaPublic.FromXmlString(xmlPublicKey);
            return rsaPublic.Encrypt(data, false);
        }

        public static byte[] DecryptWithPrivateKey(byte[] data)
        {
            if (rsa == null)
            {
                _logger.Log("RSA provider no ha sido inicializado ", LogLevel.ERROR);
                throw new InvalidOperationException("RSA provider is not initialized.");
            }
            try
            {
                return rsa.Decrypt(data, false);
            }
            catch (CryptographicException ex)
            {
                _logger.Log($"Error durante el desincriptado: {ex.Message}", LogLevel.ERROR);
                throw new InvalidOperationException("Decryption failed.", ex);
            }
        }
    }
}
