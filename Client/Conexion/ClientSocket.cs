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
        private static readonly int serverPort = 12253;


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
            byte[] buffer = new byte[1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int bytesRead;
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    string chunkAsString = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    if (chunkAsString.Contains("FILE_END"))
                    {
                        int endIndex = chunkAsString.IndexOf("FILE_END");
                        ms.Write(buffer, 0, endIndex);
                        break;
                    }
                    else
                    {
                        ms.Write(buffer, 0, bytesRead);
                    }
                }
                File.WriteAllBytes(fileName, ms.ToArray());
            }
        }
    }
}
