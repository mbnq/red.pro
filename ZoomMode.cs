using System;
using System.Drawing;
using System.Windows.Forms;

namespace RED.mbnq
{
    public static class ZoomMode
    {
        private static Timer holdTimer;
        private static Timer zoomUpdateTimer;
        private static CustomZoomForm zoomForm;
        private static bool isZooming = false;
        private static ControlPanel controlPanel;
        private static Bitmap zoomBitmap;

        public static void InitializeZoomMode(ControlPanel panel)
        {
            controlPanel = panel; // Assign the control panel reference

            holdTimer = new Timer
            {
                Interval = 1000 // 1 second delay before showing zoom
            };
            holdTimer.Tick += HoldTimer_Tick;

            // Timer for continuous updates to the zoom display
            zoomUpdateTimer = new Timer
            {
                Interval = 16 // Approximately 60fps
            };
            zoomUpdateTimer.Tick += ZoomUpdateTimer_Tick;

            // Initialize the bitmap to be reused
            int zoomSize = 256;
            zoomBitmap = new Bitmap(zoomSize, zoomSize);
        }

        private static void HoldTimer_Tick(object sender, EventArgs e)
        {
            holdTimer.Stop();
            ShowZoomOverlay();
        }

        private static void ZoomUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (zoomForm != null)
            {
                zoomForm.Invalidate(); // Forces the zoomForm to repaint, which triggers ZoomForm_Paint
            }
        }

        public static void StartHoldTimer()
        {
            if (!isZooming)
            {
                holdTimer.Start();
            }
        }

        public static void StopHoldTimer()
        {
            holdTimer.Stop();
        }

        private static void ShowZoomOverlay()
        {
            if (zoomForm == null)
            {
                zoomForm = new CustomZoomForm
                {
                    FormBorderStyle = FormBorderStyle.None,
                    Size = new Size(512, 512),
                    StartPosition = FormStartPosition.Manual, // Set the position manually
                    Location = new Point(controlPanel.MainDisplay.Location.X, controlPanel.MainDisplay.Location.Y), // Set the location manually using a Point
                    TopMost = true,
                    ShowInTaskbar = false,
                    TransparencyKey = Color.Magenta,
                    BackColor = Color.Magenta
                };

                zoomForm.Paint += ZoomForm_Paint;
            }

            // Position the zoomForm in the bottom-right corner
            Rectangle screenBounds = Screen.PrimaryScreen.Bounds;
            zoomForm.Left = screenBounds.Width - zoomForm.Width - 10;
            zoomForm.Top = screenBounds.Height - zoomForm.Height - 10;

            zoomForm.Show();
            isZooming = true;
            zoomUpdateTimer.Start(); // Start the update timer for real-time zoom
        }

        private static void ZoomForm_Paint(object sender, PaintEventArgs e)
        {
            if (controlPanel == null || controlPanel.MainDisplay == null) return;

            // Get the current graphics context
            Graphics g = e.Graphics;

            // Set high-quality rendering options
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

            // Define the zoom area to capture
            int zoomSize = 256;
            Point centeredPosition = controlPanel.GetCenteredPosition();
            Rectangle captureRect = new Rectangle(centeredPosition.X - (zoomSize / 2), centeredPosition.Y - (zoomSize / 2), zoomSize, zoomSize);

            // Reuse the bitmap to capture the screen area
            using (Graphics captureGraphics = Graphics.FromImage(zoomBitmap))
            {
                // Copy the screen area into the bitmap
                captureGraphics.CopyFromScreen(captureRect.Location, Point.Empty, captureRect.Size);
            }

            // Define the destination rectangle that represents the entire zoomForm
            Rectangle destRect = new Rectangle(0, 0, zoomForm.Width, zoomForm.Height);

            // Draw the captured bitmap, stretched to fill the zoomForm
            g.DrawImage(zoomBitmap, destRect);
        }

        public static void HideZoomOverlay()
        {
            if (zoomForm != null && isZooming)
            {
                zoomForm.Hide();
                isZooming = false;
                zoomUpdateTimer.Stop(); // Stop the update timer when zooming ends
            }
        }
    }

    // Define CustomZoomForm class here
    public class CustomZoomForm : Form
    {
        public CustomZoomForm()
        {
            // Set the control styles to reduce flickering
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();
        }
    }
}
