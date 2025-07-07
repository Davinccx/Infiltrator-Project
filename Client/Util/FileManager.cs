

using System.Text;

namespace Client.Util
{
    public static class FileManager
    {



        public static string ListDirectory(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                    return "[ERROR] El directorio no existe.";

                var dirs = Directory.GetDirectories(path);
                var files = Directory.GetFiles(path);

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("[DIRS]");
                foreach (var dir in dirs)
                    sb.AppendLine(dir);

                sb.AppendLine("[FILES]");
                foreach (var file in files)
                    sb.AppendLine(file);

                return sb.ToString();
            }
            catch (Exception ex)
            {
                return "[ERROR] " + ex.Message;
            }
        }

    }
}
