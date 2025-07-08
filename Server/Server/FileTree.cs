using System.Net.Sockets;
using System.Text;
using Server.Conexion;

namespace Server
{
    public partial class FileTree : Form
    {
        public static FileTree Instance { get; private set; }
        private TcpClient _cliente;
        private int _idCliente;
        private TreeNode _clickedNode = null;
        private string serverFilePath;

        public FileTree(int id, TcpClient cliente)
        {
            InitializeComponent();
            Instance = this;
            _cliente = cliente;
            _idCliente = id;
            // Buscar el cliente por su ID
            var clienteInfo = ServerSocket.clientesConectados.FirstOrDefault(c => c.ID == _idCliente);
            if (clienteInfo != null)
            {
                label2.Text = $"{clienteInfo.Equipo} - {clienteInfo.Usuario} Directory Tree";
            }
            else
            {
                label2.Text = $"Cliente {_idCliente} - Directory Tree";
            }
            this.treeViewArchivos.BeforeExpand += new TreeViewCancelEventHandler(treeviewArchivos_BeforeExpand);
            this.treeViewArchivos.MouseDown += treeviewArchivos_MouseDown;
            treeViewArchivos.ContextMenuStrip = contextMenuStrip1;


        }

        private void treeviewArchivos_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                TreeNode clicked = treeViewArchivos.GetNodeAt(e.X, e.Y);
                if (clicked != null)
                {
                    treeViewArchivos.SelectedNode = clicked;
                    _clickedNode = clicked;

                    // Mostrar menú manualmente
                    if (_clickedNode.ForeColor == Color.Gray) // solo archivos
                        contextMenuStrip1.Show(treeViewArchivos, e.Location);
                }
            }
        }


        private void FileTree_Load(object sender, EventArgs e)
        {

            treeViewArchivos.Nodes.Clear();

            TreeNode rootNode = new TreeNode("C:\\");
            rootNode.Tag = "C:/";
            rootNode.Nodes.Add("..."); // Nodo falso para expandir
            treeViewArchivos.Nodes.Add(rootNode);

            // Solicitar contenido de C:\
            ServerSocket.SendCommand(_idCliente, "C:/", Channel.FileManager);
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = e.Node;
            if (node == null || node.Tag == null) return;

            string ruta = node.Tag.ToString();
            if (!ruta.EndsWith("/")) ruta += "/";
        }

        private void treeviewArchivos_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            TreeNode node = e.Node;

            if (node.Nodes.Count == 1 && node.Nodes[0].Text == "...")
            {
                node.Nodes.Clear(); // limpia el nodo falso
                string ruta = node.Tag.ToString();
                ServerSocket.SendCommand(_idCliente, ruta, Channel.FileManager);
            }
        }


        public static void UpdateDirectoryTree(string listing, int clientId)
        {
            if (Instance.InvokeRequired)
            {
                Instance.Invoke(new Action(() => UpdateDirectoryTree(listing, clientId)));
                return;
            }

            var lines = listing.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length == 0) return;

            string parentPath = lines[0];
            var treeView = FileTree.Instance.treeViewArchivos;
            TreeNode parent = FindNodeByTag(treeView.Nodes, parentPath);
            if (parent == null) return;

            parent.Nodes.Clear();

            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i];

                if (line.StartsWith("[DIR]"))
                {
                    string path = line.Substring(5);
                    TreeNode node = new TreeNode(Path.GetFileName(path));
                    node.Tag = path;
                    node.Nodes.Add("..."); // Nodo falso
                    parent.Nodes.Add(node);
                }
                else if (line.StartsWith("[FILE]"))
                {
                    string path = line.Substring(6);
                    TreeNode node = new TreeNode(Path.GetFileName(path));
                    node.Tag = path;
                    node.ForeColor = Color.Gray;
                    parent.Nodes.Add(node);
                }
            }

            parent.Expand();
        }

        private static TreeNode FindNodeByTag(TreeNodeCollection nodes, string tag)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Tag != null && node.Tag.ToString().Equals(tag, StringComparison.OrdinalIgnoreCase))
                    return node;

                TreeNode found = FindNodeByTag(node.Nodes, tag);
                if (found != null) return found;
            }
            return null;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void descargarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_clickedNode == null || _clickedNode.ForeColor != Color.Gray || _clickedNode.Tag == null)
            {
                MessageBox.Show("Selecciona un archivo válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string rutaArchivo = _clickedNode.Tag.ToString(); // ruta completa
            MessageBox.Show($"El archivo {rutaArchivo} ha sido descargado con éxito!.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ServerSocket.SendCommand(_idCliente, rutaArchivo, Channel.File);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                serverFilePath = ofd.FileName;
                textBox1.Text = ofd.FileName;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(serverFilePath) || !File.Exists(serverFilePath))
                {
                    MessageBox.Show("Archivo no encontrado.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(textBox2.Text))
                {
                    MessageBox.Show("Ruta de destino vacía.");
                    return;
                }

                byte[] sendFile = File.ReadAllBytes(serverFilePath);

                // Asegúrate de usar "/" como separador para evitar problemas de codificación
                string pathDestino = textBox2.Text.Replace("\\", "/").TrimEnd('/') + "/" + Path.GetFileName(serverFilePath);
                byte[] pathBytes = Encoding.UTF8.GetBytes(pathDestino);
                byte[] pathLength = BitConverter.GetBytes(pathBytes.Length);

                byte[] payload = pathLength
                    .Concat(pathBytes)
                    .Concat(sendFile)
                    .ToArray();

                Protocol.Send(ServerSocket.getClientById(_idCliente).GetStream(), Channel.ServerFileUpload, payload);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al enviar archivo: " + ex.Message);
            }
        }
    }
}

