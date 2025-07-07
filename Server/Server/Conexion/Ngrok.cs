using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Server.Log;

namespace Server.Conexion
{
    static class Ngrok
    {
        public static string tunnelIP = "x.x.x.x";
        public static string tunnelPort = "";
        private static Logger _logger = Logger.getInstance();

        public static async Task StartNgrokAsync()
        {
            try
            {
                // Ejecuta ngrok tcp 443 en segundo plano
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "ngrok.exe",
                    Arguments = "tcp 443",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                    WindowStyle = ProcessWindowStyle.Hidden
                };

                Process.Start(startInfo);

                // Esperar a que la API de ngrok esté disponible
                using (HttpClient client = new HttpClient())
                {
                    for (int i = 0; i < 10; i++) // Intentar hasta 10 veces (aprox 10 segundos)
                    {
                        try
                        {
                            string json = await client.GetStringAsync("http://127.0.0.1:4040/api/tunnels");
                            Match match = Regex.Match(json, @"tcp://([a-z0-9\-\.]+):(\d+)");
                            if (match.Success)
                            {
                                tunnelIP = match.Groups[1].Value;
                                tunnelPort = match.Groups[2].Value;
                                _logger.Log($"Ngrok IP: {tunnelIP}, Puerto: {tunnelPort}", LogLevel.INFO);
                                return;
                            }
                        }
                        catch
                        {
                            await Task.Delay(1000); // Espera 1 segundo e intenta de nuevo
                        }
                    }

                    _logger.Log("No se pudo obtener la IP y puerto de ngrok tras varios intentos.", LogLevel.ERROR);
                }
            }
            catch (Exception ex)
            {
                _logger.Log($"Error al iniciar ngrok: {ex.Message}", LogLevel.ERROR);
            }
        }
    }
}
