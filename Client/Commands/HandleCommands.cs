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
    }
}
