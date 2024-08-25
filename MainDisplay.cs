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
            this.Size = new Size(ControlPanel.mPNGMaxWidth, ControlPanel.mPNGMaxHeight);
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

        public void SetCustomOverlay()
        {
            string filePath = Path.Combine(SaveLoad.SettingsDirectory, "RED.custom.png");

            try
            {
                if (File.Exists(filePath))
                {
                    using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(filePath)))
                    {
                        using (var img = Image.FromStream(ms))
                        {
                            if (img.Width <= ControlPanel.mPNGMaxWidth && img.Height <= ControlPanel.mPNGMaxHeight && img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Png))
                            {
                                // Dispose of the existing overlay if it exists
                                customOverlay?.Dispose();
                                customOverlay = new Bitmap(img);
                                this.Invalidate();  // Force the control to redraw with the new image

                                Console.WriteLine("Custom overlay successfully loaded.");
                            }
                            else
                            {
                                MessageBox.Show("The custom overlay image exceeds the maximum allowed dimensions or is not a valid PNG.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                File.Delete(filePath);
                                customOverlay = null;
                                Console.WriteLine("Custom overlay failed to load: Invalid dimensions or format.");
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("The specified custom overlay file does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    customOverlay = null;
                    Console.WriteLine("Custom overlay file does not exist.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load the custom overlay: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                customOverlay = null;
                Console.WriteLine($"Exception occurred while loading custom overlay: {ex.Message}");
            }

            // Refresh the display
            this.Invalidate();
        }
        public void RemoveCustomOverlay()
        {
            string customFilePath = Path.Combine(SaveLoad.SettingsDirectory, "RED.custom.png");
            if (File.Exists(customFilePath))
            {
                string backupFileName = $"old.{DateTime.Now:yyyyMMddHHmmss}.custom.png";
                string backupFilePath = Path.Combine(SaveLoad.SettingsDirectory, backupFileName);
                File.Move(customFilePath, backupFilePath);

                // Dispose of the overlay
                customOverlay?.Dispose();
                customOverlay = null;

                // Refresh the display
                this.Invalidate();
            }
        }
        private void MainDisplay_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            if (customOverlay != null)
            {
                Console.WriteLine("Drawing custom overlay.");
                // Draw the image at the top-left corner
                g.DrawImage(customOverlay, 0, 0, this.ClientSize.Width, this.ClientSize.Height);
            }
            else
            {
                Console.WriteLine("Custom overlay is null, drawing fallback rectangle.");
                DrawFallbackRectangle(g);
            }
        }
        private void DrawFallbackRectangle(Graphics g)
        {
            g.FillRectangle(new SolidBrush(this.BackColor), this.ClientRectangle); // Default red fill as a fallback
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
        public bool HasCustomOverlay
        {
            get { return customOverlay != null; }
        }
    }
}
