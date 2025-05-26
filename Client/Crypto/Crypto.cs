using System.Security.Cryptography;
using System.Text;

public class Crypto
{
    public static RSAParameters GenerateRSAKeys(out string publicKeyXml)
    {
        using (var rsa = RSA.Create(2048))
        {
            publicKeyXml = rsa.ToXmlString(false);
            return rsa.ExportParameters(true);
        }
    }

    public static byte[] EncryptWithRSA(string publicKeyXml, byte[] data)
    {
        using (var rsa = RSA.Create())
        {
            rsa.FromXmlString(publicKeyXml);
            return rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA256);
        }
    }

    public static byte[] DecryptWithRSA(RSAParameters privateKey, byte[] data)
    {
        using (var rsa = RSA.Create())
        {
            rsa.ImportParameters(privateKey);
            return rsa.Decrypt(data, RSAEncryptionPadding.OaepSHA256);
        }
    }

    public static (byte[] Encrypted, byte[] IV, byte[] Key) EncryptWithAES(string plainText)
    {
        using (var aes = Aes.Create())
        {
            aes.GenerateIV();
            aes.GenerateKey();

            using var encryptor = aes.CreateEncryptor();
            var data = Encoding.UTF8.GetBytes(plainText);
            var encrypted = encryptor.TransformFinalBlock(data, 0, data.Length);

            return (encrypted, aes.IV, aes.Key);
        }
    }

    public static string DecryptWithAES(byte[] encrypted, byte[] key, byte[] iv)
    {
        using (var aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor();
            var decrypted = decryptor.TransformFinalBlock(encrypted, 0, encrypted.Length);
            return Encoding.UTF8.GetString(decrypted);
        }
    }
}
