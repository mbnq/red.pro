using System;
using System.Drawing;
using System.Windows.Forms;

namespace RED.mbnq
{
    public static class ZoomMode
    {
        private static Timer holdTimer;
        private static Form zoomForm;
        private static bool isZooming = false;
        private static ControlPanel controlPanel;

        public static void InitializeZoomMode(ControlPanel panel)
        {
            controlPanel = panel; // Assign the control panel reference

            holdTimer = new Timer
            {
                Interval = 2000 // 2 seconds
            };
            holdTimer.Tick += HoldTimer_Tick;
        }

        private static void HoldTimer_Tick(object sender, EventArgs e)
        {
            holdTimer.Stop();
            ShowZoomOverlay();
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
                zoomForm = new Form
                {
                    FormBorderStyle = FormBorderStyle.None,
                    Size = new Size(256, 256),
                    StartPosition = FormStartPosition.Manual,
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
            zoomForm.Invalidate(); // Trigger the Paint event to draw the zoomed area
        }

        private static void ZoomForm_Paint(object sender, PaintEventArgs e)
        {
            if (controlPanel == null) return;

            // Use GetCenteredPosition to determine the zoom area center
            Point centeredPosition = controlPanel.GetCenteredPosition();
            int zoomSize = 256;

            int captureX = centeredPosition.X - (zoomSize / 2);
            int captureY = centeredPosition.Y - (zoomSize / 2);

            // Draw the zoomed portion of the screen directly using the Graphics object
            Graphics g = e.Graphics;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            g.CopyFromScreen(captureX, captureY, 0, 0, new Size(zoomSize, zoomSize));
        }

        public static void HideZoomOverlay()
        {
            if (zoomForm != null && isZooming)
            {
                zoomForm.Hide();
                isZooming = false;
            }
        }
    }
}
