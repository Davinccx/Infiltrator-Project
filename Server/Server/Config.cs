using System;
using System.IO;
using Server.Log;


namespace Server
{
    static class Config
    {
        public static string ServerIP { get; set; }
        public static int ServerPort { get; set; }
        public static int BufferLength { get; set; }
        public static string Version { get; set; }

        private static Logger _logger = Logger.getInstance();

        public static void LoadConfig()
        {
            try
            {
                string json = File.ReadAllText("config.json");
                ParseConfig(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ERROR] Error al cargar la configuración: {ex.Message}");
                _logger.Log($"Error al cargar la configuración: {ex.Message}", LogLevel.ERROR);
                Environment.Exit(1);
            }
        }

        private static void ParseConfig(string json)
        {
            try
            {
                // Limpiar espacios en blanco innecesarios y caracteres especiales
                json = json.Replace("\r", "").Replace("\n", "").Replace("\t", "").Trim();

                // Validar que el JSON comience y termine correctamente
                if (!json.StartsWith("{") || !json.EndsWith("}"))
                {
                    _logger.Log("El archivo de configuración JSON no está en el formato esperado.", LogLevel.ERROR);
                    throw new FormatException("El archivo de configuración JSON no está en el formato esperado.");

                }

                // Eliminar las llaves externas
                json = json.Substring(1, json.Length - 2).Trim();

                // Dividir por las comas y dos puntos para obtener pares key-value
                string[] keyValuePairs = json.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var pair in keyValuePairs)
                {
                    // Dividir por el primer ':' para obtener la key y el value
                    int colonIndex = pair.IndexOf(':');
                    if (colonIndex <= 0)
                    {
                        _logger.Log("Error al analizar el par key-value en el archivo de configuración.", LogLevel.ERROR);
                        throw new FormatException("Error al analizar el par key-value en el archivo de configuración.");
                    }

                    string key = pair.Substring(0, colonIndex).Trim().Trim('"');
                    string value = pair.Substring(colonIndex + 1).Trim().Trim('"');

                    // Asignar valores a las propiedades según la key
                    switch (key)
                    {
                        case "ServerIP":
                            ServerIP = value;
                            break;
                        case "ServerPort":
                            if (!int.TryParse(value, out int port))
                            {
                                _logger.Log("El puerto del servidor no es un número válido.", LogLevel.ERROR);
                                throw new FormatException("El puerto del servidor no es un número válido.");
                            }
                            ServerPort = port;
                            break;
                        case "bufferLength":
                            if (!int.TryParse(value, out int bufferLength) || bufferLength >= 102400)
                            {
                                _logger.Log("Error en el tamaño del buffer (Máximo 100 MB).", LogLevel.ERROR);
                                throw new FormatException("Error en el tamaño del buffer (Máximo 100 MB).");
                            }
                            BufferLength = bufferLength;
                            break;
                        case "version":
                            Version = value;
                            break;
                        default:
                            _logger.Log($"Clave desconocida en el archivo de configuración: {key}", LogLevel.ERROR);
                            throw new FormatException($"Clave desconocida en el archivo de configuración: {key}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log($"Error al analizar el archivo de configuración: {ex.Message}", LogLevel.ERROR);
                throw new FormatException($"Error al analizar el archivo de configuración: {ex.Message}");
            }
        }
    }
}
