using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace Client.Util
{
    static class SystemInfo
    {
        public static string User = Environment.UserName;
        public static string MachineName = Environment.MachineName;
        public static string Culture = CultureInfo.CurrentCulture.ToString();
        

        private static string GetWindowsVersionName()
        {
            var sData = "Unknown System";
            try
            {
                using (var mSearcher =
                       new ManagementObjectSearcher(@"root\CIMV2", " SELECT * FROM win32_operatingsystem"))
                {
                    foreach (var o in mSearcher.Get())
                    {
                        var tObj = (ManagementObject)o;
                        sData = Convert.ToString(tObj["Name"]);
                    }

                    sData = sData.Split('|')[0];
                    var iLen = sData.Split(' ')[0].Length;
                    sData = sData.Substring(iLen).TrimStart().TrimEnd();
                }
            }
            catch
            {
                // ignored
            }

            return sData;
        }

        private static string GetBitVersion()
        {
            try
            {
                return Registry.LocalMachine.OpenSubKey(@"HARDWARE\Description\System\CentralProcessor\0")
                    .GetValue("Identifier")
                    .ToString()
                    .Contains("x86")
                    ? "(32 Bit)"
                    : "(64 Bit)";
            }
            catch
            {
                // ignored
            }

            return "(Unknown)";
        }

        // Get system version
        public static string GetSystemVersion()
        {
            return GetWindowsVersionName() + " " + GetBitVersion();
        }


        // Get public IP
        public static async Task<string> GetPublicIpAsync()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var url = "http://icanhazip.com";
                    var externalip = await client.GetStringAsync(url).ConfigureAwait(false);
                    return externalip.Replace("\n", "").Trim();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return "Request failed";
            }
        }

        public static string GetCpuName()
        {
            try
            {
                var mSearcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor");
                foreach (var o in mSearcher.Get())
                {
                    var mObject = (ManagementObject)o;
                    return mObject["Name"].ToString();
                }
            }
            catch
            {
                // ignored
            }

            return "Unknown";
        }

        // Get GPU name
        public static string GetGpuName()
        {
            try
            {
                var mSearcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_VideoController");
                foreach (var o in mSearcher.Get())
                {
                    var mObject = (ManagementObject)o;
                    return mObject["Name"].ToString();
                }
            }
            catch
            {
                // ignored
            }

            return "Unknown";
        }

        public static string GetRamAmount()
        {
            try
            {
                var ramAmount = 0;
                using (var mos = new ManagementObjectSearcher("Select * From Win32_ComputerSystem"))
                {
                    foreach (var o in mos.Get())
                    {
                        var mo = (ManagementObject)o;
                        var bytes = Convert.ToDouble(mo["TotalPhysicalMemory"]);
                        ramAmount = (int)(bytes / 1048576);
                        break;
                    }
                }

                return ramAmount + "MB";
            }
            catch
            {
                // ignored
            }

            return "-1";
        }

        public static async Task<string>  GetNetworkInfo()
        {
            StringBuilder networkInfo = new StringBuilder();
            string publicIP = await GetPublicIpAsync();

            networkInfo.AppendLine("\n");
            networkInfo.AppendLine("====================================================================");
            networkInfo.AppendLine("|                       Información de la Red                      |");
            networkInfo.AppendLine("====================================================================");
            networkInfo.AppendLine($" IP Pública: {publicIP} ");
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface netInterface in interfaces)
            {
                networkInfo.AppendLine($" Nombre: {netInterface.Name,-44} ");
                networkInfo.AppendLine($" Descripción: {netInterface.Description,-39} ");
                networkInfo.AppendLine($" Tipo de interfaz: {netInterface.NetworkInterfaceType,-34} ");
                networkInfo.AppendLine($" Estado: {netInterface.OperationalStatus,-39} ");
                networkInfo.AppendLine($" Velocidad: {netInterface.Speed} bps ");
                
                IPInterfaceProperties ipProperties = netInterface.GetIPProperties();
                foreach (UnicastIPAddressInformation ip in ipProperties.UnicastAddresses)
                {
                    networkInfo.AppendLine($" Dirección IP: {ip.Address,-43} ");
                    networkInfo.AppendLine($" Máscara de subred: {ip.IPv4Mask,-37} ");
                }

                foreach (GatewayIPAddressInformation gateway in ipProperties.GatewayAddresses)
                {
                    networkInfo.AppendLine($" Puerta de enlace predeterminada: {gateway.Address,-34} ");
                }

                networkInfo.AppendLine("============================================");
            }



            return networkInfo.ToString();
        }

        public static async Task<string> GetSystemInfo()
        {
            StringBuilder systemInfo = new StringBuilder();
            string publicIP = await GetPublicIpAsync();

            systemInfo.Append("SYSINFO:");
            systemInfo.AppendLine($" Nombre del equipo: {MachineName,-34} ");
            systemInfo.AppendLine($" Usuario: {User,-34} ");
            systemInfo.AppendLine($" IP: {publicIP,-34} ");
            systemInfo.AppendLine($" Sistema operativo: {GetSystemVersion(),-34} ");
            systemInfo.AppendLine($" Versión de .NET: {Environment.Version,-34} ");
            systemInfo.AppendLine($" Número de procesadores: {Environment.ProcessorCount,-34} ");
            systemInfo.AppendLine($" Memoria RAM: {GetRamAmount(),-34} ");
            systemInfo.AppendLine($" Procesador: {GetCpuName(),-34} ");
            systemInfo.AppendLine($" Tarjeta Gráfica: {GetGpuName(),-34} ");
            systemInfo.Append("ENDSYSINFO:");
            return systemInfo.ToString();
        }


    }
}
