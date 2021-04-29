using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenshotCreator
{
    public class ScreenshotHost : IDisposable
    {
        #region PrivateFields
        private PictureBox _Frame;

        private Form _ScreenshotHost;

        private Point p1, p2;

        private bool isDrawing;
        #endregion

        #region Properties
        /// <summary>
        /// Начальная прозрачность
        /// </summary>
        public double Opacity { get; set; } = 0.5;

        /// <summary>
        /// Цвет заливки
        /// </summary>
        public Color Background { get; set; } = Color.Gray;

        /// <summary>
        /// Ключ прозрачности
        /// </summary>
        public Color TransparancyKeyColor { get; set; } = Color.Yellow;
        #endregion

        #region ScreenshotMethods

        /// <summary>
        /// Получить границы всех экранов
        /// </summary>
        public static Rectangle GetBounds()
        {
            Rectangle dispRect = new();
            foreach (var scr in Screen.AllScreens)
            {
                dispRect.X = Math.Min(dispRect.X, scr.Bounds.X);
                dispRect.Y = Math.Min(dispRect.Y, scr.Bounds.Y);
                dispRect.Width += scr.Bounds.Width;
                dispRect.Height = Math.Max(dispRect.Height, scr.Bounds.Height);
            }
            return dispRect;
        }

        /// <summary>
        /// Скриншот области экрана
        /// </summary>
        public Bitmap GetScreenshot()
        {
            _Frame = new()
            {
                BackColor = TransparancyKeyColor,
                BorderStyle = BorderStyle.FixedSingle,
                Size = new Size(0, 0),
                Visible = false
            };

            Rectangle dispRect = GetBounds();
            _ScreenshotHost = new()
            {
                ShowInTaskbar = false,
                FormBorderStyle = FormBorderStyle.None,
                Opacity = Opacity,
                BackColor = Background,
                TransparencyKey = TransparancyKeyColor,
                KeyPreview = true,
                Cursor = Cursors.Cross
            };
            _ScreenshotHost.Controls.Add(_Frame);
            _ScreenshotHost.MouseDown += ScreenshotHost_MouseDown;
            _ScreenshotHost.MouseMove += ScreenshotHost_MouseMove;
            _ScreenshotHost.MouseUp += ScreenshotHost_MouseUp;
            _ScreenshotHost.KeyUp += ScreenshotHost_KeyUp;
            _ScreenshotHost.Load += (_, _) =>
            {
                _ScreenshotHost.Location = dispRect.Location;
                _ScreenshotHost.Size = dispRect.Size;
                _ScreenshotHost.TopMost = true;
            };

            _ScreenshotHost.Activated += (_, _) =>
            {
                _ScreenshotHost.BringToFront();
                _ScreenshotHost.Select();
                _ScreenshotHost.Focus();
                Debug.WriteLine("Activated");
            };
            _ScreenshotHost.Deactivate += (_, _) => Debug.WriteLine("Deactivated");

            if (_ScreenshotHost.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _ScreenshotHost.Opacity = 0;
                    _Frame.BorderStyle = BorderStyle.None;
                    Bitmap bmp = new(_Frame.Width, _Frame.Height);
                    Graphics g = Graphics.FromImage(bmp);
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.CopyFromScreen(_ScreenshotHost.Left + _Frame.Left, _ScreenshotHost.Top + _Frame.Top, 0, 0, _Frame.Size);
                    return bmp;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return null;
        }

        /// <summary>
        /// Скриншот всех экранов
        /// </summary>
        public Bitmap GetFullscreenScreenshot()
        {
            Rectangle dispRect = GetBounds();
            try
            {
                Bitmap bmp = new(dispRect.Width, dispRect.Height);
                Graphics g = Graphics.FromImage(bmp);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.CopyFromScreen(dispRect.Left, dispRect.Top, 0, 0, dispRect.Size);
                return bmp;
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        /// <summary>
        /// Скриншот области экрана
        /// </summary>
        public async Task<Bitmap> GetScreenshotAsync() => await Task.Run(GetScreenshot);

        /// <summary>
        /// Скриншот всех экранов
        /// </summary>
        public async Task<Bitmap> GetFullscreenScreenshotAsync() => await Task.Run(GetFullscreenScreenshot);

        #endregion

        #region Mouse
        private void ScreenshotHost_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                _ScreenshotHost.DialogResult = DialogResult.Cancel;
        }
        private void ScreenshotHost_MouseDown(object sender, MouseEventArgs e)
        {
            _Frame.Location = e.Location;
            p1 = e.Location;
            p2 = e.Location;
            _Frame.Visible = true;
            isDrawing = true;
        }

        private void ScreenshotHost_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDrawing) return;
            p2 = e.Location;
            _Frame.Location = new Point(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y));
            _Frame.Size = new Size(Math.Max(p1.X, p2.X) - _Frame.Location.X, Math.Max(p1.Y, p2.Y) - _Frame.Location.Y);

        }

        private void ScreenshotHost_MouseUp(object sender, MouseEventArgs e)
        {
            _ScreenshotHost.DialogResult = DialogResult.OK;
        } 
        #endregion

        public void Dispose()
        {
            _Frame = null;
            _ScreenshotHost?.Dispose();
            _ScreenshotHost = null;
            GC.SuppressFinalize(this);
        }
    }
}
