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
        private static Logger logger = Logger.getInstance();

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
            logger.Log($"Servidor Infiltrator iniciado en {Config.ServerIP}:{Config.ServerPort}",LogLevel.INFO);
            Thread acceptClientsThread = new Thread(AcceptClients);
            acceptClientsThread.Start();
            isRunning = true;
        }

        public static void stopServer()
        {
            listener.Stop();
            logger.Log("Deteniendo servidor Infiltrator...", LogLevel.INFO);
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
                        logger.Log($"Recibiendo archivo {fileName}", LogLevel.INFO);
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
                logger.Log($"Error al manejar al cliente {clientId}: {ex.Message}", LogLevel.ERROR);
            }

            lock (clients)
            {
                clients.Remove(clientId);
            }

            client.Close();
            Console.WriteLine($"\nInfiltrator Server 1.0> [INFO] Cliente {clientId} desconectado.");
            logger.Log($"Cliente {clientId} desconectado.", LogLevel.INFO);
        }

        public static void SaveFile(string fileName, byte[] fileData)
        {
            try
            {
                File.WriteAllBytes(fileName, fileData);
                Console.WriteLine($"\nInfiltrator Server 1.0> [INFO] Archivo '{fileName}' recibido y guardado correctamente.");
                logger.Log($"Archivo '{fileName}' recibido y guardado correctamente.", LogLevel.INFO);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nInfiltrator Server 1.0> [ERROR] Error al guardar el archivo '{fileName}': {ex.Message}");
                logger.Log($"Error al guardar el archivo '{fileName}': {ex.Message}", LogLevel.ERROR);
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
                    logger.Log($"Cliente {clientId} no encontrado.",LogLevel.ERROR);
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
                    logger.Log($"Cliente {clientId} conectado.", LogLevel.INFO);
                    Thread clientThread = new Thread(() => HandleClient(client, clientId));
                    clientThread.Start();
                }
            }
            catch (SocketException s)
            {
                // Se produce cuando listener.Stop() es llamado
                Console.WriteLine("\nInfiltrator Server 1.0> [INFO] El servidor ha sido detenido.");
                logger.Log($"El servidor se ha detenido: {s.Message}", LogLevel.WARNING);
            }
        }

        public static void setWaiting(bool status) {

            waitingForResponse = status;
        }
    }
}
