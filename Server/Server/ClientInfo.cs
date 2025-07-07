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
            this.Text = $"Infiltrator Project - Info Cliente {_idCliente}";
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
            tabControl1.Appearance = TabAppearance.FlatButtons;
            tabControl1.ItemSize = new Size(0, 1);
            tabControl1.SizeMode = TabSizeMode.Fixed;
            tabControl1.Multiline = false;

            // Oculta las pestañas visualmente
            tabControl1.TabStop = false;

            // Fondo oscuro para las páginas
            foreach (TabPage tab in tabControl1.TabPages)
                tab.BackColor = Color.FromArgb(30, 30, 40);

            // Estilo oscuro para los botones
            SetButtonStyle(button2);
            SetButtonStyle(button3);
            SetButtonStyle(button4);

            // Pestaña activa por defecto
            tabControl1.SelectedTab = tabPage1;
            button2.BackColor = Color.FromArgb(60, 60, 80);
        }

        private void SetButtonStyle(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = Color.FromArgb(45, 45, 60);
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 9, FontStyle.Bold);
        }
        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage1;
            HighlightButton(button2);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage2;
            HighlightButton(button3);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage3;
            HighlightButton(button4);
        }

        private void HighlightButton(Button activeBtn)
        {
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is Button btn && btn != activeBtn)
                    btn.BackColor = Color.FromArgb(45, 45, 60);
            }

            activeBtn.BackColor = Color.FromArgb(60, 60, 80);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
