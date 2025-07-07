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
    public partial class FileTree : Form
    {
        public static FileTree Instance { get; private set; }
        private TcpClient _cliente;
        private int _idCliente;
        public FileTree(int id, TcpClient cliente)
        {
            InitializeComponent();
            Instance = this;
            _cliente = cliente;
            _idCliente = id;
            this.treeViewArchivos.BeforeExpand += new TreeViewCancelEventHandler(treeviewArchivos_BeforeExpand);
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
    }
}

