using System.Runtime.InteropServices;
using System.Text;

namespace Mikan.Toolkit.Bindings
{
    public static class Win32Interop
    {
        // Structures
        public struct WINDOWINFO
        {
            public uint cbSize;
            public RECT rcWindow;
            public RECT rcClient;
            public uint dwStyle;
            public uint dwExStyle;
            public uint dwWindowStatus;
            public uint cxWindowBorders;
            public uint cyWindowBorders;
            public ushort atomWindowType;
            public ushort wCreatorVersion;
        }

        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public struct NOTIFYICONDATA
        {
            public uint cbSize;
            public nint hWnd;
            public uint uID;
            public uint uFlags;
            public uint uCallbackMessage;
            public nint hIcon;
            public string szTip;
        }

        // Methods

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern nint FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(nint windowHandle, out RECT windowRect);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(nint windowHandle, nint windowAfter, int x, int y, int width, int height, uint flags);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(nint windowHandle, int nIndex);

        [DllImport("user32.dll")]
        private static extern nint GetFocus();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowLong(nint windowHandle, int nIndex, int newLongValue);

        [DllImport("user32.dll")]
        private static extern nint SetFocus(nint windowHandle);

        [DllImport("user32.dll")]
        private static extern bool GetWindowInfo(nint windowHandle, ref WINDOWINFO windowInfo);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetLayeredWindowAttributes(nint windowHandle, uint colorKey, byte alpha, uint flags);

        [DllImport("shell32.dll")]
        private static extern bool Shell_NotifyIcon(uint message, ref NOTIFYICONDATA notificationData);

        [DllImport("user32.dll")]
        private static extern int GetClassName(nint windowHandle, StringBuilder classNameBuffer, int maxLength);

        /// <summary>
        /// Finds a window by its class name and window title.
        /// </summary>
        public static nint FindWindowByClassName(string className)
        {
            return FindWindow(className, null);
        }

        /// <summary>
        /// Finds a window by its title.
        /// </summary>
        public static nint FindWindowByTitle(string windowName)
        {
            return FindWindow(null, windowName);
        }

        /// <summary>
        /// Gets the rectangle coordinates of the window.
        /// </summary>
        public static bool GetWindowBounds(nint windowHandle, out RECT windowRect)
        {
            return GetWindowRect(windowHandle, out windowRect);
        }

        /// <summary>
        /// Sets the position and size of the window.
        /// </summary>
        public static bool SetWindowPosition(nint windowHandle, int x, int y, int width, int height, uint flags = 0)
        {
            return SetWindowPos(windowHandle, nint.Zero, x, y, width, height, flags);
        }

        /// <summary>
        /// Gets a property from the window.
        /// </summary>
        public static int GetWindowLongProperty(nint windowHandle, int nIndex)
        {
            return GetWindowLong(windowHandle, nIndex);
        }

        /// <summary>
        /// Sets a property for the window.
        /// </summary>
        public static int SetWindowLongProperty(nint windowHandle, int nIndex, int newLongValue)
        {
            return SetWindowLong(windowHandle, nIndex, newLongValue);
        }

        /// <summary>
        /// Gets the currently focused window handle.
        /// </summary>
        public static nint GetCurrentFocus()
        {
            return GetFocus();
        }

        /// <summary>
        /// Sets the focus to the specified window.
        /// </summary>
        public static nint SetWindowFocus(nint windowHandle)
        {
            return SetFocus(windowHandle);
        }



        /// <summary>
        /// Gets information about a window.
        /// </summary>
        public static bool GetWindowInformation(nint windowHandle, ref WINDOWINFO windowInfo)
        {
            return GetWindowInfo(windowHandle, ref windowInfo);
        }


        /// <summary>
        /// Sets the layered window attributes (transparency).
        /// </summary>
        public static bool SetWindowTransparency(nint windowHandle, uint colorKey, byte alpha, uint flags)
        {
            return SetLayeredWindowAttributes(windowHandle, colorKey, alpha, flags);
        }


        /// <summary>
        /// Shows or hides a notification icon in the system tray.
        /// </summary>
        public static bool ShowNotificationIcon(ref NOTIFYICONDATA notificationData)
        {
            return Shell_NotifyIcon(0, ref notificationData);
        }


        /// <summary>
        /// Gets the class name of a window.
        /// </summary>
        public static string GetWindowClassName(nint windowHandle)
        {
            StringBuilder classNameBuffer = new StringBuilder(256);
            int length = GetClassName(windowHandle, classNameBuffer, classNameBuffer.Capacity);
            return length > 0 ? classNameBuffer.ToString() : string.Empty;
        }
    }
}
