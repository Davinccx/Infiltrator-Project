using System.Security.Cryptography;
using System.Text;


namespace Server.Crypto
{
    public static class AesHelper
    {
        public static string DecryptStringWithAes(byte[] payload, byte[] aesKey)
        {
            // El payload: primeros 16 bytes = IV, el resto = datos cifrados
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
                    byte[] decrypted = decryptor.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
                    return Encoding.UTF8.GetString(decrypted);
                }
            }
        }

        public static byte[] DecryptWithAes(byte[] payload, byte[] aesKey)
        {
            // Los primeros 16 bytes son el IV, el resto son los datos cifrados
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

        public static byte[] EncryptWithAes(byte[] plainData, byte[] aesKey)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = aesKey;
                aes.GenerateIV(); // Siempre un IV diferente
                using (var encryptor = aes.CreateEncryptor())
                {
                    byte[] encrypted = encryptor.TransformFinalBlock(plainData, 0, plainData.Length);
                    // Prepara el payload: IV + datos cifrados
                    byte[] payload = new byte[aes.IV.Length + encrypted.Length];
                    Buffer.BlockCopy(aes.IV, 0, payload, 0, aes.IV.Length);
                    Buffer.BlockCopy(encrypted, 0, payload, aes.IV.Length, encrypted.Length);
                    return payload;
                }
            }
        }
    }
}
