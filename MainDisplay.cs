/* 
    www.mbnq.pl 2024 
    mbnq00 on gmail

    This is the overlay reddot crosshair
*/

using MaterialSkin.Controls;
using System;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
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

            // The update timer
            updateTimer = new Timer();
            updateTimer.Interval = 1000;
            updateTimer.Tick += (s, e) => this.Invalidate();
            updateTimer.Start();
        }

        string filePath = Path.Combine(SaveLoad.SettingsDirectory, "RED.custom.png");
        public void SetCustomOverlay()
        {

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
                                this.Invalidate();

                                if (ControlPanel.mIsDebugOn) {Console.WriteLine("Custom overlay successfully loaded.");}
                            }
                            else
                            {
                                MaterialMessageBox.Show("The custom overlay .png file has incorrect format.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.None);
                                Sounds.PlayClickSoundOnce();
                                File.Delete(filePath);
                                customOverlay = null;
                                if (ControlPanel.mIsDebugOn) { Console.WriteLine("Custom overlay failed to load: Invalid dimensions or format."); }
                            }
                        }
                    }
                }
                else
                {
                    // MaterialMessageBox.Show("The specified custom overlay .png file does not exist.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.None);
                    Sounds.PlayClickSoundOnce();
                    customOverlay = null;
                    if (ControlPanel.mIsDebugOn) { Console.WriteLine("Custom overlay file does not exist."); }
                }
            }
            catch (Exception ex)
            {
                MaterialMessageBox.Show($"Failed to load the custom overlay: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.None);
                Sounds.PlayClickSoundOnce();
                customOverlay = null;
                if (ControlPanel.mIsDebugOn) { Console.WriteLine($"Exception occurred while loading custom overlay: {ex.Message}"); }
            }

            // Refresh the display
            this.Invalidate();
        }
        public void RemoveCustomOverlay()
        {
            string customFilePath = Path.Combine(SaveLoad.SettingsDirectory, "RED.custom.png");
            if (File.Exists(customFilePath))
            {
                // Calculate hash of the current .png file
                string currentFileHash = CalculateFileHash(customFilePath);

                // Check for existing in backup files with same hash
                var backupFiles = Directory.GetFiles(SaveLoad.SettingsDirectory, "old.*.custom.png");

                bool shouldCreateBackup = true;

                foreach (var backupFile in backupFiles)
                {
                    string backupFileHash = CalculateFileHash(backupFile);
                    if (currentFileHash == backupFileHash)
                    {
                        shouldCreateBackup = false;
                        break;
                    }
                }

                if (shouldCreateBackup)
                {
                    string backupFileName = $"old.{DateTime.Now:yyyyMMddHHmmss}.custom.png";
                    string backupFilePath = Path.Combine(SaveLoad.SettingsDirectory, backupFileName);
                    File.Move(customFilePath, backupFilePath);
                } 
                else
                {
                    File.Delete(customFilePath);
                }

                // Dispose of the overlay
                customOverlay?.Dispose();
                customOverlay = null;

                // Refresh the display
                this.Invalidate();
            }
        }

        // calculate file hash
        private string CalculateFileHash(string filePath)
        {
            using (var sha256 = SHA256.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    byte[] hash = sha256.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        // draw overlay
        private void MainDisplay_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Set graphics options for better quality rendering
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;   // HighQualityBicubic or Bicubic or Bilinear or NearestNeighbor or Default or HighQualityBilinear
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;                   // AntiAlias or HighQuality or HighSpeed or None or Default
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;               // HighQuality or HighSpeed or Hlaf or None
            g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;                // SourceOver or SourceCopy
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;         // HighQuality or HighSpeed or AssumeLinear or Default
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;           // AntiAlias or ClearTypeGridFit or SingleBitPerPixelGridFit or SingleBitPerPixel or SystemDefault

            if (customOverlay != null)
            {
                if (ControlPanel.mIsDebugOn) { Console.WriteLine("Drawing custom overlay."); }
                g.DrawImage(customOverlay, 0, 0, this.ClientSize.Width, this.ClientSize.Height);
            }
            else
            {
                if (ControlPanel.mIsDebugOn) { Console.WriteLine("Custom overlay is null, drawing fallback rectangle."); }
                DrawFallbackRectangle(g);
            }

            this.Show();
        }
        private void DrawFallbackRectangle(Graphics g)
        {
            g.FillRectangle(new SolidBrush(this.BackColor), this.ClientRectangle);
        }

        // ensure the custom overlay image is properly disposed
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
