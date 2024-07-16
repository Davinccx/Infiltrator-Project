using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GUI
{
    static class Menu
    {
        static string asciiArt = @"
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
            Console.WriteLine("\t| ID_CLIENTE exec <comando>: Ejecuta un comando en la máquina cliente.           |");
            Console.WriteLine("\t| ID_CLIENTE list_processes: Lista todos los procesos en la máquina cliente.     |");
            Console.WriteLine("\t| ID_CLIENTE get <archivo>: Obtiene un archivo de la máquina cliente.            |");
            Console.WriteLine("\t| ID_CLIENTE browsers: Lista los navegadores instalados en la máquina cliente.   |");
            Console.WriteLine("\t| ID_CLIENTE antivirus: Lista los antivirus instalados en la máquina cliente.    |");
            Console.WriteLine("\t| ID_CLIENTE system_info: Obtiene información del sistema de la máquina cliente. |");
            Console.WriteLine("\t| ID_CLIENTE network_info: Obtiene información de la red de la máquina cliente.  |");
            Console.WriteLine("\t| ID_CLIENTE disconnect: Desconecta al cliente.                                  |");
            Console.WriteLine("\t| ID_CLIENTE kill <PID>: Termina un proceso en la máquina cliente.               |");
            Console.WriteLine("\t| ID_CLIENTE screenshot: Captura una captura de pantalla de la máquina cliente.  |");
            Console.WriteLine("\t+--------------------------------------------------------------------------------+");
        }
    }
}
