using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MandelbrotFractal
{
    public class Carga1
    {
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
        static extern bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int wDest, int hDest, IntPtr hdcSrc, int xSrc, int ySrc, uint rop);

        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("gdi32.dll")]
        static extern bool DeleteDC(IntPtr hdc);

        const int SRCCOPY = 0x00CC0020;
        const int BI_RGB = 0;


        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);


        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, int dwRop);

        public static void ChuvaPixel()
        {
            Random r = new Random();
            int x = Screen.PrimaryScreen.Bounds.Width, y = Screen.PrimaryScreen.Bounds.Height;
            int numPixels = 1000;

            while (true)
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                IntPtr mhdc = CreateCompatibleDC(hdc);
                IntPtr hbit = CreateCompatibleBitmap(hdc, x, y);
                IntPtr holdbit = SelectObject(mhdc, hbit);
                BitBlt(mhdc, 0, 0, x, y, hdc, 0, 0, SRCCOPY);

                // Aplicar "chuva" de pixels
                using (Graphics g = Graphics.FromHdc(mhdc))
                {
                    for (int i = 0; i < numPixels; i++)
                    {
                        int px = r.Next(0, x);
                        int py = r.Next(0, y);
                        Color color = Color.FromArgb(r.Next(256), r.Next(256), r.Next(256));
                        g.FillRectangle(new SolidBrush(color), px, py, 1, 1);
                    }
                }

                BitBlt(hdc, 0, 0, x, y, mhdc, 0, 0, SRCCOPY);

                SelectObject(mhdc, holdbit);
                DeleteObject(holdbit);
                DeleteObject(hbit);
                DeleteObject(mhdc);
                ReleaseDC(IntPtr.Zero, hdc);
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

        [DllImport("gdi32.dll")]
        static extern IntPtr CreateSolidBrush(uint color);

        public static void ZoomEffect()
        {
            IntPtr hdc = GetDC(IntPtr.Zero);
            IntPtr dcCopy = CreateCompatibleDC(hdc);

            try
            {
                BITMAPINFO bmi = new BITMAPINFO();
                bmi.biSize = Marshal.SizeOf(bmi);
                bmi.biWidth = w;
                bmi.biHeight = -h;
                bmi.biPlanes = 1;
                bmi.biBitCount = 32;
                bmi.biCompression = BI_RGB;

                IntPtr ppvBits;
                IntPtr hbmp = CreateDIBSection(hdc, ref bmi, BI_RGB, out ppvBits, IntPtr.Zero, 0);
                IntPtr oldBmp = SelectObject(dcCopy, hbmp);

                double scale = 1.0;

                while (true)
                {
                    BitBlt(dcCopy, 0, 0, w, h, hdc, 0, 0, SRCCOPY);

                    // Draw zoom effect
                    int zoomWidth = (int)(w * scale);
                    int zoomHeight = (int)(h * scale);
                    int x = (w - zoomWidth) / 2;
                    int y = (h - zoomHeight) / 2;

                    StretchBlt(hdc, 0, 0, w, h, dcCopy, x, y, zoomWidth, zoomHeight, SRCCOPY);
                    StretchBlt(hdc, -100, -100, w + 10, h + 10, dcCopy, x, y, zoomWidth, zoomHeight, 0x333333);

                    scale += 0.01;
                    if (scale > 1.5 || scale < 0.5)
                    {
                        scale = Math.Max(0.5, Math.Min(1.5, scale));
                    }

                    Thread.Sleep(100);
                }
            }
            finally
            {
                DeleteDC(dcCopy);
                ReleaseDC(IntPtr.Zero, hdc);
            }
        }
    }
}
