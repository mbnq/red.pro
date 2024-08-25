/* 
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

            // Setup the update timer
            updateTimer = new Timer();
            updateTimer.Interval = 1000;
            updateTimer.Tick += (s, e) => this.Invalidate();
            updateTimer.Start();
        }

        public void SetCustomOverlay(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(filePath)))
                    {
                        using (var img = Image.FromStream(ms))
                        {
                            // Perform additional checks on the image to ensure it is valid
                            if (img.Width <= 128 && img.Height <= 128 && img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Png))
                            {
                                // Dispose of any existing overlay
                                customOverlay?.Dispose();
                                customOverlay = new Bitmap(img);
                            }
                            else
                            {
                                MessageBox.Show("The custom overlay image exceeds the maximum allowed dimensions or is not a valid PNG.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                File.Delete(filePath);
                                customOverlay = null;
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("The specified custom overlay file does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load the custom overlay: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                customOverlay = null;
            }
        }

        private void MainDisplay_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            if (customOverlay != null)
            {
                try
                {
                    if (customOverlay.Width > 0 && customOverlay.Height > 0)
                    {
                        g.DrawImage(customOverlay, 0, 0, this.ClientSize.Width, this.ClientSize.Height);
                    }
                    else
                    {
                        FallbackDrawing(g);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while drawing the custom overlay: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    FallbackDrawing(g);
                }
            }
            else
            {
                FallbackDrawing(g);
            }
        }

        private void FallbackDrawing(Graphics g)
        {
            g.FillRectangle(Brushes.Red, this.ClientRectangle); // Default red fill as a fallback
        }

        // Dispose method to ensure the custom overlay image is properly disposed
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
