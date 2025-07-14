using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms; // Necesario para usar Screen y Size

namespace Client.Util
{
    static class Screenshot
    {

        public static byte[] CaptureScreen()
        {

            var bounds = Screen.PrimaryScreen.Bounds;
            using var bitmap = new Bitmap(bounds.Width, bounds.Height);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
            }

            using var ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Jpeg); // JPEG ocupa menos que PNG
            return ms.ToArray();

        }


    }
}
