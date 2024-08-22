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
    public class IconsSpawn
    {
        static int w = Screen.PrimaryScreen.Bounds.Width;
        static int h = Screen.PrimaryScreen.Bounds.Height;
        static int iconSize = 40; // Tamanho do ícone

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

        public static void MovingIconEffect()
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

                int xOffset = 0;
                double amplitude = 600; // Amplitude da onda
                double frequency = 140;
                int yPos = h / 2;

                while (true)
                {
                    // Clear screen
                    BitBlt(hdc, 0, 0, w, h, IntPtr.Zero, 0, 0, SRCCOPY);

                    // Calculate new position
                    int xPos = (int)(xOffset % w);
                    int yOffset = (int)(amplitude * Math.Sin(frequency * xOffset));
                    int y = yPos + yOffset;

                    // Draw icon
                    using (Graphics g = Graphics.FromHdc(hdc))
                    {
                        g.DrawImage(iconBitmap, xPos, y, iconSize, iconSize);
                    }

                    // Update xOffset for movement
                    xOffset += 5; // Move 5 pixels per frame

                    //Thread.Sleep(10); // Adjust the speed of the movement
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
