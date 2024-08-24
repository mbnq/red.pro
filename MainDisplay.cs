/* 
    www.mbnq.pl 2024 
    mbnq00 on gmail
*/

using System;
using System.Drawing;
using System.Windows.Forms;

namespace RED.mbnq
{
    public class MainDisplay : Form
    {
        private Timer updateTimer;
        public MainDisplay()
        {
            // defaults
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.MinimumSize = new Size(1, 1);
            this.Size = new Size(4, 4);
            this.BackColor = Color.Red;
            this.Opacity = 0.5;
            this.TopMost = true;
            this.DoubleBuffered = true;
            this.Paint += MainDisplay_Paint;
            this.ShowInTaskbar = false;
            // this.Text = "RED.+ (mbnq.pl)";
            // this.Icon = new Icon("mbnqIcon.ico");

            updateTimer = new Timer();
            updateTimer.Interval = 1000;
            updateTimer.Tick += (s, e) => this.Invalidate();
            updateTimer.Start();
        }
        private void MainDisplay_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.FillRectangle(new SolidBrush(this.BackColor), this.ClientRectangle);
        }
    }
}
