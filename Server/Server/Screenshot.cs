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



        public void DisplayScreenshotBytes(byte[] imgBytes)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<byte[]>(DisplayScreenshotBytes), imgBytes);
                return;
            }

            try
            {
                if (pictureBox1.Image != null)
                {
                    pictureBox1.Image.Dispose();
                    pictureBox1.Image = null;
                }

                using (var ms = new MemoryStream(imgBytes))
                {
                    using (Image img = Image.FromStream(ms))
                    {
                       
                        pictureBox1.Image = new Bitmap(img);
                       
                    }
                }
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL MOSTRAR: " + ex.ToString());
            }
        }



    }
}
