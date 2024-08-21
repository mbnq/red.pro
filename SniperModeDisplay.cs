using System;
using System.Drawing;
using System.Windows.Forms;

namespace RED.mbnq
{
    public class SniperModeDisplay : Form
    {
        private Timer updateTimer;
        private bool isMoving;
        private Point lastMousePos;

        public SniperModeDisplay()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(100, 100);  // Default size
            this.BackColor = Color.Blue;     // Default color
            this.Opacity = 0.5;              // Default transparency
            this.TopMost = true;
            this.DoubleBuffered = true;
            this.MouseDown += SniperModeDisplay_MouseDown;
            this.MouseMove += SniperModeDisplay_MouseMove;
            this.MouseUp += SniperModeDisplay_MouseUp;
            this.Paint += SniperModeDisplay_Paint;

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

        private void SniperModeDisplay_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.FillRectangle(new SolidBrush(this.BackColor), this.ClientRectangle);
        }

        public void UpdateTimerInterval(int interval)
        {
            updateTimer.Interval = interval;
        }

        private void SniperModeDisplay_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isMoving = true;
                lastMousePos = e.Location;
            }
        }

        private void SniperModeDisplay_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMoving)
            {
                this.Left += e.X - lastMousePos.X;
                this.Top += e.Y - lastMousePos.Y;
            }
        }

        private void SniperModeDisplay_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isMoving = false;
            }
        }
    }
}
