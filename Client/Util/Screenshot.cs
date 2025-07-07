using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms; // Necesario para usar Screen y Size

namespace Client.Util
{
    static class Screenshot
    {
        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public static void CaptureScreen(string filename)
        {
            try
            {
                // Captura toda la superficie de escritorio (incluso si hay varias pantallas)
                Rectangle bounds = SystemInformation.VirtualScreen;

                using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.CopyFromScreen(bounds.X, bounds.Y, 0, 0, bounds.Size);
                    }

                    bitmap.Save(filename, ImageFormat.Png);
                }
            }
            catch (Exception ex)
            {
                // Puedes registrar el error si lo deseas
                Console.WriteLine("Error al capturar pantalla: " + ex.Message);
            }
        }


    }
}
