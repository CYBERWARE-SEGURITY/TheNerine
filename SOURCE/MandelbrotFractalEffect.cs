using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MandelbrotFractal
{
    public class MandelbrotFractalEffect : Form
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        public static extern int ReleaseDC(IntPtr hwnd, IntPtr dc);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool StretchBlt(IntPtr hdcDest, int xDest, int yDest, int wDest, int hDest, IntPtr hdcSrc, int xSrc, int ySrc, int wSrc, int hSrc, uint rop);

        const int SRCCOPY = 0x00CC0020;

        private double zoom = 1.0;
        private double moveX = -0.5, moveY = 0;
        private int maxIterations = 1000;
        private Bitmap bmp;
        private Graphics gfx;

        public MandelbrotFractalEffect()
        {
            this.Width = Screen.PrimaryScreen.Bounds.Width;
            this.Height = Screen.PrimaryScreen.Bounds.Height;
            this.FormBorderStyle = FormBorderStyle.None;
            this.ControlBox = false;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.MaximizeBox = true;
            this.DoubleBuffered = true;
            this.MouseClick += new MouseEventHandler(this.OnMouseClick);
            this.FormClosing += new FormClosingEventHandler(this.OnFormClosing);
            bmp = new Bitmap(this.Width, this.Height, PixelFormat.Format32bppArgb);
            gfx = Graphics.FromImage(bmp);
            Task.Run(() => RenderMandelbrot());
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            lock (bmp)
            {
                e.Graphics.DrawImage(bmp, 0, 0, this.Width, this.Height);
            }
        }

        private void RenderMandelbrot()
        {
            int screenWidth = this.Width;
            int screenHeight = this.Height;

            while (true)
            {
                Bitmap tempBmp = new Bitmap(screenWidth, screenHeight, PixelFormat.Format32bppArgb);

                Parallel.For(0, screenWidth, x =>
                {
                    for (int y = 0; y < screenHeight; y++)
                    {
                        double zx = 1.5 * (x - screenWidth / 2) / (0.5 * zoom * screenWidth) + moveX;
                        double zy = (y - screenHeight / 2) / (0.5 * zoom * screenHeight) + moveY;
                        double cX = zx;
                        double cY = zy;
                        int i = maxIterations;

                        while (zx * zx + zy * zy < 4 && i > 0)
                        {
                            double tmp = zx * zx - zy * zy + cX;
                            zy = 2.0 * zx * zy + cY;
                            zx = tmp;
                            i--;
                        }

                        int red = Math.Min(255, i % 256);
                        int green = Math.Min(255, (i * 2) % 256);
                        int blue = Math.Min(255, (i * 3) % 256);

                        lock (tempBmp)
                        {
                            tempBmp.SetPixel(x, y, Color.FromArgb(red, green, blue));
                        }
                    }
                });

                lock (bmp)
                {
                    gfx.DrawImage(tempBmp, 0, 0);
                }
                this.Invalidate();
                Thread.Sleep(100);
            }
        }

        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            // Zoom in
            if (e.Button == MouseButtons.Left)
            {
                zoom *= 1.5;
                moveX += (e.X - this.Width / 2.0) / (0.5 * zoom * this.Width);
                moveY += (e.Y - this.Height / 2.0) / (0.5 * zoom * this.Height);
            }
            // Zoom out
            else if (e.Button == MouseButtons.Right)
            {
                zoom /= 1.5;
            }

            Task.Run(() => RenderMandelbrot());
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MandelbrotFractalEffect));
            this.SuspendLayout();
            // 
            // MandelbrotFractalEffect
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.ControlBox = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.Name = "MandelbrotFractalEffect";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Mandelbrot Fractal";
            this.TopMost = true;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.ResumeLayout(false);

        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
