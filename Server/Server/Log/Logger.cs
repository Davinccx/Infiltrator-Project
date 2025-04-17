using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Log
{
    public class Logger
    {
        private static readonly object _lock = new object();
        private static Logger _instance;
        private string _logFilePath;

        private Logger() {


            string timestamp = DateTime.Now.ToString("yyHHmmss");
            string logDirectory = "logs";

            // Crear el directorio de logs si no existe
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            // Generar el nombre del archivo de log
            _logFilePath = Path.Combine(logDirectory, $"infiltrator-log-{timestamp}.txt");

            // Crear el archivo de log si no existe
            if (!File.Exists(_logFilePath))
            {
                using (StreamWriter writer = File.CreateText(_logFilePath))
                {
                    writer.WriteLine("-----------------------------------------------");
                    writer.WriteLine("------ Welcome to Infiltrator Log System ------");
                    writer.WriteLine("-----------------------------------------------");
                }
            }

        }
        public static Logger getInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new Logger();
                    }
                }
            }
            return _instance;
        }

        public void Log(string message, LogLevel level)
        {

            string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] - [{level}]: {message}";
            lock (_lock)
            {
                using (StreamWriter writer = new StreamWriter(_logFilePath, true))
                {
                    writer.WriteLine(logMessage);
                }
            }
        }

       
    }
    public enum LogLevel
    {
        INFO,
        WARNING,
        ERROR
    }
}
