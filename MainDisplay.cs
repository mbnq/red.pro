using System;
using System.Drawing;
using System.Windows.Forms;

namespace RED.mbnq
{
    public class MainDisplay : Form
    {
        private Timer updateTimer;
        private bool isLocked = false;

        public MainDisplay()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;  // Set to Manual to control exact positioning
            this.Size = new Size(200, 200);  // Default size
            this.BackColor = Color.Red;      // Default color
            this.Opacity = 0.5;              // Default transparency
            this.TopMost = true;
            this.DoubleBuffered = true;
            this.Paint += MainDisplay_Paint;

            // Center the form on the primary screen
            Rectangle screen = Screen.PrimaryScreen.WorkingArea;
            this.Location = new Point((screen.Width - this.Width) / 2, (screen.Height - this.Height) / 2);

            updateTimer = new Timer();
            updateTimer.Interval = 1000;  // Default interval
            updateTimer.Tick += (s, e) => this.Invalidate();
            updateTimer.Start();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.FillRectangle(new SolidBrush(this.BackColor), this.ClientRectangle);
        }

        private void MainDisplay_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.FillRectangle(new SolidBrush(this.BackColor), this.ClientRectangle);
        }

        public void UpdateTimerInterval(int interval)
        {
            updateTimer.Interval = interval;
        }

        public void LockDisplay(bool lockIt)
        {
            isLocked = lockIt;
            this.Enabled = !isLocked;
        }
    }
}
