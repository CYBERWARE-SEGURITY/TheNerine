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
    public class EffectExplodeEyers
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateDIBSection(IntPtr hdc, [In] ref BITMAPINFO pbmi, uint iUsage, out IntPtr ppvBits, IntPtr hSection, uint dwOffset);

        [DllImport("gdi32.dll")]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll")]
        private static extern bool StretchBlt(IntPtr hdcDest, int nXOriginDest, int nYOriginDest, int nWidthDest, int nHeightDest,
                                              IntPtr hdcSrc, int nXOriginSrc, int nYOriginSrc, int nWidthSrc, int nHeightSrc, uint dwRop);

        private const int SRCCOPY = 0x00CC0020;
        private const int BI_RGB = 0;

        [StructLayout(LayoutKind.Sequential)]
        private struct BITMAPINFOHEADER
        {
            public uint biSize;
            public int biWidth;
            public int biHeight;
            public ushort biPlanes;
            public ushort biBitCount;
            public uint biCompression;
            public uint biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public uint biClrUsed;
            public uint biClrImportant;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct BITMAPINFO
        {
            public BITMAPINFOHEADER bmiHeader;
            public RGBQUAD bmiColors;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RGBQUAD
        {
            public byte rgbBlue;
            public byte rgbGreen;
            public byte rgbRed;
            public byte rgbReserved;
        }
        public static void DistorcaoOndular()
        {
            IntPtr dc = GetDC(IntPtr.Zero);
            IntPtr dcCopy = CreateCompatibleDC(dc);
            int w = Screen.PrimaryScreen.Bounds.Width;
            int h = Screen.PrimaryScreen.Bounds.Height;

            BITMAPINFO bmpi = new BITMAPINFO();
            bmpi.bmiHeader.biSize = (uint)Marshal.SizeOf(bmpi.bmiHeader);
            bmpi.bmiHeader.biWidth = w;
            bmpi.bmiHeader.biHeight = h;
            bmpi.bmiHeader.biPlanes = 1;
            bmpi.bmiHeader.biBitCount = 32;
            bmpi.bmiHeader.biCompression = BI_RGB;

            IntPtr ppvBits;
            IntPtr bmp = CreateDIBSection(dc, ref bmpi, 0, out ppvBits, IntPtr.Zero, 0);
            SelectObject(dcCopy, bmp);

            unsafe
            {
                RGBQUAD* rgbquad = (RGBQUAD*)ppvBits;
                int i = 0;

                while (true)
                {
                    StretchBlt(dcCopy, 0, 0, w, h, dc, 0, 0, w, h, SRCCOPY);

                    double amplitudeX = 10.0 * Math.Sin(i * 0.20); // Amplitude da ondulação no eixo X
                    double amplitudeY = 10.0 * Math.Cos(i * 0.10); // Amplitude da ondulação no eixo Y

                    for (int y = 0; y < h; y++)
                    {
                        for (int x = 0; x < w; x++)
                        {
                            int index = y * w + x;

                            // Calcula o deslocamento com base na posição atual e nas amplitudes
                            int newX = x + (int)(amplitudeX * Math.Sin(40 * 2 + 40 * y / (double)h));
                            int newY = y + (int)(amplitudeY * Math.Cos(40 * 2 + 40 * x / (double)w));

                            if (newX >= 0 && newX < w && newY >= 0 && newY < h)
                            {
                                int newIndex = newY * w + newX;
                                rgbquad[index] = rgbquad[newIndex];
                            }
                        }
                    }

                    i++;
                    StretchBlt(dc, 0, 0, w, h, dcCopy, 0, 0, w, h, SRCCOPY);
                }
            }

            ReleaseDC(IntPtr.Zero, dc);
        }
    }
}
