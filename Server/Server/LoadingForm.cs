using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Server.Log;

namespace Server
{
    public partial class LoadingForm : Form
    {
        private static Logger _logger = Logger.getInstance();
        private CancellationTokenSource _cts = new CancellationTokenSource();

        public LoadingForm()
        {
            InitializeComponent();
        }

        private async void LoadingForm_Load(object sender, EventArgs e)
        {
            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.MarqueeAnimationSpeed = 30;

            // Iniciar animación de texto
            var textTask = UpdateStatusTextAsync(_cts.Token);

            try
            {
                await Task.Run(() =>
                {
                    Config.LoadConfig(); // operación que puede tardar
                    _logger.Log("Configuración del servidor cargada correctamente.", LogLevel.INFO);
                    Thread.Sleep(2000); // Simula demora
                });

                _cts.Cancel(); // Detiene la animación del texto

                label2.Text = "¡Configuración cargada correctamente!";
                await Task.Delay(1000);

                this.Hide();
                MainForm mainForm = new MainForm();
                mainForm.ShowDialog();
                this.Close();
            }
            catch (Exception ex)
            {
                _cts.Cancel();

                // Mostrar el error en el label
                label2.Text = "⚠️ Error al cargar la configuración";

                // Mostrar mensaje emergente
                MessageBox.Show($"[ERROR] No se pudo cargar la configuración del servidor: {ex.Message}",
                                "Error de configuración", MessageBoxButtons.OK, MessageBoxIcon.Error);

                _logger.Log($"[ERROR] No se pudo cargar la configuración del servidor: {ex.Message}", LogLevel.ERROR);

                // NO cerrar la app, se queda en la pantalla de carga mostrando el error
                // Opcional: desactivar la ProgressBar
                progressBar1.Style = ProgressBarStyle.Blocks;
                progressBar1.MarqueeAnimationSpeed = 0;

            }
        }

        private async Task UpdateStatusTextAsync(CancellationToken token)
        {
            string[] mensajes = new string[]
            {
                "Cargando configuración...",
                "Asignando IP y puerto al servidor...",
                "Determinando el tamaño del buffer....",
                "Casi listo..."
            };

            int i = 0;
            while (!token.IsCancellationRequested)
            {
                label2.Invoke((MethodInvoker)(() =>
                {
                    label2.Text = mensajes[i % mensajes.Length];
                }));

                i++;
                await Task.Delay(1000, token); // Cambia cada segundo
            }
        }
    }
}
