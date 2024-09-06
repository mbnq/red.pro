using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace RED.mbnq
{
    public class mbnqFLIR : Form
    {
        private bool isOverlayVisible = false; // Whether the overlay is visible

        public static bool mbEnableFlir = false; // Global variable to control overlay, should be false by default

        public mbnqFLIR()
        {
            // Set the form style to remove borders and make it topmost
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.TopMost = true;

            // Set opacity if desired (e.g., 90% opacity)
            this.Opacity = 0.05;  // Adjust this value if needed

            // Disable interaction with the form (makes it click-through)
            this.ShowInTaskbar = false;

            // Start the async task for updating the grayscale overlay
            _ = ManageGrayscaleOverlayAsync();  // Main grayscale overlay, updates every 100ms
        }

        // Async method to manage grayscale overlay (updates every 100ms)
        private async Task ManageGrayscaleOverlayAsync()
        {
            while (true)
            {
                if (mbEnableFlir)
                {
                    // If the overlay is not visible, show it
                    if (!isOverlayVisible)
                    {
                        this.Invoke((Action)(() => this.Show()));
                        isOverlayVisible = true;
                    }

                    // Redraw the main grayscale overlay
                    this.Invoke((Action)(() => this.Invalidate()));
                }
                else
                {
                    // Hide the overlay if FLIR is disabled
                    if (isOverlayVisible)
                    {
                        this.Invoke((Action)(() => this.Hide()));
                        isOverlayVisible = false;
                    }
                }

                // Sleep for 100ms before updating again
                await Task.Delay(100);
            }
        }

        // Override the OnPaint method to apply the solid gray FLIR overlay
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Get the dimensions of the screen
            Rectangle screenRect = this.ClientRectangle;

            // Fill the rectangle with a medium gray color to simulate FLIR display
            using (SolidBrush solidGrayBrush = new SolidBrush(Color.FromArgb(56, 255, 56))) // Medium gray
            {
                e.Graphics.FillRectangle(solidGrayBrush, screenRect);
            }
        }

        // Ensure form is click-through
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.MakeFormClickThrough();
        }

        // Method to make the form click-through (invisible to mouse clicks)
        private void MakeFormClickThrough()
        {
            const int WS_EX_TRANSPARENT = 0x20;
            const int WS_EX_LAYERED = 0x80000;

            int initialStyle = (int)NativeMethods.GetWindowLong(this.Handle, NativeMethods.GWL_EXSTYLE);
            NativeMethods.SetWindowLong(this.Handle, NativeMethods.GWL_EXSTYLE, initialStyle | WS_EX_TRANSPARENT | WS_EX_LAYERED);
        }

        // Native methods for click-through functionality
        internal static class NativeMethods
        {
            public const int GWL_EXSTYLE = -20;

            [DllImport("user32.dll")]
            public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

            [DllImport("user32.dll")]
            public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        }
    }
}
