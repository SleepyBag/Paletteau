using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics.Eventing.Reader;

namespace Paletteau.Infrastructure.Windows
{
    public class WindowHandler
    {
        #region consts
        // used for SetWinodwPos
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);

        private const UInt32 SWP_ASYNCWINDOWPOS = 0x4000;
        private const UInt32 SWP_DEFERERASE = 0x2000;
        private const UInt32 SWP_DRAWFRAME = 0x0020;
        private const UInt32 SWP_FRAMECHANGED = 0x0020;
        private const UInt32 SWP_HIDEWINDOW = 0x0080;
        private const UInt32 SWP_NOACTIVATE = 0x0010;
        private const UInt32 SWP_NOCOPYBITS = 0x0100;
        private const UInt32 SWP_NOMOVE = 0x0002;
        private const UInt32 SWP_NOOWNERZORDER = 0x0200;
        private const UInt32 SWP_NOREDRAW = 0x0008;
        private const UInt32 SWP_NOREPOSITION = 0x0200;
        private const UInt32 SWP_NOSENDCHANGING = 0x0400;
        private const UInt32 SWP_NOSIZE = 0x0001;
        private const UInt32 SWP_NOZORDER = 0x0004;
        private const UInt32 SWP_SHOWWINDOW = 0x0040;

        private const UInt32 NO_RESIZE = SWP_NOMOVE | SWP_NOSIZE;

        // used for ShowWindow
        private const int SW_HIDE = 0;                 // Hides the window and activates another window.
        private const int SW_NORMAL = 1;               // Activates and displays a window. If the window is minimized or maximized, the system restores it to its original size and position. An application should specify this flag when displaying the window for the first time.
        private const int SW_SHOWMINIMIZED = 2;        // Activates the window and displays it as a minimized window.
        private const int SW_SHOWMAXIMIZED = 3;        // Activates the window and displays it as a maximized window.
        private const int SW_SHOWNOACTIVATE = 4;       // Displays a window in its most recent size and position. This value is similar to SW_SHOWNORMAL, except that the window is not activated.
        private const int SW_SHOW = 5;                 // Activates the window and displays it in its current size and position.
        private const int SW_MINIMIZE = 6;             // Minimizes the specified window and activates the next top-level window in the Z order.
        private const int SW_SHOWMINNOACTIVE = 7;      // Displays the window as a minimized window. This value is similar to SW_SHOWMINIMIZED, except the window is not activated.
        private const int SW_SHOWNA = 8;               // Displays the window in its current size and position. This value is similar to SW_SHOW, except that the window is not activated.
        private const int SW_RESTORE = 9;              // Activates and displays the window. If the window is minimized or maximized, the system restores it to its original size and position. An application should specify this flag when restoring a minimized window.
        private const int SW_SHOWDEFAULT = 10;         // Sets the show state based on the SW_ value specified in the STARTUPINFO structure passed to the CreateProcess function by the program that started the application.
        private const int SW_FORCEMINIMIZE = 11;       // Minimizes a window, even if the thread that owns the window is not responding. This flag should only be used when minimizing windows from a different thread.

        // used for GetWindowLong
        const int GWL_EXSTYLE = (-20);

        const UInt32 WS_EX_TOPMOST = 0x0008;
        #endregion

        #region user32.dll
        [DllImport("user32.dll", SetLastError = true)] 
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetActiveWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool IsZoomed(IntPtr hWnd);
        #endregion

        public IntPtr hWnd;

        public WindowHandler(IntPtr hWnd)
        {
            this.hWnd = hWnd;
        }

        public bool IsOnTop()
        {
            int dwExStyle = GetWindowLong(this.hWnd, GWL_EXSTYLE);
            return (dwExStyle & WS_EX_TOPMOST) != 0;
        }

        public bool PinOnTop()
        {
            return SetWindowPos(this.hWnd, HWND_TOPMOST, 0, 0, 0, 0, NO_RESIZE);
        }

        public bool UnpinOnTop()
        {
            return SetWindowPos(this.hWnd, HWND_NOTOPMOST, 0, 0, 0, 0, NO_RESIZE);
        }

        public bool Hide()
        {
            return SetWindowPos(this.hWnd, IntPtr.Zero, 0, 0, 0, 0, NO_RESIZE | SWP_HIDEWINDOW);
        }

        public bool Show()
        {
            return SetWindowPos(this.hWnd, IntPtr.Zero, 0, 0, 0, 0, NO_RESIZE | SWP_SHOWWINDOW);
        }

        public bool Exists()
        {
            return IsWindow(this.hWnd);
        }

        public void SetActive()
        {
            SetActiveWindow(this.hWnd);
        }

        public string GetWindowTitle()
        {
            var length = GetWindowTextLength(this.hWnd) + 1;
            var title = new StringBuilder(length);
            GetWindowText(this.hWnd, title, length);
            return title.ToString();
        }
        
        public bool Minimize()
        {
            return ShowWindow(this.hWnd, SW_MINIMIZE);
        }

        public bool Restore()
        {
            return ShowWindow(this.hWnd, SW_RESTORE);
        }

        public bool Maximize()
        {
            return ShowWindow(this.hWnd, SW_SHOWMAXIMIZED);
        }

        public bool IsMaximized()
        {
            return IsZoomed(this.hWnd);
        }

        public bool IsMinimized()
        {
            return IsIconic(this.hWnd);
        }
    }
}
