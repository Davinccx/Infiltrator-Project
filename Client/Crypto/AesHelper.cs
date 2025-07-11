

using System.Security.Cryptography;
using Client.Conexion;

namespace Client.Crypto
{
    public static class AesHelper
    {
        public static byte[] EncryptWithAes(byte[] plainData, byte[] aesKey)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = aesKey;
                aes.GenerateIV();
                using (var encryptor = aes.CreateEncryptor())
                {
                    byte[] encrypted = encryptor.TransformFinalBlock(plainData, 0, plainData.Length);
                    byte[] payload = new byte[aes.IV.Length + encrypted.Length];
                    Buffer.BlockCopy(aes.IV, 0, payload, 0, aes.IV.Length);
                    Buffer.BlockCopy(encrypted, 0, payload, aes.IV.Length, encrypted.Length);
                    return payload;
                }
            }
        }

        public static byte[] DecryptWithAes(byte[] payload, byte[] aesKey)
        {
            byte[] iv = new byte[16];
            Buffer.BlockCopy(payload, 0, iv, 0, 16);
            byte[] encryptedData = new byte[payload.Length - 16];
            Buffer.BlockCopy(payload, 16, encryptedData, 0, encryptedData.Length);

            using (Aes aes = Aes.Create())
            {
                aes.Key = aesKey;
                aes.IV = iv;
                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                {
                    return decryptor.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
                }
            }
        }

    }
}
