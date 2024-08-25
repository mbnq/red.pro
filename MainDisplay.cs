﻿/* 
    www.mbnq.pl 2024 
    mbnq00 on gmail

    This is the overlay reddot crosshair
*/

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace RED.mbnq
{
    public class MainDisplay : Form
    {
        private Timer updateTimer;
        private Image customOverlay;

        public MainDisplay()
        {
            // Defaults
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

            updateTimer = new Timer();
            updateTimer.Interval = 1000;
            updateTimer.Tick += (s, e) => this.Invalidate();
            updateTimer.Start();

            var customFilePath = Path.Combine(SaveLoad.SettingsDirectory, "RED.custom.png");
            if (File.Exists(customFilePath))
            {
                customOverlay = Image.FromFile(customFilePath);
            }
        }

        private void MainDisplay_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            if (customOverlay != null)
            {
                g.DrawImage(customOverlay, 0, 0, this.ClientSize.Width, this.ClientSize.Height);
            }
            else
            {
                g.FillRectangle(new SolidBrush(this.BackColor), this.ClientRectangle);
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                customOverlay?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
