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

namespace Server
{
    public partial class ClientInfo : Form
    {
        public static ClientInfo Instance { get; private set; }
        private TcpClient _cliente;
        private int _idCliente;
        public ClientInfo(int id, TcpClient cliente)
        {
            InitializeComponent();
            Instance = this;
            _cliente = cliente;
            _idCliente = id;
            label1.Text = $"Información Cliente {_idCliente}";
            if (_cliente != null && _cliente.Connected)
            {
                label2.Text = "Estado: Activo";
                label2.ForeColor = Color.Lime;
            }
            else
            {
                label2.Text = "Estado: Desconectado";
                label2.ForeColor = Color.Red;
            }
        }

        private void ClientInfo_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
