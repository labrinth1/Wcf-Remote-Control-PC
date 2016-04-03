using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace ConsoleApplication1
{
    class Program
    {
        public static ServiceReference1.ServiceClient dataSvc;

        static int ScreenWidth = Screen.PrimaryScreen.Bounds.Width;
        static int ScreenHeight = Screen.PrimaryScreen.Bounds.Height;
        static int ImgTargetWidth = 1024;
        static int ImgTargetHeight = 600;

        static bool Run = false;

        static void Main(string[] args)
        {
#if DEBUG
            Console.WriteLine("Client running in debug mode Keyboard and mouseevets will not be handled!");
#endif

            dataSvc = new ServiceReference1.ServiceClient();

            Thread aThread = new Thread(new ThreadStart(StartStream));
            aThread.Start();
        }
        #region User32
        const uint KEYEVENTF_KEYUP = 0x0002;
        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        private const int MOUSEEVENTF_MOVE = 0x0001;
        private const int MOUSEEVENTF_ABSOLUTE = 0x8000;
        [DllImport("user32.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        [DllImport("user32.dll")]
        static extern bool GetCursorPos(ref Point lpPoint);


        #endregion
        #region Threads

        private static void CreateThreads()
        {
            new Thread(KeyBoardListner).Start();
            new Thread(MouseMoveListner).Start();
            new Thread(MouseEventsListner).Start();
            new Thread(SendScreenShot).Start();
        }
        private static void StartStream()
        {
            Thread.Sleep(3000);
            Console.WriteLine("Waiting for startcommand.");
            while (true)
            {
                var con = dataSvc.GetStartEvent();
                if (con == 1 && !Run)
                {
                    Run = true;
                    CreateThreads();
                    Console.WriteLine("Createing listen Threads.");
                }
                else if (con == 0)
                {
                    Run = false;
                }
                Thread.Sleep(300);
            }
        }

        private static void MouseEventsListner()
        {
            while (Run)
            {
                var mouseEvent = dataSvc.GetMouseEvents();
                if (mouseEvent != null && mouseEvent != 0)
                {
                    MouseClick((int)mouseEvent);
                }

                Thread.Sleep(100);
            }
        }

        private static void MouseMoveListner()
        {
            int prevX = 0;
            int prevY = 0;
            while (Run)
            {
                var mouseCoordinates = dataSvc.GetMouseCoords();
                if (mouseCoordinates[0] != 0 && !(mouseCoordinates[0] == prevX && mouseCoordinates[1] == prevY))
                {
                    prevX = mouseCoordinates[0];
                    prevY = mouseCoordinates[1];
                    MouseMove(prevX, prevY);
                }

                Thread.Sleep(100);
            }
        }

        private static void KeyBoardListner()
        {
            while (Run)
            {
                var keyBoardEvent = dataSvc.GetKeyBordEvent();
                if (keyBoardEvent != 0 && keyBoardEvent != null)
                {
                    int keyDown = (int)keyBoardEvent % 10; // Determin keyup or down.
                    int keycode = (int)keyBoardEvent / 10; // Raw keycode

                    KeyboardEvent(keycode, keyDown);
                }

                Thread.Sleep(100);
            }
        }
        #endregion
        #region Keyboard

        public static void KeyboardEvent(int keycode, int keyDown)
        {
            if (keyDown == 1)
            {
                Console.WriteLine("KeyDown: " + keycode);
#if !DEBUG
                keybd_event((byte)keycode, 0, 0, 0);
#endif
            }
            else
            {
                Console.WriteLine("KeyUp: " + keycode);
#if !DEBUG
                keybd_event((byte)keycode, 0, KEYEVENTF_KEYUP, 0);
#endif
            }
        }
        #endregion
        #region Mouse

        public static void MouseMove(int x, int y)
        {
            // Convert incomming pixels from 1024x600 format to the actual screensize.
            double ConvertedX = (double)ScreenWidth / (double)ImgTargetWidth * x;
            double ConvertedY = (double)ScreenHeight / (double)ImgTargetHeight * y;

            double outputX = ConvertedX * 65535 / ScreenWidth;
            double outputY = ConvertedY * 65535 / ScreenHeight;

            Console.WriteLine("Mouse move to X: {0} Y: {1}", ConvertedX, ConvertedY);
#if !DEBUG
            mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE, (int)outputX, (int)outputY, 0, 0);
#endif
        }
        public static void MouseClick(int dwFlag)
        {
            var curPos = GetCursorPosition();

            Console.WriteLine("MouseClick: {0} at X: {1} Y: {2}", dwFlag,curPos.X, curPos.Y);
#if !DEBUG
            mouse_event(dwFlag, cur.X, cur.Y, 0, 0);
#endif
        }

        public static Point GetCursorPosition()
        {
            Point defPnt = new Point();
            GetCursorPos(ref defPnt);

            return defPnt;
        }

        #endregion
        #region imagehandler
        /// <summary>
        /// Take the screenshot.
        /// Convert it to 1024x600 to be used direct in webbrowser.
        /// Create bytearray and send.
        /// </summary>
        public static void SendScreenShot()
        {
            Rectangle destinationRectangle = new Rectangle(0, 0, ImgTargetWidth, ImgTargetHeight);
            using (Bitmap bmpScreenshot = new Bitmap(ScreenWidth, ScreenHeight, PixelFormat.Format32bppArgb))
            {
                var gfxScreenshot = Graphics.FromImage(bmpScreenshot);

                using (Bitmap bmpScreenShotTargetSize = new Bitmap(ImgTargetWidth, ImgTargetHeight, PixelFormat.Format32bppArgb))
                {
                    var gfxDestination = Graphics.FromImage(bmpScreenShotTargetSize);

                    while (Run)
                    {
                        gfxScreenshot.CopyFromScreen(Screen.PrimaryScreen.Bounds.X,
                                                    Screen.PrimaryScreen.Bounds.Y, 0, 0,
                                                    Screen.PrimaryScreen.Bounds.Size,
                                                    CopyPixelOperation.SourceCopy);

                        gfxDestination.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        gfxDestination.CompositingQuality = CompositingQuality.HighQuality;
                        gfxDestination.PixelOffsetMode = PixelOffsetMode.HighQuality;

                        gfxDestination.DrawImage(bmpScreenshot, destinationRectangle);
                        dataSvc.SetData(ImageToByte(bmpScreenShotTargetSize));

                        Console.WriteLine("Sending screenshot.");
                        Thread.Sleep(500);

                    }
                }

            }
        }

        public static ImageConverter converter = new ImageConverter();
        public static byte[] ImageToByte(Image img)
        {
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }
        #endregion
    }
}
