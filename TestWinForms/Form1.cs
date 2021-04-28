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
            Bitmap bmp = host.GetScreenshot();
            pictureBox1.Image = bmp;
        }
    }
}
