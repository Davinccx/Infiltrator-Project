using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

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
            // Obtener el tamaño de la pantalla
            RECT rect;
            IntPtr hWnd = GetDesktopWindow();
            GetWindowRect(hWnd, out rect);

            int width = rect.Right - rect.Left;
            int height = rect.Bottom - rect.Top;

            // Crear un bitmap del tamaño de la pantalla
            using (Bitmap bitmap = new Bitmap(width, height))
            {
                // Crear un objeto Graphics desde el bitmap
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    // Copiar la pantalla en el bitmap
                    g.CopyFromScreen(0, 0, 0, 0, new Size(width, height));
                }

                try
                {
                    // Guardar el bitmap en un archivo
                    bitmap.Save(filename, ImageFormat.Png);
                    
                }
                catch (Exception ex)
                {
                    
                }
            }
        }
    }
}
