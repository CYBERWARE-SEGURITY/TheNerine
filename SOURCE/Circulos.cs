using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MandelbrotFractal
{
    public class Circulos
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        public static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("gdi32.dll")]
        public static extern bool Ellipse(IntPtr hdc, int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateSolidBrush(uint color);

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();

        const int SM_CXSCREEN = 0;
        const int SM_CYSCREEN = 1;

        public static void CirculosMoveEffect()
        {
            RECT screenBounds = GetScreenBounds();
            int screenWidth = screenBounds.right - screenBounds.left;
            int screenHeight = screenBounds.bottom - screenBounds.top;

            Circle[] circles = new Circle[100];
            for (int i = 0; i < circles.Length; i++)
            {
                circles[i] = new Circle(screenWidth, screenHeight);
            }

            while (true)
            {
                IntPtr desk = GetDC(IntPtr.Zero);

                foreach (Circle circle in circles)
                {
                    circle.Move(screenBounds);
                    circle.CheckCollision(circles);
                }

                // Desenha os círculos na tela
                foreach (Circle circle in circles)
                {
                    circle.Draw(desk);
                }

                ReleaseDC(GetDesktopWindow(), desk);
            }
        }

        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(int nIndex);

        [DllImport("user32.dll")]
        public static extern bool SystemParametersInfo(int uiAction, int uiParam, ref RECT pvParam, int fWinIni);

        const int SPI_GETWORKAREA = 48;

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        static RECT GetScreenBounds()
        {
            RECT rect = new RECT();
            SystemParametersInfo(SPI_GETWORKAREA, 0, ref rect, 0);
            return rect;
        }

        class Circle
        {
            private int x, y;
            private int diameter;
            private int deltaX, deltaY;
            private uint color;

            public Circle(int screenWidth, int screenHeight)
            {
                Random rand = new Random();
                this.x = rand.Next(screenWidth);
                this.y = rand.Next(screenHeight);
                this.diameter = rand.Next(60, 80);
                this.deltaX = rand.Next(1, 3);
                this.deltaY = rand.Next(1, 3);
                this.color = (uint)((rand.Next(256) << 16) | (rand.Next(256) << 8) | rand.Next(256));
            }

            public void Move(RECT screenBounds)
            {
                this.x += this.deltaX;
                this.y += this.deltaY;

                if (this.x <= screenBounds.left || this.x + this.diameter >= screenBounds.right)
                    this.deltaX *= -1;
                if (this.y <= screenBounds.top || this.y + this.diameter >= screenBounds.bottom)
                    this.deltaY *= -1;
            }

            public void Draw(IntPtr hdc)
            {
                IntPtr brush = CreateSolidBrush(this.color);
                Ellipse(hdc, this.x, this.y, this.x + this.diameter, this.y + this.diameter);
                DeleteObject(brush);
            }

            public void CheckCollision(Circle[] circles)
            {
                foreach (Circle otherCircle in circles)
                {
                    if (otherCircle != this)
                    {
                        double distance = Math.Sqrt(Math.Pow(this.x - otherCircle.x, 2) + Math.Pow(this.y - otherCircle.y, 2));
                        if (distance <= (this.diameter + otherCircle.diameter) / 2)
                        {
                            // Colisão detectada, inverte as direções
                            int tempDeltaX = this.deltaX;
                            int tempDeltaY = this.deltaY;
                            this.deltaX = otherCircle.deltaX;
                            this.deltaY = otherCircle.deltaY;
                            otherCircle.deltaX = tempDeltaX;
                            otherCircle.deltaY = tempDeltaY;
                        }
                    }
                }
            }
        }

        static int w = Screen.PrimaryScreen.Bounds.Width;
        static int h = Screen.PrimaryScreen.Bounds.Height;

        [DllImport("gdi32.dll")]
        static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        static extern IntPtr CreateDIBSection(IntPtr hdc, ref BITMAPINFO pbmi, uint pila, out IntPtr ppvBits, IntPtr hSection, uint dwOffset);

        [DllImport("gdi32.dll")]
        static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll")]
        static extern bool StretchBlt(IntPtr hdcDest, int xDest, int yDest, int wDest, int hDest, IntPtr hdcSrc, int xSrc, int ySrc, int wSrc, int hSrc, uint rop);

        [DllImport("gdi32.dll")]
        static extern bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        static extern bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int wDest, int hDest, IntPtr hdcSrc, int xSrc, int ySrc, uint rop);

        [DllImport("gdi32.dll")]
        static extern bool SetStretchBltMode(IntPtr hdc, int mode);

        const int SRCCOPY = 0x00CC0020;
        const int HALFTONE = 4;
        const int BI_RGB = 0;

        public static void BlurDark()
        {
            IntPtr hdc = GetDC(IntPtr.Zero);
            IntPtr dcCopy = CreateCompatibleDC(hdc);

            try
            {
                int ws = w / 4;
                int hs = h / 4;

                BITMAPINFO bmi = new BITMAPINFO();
                bmi.biSize = Marshal.SizeOf(bmi);
                bmi.biWidth = ws;
                bmi.biHeight = hs;
                bmi.biPlanes = 1;
                bmi.biBitCount = 32;
                bmi.biCompression = BI_RGB;

                IntPtr ppvBits;
                IntPtr hbmp = CreateDIBSection(hdc, ref bmi, BI_RGB, out ppvBits, IntPtr.Zero, 0);
                IntPtr oldBmp = SelectObject(dcCopy, hbmp);

                int i = 0;
                double angle = 0.0;

                // Configura o modo de estiramento para HALFTONE
                SetStretchBltMode(dcCopy, HALFTONE);
                SetStretchBltMode(hdc, HALFTONE);

                Random random = new Random();

                while (true)
                {
                    StretchBlt(dcCopy, 0, 0, ws, hs, hdc, 0, 0, w, h, SRCCOPY);

                    unsafe
                    {
                        RGBQUAD* rgbquad = (RGBQUAD*)ppvBits.ToPointer();

                        for (int x = 0; x < ws; x++)
                        {
                            for (int y = 0; y < hs; y++)
                            {
                                int index = y * ws + x;

                                int cx = Math.Abs(x - (ws / 2));
                                int cy = Math.Abs(y - (hs / 2));

                                double zx = Math.Cos(angle) + cx + Math.Sin(angle) + cy;
                                double zy = Math.Sin(angle) - cx * Math.Cos(angle) - cy;

                                int fx = (int)(Math.Pow(zx - i, zy - i));

                                rgbquad[index].rgbRed = (byte)Math.Min(255, rgbquad[index].rgbRed + fx);
                                rgbquad[index].rgbGreen = (byte)Math.Min(255, rgbquad[index].rgbGreen + fx);
                                rgbquad[index].rgbBlue = (byte)Math.Min(255, rgbquad[index].rgbBlue + fx);
                            }
                        }
                    }

                    i++;
                    angle += 0.01;

                    StretchBlt(hdc, 0, 0, w, h, dcCopy, 0, 0, ws, hs, SRCCOPY);

                    Thread.Sleep(random.Next(10));
                    RedrawScreen();
                }
            }
            finally
            {
                DeleteDC(dcCopy);
                ReleaseDC(IntPtr.Zero, hdc);
            }
        }

        static void RedrawScreen()
        {
            IntPtr desktop = GetDC(IntPtr.Zero);
            BitBlt(desktop, 0, 0, w, h, desktop, 0, 0, SRCCOPY);
            ReleaseDC(IntPtr.Zero, desktop);
        }

        [DllImport("user32.dll")]
        static extern bool InvalidateRect(IntPtr hWnd, IntPtr lpRect, bool bErase);

        public static void LimparTela()
        {
            for (int num = 0; num < 10; num++)
            {
                InvalidateRect(IntPtr.Zero, IntPtr.Zero, true);
                Thread.Sleep(10);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        struct BITMAPINFO
        {
            public int biSize;
            public int biWidth;
            public int biHeight;
            public short biPlanes;
            public short biBitCount;
            public int biCompression;
            public int biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public int biClrUsed;
            public int biClrImportant;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct RGBQUAD
        {
            public byte rgbBlue;
            public byte rgbGreen;
            public byte rgbRed;
            public byte rgbReserved;
        }

    }
}
