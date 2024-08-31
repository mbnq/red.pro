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
using System.Diagnostics;

namespace RED.mbnq
{
    public class mbnqCrosshair : Form
    {
        private Timer updateTimer;
        private Image crosshairOverlay;

        public mbnqCrosshair()
        {
            this.DoubleBuffered = true; // Enforce double buffering
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();

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
            // this.Enabled = false;            // Comment this out if the overlay should still display correctly without disabling the form entirely

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
                                crosshairOverlay?.Dispose();
                                crosshairOverlay = new Bitmap(img);
                                this.Invalidate();
                                Debug.WriteLineIf(ControlPanel.mIsDebugOn, "mbnq: Custom overlay successfully loaded.");
                            }
                            else
                            {
                                MaterialMessageBox.Show("The custom overlay .png file has incorrect format.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.None);
                                Sounds.PlayClickSoundOnce();
                                File.Delete(filePath);
                                crosshairOverlay = null;
                                Debug.WriteLineIf(ControlPanel.mIsDebugOn, "mbnq: Custom overlay failed to load: Invalid dimensions or format.");
                            }
                        }
                    }
                }
                else
                {
                    // MaterialMessageBox.Show("The specified custom overlay .png file does not exist.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.None);
                    Sounds.PlayClickSoundOnce();
                    crosshairOverlay = null;
                    Debug.WriteLineIf(ControlPanel.mIsDebugOn, "mbnq: Custom overlay file does not exist.");
                }
            }
            catch (Exception ex)
            {
                MaterialMessageBox.Show($"Failed to load the custom overlay: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.None);
                Sounds.PlayClickSoundOnce();
                crosshairOverlay = null;
                if (ControlPanel.mIsDebugOn) { Console.WriteLine($"Exception occurred while loading custom overlay: {ex.Message}"); }
                Debug.WriteLineIf(ControlPanel.mIsDebugOn, $"mbnq: Exception occurred while loading custom overlay: {ex.Message}");
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
                string currentFileHash = mbFnc.CalculateFileHash(customFilePath);

                // Check for existing in backup files with same hash
                var backupFiles = Directory.GetFiles(SaveLoad.SettingsDirectory, "old.*.custom.png");

                bool shouldCreateBackup = true;

                foreach (var backupFile in backupFiles)
                {
                    string backupFileHash = mbFnc.CalculateFileHash(backupFile);
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
                crosshairOverlay?.Dispose();
                crosshairOverlay = null;

                // Refresh the display
                this.Invalidate();
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

            if (crosshairOverlay != null)
            {
                // Debug.WriteLineIf(ControlPanel.mIsDebugOn, "mbnq: Drawing custom overlay.");
                g.DrawImage(crosshairOverlay, 0, 0, this.ClientSize.Width, this.ClientSize.Height);
            }
            else
            {
                // Debug.WriteLineIf(ControlPanel.mIsDebugOn, "mbnq: Custom overlay is null, drawing fallback rectangle.");
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
                crosshairOverlay?.Dispose();
                Cursor.Show();
            }
            base.Dispose(disposing);
        }
        public bool HasCustomOverlay
        {
            get { return crosshairOverlay != null; }
        }

        /* --- --- --- Let's sure overlay is non-clickable --- --- --- */
        protected override void WndProc(ref Message m)
        {
            const int WM_NCHITTEST = 0x84;
            const int HTTRANSPARENT = -1;

            if (m.Msg == WM_NCHITTEST)
            {
                m.Result = (IntPtr)HTTRANSPARENT; // Make the overlay non-clickable
                return;
            }

            base.WndProc(ref m);
        }
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x20; // WS_EX_TRANSPARENT - Makes the form transparent to mouse events
                cp.ExStyle |= 0x80; // WS_EX_NOACTIVATE - Prevents the form from becoming the foreground window
                return cp;
            }
        }

        /* --- --- ---  --- --- --- */
    }
}
