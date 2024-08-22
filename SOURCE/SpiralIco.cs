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
    public class SpiralIco
    {
        static int w = Screen.PrimaryScreen.Bounds.Width;
        static int h = Screen.PrimaryScreen.Bounds.Height;
        static int iconSize = 50;
        static double spiralSpeed = 20;

        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("gdi32.dll")]
        static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll")]
        static extern bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        static extern bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int wDest, int hDest, IntPtr hdcSrc, int xSrc, int ySrc, uint rop);

        const int SRCCOPY = 0x00CC0020;

        public static void SpiralDrawingEffect()
        {
            IntPtr hdc = GetDC(IntPtr.Zero);
            IntPtr dcMem = CreateCompatibleDC(hdc);

            try
            {
                // Prepare icon
                Icon errorIcon = SystemIcons.Error;
                Bitmap iconBitmap = errorIcon.ToBitmap();
                IntPtr hBitmap = iconBitmap.GetHbitmap();
                IntPtr oldBitmap = SelectObject(dcMem, hBitmap);

                double angle = 0;
                double radius = 0;
                double centerX = w / 2;
                double centerY = h / 2;

                while (true)
                {
                    // Clear screen
                    BitBlt(hdc, 0, 0, w, h, IntPtr.Zero, 0, 0, SRCCOPY);


                    double x = centerX + radius * Math.Cos(angle) - iconSize / 2;
                    double y = centerY + radius * Math.Sin(angle) - iconSize / 2;

                    // Draw icon
                    using (Graphics g = Graphics.FromHdc(hdc))
                    {
                        g.DrawImage(iconBitmap, (float)x, (float)y, iconSize, iconSize);
                    }

                    angle += spiralSpeed;
                    radius += 0.5;
                }
            }
            finally
            {
                DeleteDC(dcMem);
                ReleaseDC(IntPtr.Zero, hdc);
            }
        }
    }
}
