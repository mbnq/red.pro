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
                Interval = 1000 // 2 seconds
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
                    Size = new Size(512, 512),
                    StartPosition = FormStartPosition.Manual, // This indicates that you want to set the position manually
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
            zoomForm.Invalidate(); // Trigger the Paint event to draw the zoomed area
        }
        private static void ZoomForm_Paint(object sender, PaintEventArgs e)
        {
            if (controlPanel == null || controlPanel.MainDisplay == null) return;

            // Define the zoom size
            int zoomSize = 256;

            Point centeredPosition = controlPanel.GetCenteredPosition();

            // Draw the zoomed portion of the screen directly using the Graphics object
            Graphics g = e.Graphics;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Capture and draw the area centered at the calculated captureX and captureY
            // g.CopyFromScreen((controlPanel.MainDisplay.Location.X), (controlPanel.MainDisplay.Location.Y), 0, 0, new Size(zoomSize, zoomSize));
            g.CopyFromScreen((centeredPosition.X + (zoomSize) - 30), (centeredPosition.Y + (zoomSize/2)), 0, 0, new Size(zoomSize, zoomSize));
        }


        /*
        private static void ZoomForm_Paint(object sender, PaintEventArgs e)
        {
            if (controlPanel == null || controlPanel.MainDisplay == null) return;

            // Define the zoom size
            int zoomSize = 256;

            // Calculate the center of MainDisplay
            int captureX = controlPanel.MainDisplay.Location.X;
            int captureY = controlPanel.MainDisplay.Location.Y;

            // Draw the zoomed portion of the screen directly using the Graphics object
            Graphics g = e.Graphics;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Capture and draw the area centered at the calculated captureX and captureY
            g.CopyFromScreen(captureX, captureY, 0, 0, new Size(zoomSize, zoomSize));
        }

        */


        /*
                private static void ZoomForm_Paint(object sender, PaintEventArgs e)
                {
                    if (controlPanel == null || controlPanel.MainDisplay == null) return;

                    // Get the current position of the MainDisplay
                    Point mainDisplayPosition = controlPanel.MainDisplay.Location;
                    Rectangle screenBounds = Screen.PrimaryScreen.Bounds;

                    int zoomSize = 256;

                    // Calculate the center of the MainDisplay
                    int centerX = mainDisplayPosition.X;
                    int centerY = mainDisplayPosition.Y;

                    // Adjust captureX and captureY to be the top-left corner of the capture area
                    int captureX = centerX + (zoomSize / 2);
                    int captureY = centerY + (zoomSize / 2) + (screenBounds.Height / zoomSize);

                    // Draw the zoomed portion of the screen directly using the Graphics object
                    Graphics g = e.Graphics;
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                    g.CopyFromScreen(captureX, captureY, 0, 0, new Size(zoomSize, zoomSize));
                }
        */
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
