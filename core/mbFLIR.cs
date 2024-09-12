
/* 

    www.mbnq.pl 2024 
    https://mbnq.pl/
    mbnq00 on gmail

*/

using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace RED.mbnq
{
    public class mbnqFLIR : Form
    {
        private bool isOverlayVisible = false;
        private Random random = new Random();           // rng
        private int red = 192;
        private int green = 192;
        private int blue = 192;
        private Timer repaintTimer;
        public static bool mbEnableFlirLogic = false;    // for general enabling and disabling the flir logic
        public static bool mbEnableFlir = false;        // for dynamic enabling with checkbox 

        public mbnqFLIR()
        {
            // Set the form style to remove borders and make it topmost
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.TopMost = true;

            // Set initial opacity
            this.Opacity = 0.24;  // Adjust this value if needed

            // Disable interaction with the form (makes it click-through)
            this.ShowInTaskbar = false;

            // Start the timer for continuous repaints
            InitializeRepaintTimer();

            // Start the async task for updating the grayscale overlay
            _ = ManageGrayscaleOverlayAsync();  // Main grayscale overlay, updates every 100ms
        }

        // Initialize and start the timer for forcing repaints
        private void InitializeRepaintTimer()
        {
            repaintTimer = new Timer();
            repaintTimer.Interval = (1 + random.Next(6)); // Trigger every 32ms (~30 FPS)
            repaintTimer.Tick += (sender, args) =>
            {
                if (mbEnableFlir)
                {
                    // Randomize the color values (RGB) inside the timer loop
                    red = Clamp(192 + random.Next(-16, 16), 0, 255);   // Vary red by ±10
                    green = Clamp(192 + random.Next(-16, 16), 0, 255); // Vary green by ±15
                    blue = Clamp(192 + random.Next(-16, 16), 0, 255);  // Vary blue by ±10

                    // Randomize opacity between 0.03 and 0.07 for slight variation
                    this.Opacity = 0.2 + (0.01 * random.NextDouble());

                    // Force the form to repaint
                    this.Invalidate(true);
                }
            };
            repaintTimer.Start();
        }

        // Async method to manage grayscale overlay (updates overlay visibility)
        private async Task ManageGrayscaleOverlayAsync()
        {
            while (mbEnableFlirLogic)
            {
                if (mbEnableFlir)
                {
                    // If the overlay is not visible, show it
                    if (!isOverlayVisible)
                    {
                        this.Invoke((Action)(() => this.Show()));
                        isOverlayVisible = true;
                    }
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

                // Sleep for (X)ms before checking again
                await Task.Delay(256);
            }
        }

        // Override the OnPaint method to apply the solid gray FLIR overlay

        /*
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Get the dimensions of the screen
            Rectangle screenRect = this.ClientRectangle;

            // Debugging - ensure OnPaint is called
            // Console.WriteLine("OnPaint called");

            // Fill the rectangle with the dynamically adjusted color
            using (SolidBrush solidGrayBrush = new SolidBrush(Color.FromArgb(red, green, blue)))
            {
                e.Graphics.FillRectangle(solidGrayBrush, screenRect);
            }
        }
        */
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Get the dimensions of the screen
            Rectangle screenRect = this.ClientRectangle;

            // Capture the screen image (or you can use an existing image)
            Bitmap screenImage = CaptureScreenImage();

            // Apply grayscale effect
            Bitmap grayImage = ApplyGrayscale(screenImage);

            // Draw the grayscale image as the background
            e.Graphics.DrawImage(grayImage, screenRect);
        }

        private Bitmap CaptureScreenImage()
        {
            Rectangle primaryScreenBounds = Screen.PrimaryScreen.Bounds;
            Bitmap screenshot = new Bitmap(primaryScreenBounds.Width, primaryScreenBounds.Height);

            using (Graphics g = Graphics.FromImage(screenshot))
            {
                g.CopyFromScreen(primaryScreenBounds.Location, Point.Empty, primaryScreenBounds.Size);
            }

            return screenshot;
        }

        // Apply grayscale effect using a ColorMatrix
        private Bitmap ApplyGrayscale(Bitmap original)
        {
            Bitmap grayscaleImage = new Bitmap(original.Width, original.Height);

            using (Graphics g = Graphics.FromImage(grayscaleImage))
            {
                // Create a color matrix to convert the image to grayscale
                System.Drawing.Imaging.ColorMatrix colorMatrix = new System.Drawing.Imaging.ColorMatrix(
                    new float[][]
                    {
                new float[] { 0.3f, 0.3f, 0.3f, 0, 0 },
                new float[] { 0.59f, 0.59f, 0.59f, 0, 0 },
                new float[] { 0.11f, 0.11f, 0.11f, 0, 0 },
                new float[] { 0, 0, 0, 1, 0 },
                new float[] { 0, 0, 0, 0, 1 }
                    });

                // Create image attributes and set the color matrix
                var imageAttributes = new System.Drawing.Imaging.ImageAttributes();
                imageAttributes.SetColorMatrix(colorMatrix);

                // Draw the original image with the grayscale color matrix applied
                g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height), 0, 0, original.Width, original.Height, GraphicsUnit.Pixel, imageAttributes);
            }

            return grayscaleImage;
        }

        // Helper method to clamp values between a min and max
        public static int Clamp(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
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
