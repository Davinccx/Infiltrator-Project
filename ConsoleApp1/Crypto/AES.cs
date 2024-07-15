using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Server.Crypto
{
    static class AES
    {
        private static readonly byte[] key = Encoding.ASCII.GetBytes("ThisIsA16ByteKey");
        private static readonly byte[] iv = { 0xBF, 0xEB, 0x1E, 0x56, 0xFB, 0xCD, 0x97, 0x3B, 0xB2, 0x19, 0x02, 0x24, 0x30, 0xA5, 0x78, 0x43 };

        private static Aes aesAlg = Aes.Create();
        private static ICryptoTransform encriptador;
        private static ICryptoTransform decryptor;

        private static CryptoStream csEncrypt;
        private static StreamWriter swEncrypt;
        private static MemoryStream msEncrypt;

        private static CryptoStream csDecrypt;
        private static StreamReader srDecrypt;
        private static MemoryStream msDecrypt;


        public static byte[] EncryptString(string data)
        {
            aesAlg.Key = key;
            aesAlg.IV = iv;

            encriptador = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            msEncrypt = new MemoryStream();
            csEncrypt = new CryptoStream(msEncrypt, encriptador, CryptoStreamMode.Write);
            swEncrypt = new StreamWriter(csEncrypt);
            swEncrypt.Write(data);

            return msEncrypt.ToArray();
        }

        public static byte[] EncryptByte(byte[] data)
        {
            aesAlg.Key = key;
            aesAlg.IV = iv;

            ICryptoTransform encriptador = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);


            using (MemoryStream msEncrypt = new MemoryStream())
            {
                // Crear un CryptoStream usando el encriptador y el MemoryStream
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encriptador, CryptoStreamMode.Write))
                {

                    csEncrypt.Write(data, 0, data.Length);
                }


                return msEncrypt.ToArray();
            }
        }

        public static string DecryptData(byte[] encryptedData, int length)
        {
            aesAlg.Key = key;
            aesAlg.IV = iv;

            decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
            msDecrypt = new MemoryStream(encryptedData, 0, length);
            csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            srDecrypt = new StreamReader(csDecrypt);

            return srDecrypt.ReadToEnd();
        }

    }
}
