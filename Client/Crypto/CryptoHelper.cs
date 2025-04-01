using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Client.Crypto
{
    public static class CryptoHelper
    {
        /// <summary>
        /// Genera una clave AES de 256 bits y su IV.
        /// </summary>
        public static (byte[] Key, byte[] IV) GenerateAesKey()
        {
            using (Aes aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.GenerateKey();
                aes.GenerateIV();
                return (aes.Key, aes.IV);
            }
        }

        /// <summary>
        /// Combina la clave AES y el IV en un solo array de bytes.
        /// </summary>
        public static byte[] CombineKeyAndIV(byte[] key, byte[] iv)
        {
            byte[] combined = new byte[key.Length + iv.Length];
            Buffer.BlockCopy(key, 0, combined, 0, key.Length);
            Buffer.BlockCopy(iv, 0, combined, key.Length, iv.Length);
            return combined;
        }

        /// <summary>
        /// Cifra la clave AES (junto con el IV) usando la clave pública RSA del servidor.
        /// </summary>
        /// <param name="aesKey">Clave AES.</param>
        /// <param name="aesIV">Vector de inicialización.</param>
        /// <param name="rsaPublicKeyXml">Clave pública RSA en formato XML.</param>
        /// <returns>Arreglo de bytes con la clave cifrada.</returns>
        public static byte[] EncryptSessionKey(byte[] aesKey, byte[] aesIV, string rsaPublicKeyXml)
        {
            byte[] keyAndIV = CombineKeyAndIV(aesKey, aesIV);
            using (RSA rsa = RSA.Create())
            {
                rsa.FromXmlString(rsaPublicKeyXml);
                // Se utiliza RSAEncryptionPadding.Pkcs1 para el cifrado
                return rsa.Encrypt(keyAndIV, RSAEncryptionPadding.Pkcs1);
            }
        }

        /// <summary>
        /// Cifra datos usando AES.
        /// </summary>
        /// <param name="plainData">Datos en claro a cifrar.</param>
        /// <param name="aesKey">Clave AES.</param>
        /// <param name="aesIV">Vector de inicialización.</param>
        /// <returns>Datos cifrados.</returns>
        public static byte[] EncryptData(byte[] plainData, byte[] aesKey, byte[] aesIV)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = aesKey;
                aes.IV = aesIV;
                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    cs.Write(plainData, 0, plainData.Length);
                    cs.FlushFinalBlock();
                    return ms.ToArray();
                }
            }
        }

    }
}
