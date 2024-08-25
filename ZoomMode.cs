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

        public static void InitializeZoomMode()
        {
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
                    Size = new Size(512, 512),
                    StartPosition = FormStartPosition.Manual,
                    TopMost = true,
                    ShowInTaskbar = false,
                    BackgroundImageLayout = ImageLayout.Stretch,
                    TransparencyKey = Color.Magenta,
                    BackColor = Color.Magenta
                };
            }

            // Capture screen
            Rectangle screenBounds = Screen.PrimaryScreen.Bounds;
            int zoomSize = 512;
            int captureX = (screenBounds.Width - zoomSize) / 2;
            int captureY = (screenBounds.Height - zoomSize) / 2;
            Bitmap bmp = new Bitmap(zoomSize, zoomSize);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(captureX, captureY, 0, 0, new Size(zoomSize, zoomSize));
                zoomForm.BackgroundImage = bmp;
            }

            // Position in the bottom-right corner
            zoomForm.Left = screenBounds.Width - zoomSize - 10;
            zoomForm.Top = screenBounds.Height - zoomSize - 10;

            zoomForm.Show();
            isZooming = true;
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
