using Microsoft.VisualBasic;
using System.Runtime.InteropServices;

namespace ThLaunchSite.Game
{
    internal class GameWindowHandler
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int MoveWindow(IntPtr hwnd, int x, int y,
            int nWidth, int nHeight, int bRepaint);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, int uFlags);

        public static int[] GetWindowSizes(string name)
        {
            //ウィンドウハンドルの取得
            IntPtr process = Process.GetProcessesByName(name)[0].MainWindowHandle;

            //ウィンドウサイズの取得
            RECT rect;
            _ = GetWindowRect(process, out rect);
            int width = rect.right - rect.left;
            int height = rect.bottom - rect.top;

            int[] sizes = new int[] { width, height };
            return sizes;
        }

        public static void ResizeWindow(string name, int width, int height)
        {
            Process process = Process.GetProcessesByName(name)[0];
            IntPtr windowHandle = process.MainWindowHandle;
            _ = MoveWindow(windowHandle, 100, 100, width, height, 1);

            Interaction.AppActivate(process.Id);
        }
    }
}
