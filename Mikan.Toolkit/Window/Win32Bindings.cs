using System.Runtime.InteropServices;
using System.Text;

namespace Mikan.Toolkit.Window
{
    public static class Win32Bindings
    {
        /// <summary>
        /// Contains information about the window, including its size, style, and status.
        /// This structure is used by the GetWindowInfo function to retrieve details about a window's attributes.
        /// </summary>
        public struct WINDOWINFO
        {
            /// <summary>
            /// The size of the structure, in bytes. This should be initialized to `sizeof(WINDOWINFO)` before calling `GetWindowInfo`.
            /// </summary>
            public uint cbSize;

            /// <summary>
            /// The bounding rectangle of the window, in screen coordinates.
            /// </summary>
            public RECT rcWindow;

            /// <summary>
            /// The client area of the window, in screen coordinates (the area excluding borders, title bar, etc.).
            /// </summary>
            public RECT rcClient;

            /// <summary>
            /// The style of the window (e.g., `WS_BORDER`, `WS_OVERLAPPED`).
            /// </summary>
            public uint dwStyle;

            /// <summary>
            /// The extended window style (e.g., `WS_EX_TOPMOST`, `WS_EX_TOOLWINDOW`).
            /// </summary>
            public uint dwExStyle;

            /// <summary>
            /// The status of the window (e.g., `WS_MAXIMIZED`, `WS_MINIMIZED`).
            /// </summary>
            public uint dwWindowStatus;

            /// <summary>
            /// The width of the window borders.
            /// </summary>
            public uint cxWindowBorders;

            /// <summary>
            /// The height of the window borders.
            /// </summary>
            public uint cyWindowBorders;

            /// <summary>
            /// A unique identifier for the window type (class atom).
            /// </summary>
            public ushort atomWindowType;

            /// <summary>
            /// The version number of the window creator.
            /// </summary>
            public ushort wCreatorVersion;
        }

        /// <summary>
        /// Represents a rectangle by its left, top, right, and bottom coordinates.
        /// Often used for window sizes or screen areas.
        /// </summary>
        public struct RECT
        {
            /// <summary>
            /// The x-coordinate of the left side of the rectangle.
            /// </summary>
            public int Left;

            /// <summary>
            /// The y-coordinate of the top side of the rectangle.
            /// </summary>
            public int Top;

            /// <summary>
            /// The x-coordinate of the right side of the rectangle.
            /// </summary>
            public int Right;

            /// <summary>
            /// The y-coordinate of the bottom side of the rectangle.
            /// </summary>
            public int Bottom;
        }

        /// <summary>
        /// Contains information about an icon displayed in the system tray (notification area).
        /// This structure is used with the `Shell_NotifyIcon` function to add, update, or remove icons from the system tray.
        /// </summary>
        public struct NOTIFYICONDATA
        {
            /// <summary>
            /// The size of the structure, in bytes.
            /// </summary>
            public uint cbSize;

            /// <summary>
            /// The handle to the window associated with the icon. If you want to associate a specific window, provide its handle.
            /// </summary>
            public IntPtr hWnd;

            /// <summary>
            /// A unique identifier for the icon. Use this to distinguish multiple icons.
            /// </summary>
            public uint uID;

            /// <summary>
            /// Flags indicating the icon's behavior. Can be a combination of `NIF_ICON`, `NIF_MESSAGE`, `NIF_TIP`, etc.
            /// </summary>
            public uint uFlags;

            /// <summary>
            /// The message that will be sent to the window when the icon is clicked. This can be used to capture clicks on the tray icon.
            /// </summary>
            public uint uCallbackMessage;

            /// <summary>
            /// The handle to the icon that will be displayed in the system tray.
            /// </summary>
            public IntPtr hIcon;

            /// <summary>
            /// A tooltip text that appears when the mouse hovers over the icon.
            /// </summary>
            public string szTip;
        }

