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
    public partial class Screenshot : Form
    {

        public static Screenshot Instance { get; private set; }
        private TcpClient _cliente;
        private int _idCliente;
        public Screenshot(int id, TcpClient cliente)
        {
            InitializeComponent();
            Instance = this;
            _cliente = cliente;
            _idCliente = id;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Screenshot_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {

                ServerSocket.SendCommand(_idCliente, "screenshot", Channel.Screenshot);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al capturar la pantalla: " + ex.Message);
            }
        }

        public void DisplayScreenshot(string imagePath)
        {

            if (InvokeRequired)
            {
                Invoke(new Action<string>(DisplayScreenshot), imagePath);
                return;
            }

            try
            {
                if (File.Exists(imagePath))
                {
                    using (FileStream stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                    {
                        Image img = Image.FromStream(stream);
                        pictureBox1.Image = new Bitmap(img); // Copia para liberar el stream
                    }

                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom; // Asegura que la imagen se ajuste
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al mostrar la captura: " + ex.Message);
            }


        }
    }
}
