using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Server.Crypto
{
    public static class CryptoHelper
    {
        /// <summary>
        /// Separa un arreglo combinado en clave AES y IV.
        /// </summary>
        public static (byte[] Key, byte[] IV) SplitKeyAndIV(byte[] combined, int keySize)
        {
            byte[] key = new byte[keySize];
            byte[] iv = new byte[combined.Length - keySize];
            Buffer.BlockCopy(combined, 0, key, 0, keySize);
            Buffer.BlockCopy(combined, keySize, iv, 0, iv.Length);
            return (key, iv);
        }

        /// <summary>
        /// Descifra la clave AES (y el IV) utilizando la clave privada RSA del servidor.
        /// </summary>
        /// <param name="encryptedSessionKey">Clave cifrada (AES+IV combinados).</param>
        /// <param name="rsaPrivateKeyXml">Clave privada RSA en formato XML.</param>
        /// <param name="keySize">Tamaño de la clave AES en bytes (32 para 256 bits).</param>
        /// <returns>Tupla con la clave AES y el IV.</returns>
        public static (byte[] Key, byte[] IV) DecryptSessionKey(byte[] encryptedSessionKey, string rsaPrivateKeyXml, int keySize)
        {
            using (RSA rsa = RSA.Create())
            {
                rsa.FromXmlString(rsaPrivateKeyXml);
                byte[] combined = rsa.Decrypt(encryptedSessionKey, RSAEncryptionPadding.Pkcs1);
                return SplitKeyAndIV(combined, keySize);
            }
        }

        /// <summary>
        /// Descifra datos usando AES.
        /// </summary>
        /// <param name="cipherData">Datos cifrados.</param>
        /// <param name="aesKey">Clave AES.</param>
        /// <param name="aesIV">Vector de inicialización.</param>
        /// <returns>Datos en claro.</returns>
        public static byte[] DecryptData(byte[] cipherData, byte[] aesKey, byte[] aesIV)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = aesKey;
                aes.IV = aesIV;
                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream(cipherData))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var result = new MemoryStream())
                {
                    cs.CopyTo(result);
                    return result.ToArray();
                }
            }
        }
    }
}
