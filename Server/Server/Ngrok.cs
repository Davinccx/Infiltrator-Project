using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Server.Log;

namespace Server
{
    static  class Ngrok
    {
        public static string  tunnelIP="x.x.x.x";
        public static string tunnelPort = "";
        private static Logger _logger = Logger.getInstance();

        public static void startNgrok()
        {

            var startInfo = new ProcessStartInfo
            {
                FileName = "ngrok.exe",
                Arguments = "tcp 443",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,  // Redirige la salida estándar
                WindowStyle = ProcessWindowStyle.Hidden
            };

            try
            {
                Process process = Process.Start(startInfo);

                // Leer la salida del proceso (ngrok)
                StreamReader reader = process.StandardOutput;

                // Leemos hasta que encontremos la línea que contiene la URL pública
                string output = reader.ReadToEnd();

                // Regex para extraer la URL del túnel TCP
                string pattern = @"tcp://([a-z0-9]+\.tcp\.ngrok\.io):(\d+)";
                Match match = Regex.Match(output, pattern);

                if (match.Success)
                {
                    string ip = match.Groups[1].Value;         // Parte de la IP (ej: xxxx.tcp.ngrok.io)
                    string port = match.Groups[2].Value;       // Puerto asignado (ej: 443)

                    tunnelIP = ip;
                    tunnelPort = port;
                }
                else
                {
                    _logger.Log("Error al obtener la información de Ngrok", LogLevel.ERROR);
                }
            }
            catch (Exception ex)
            {
                _logger.Log($"Error al iniciar ngrok: {ex.Message}", LogLevel.ERROR);
            }
        }
    }
}
