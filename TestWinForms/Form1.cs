using ScreenshotCreator;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace TestWinForms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using ScreenshotHost host = new();
            Hide();
            Bitmap bmp = host.GetScreenshot();
            pictureBox1.Image = bmp;
            Show();
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            using ScreenshotHost host = new();
            WindowState = FormWindowState.Minimized;
            Bitmap bmp = await host.GetScreenshotAsync();
            pictureBox1.Image = bmp;
            WindowState = FormWindowState.Normal;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using ScreenshotHost host = new();
            pictureBox1.Image = host.GetFullscreenScreenshot();

        }

        private async void button4_Click(object sender, EventArgs e)
        {
            using ScreenshotHost host = new();
            pictureBox1.Image = await host.GetFullscreenScreenshotAsync();

        }
    }
}
