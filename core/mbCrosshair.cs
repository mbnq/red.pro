
/* 

    www.mbnq.pl 2024 
    https://mbnq.pl/
    mbnq00 on gmail

*/

using MaterialSkin.Controls;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using static mbFnc;
using System.Linq;
using System.Drawing.Imaging;

namespace RED.mbnq
{
    public class mbCrosshair : Form
    {
        #region Variables and settings
        /* --- --- ---  --- --- --- */

        private Timer crosshairRefreshTimer;
        private Image crosshairPngOverlay;
        private bool isAnimated = true;
        private EventHandler frameChangedHandler;
        private EventHandler applicationIdleHandler;
        private ControlPanel controlPanelInstance;
        private static string mbCrosshairDefaultPath = Directory.GetFiles(ControlPanel.mbUserFilesPath, "RED.custom.*").FirstOrDefault();
        public int mbXhairPaintCount = 0;
        public void SetControlPanelInstance(ControlPanel controlPanel)
        {
            controlPanelInstance = controlPanel;
        }

        /* --- --- ---  --- --- --- */
        #endregion

        #region mbCrosshair
        /* --- --- ---  --- --- --- */

        public mbCrosshair()
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();

            // Defaults
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.MinimumSize = new Size(1, 1);
            this.Size = new Size(ControlPanel.mbPNGMaxWidth, ControlPanel.mbPNGMaxHeight);
            this.BackColor = Color.Red;
            this.Opacity = 0.5;
            this.TopMost = true;
            this.DoubleBuffered = true;
            this.Paint += crosshair_Paint;
            this.ShowInTaskbar = false;
            // this.Enabled = false; 

            // The update timer
            crosshairRefreshTimer = new Timer();
            crosshairRefreshTimer.Interval = ControlPanel.mbCrosshairRedrawTime;

            crosshairRefreshTimer.Tick += (s, e) =>
            {
                this.Invalidate();
                // Debug.WriteLineIf(ControlPanel.mIsDebugOn, $"mbnq: Crosshair redrawn for {mbXhairPaintCount} time(s)");
            };
            crosshairRefreshTimer.Start();
        }

        /* --- --- ---  --- --- --- */
        #endregion

        #region Custom .png
        /* --- --- ---  --- --- --- */

        /* --- --- --- Load --- --- --- */
        public void LoadCustomCrosshairFnc()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = ControlPanel.mbUserFilesPath;
                openFileDialog.Filter = "Image files (*.png;*.gif)|*.png;*.gif";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    string fileName = Path.GetFileName(filePath);

                    // Exclude "RED.custom.png"
                    if ( (fileName.Equals("RED.custom.png", StringComparison.OrdinalIgnoreCase)) || (fileName.Equals("RED.custom.gif", StringComparison.OrdinalIgnoreCase)) )
                    {
                        Sounds.PlayClickSoundOnce();
                        return;
                    }

                    string destinationPath = Path.Combine(ControlPanel.mbUserFilesPath, "RED.custom" + Path.GetExtension(fileName));

                    if (File.Exists(destinationPath))
                    {
                        File.Delete(destinationPath);
                    }

                    File.Copy(filePath, destinationPath);

