using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenshotCreator
{
    public class ScreenshotHost : IDisposable
    {
        /// <summary>
        /// Начальная прозрачность
        /// </summary>
        public double Opacity { get; set; } = 0.5;

        /// <summary>
        /// Цвет заливки
        /// </summary>
        public Color Background { get; set; } = Color.Gray;

        private PictureBox frame;

        private Form screenshotHost;

        private Point p1, p2;

        private bool isDrawing;

        public Bitmap GetScreenshot()
        {
            frame = new()
            {
                BackColor = Color.Yellow,
                BorderStyle = BorderStyle.FixedSingle,
                Size = new Size(0,0),
                Visible = false
            };
            Rectangle DispRect = new();
            foreach (var Scr in Screen.AllScreens)
            {
                DispRect.X = Math.Min(DispRect.X, Scr.Bounds.X);
                DispRect.Y = Math.Min(DispRect.Y, Scr.Bounds.Y);
                DispRect.Width += Scr.Bounds.Width;
                DispRect.Height = Math.Max(DispRect.Height, Scr.Bounds.Height);
            }
            screenshotHost = new()
            {
                ShowInTaskbar = false,
                FormBorderStyle = FormBorderStyle.None,
                TopMost = true,
                Opacity = Opacity,
                BackColor = Background,
                TransparencyKey = Color.Yellow,
                Cursor = Cursors.Cross,
                Location=DispRect.Location,
                Size = DispRect.Size
            };
            screenshotHost.Controls.Add(frame);
            screenshotHost.MouseDown += ScreenshotHost_MouseDown;
            screenshotHost.MouseMove += ScreenshotHost_MouseMove;
            screenshotHost.MouseUp += ScreenshotHost_MouseUp;
            screenshotHost.KeyUp += ScreenshotHost_KeyUp;
            screenshotHost.Activated += (_, _) => screenshotHost.Focus();

            if (screenshotHost.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    screenshotHost.Opacity = 0;
                    frame.BorderStyle = BorderStyle.None;
                    Bitmap bmp = new(frame.Width, frame.Height);
                    Graphics g = Graphics.FromImage(bmp);
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.CopyFromScreen(screenshotHost.Left + frame.Left, screenshotHost.Top + frame.Top, 0, 0, frame.Size);
                    return bmp;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return null;
        }

        public async Task<Bitmap> GetScreenshotAsync()
        {
            return await Task.Run(() => GetScreenshot());
        }


        private void ScreenshotHost_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                screenshotHost.DialogResult = DialogResult.Cancel;
        }

        private void ScreenshotHost_MouseDown(object sender, MouseEventArgs e)
        {
            frame.Location = e.Location;
            p1 = e.Location;
            p2 = e.Location;
            frame.Visible = true;
            isDrawing = true;
        }

        private void ScreenshotHost_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDrawing) return;
            p2 = e.Location;
            frame.Location = new Point(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y));
            frame.Size = new Size(Math.Max(p1.X, p2.X) - frame.Location.X, Math.Max(p1.Y, p2.Y) - frame.Location.Y);

        }

        private void ScreenshotHost_MouseUp(object sender, MouseEventArgs e)
        {
            screenshotHost.DialogResult = DialogResult.OK;
        }

        public void Dispose()
        {
            frame = null;
            screenshotHost.Dispose();
            screenshotHost = null;
        }
    }
}
