using Server.Conexion;
using Server.Log;


namespace Server
{
    internal static class Program
    {
        private static Logger _logger = Logger.getInstance();
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            
            ApplicationConfiguration.Initialize();

            Application.Run(new LoadingForm());
           
            
        }
    }
}