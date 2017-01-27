using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Windows.Enum;

namespace Windows
{
    public static class User32
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool EnumDesktopWindows(HWnd hDesktop,
        EnumWindowsProc lpfn, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "GetWindowTextA")]
        public static extern int GetWindowText(HWnd hwnd, StringBuilder result, int maxCount);

        [DllImport("user32.dll", EntryPoint = "GetWindowTextLengthA")]
        public static extern int GetWindowTextLength(HWnd hwnd);

        [DllImport("user32.dll")]
        public static extern bool GetWindowInfo(HWnd hwnd, ref WindowInfo info);


        [DllImport("user32.dll", EntryPoint = "SendMessageA")]
        public static extern IntPtr SendMessage(HWnd hwnd, WindowsMessage message, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", EntryPoint = "SendMessageA")]
        public static extern IntPtr SendMessage(HWnd hwnd, WindowsMessage message, int wParam, int lParam);
        [DllImport("user32.dll", EntryPoint = "SendMessageA")]
        public static extern IntPtr SendMessage(HWnd hwnd, WindowsMessage message, IntPtr wParam, int lParam);
        [DllImport("user32.dll", EntryPoint = "SendMessageA")]
        public static extern IntPtr SendMessage(HWnd hwnd, WindowsMessage message, int wParam, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "GetClassLongA")]
        public static extern IntPtr GetClassLong(HWnd hwnd, IntPtr nIndex);


        [DllImport("user32.dll", EntryPoint = "GetClassLong")]
        public static extern IntPtr GetClassLong32(HWnd hWnd, GetClassLongVal nIndex);

        [DllImport("user32.dll", EntryPoint = "GetClassLongPtr")]
        public static extern IntPtr GetClassLong64(HWnd hWnd, GetClassLongVal nIndex);

        public delegate bool EnumWindowsProc(HWnd hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern Boolean GetWindowRect(HWnd hwnd, out RECT lpRect);

        [DllImport("user32.dll")]
        public static extern RegionType ScrollWindowEx(HWnd hwnd, int dx, int dy, ref RECT Scroll, ref RECT Clip, HRgn Update, out RECT lprcUpdate, Enum.ShowWindow flags);
        [DllImport("user32.dll")]
        public static extern RegionType ScrollWindowEx(HWnd hwnd, int dx, int dy, IntPtr Scroll, IntPtr Clip, HRgn Update, out RECT lprcUpdate, Enum.ShowWindow flags);
        [DllImport("user32.dll")]
        public static extern RegionType ScrollWindowEx(HWnd hwnd, int dx, int dy, ref RECT Scroll, ref RECT Clip, HRgn Update, IntPtr nullLprcUpdate, Enum.ShowWindow flags);
        [DllImport("user32.dll")]
        public static extern RegionType ScrollWindowEx(HWnd hwnd, int dx, int dy, IntPtr Scroll, IntPtr Clip, HRgn Update, IntPtr nullLprcUpdate, Enum.ShowWindow flags);

        public static IntPtr GetClassLongPtr(HWnd hWnd, GetClassLongVal nIndex) {
            if (IntPtr.Size > 4)
                return GetClassLong64(hWnd, nIndex);
            else
                return GetClassLong32(hWnd, nIndex);
        }


        /// <summary>
        /// Loads the specified bitmap resource from a module's executable file.      
        /// </summary>
        /// <param name="intstance">Handle to the instance of the module whose executable file contains the bitmap to be loaded.</param>
        /// <param name="resourceName">Pointer to a null-terminated string that contains the name of the bitmap resource to be loaded. </param>
        /// <returns>Handle to the specified bitmap, or null or failure.</returns>
        [DllImport("user32.dll", EntryPoint = "LoadBitmapA")]
        public static extern HBitmap LoadBitmap(HInstance intstance, [MarshalAs(UnmanagedType.LPStr)] string resourceName);
        /// <summary>
        /// Loads the specified bitmap resource from a module's executable file.      
        /// </summary>
        /// <param name="intstance">Handle to the instance of the module whose executable file contains the bitmap to be loaded.</param>
        /// <param name="ResID">The resource identifier in the low-order word and zero in the high-order word.</param>
        /// <returns>Handle to the specified bitmap, or null or failure.</returns>
        [DllImport("user32.dll", EntryPoint = "LoadBitmapA")]
        public static extern HBitmap LoadBitmap(HInstance intstance, int ResID);

        /// <summary>
        /// Converts the client-area coordinates of a specified points to screen coordinates.
        /// </summary>
        /// <param name="window">Handle to the window whose client area is used for the conversion.</param>
        /// <param name="points">Pointer to a POINT structure that contains the client coordinates to be converted. The new screen coordinates are copied into this structure if the function succeeds.</param>
        /// <returns>TRUE for success, FALSE for failure.</returns>
        [DllImport("user32.dll")]
        public static extern BOOL ClientToScreen(HWnd window, ref Gdi.POINT point);
        /// <summary>
        /// Converts screen coordinates of a specified points to the client-area coordinates.
        /// </summary>
        /// <param name="window">Handle to the window whose client area is used for the conversion.</param>
        /// <param name="points">Pointer to a POINT structure that contains the coordinates to be converted. The new coordinates are copied into this structure if the function succeeds.</param>
        /// <returns>TRUE for success, FALSE for failure.</returns>
        [DllImport("user32.dll")]
        public static extern int ScreenToClient(HWnd window, ref Gdi.POINT point);

        /// <summary>
        /// This function retrieves a handle to a display device context (DC) for the client area of the specified window.
        /// </summary>
        /// <param name="window">Handle to the window whose device context is to be retrieved. If this value is NULL, GetDC retrieves the device context for the entire screen.</param>
        /// <returns>The handle the device context for the specified window's client area indicates success. NULL indicates failure. </returns>
        [DllImport("user32.dll")]
        public static extern HDc GetDC(HWnd window);

        /// <summary>
        /// This function releases a device context (DC), freeing it for use by other applications.
        /// </summary>
        /// <param name="window">Handle to the window whose device context is to be released. </param>
        /// <param name="dc">Handle to the device context to be released.</param>
        /// <returns>One to indicate that the device context is released, Zero to indicate that the device context is not released.</returns>
        [DllImport("user32.dll")]
        public static extern BOOL ReleaseDC(HWnd window, HDc dc);

        /// <summary>
        /// This function retrieves the device context (DC) for the entire window, including title bar, menus, and scroll bars.
        /// </summary>
        /// <param name="window">Handle to the window with a device context that is to be retrieved. </param>
        /// <returns>The handle of a device context for the specified window indicates success. NULL indicates an error or an invalid hWnd parameter. </returns>
        [DllImport("user32.dll")]
        public static extern HDc GetWindowDC(HWnd window);

    }
}
