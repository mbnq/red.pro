using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace RED.mbnq
{
    public class ZoomMode
    {
        private static Timer holdTimer;
        private static Timer zoomUpdateTimer;
        private static CustomZoomForm zoomForm;
        private static bool isZooming = false;
        private static ControlPanel controlPanel;
        private static Bitmap zoomBitmap;
        private static int zoomSizeSet = 64;   // Reduced zoom area for performance
        public static int zoomMultiplier = 1;
        public static bool IsZoomModeEnabled = false;

        private static int centeredX;
        private static int centeredY;

        /// <summary>
        /// Updates the zoom multiplier and adjusts related components.
        /// </summary>
        public static void UpdateZoomMultiplier(int newZoomMultiplier)
        {
            zoomMultiplier = newZoomMultiplier;

            if (zoomBitmap != null)
            {
                zoomBitmap.Dispose();
            }

            int bitmapSize = zoomSizeSet * zoomMultiplier;
            zoomBitmap = new Bitmap(bitmapSize, bitmapSize);

            if (zoomForm != null)
            {
                zoomForm.Size = new Size(bitmapSize, bitmapSize);
                zoomForm.ApplyCircularRegion();
                zoomForm.Invalidate(); // Force the form to repaint with the new size
            }

            UpdateCenteredCoordinates();
        }

        /// <summary>
        /// Initializes the zoom mode with the specified control panel.
        /// </summary>
        public static void InitializeZoomMode(ControlPanel panel)
        {
            controlPanel = panel;

            holdTimer = new Timer
            {
                Interval = 500 // Time before showing zoom in milliseconds
            };
            holdTimer.Tick += HoldTimer_Tick;

            // Timer for continuous updates to the zoom display
            zoomUpdateTimer = new Timer
            {
                Interval = 33 // Increased interval to reduce CPU usage
            };
            zoomUpdateTimer.Tick += ZoomUpdateTimer_Tick;

            int bitmapSize = zoomSizeSet * zoomMultiplier;
            zoomBitmap = new Bitmap(bitmapSize, bitmapSize);

            UpdateCenteredCoordinates();
        }

        /// <summary>
        /// Updates the centered coordinates based on the screen center.
        /// </summary>
        private static void UpdateCenteredCoordinates()
        {
            Point screenCenter = mbFnc.mGetPrimaryScreenCenter2();
            centeredX = screenCenter.X - (zoomSizeSet * zoomMultiplier / 2);
            centeredY = screenCenter.Y - (zoomSizeSet * zoomMultiplier / 2);
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
                zoomForm.Invalidate(); // Forces the zoomForm to repaint
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

        /// <summary>
        /// Handles the Paint event for the zoom form.
        /// </summary>
        private static void ZoomForm_Paint(object sender, PaintEventArgs e)
        {
            if (controlPanel == null || controlPanel.mbCrosshairOverlay == null) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.None;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = PixelOffsetMode.None;

            // Capture the screen directly into zoomBitmap
            CaptureScreenToBitmap();

            // Draw the zoomed image
            zoomForm.ApplyClipping(g);
            g.DrawImage(zoomBitmap, new Rectangle(0, 0, zoomForm.Width, zoomForm.Height));

            // Draw border
            using (Pen borderPen = new Pen(Color.Black, 2))
            {
                g.DrawEllipse(borderPen, 0, 0, zoomForm.Width, zoomForm.Height);
            }
        }

        /// <summary>
        /// Captures the screen area into the bitmap using BitBlt for performance.
        /// </summary>
        private static void CaptureScreenToBitmap()
        {
            // Use BitBlt for faster screen capture
            using (Graphics gDest = Graphics.FromImage(zoomBitmap))
            {
                IntPtr hdcDest = gDest.GetHdc();
                IntPtr hdcSrc = GetDC(IntPtr.Zero);

                BitBlt(hdcDest, 0, 0, zoomBitmap.Width, zoomBitmap.Height, hdcSrc, centeredX, centeredY, SRCCOPY);

                gDest.ReleaseHdc(hdcDest);
                ReleaseDC(IntPtr.Zero, hdcSrc);
            }
        }

        /// <summary>
        /// Displays the zoom overlay.
        /// </summary>
        public static void ShowZoomOverlay()
        {
            if (zoomForm == null)
            {
                zoomForm = new CustomZoomForm
                {
                    FormBorderStyle = FormBorderStyle.None,
                    Size = new Size(zoomSizeSet * zoomMultiplier, zoomSizeSet * zoomMultiplier),
                    StartPosition = FormStartPosition.Manual,
                    Location = new Point(0, 0),
                    TopMost = true,
                    ShowInTaskbar = false,
                    TransparencyKey = Color.Magenta,
                    BackColor = Color.Black
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

        /// <summary>
        /// Hides the zoom overlay.
        /// </summary>
        public static void HideZoomOverlay()
        {
            if (zoomForm != null && isZooming)
            {
                zoomUpdateTimer.Stop(); // Stop the update timer when zooming ends
                zoomForm.Hide();
                isZooming = false;
            }
        }

        // PInvoke declarations for BitBlt
        private const int SRCCOPY = 0x00CC0020;

        [DllImport("gdi32.dll")]
        private static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest,
            int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, int dwRop);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
    }

    /// <summary>
    /// Custom form for displaying the zoom overlay.
    /// </summary>
    public class CustomZoomForm : Form
    {
        private GraphicsPath ellipsePath;

        public CustomZoomForm()
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint, true);
            this.UpdateStyles();
            this.ApplyCircularRegion();
        }

        /// <summary>
        /// Applies a circular region to the form to create a circular window.
        /// </summary>
        public void ApplyCircularRegion()
        {
            ellipsePath = new GraphicsPath();
            ellipsePath.AddEllipse(0, 0, this.Width, this.Height);
            this.Region = new Region(ellipsePath);
        }

        /// <summary>
        /// Applies clipping to ensure the drawn content fits within the circular region.
        /// </summary>
        public void ApplyClipping(Graphics g)
        {
            if (ellipsePath != null)
            {
                g.SetClip(ellipsePath);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            ApplyCircularRegion();
        }
    }
}
