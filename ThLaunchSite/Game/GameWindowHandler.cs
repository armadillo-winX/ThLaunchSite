using Microsoft.VisualBasic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Point = System.Drawing.Point;

namespace ThLaunchSite.Game
{
    internal partial class GameWindowHandler
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [LibraryImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [LibraryImport("user32.dll", SetLastError = true)]
        private static partial int MoveWindow(IntPtr hwnd, int x, int y,
            int nWidth, int nHeight, int bRepaint);

        [LibraryImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, int uFlags);

        public static string? CaptureFileDirectory { get; set; }

        public static string? CaptureFileFormat { get; set; }


        public static int[] GetWindowSizes(int gameProcessId)
        {
            //ウィンドウハンドルの取得
            IntPtr gameProcessMainWindow = Process.GetProcessById(gameProcessId).MainWindowHandle;

            //ウィンドウサイズの取得
            _ = GetWindowRect(gameProcessMainWindow, out RECT rect);
            int width = rect.right - rect.left;
            int height = rect.bottom - rect.top;

            int[] sizes = [width, height];
            return sizes;
        }

        public static void ResizeWindow(int gameProcessId, int width, int height)
        {
            Process gameProcess = Process.GetProcessById(gameProcessId);
            IntPtr gameProcessMainWindow = gameProcess.MainWindowHandle;
            _ = MoveWindow(gameProcessMainWindow, 100, 100, width, height, 1);

            Interaction.AppActivate(gameProcess.Id);
        }

        public static void FixTopMost(int gameProcessId)
        {
            //ウィンドウハンドルの取得
            IntPtr gameProcessMainWindow = Process.GetProcessById(gameProcessId).MainWindowHandle;

            const int SWP_NOSIZE = 0x0001;
            const int SWP_NOMOVE = 0x0002;

            const int HWND_TOPMOST = -1;
            //ウィンドウを最前面に固定
            _ = SetWindowPos(gameProcessMainWindow, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
        }

        public static void ReleaseTopMost(int gameProcessId)
        {
            //ウィンドウハンドルの取得
            IntPtr gameProcessMainWindow = Process.GetProcessById(gameProcessId).MainWindowHandle;

            const int SWP_NOSIZE = 0x0001;
            const int SWP_NOMOVE = 0x0002;

            const int HWND_TOPMOST = -2;
            //ウィンドウの最前面固定を解除
            _ = SetWindowPos(gameProcessMainWindow, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
        }

        public static void GetGameWindowCapture(string gameId, int gameProcessId)
        {
            if (!Directory.Exists(CaptureFileDirectory))
            {
                Directory.CreateDirectory(CaptureFileDirectory);
            }
            Process gameProcess = Process.GetProcessById(gameProcessId);
            IntPtr gameProcessMainWindow = gameProcess.MainWindowHandle;

            Interaction.AppActivate(gameProcessId);
            //最前面に固定
            FixTopMost(gameProcessId);

            //ウィンドウサイズの取得
            _ = GetWindowRect(gameProcessMainWindow, out RECT rect);
            int width = rect.right - rect.left;
            int height = rect.bottom - rect.top;

            Rectangle rectangle = new(rect.left, rect.top, width, height);


            Bitmap bitmap = new(rectangle.Width, rectangle.Height);

            Graphics graphics = Graphics.FromImage(bitmap);

            // 画面をコピー
            graphics.CopyFromScreen(new Point(rectangle.X, rectangle.Y), new Point(0, 0), rectangle.Size);
            graphics.Dispose();

            //最前面固定解除
            ReleaseTopMost(gameProcessId);

            string captureFileName = $"{gameId}-{DateAndTime.Now:yyyy-MM-dd_HH-mm-ss}";

            if (CaptureFileFormat == "PNG")
            {
                bitmap.Save($"{CaptureFileDirectory}\\{captureFileName}.png", ImageFormat.Png);
            }
            else if (CaptureFileFormat == "JPEG")
            {
                bitmap.Save($"{CaptureFileDirectory}\\{captureFileName}.jpeg", ImageFormat.Jpeg);
            }
            else
            {
                bitmap.Save($"{CaptureFileDirectory}\\{captureFileName}.bmp", ImageFormat.Bmp);
            }
        }
    }
}
