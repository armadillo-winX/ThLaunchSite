using Microsoft.VisualBasic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Point = System.Drawing.Point;

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

        public static string? CaptureFileDirectory { get; set; }

        public static string? CaptureFileFormat { get; set; }


        public static int[] GetWindowSizes(string gameProcessName)
        {
            //ウィンドウハンドルの取得
            IntPtr gameProcess = Process.GetProcessesByName(gameProcessName)[0].MainWindowHandle;

            //ウィンドウサイズの取得
            RECT rect;
            _ = GetWindowRect(gameProcess, out rect);
            int width = rect.right - rect.left;
            int height = rect.bottom - rect.top;

            int[] sizes = new int[] { width, height };
            return sizes;
        }

        public static void ResizeWindow(string gameProcessName, int width, int height)
        {
            Process gameProcess = Process.GetProcessesByName(gameProcessName)[0];
            IntPtr windowHandle = gameProcess.MainWindowHandle;
            _ = MoveWindow(windowHandle, 100, 100, width, height, 1);

            Interaction.AppActivate(gameProcess.Id);
        }

        public static void GetGameWindowCapture(string gameProcessName)
        {
            if (!Directory.Exists(CaptureFileDirectory))
            {
                Directory.CreateDirectory(CaptureFileDirectory);
            }

            IntPtr gameProcess = Process.GetProcessesByName(gameProcessName)[0].MainWindowHandle;

            //ウィンドウサイズの取得
            RECT rect;
            _ = GetWindowRect(gameProcess, out rect);
            int width = rect.right - rect.left;
            int height = rect.bottom - rect.top;

            Rectangle rectangle = new Rectangle(rect.left, rect.top, width, height);


            Bitmap bitmap = new Bitmap(rectangle.Width, rectangle.Height);

            Graphics graphics = Graphics.FromImage(bitmap);

            // 画面をコピー
            graphics.CopyFromScreen(new Point(rectangle.X, rectangle.Y), new Point(0, 0), rectangle.Size);

            string captureFileName = $"{gameProcessName}-{DateAndTime.Now:yyyy-MM-dd_HH-mm-ss}";

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