        /// <summary>
        /// Finds a window by its class name and window name (title).
        /// This function retrieves the handle to the first window that matches both the class name and the window title.
        /// </summary>
        /// <param name="lpClassName">The class name of the window. Can be null if you don't want to match by class name.</param>
        /// <param name="lpWindowName">The window title. Can be null if you don't want to match by title.</param>
        /// <returns>The handle to the window, or `IntPtr.Zero` if no matching window is found.</returns>
        [LibraryImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        /// <summary>
        /// Retrieves the bounding rectangle of a window, in screen coordinates.
        /// This function provides the full dimensions of the window, including the non-client area (e.g., borders, title bar).
        /// </summary>
        /// <param name="windowHandle">The handle to the window whose rectangle you want to retrieve.</param>
        /// <param name="windowRect">The structure that will hold the window's rectangle (left, top, right, bottom) after the call.</param>
        /// <returns>True if the function succeeds, false if it fails (you can call `Marshal.GetLastWin32Error` for more info).</returns>
        [LibraryImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr windowHandle, out RECT windowRect);

        /// <summary>
        /// Sets the position and size of a window, and controls its Z-order (position relative to other windows).
        /// This function allows you to move or resize a window and control whether it stays in front or behind other windows.
        /// </summary>
        /// <param name="windowHandle">The handle to the window you want to move or resize.</param>
        /// <param name="windowAfter">The window handle that the target window will be placed after in the Z-order. Use `IntPtr.Zero` to place it at the top.</param>
        /// <param name="x">The new X-coordinate (horizontal position) of the window.</param>
        /// <param name="y">The new Y-coordinate (vertical position) of the window.</param>
        /// <param name="width">The new width of the window.</param>
        /// <param name="height">The new height of the window.</param>
        /// <param name="flags">Flags that control the window's behavior (e.g., `SWP_NOMOVE`, `SWP_NOSIZE`).</param>
        /// <returns>True if the function succeeds, false if it fails.</returns>
        [LibraryImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr windowHandle, IntPtr windowAfter, int x, int y, int width, int height, uint flags);

        /// <summary>
        /// Retrieves the window styles (such as borders, title bar, etc.) for a specific window.
        /// This function can be used to query the properties of a window, like whether it has a border, is resizable, etc.
        /// </summary>
        /// <param name="windowHandle">The handle to the window you want to query.</param>
        /// <param name="nIndex">The index that specifies which style information to retrieve. For example, use `GWL_STYLE` to get the window's basic style.</param>
        /// <returns>The window's style as an integer value. You can use bitwise operations to check for specific styles.</returns>
        [LibraryImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr windowHandle, int nIndex);

        /// <summary>
        /// Sets a new window style for a specified window. This can be used to change the appearance or behavior of a window (e.g., making it resizable).
        /// </summary>
        /// <param name="windowHandle">The handle to the window you want to modify.</param>
        /// <param name="nIndex">The index that specifies which style to change (e.g., `GWL_STYLE` to change the window's style).</param>
        /// <param name="newLongValue">The new window style to apply. You can use bitwise operations to combine different styles.</param>
        /// <returns>The previous window style. This allows you to restore the window's style if needed.</returns>
        [LibraryImport("user32.dll", SetLastError = true)]
        public static extern int SetWindowLong(IntPtr windowHandle, int nIndex, int newLongValue);

        /// <summary>
        /// Retrieves the handle of the window that currently has keyboard focus.
        /// This function is useful for determining which window the user is actively interacting with.
        /// </summary>
        /// <returns>The handle to the currently focused window, or `IntPtr.Zero` if no window has focus.</returns>
        [LibraryImport("user32.dll")]
        public static extern IntPtr GetFocus();

        /// <summary>
        /// Sets the input focus to the specified window, so it becomes the target of keyboard input.
        /// This is useful for directing keyboard input to a specific window (e.g., a text editor or a game window).
        /// </summary>
        /// <param name="windowHandle">The handle to the window you want to set focus to.</param>
        /// <returns>The handle to the previously focused window, or `IntPtr.Zero` if the function succeeds and no previous focus was set.</returns>
        [LibraryImport("user32.dll")]
        public static extern IntPtr SetFocus(IntPtr windowHandle);

        /// <summary>
        /// Retrieves detailed information about a window, including its size, style, and other attributes.
        /// This function fills a `WINDOWINFO` structure with the requested window data.
        /// </summary>
        /// <param name="windowHandle">The handle to the window whose information you want to retrieve.</param>
        /// <param name="windowInfo">The `WINDOWINFO` structure to be filled with window data.</param>
        /// <returns>True if the function succeeds, false if it fails.</returns>
        [LibraryImport("user32.dll")]
        public static extern bool GetWindowInfo(IntPtr windowHandle, ref WINDOWINFO windowInfo);

        /// <summary>
        /// Sets the transparency or alpha blending for a window, which can be used to create transparent or semi-transparent windows.
        /// This function is useful for creating windows with custom appearances (e.g., a floating toolbar with transparency).
        /// </summary>
        /// <param name="windowHandle">The handle to the window whose transparency you want to change.</param>
        /// <param name="colorKey">A color that should be treated as fully transparent. Set to `0` for no color key.</param>
        /// <param name="alpha">The alpha value (transparency level), ranging from 0 (fully transparent) to 255 (fully opaque).</param>
        /// <param name="flags">Flags that specify how the transparency is applied. Use `LWA_ALPHA` to adjust transparency based on the alpha value.</param>
        /// <returns>True if the function succeeds, false if it fails.</returns>
        [LibraryImport("user32.dll", SetLastError = true)]
        public static extern bool SetLayeredWindowAttributes(IntPtr windowHandle, uint colorKey, byte alpha, uint flags);

        /// <summary>
        /// Adds, updates, or removes an icon from the system tray (notification area).
        /// This function allows interaction with the system tray and can be used for applications that run in the background and need to display status icons.
        /// </summary>
        /// <param name="message">The operation to perform (e.g., `NIM_ADD`, `NIM_MODIFY`, `NIM_DELETE`).</param>
        /// <param name="notificationData">A `NOTIFYICONDATA` structure that contains information about the icon (e.g., its handle, tooltip, callback message, etc.).</param>
        /// <returns>True if the operation succeeded, false if it failed.</returns>
        [LibraryImport("shell32.dll")]
        public static extern bool Shell_NotifyIcon(uint message, ref NOTIFYICONDATA notificationData);

        /// <summary>
        /// Retrieves the class name of a window, which is often used to identify the type of window (e.g., a button, a main window, etc.).
        /// This can be helpful for interacting with specific window types in automation or debugging scenarios.
        /// </summary>
        /// <param name="windowHandle">The handle of the window whose class name you want to retrieve.</param>
        /// <param name="classNameBuffer">A buffer to store the class name of the window. It will be filled with the class name after the call.</param>
        /// <param name="maxLength">The maximum length of the class name buffer. Ensure that the buffer is large enough to hold the class name.</param>
        /// <returns>The length of the class name string, or 0 if the function fails. You can use this to check if the class name was successfully retrieved.</returns>
        [LibraryImport("user32.dll")]
        public static extern int GetClassName(IntPtr windowHandle, StringBuilder classNameBuffer, int maxLength);

    }
}
