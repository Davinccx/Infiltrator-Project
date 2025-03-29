using System.Net.Sockets;
using System.Text;
namespace Client.Conexion
{
    static class ClientSocket
    {
        private static TcpClient client;
        private static NetworkStream stream;
        private static bool connected = true;
        private static readonly string serverAddr = "0.tcp.eu.ngrok.io";
        private static readonly int serverPort = 18766;


        public static void connect()
        {

            client = new TcpClient(serverAddr, serverPort);
            stream = client.GetStream();

        }

        public static void disconnect()
        {
            client.Close();
        }

        public static void SendFile(string fileName)
        {
            try
            {
                byte[] fileNameBytes = Encoding.UTF8.GetBytes($"FILE: {fileName}\n");
                stream.Write(fileNameBytes, 0, fileNameBytes.Length);

                byte[] fileData = File.ReadAllBytes(fileName);

                stream.Write(fileData, 0, fileData.Length);

                byte[] fileEndBytes = Encoding.UTF8.GetBytes("FILE_END\n");

                stream.Write(fileEndBytes, 0, fileEndBytes.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al enviar el archivo: {ex.Message}");
            }
        }

        public static void SendResponse(string response)
        {
            byte[] data = Encoding.UTF8.GetBytes(response);
            stream.Write(data, 0, data.Length);
        }


        public static bool isConnected() { return connected; }

        public static NetworkStream getClientStream() { return stream; }

        public static void setConnected(bool con) { connected = con; }

        private static void SaveFile(string fileName, byte[] fileData)
        {
            try
            {
                File.WriteAllBytes(fileName, fileData);
                Console.WriteLine($"Archivo '{fileName}' recibido y guardado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar el archivo '{fileName}': {ex.Message}");
            }
        }

        public static void ReceiveFile(string fileName)
        {
            using (NetworkStream stream = ClientSocket.getClientStream())
            {
                // Leer el tamaño del archivo primero (si se envió un prefijo)
                byte[] sizeBuffer = new byte[4];
                stream.Read(sizeBuffer, 0, sizeBuffer.Length);
                int fileSize = BitConverter.ToInt32(sizeBuffer, 0);

                // Leer el archivo en bloques
                byte[] fileData = new byte[fileSize];
                int bytesRead = 0;
                int totalBytesRead = 0;

                while (totalBytesRead < fileSize)
                {
                    bytesRead = stream.Read(fileData, totalBytesRead, fileSize - totalBytesRead);
                    if (bytesRead == 0)
                    {
                        break; // Fin de la transmisión
                    }
                    totalBytesRead += bytesRead;
                }

                // Guardar el archivo en disco
                File.WriteAllBytes(fileName, fileData);
                
            }
        }
    }
}
