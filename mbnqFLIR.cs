using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace RED.mbnq
{
    public class mbnqFLIR : Form
    {
        public static bool mbEnableFlir = false; // Global variable to control overlay, should be false by default
                                                // Initialize and show fullscreen overlay (mbnqFLIR)
        public mbnqFLIR()
        {
            // Set the form style to remove borders and make it topmost
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.TopMost = true;

            // Set transparency settings
            this.BackColor = Color.LightGray; // Temporary background to be replaced
            // this.TransparencyKey = Color.Gray; // Make the temporary background color transparent

            // Set opacity if desired (e.g., 70% opacity)
            this.Opacity = 0.7;

            // Disable interaction with the form (makes it click-through)
            this.ShowInTaskbar = false;
        }

        // Override the OnPaint method to apply the filter
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Apply a color filter using a semi-transparent brush
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(128, Color.Blue))) // Adjust color here
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle); // Fill the entire form
            }
        }

        // Async function to handle enabling and disabling the overlay
        public async Task ManageOverlayAsync()
        {
            while (true)
            {
                if (mbEnableFlir)
                {
                    // If the overlay is not visible, show it
                    if (!this.Visible)
                    {
                        this.Invoke((Action)(() => this.Show()));
                    }
                }
                else
                {
                    // If mbEnableFlir is false, hide the overlay
                    if (this.Visible)
                    {
                        this.Invoke((Action)(() => this.Hide()));
                    }
                }

                // Sleep for a short time before checking again
                await Task.Delay(500); // Poll every 500ms
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