                    SetCustomPNG();
                }
            }
        }


        /* --- --- --- Set --- --- --- */

        // string crosshairFilePath = Path.Combine(ControlPanel.mbUserFilesPath, "RED.custom.png");
        string crosshairFilePath = mbCrosshairDefaultPath;
        public void SetCustomPNG()
        {
            try
            {
                crosshairFilePath = mbCrosshairDefaultPath;

                if (!string.IsNullOrEmpty(crosshairFilePath) && File.Exists(crosshairFilePath))
                {
                    using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(crosshairFilePath)))
                    {
                        using (var img = Image.FromStream(ms))
                        {
                            if (img.Width <= ControlPanel.mbPNGMaxWidth && img.Height <= ControlPanel.mbPNGMaxHeight)
                            {
                                // Dispose of the existing overlay if it exists
                                crosshairPngOverlay?.Dispose();
                                crosshairPngOverlay = new Bitmap(img);

                                // Determine if image is animated
                                FrameDimension dimension = new FrameDimension(img.FrameDimensionsList[0]);
                                int frameCount = img.GetFrameCount(dimension);

                                if (frameCount > 1)
                                {
                                    isAnimated = true;

                                    // Initialize event handlers
                                    frameChangedHandler = new EventHandler(OnFrameChanged);
                                    applicationIdleHandler = new EventHandler(OnApplicationIdle);

                                    ImageAnimator.Animate(crosshairPngOverlay, frameChangedHandler);
                                    Application.Idle += applicationIdleHandler;
                                }
                                else
                                {
                                    isAnimated = false;
                                }

                                ControlPanel.mbImageARatio = (double)img.Width / img.Height;
                                this.Invalidate();
                                Debug.WriteLineIf(ControlPanel.mbIsDebugOn, "mbnq: Custom overlay successfully loaded.");
                            }
                            else
                            {
                                MaterialMessageBox.Show("The custom overlay image has incorrect dimensions.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.None);
                                Sounds.PlayClickSoundOnce();
                                File.Delete(crosshairFilePath);
                                crosshairPngOverlay = null;
                                Debug.WriteLineIf(ControlPanel.mbIsDebugOn, "mbnq: Custom overlay failed to load: Invalid dimensions.");
                                ControlPanel.mbImageARatio = 1.00f;
                            }
                        }
                    }
                }
                else
                {
                    Sounds.PlayClickSoundOnce();
                    crosshairPngOverlay = null;
                    Debug.WriteLineIf(ControlPanel.mbIsDebugOn, "mbnq: Custom overlay file does not exist.");
                }
            }
            catch (Exception ex)
            {
                MaterialMessageBox.Show($"Failed to load the custom overlay: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.None);
                Debug.WriteLineIf(ControlPanel.mbIsDebugOn, $"mbnq: Exception occurred while loading custom overlay: {ex.Message}");
                crosshairPngOverlay = null;
                Sounds.PlayClickSoundOnce();
            }

            // Refresh the display
            this.Invalidate();
            controlPanelInstance?.UpdateAllUI();
        }
        private void OnApplicationIdle(object sender, EventArgs e)
        {
            if (isAnimated && crosshairPngOverlay != null)
            {
                ImageAnimator.UpdateFrames(crosshairPngOverlay);
            }
        }
        private void OnFrameChanged(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        /* --- --- --- Apply --- --- --- */
        public void ApplyCustomCrosshairFnc()
        {
            var customFilePath = mbCrosshairDefaultPath;
            if (File.Exists(customFilePath))
            {
                try
                {
                    using (var img = Image.FromFile(customFilePath))
                    {
                        if (img.Width <= ControlPanel.mbPNGMaxWidth && img.Height <= ControlPanel.mbPNGMaxHeight)
                        {
                            ControlPanel.mbCrosshairDisplay.SetCustomPNG();
                        }
                        else
                        {
                            MaterialMessageBox.Show($"Maximum allowed .png dimensions are {ControlPanel.mbPNGMaxHeight}x{ControlPanel.mbPNGMaxWidth} pixels.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.None);
                            Sounds.PlayClickSoundOnce();
                            File.Delete(customFilePath);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MaterialMessageBox.Show($"Failed to load the custom crosshair: {ex.Message}", "Error!", MessageBoxButtons.OK, MessageBoxIcon.None);
                    Sounds.PlayClickSoundOnce();
                }
            }
        }
        public void RemoveCustomCrosshairFnc()
        {
            string customFilePath = mbCrosshairDefaultPath;
            if (!string.IsNullOrEmpty(customFilePath) && File.Exists(customFilePath))
            {
                // Calculate hash of the current .png file
                string currentFileHash = CalculateFileHash(customFilePath);

                // Check for existing in backup files with same hash
                var backupFiles = Directory.GetFiles(ControlPanel.mbUserFilesPath, "old.*.custom.png");

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
                    string backupFilePath = Path.Combine(ControlPanel.mbUserFilesPath, backupFileName);
                    File.Move(customFilePath, backupFilePath);
                }
                else
                {
                    File.Delete(customFilePath);
                }
            }

            // Dispose of the overlay
            crosshairPngOverlay?.Dispose();
            crosshairPngOverlay = null;

            // Refresh the crosshair
            this.Invalidate();
        }

        /* --- --- --- Dispose --- --- --- */

        // ensure the custom overlay image is properly disposed
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (isAnimated && crosshairPngOverlay != null)
                {
                    ImageAnimator.StopAnimate(crosshairPngOverlay, frameChangedHandler);
                    Application.Idle -= applicationIdleHandler;
                }
                crosshairPngOverlay?.Dispose();
                Cursor.Show();
            }
            base.Dispose(disposing);
        }


        public bool HasCustomOverlay
        {
            get { return crosshairPngOverlay != null; }
        }

        /* --- --- ---  --- --- --- */
        #endregion

        #region drawn painted overlay crosshair
        /* --- --- ---  --- --- --- */

        // draw overlay
        private void crosshair_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Set graphics options for better quality rendering
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;   // HighQualityBicubic or Bicubic or Bilinear or NearestNeighbor or Default or HighQualityBilinear
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;                     // AntiAlias or HighQuality or HighSpeed or None or Default
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;               // HighQuality or HighSpeed or Hlaf or None
            g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;                // SourceOver or SourceCopy
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;         // HighQuality or HighSpeed or AssumeLinear or Default
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;           // AntiAlias or ClearTypeGridFit or SingleBitPerPixelGridFit or SingleBitPerPixel or SystemDefault

            if (crosshairPngOverlay != null)
            {
                if (isAnimated)
                {
                    // ImageAnimator.UpdateFrames(crosshairPngOverlay);
                }

                g.DrawImage(crosshairPngOverlay, 0, 0, this.ClientSize.Width, this.ClientSize.Height);
                mbXhairPaintCount++;
            }
            else
            {
                // Draw default crosshair
                g.FillRectangle(new SolidBrush(this.BackColor), this.ClientRectangle);
                mbXhairPaintCount++;
            }
        }

        /* --- --- ---  --- --- --- */
        #endregion

        #region nonclickable
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
        #endregion
    }
}
