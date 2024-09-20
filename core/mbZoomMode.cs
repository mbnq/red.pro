
/* 

    www.mbnq.pl 2024 
    https://mbnq.pl/
    mbnq00 on gmail

*/

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace RED.mbnq
{
    public class ZoomMode
    {
        /* --- --- ---  --- --- --- */
        #region init
        private static Timer holdTimer;
        private static Timer zoomUpdateTimer;
        private static mbZoomForm zoomForm;
        private static bool isZooming = false;                                          // init only
        private static ControlPanel controlPanel;
        private static Bitmap zoomBitmap;

        public static int zoomDisplaySize = (mbFnc.mGetPrimaryScreenCenter2().Y);       // init only
        public static int zoomMultiplier = 1;                                           // init only
        public static int startInterval = 1000;                                         // init only
        public static int zoomRefreshIntervalInternal = Program.mbFrameDelay;           // init only

        public static bool IsZoomModeEnabled = false;

        private static int centeredX;
        private static int centeredY;

        // PInvoke declarations for BitBlt
        private const int SRCCOPY = 0x00CC0020;

        [DllImport("gdi32.dll")]
        private static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest,
            int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, int dwRop);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        public static void InitializeZoomMode(ControlPanel panel)
        {
            controlPanel = panel;

            holdTimer = new Timer
            {
                Interval = startInterval                                 // Time of holding RMB before showing zoom in milliseconds
            };
            holdTimer.Tick += HoldTimer_Tick;

            zoomUpdateTimer = new Timer
            {
                Interval = zoomRefreshIntervalInternal                  // refresh rate
            };
            zoomUpdateTimer.Tick += ZoomUpdateTimer_Tick;

            int captureSize = zoomDisplaySize / zoomMultiplier;         // could modiffy size here?
            if (captureSize <= 0) captureSize = 1;

            zoomBitmap = new Bitmap(captureSize, captureSize);

            UpdateCenteredCoordinates();
        }
        #endregion
        /* --- --- ---  --- --- --- */

        /* --- --- ---  --- --- --- */
        #region Update fncs

        // Updates the zoom multiplier and adjusts related components.
        public static void UpdateZoomMultiplier(int newZoomMultiplier)
        {
            zoomMultiplier = newZoomMultiplier;

            if (zoomBitmap != null)
            {
                zoomBitmap.Dispose();
            }

            if (zoomMultiplier <= 1) zoomMultiplier = 1;

            int captureSize = zoomDisplaySize / zoomMultiplier;
            if (captureSize <= 0) captureSize = 1;

            zoomBitmap = new Bitmap(captureSize, captureSize);

            if (zoomForm != null)
            {
                // Do not change the size of zoomForm
                // zoomForm.Size = new Size(zoomSizeSet, zoomSizeSet);
                zoomForm.Invalidate(); // Force the form to repaint
            }

            UpdateCenteredCoordinates();
        }
        public static void UpdateStartInterval(int InputStartInterval)
        {
            startInterval = InputStartInterval;

            if (InputStartInterval <= 1)
            {
                InputStartInterval = 1;
            }
        }
        public static void UpdateRefreshInterval(int InputTimeInverval)
        {
            if ( (InputTimeInverval < 1 || InputTimeInverval > 100) || (zoomRefreshIntervalInternal < 1 || zoomRefreshIntervalInternal > 100) ) 
            { 
                zoomRefreshIntervalInternal = Program.mbFrameDelay;  
            }

            zoomRefreshIntervalInternal = InputTimeInverval;
        }

        // Updates the centered coordinates based on the screen center.
        private static void UpdateCenteredCoordinates()
        {
            Point screenCenter = mbFnc.mGetPrimaryScreenCenter2();
            int captureSize = zoomDisplaySize / zoomMultiplier;
            centeredX = screenCenter.X - (captureSize / 2);
            centeredY = screenCenter.Y - (captureSize / 2);
        }

        #endregion
        /* --- --- ---  --- --- --- */

        /* --- --- ---  --- --- --- */
        #region timers
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
                holdTimer.Interval = startInterval;
                holdTimer.Start();
            }
        }
        public static void StopHoldTimer()
        {
            holdTimer.Stop();
        }

        #endregion
        /* --- --- ---  --- --- --- */

        /* --- --- ---  --- --- --- */
        #region Gaphics
        // Handles the Paint event for the zoom form.
        private static void ZoomForm_Paint(object sender, PaintEventArgs e)
        {
            if (controlPanel == null || controlPanel.mbCrosshairOverlay == null) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.None;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = PixelOffsetMode.None;

            // Capture the screen directly into zoomBitmap
            CaptureScreenToBitmap();

            // Draw the zoomed image scaled to fill the zoomForm
            zoomForm.ApplyClipping(g);

            g.DrawImage(zoomBitmap, new Rectangle(0, 0, zoomForm.Width, zoomForm.Height));

            // Draw crosshair lines
            int centerX = zoomForm.Width / 2;
            int centerY = zoomForm.Height / 2;

            using (Pen crosshairPen = new Pen(Color.Black, 2))
            {
                // Vertical line
                g.DrawLine(crosshairPen, centerX, 0, centerX, zoomForm.Height);

                // Horizontal line
                g.DrawLine(crosshairPen, 0, centerY, zoomForm.Width, centerY);
            }

            // Draw border
            using (Pen borderPen = new Pen(Color.Black, 2))
            {
                g.DrawEllipse(borderPen, 0, 0, zoomForm.Width, zoomForm.Height);
            }
        }

        // Captures the screen area into the bitmap using BitBlt for performance.
        private static void CaptureScreenToBitmap()
        {
            int captureSize = zoomDisplaySize / zoomMultiplier;
            if (captureSize <= 0) captureSize = 1;

            // Use BitBlt for faster screen capture
            using (Graphics gDest = Graphics.FromImage(zoomBitmap))
            {
                IntPtr hdcDest = gDest.GetHdc();
                IntPtr hdcSrc = GetDC(IntPtr.Zero);

                BitBlt(hdcDest, 0, 0, captureSize, captureSize, hdcSrc, centeredX, centeredY, SRCCOPY);

                gDest.ReleaseHdc(hdcDest);
                ReleaseDC(IntPtr.Zero, hdcSrc);
            }
        }

        /// Displays the zoom overlay.
        public static void ShowZoomOverlay()
        {
            if (zoomForm == null)
            {
                zoomForm = new mbZoomForm
                {
                    FormBorderStyle = FormBorderStyle.None,
                    Size = new Size(zoomDisplaySize, zoomDisplaySize), // Keep size constant
                    StartPosition = FormStartPosition.Manual,
                    Location = new Point(0, 0),
                    TopMost = true,
                    ShowInTaskbar = false,
                    TransparencyKey = Color.Magenta,
                    BackColor = Color.Black
                };

                zoomForm.Paint += ZoomForm_Paint;
            }

            Rectangle screenBounds = Screen.PrimaryScreen.Bounds;
            zoomForm.Left = (screenBounds.Width - zoomForm.Width);
            zoomForm.Top = (screenBounds.Height - zoomForm.Height);

            zoomForm.Show();
            isZooming = true;
            mTempHideCrosshair(true);
            zoomUpdateTimer.Interval = zoomRefreshIntervalInternal;
            zoomUpdateTimer.Start(); // Start the update timer for real-time zoom
        }

        // Hides the zoom overlay
        public static void HideZoomOverlay()
        {
            if (zoomForm != null && isZooming)
            {
                zoomUpdateTimer.Stop();                     // Stop the update timer when zooming ends
                zoomForm.Hide();
                mTempHideCrosshair(false);
                isZooming = false;
            }
        }

        // Hides crosshair when zoom overlay is active
        private static void mTempHideCrosshair(bool hideCrosshair)
        {
            if (!controlPanel.mbHideCrosshairChecked)
            {
                controlPanel.mHideCrosshair = hideCrosshair;
                controlPanel.UpdateMainCrosshair();
            }
        }

        #endregion
        /* --- --- ---  --- --- --- */

    }

    /* --- --- ---  --- --- --- */
    #region mbZoomForm
    // Custom form for displaying zoom overlay
    public class mbZoomForm : Form
    {
        private GraphicsPath ellipsePath;
        public mbZoomForm()
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint, true);
            this.UpdateStyles();
            this.ApplyCircularRegion();
        }

        // Applies a circular region to the form to create a circular window.
        public void ApplyCircularRegion()
        {
            ellipsePath = new GraphicsPath();
            ellipsePath.AddEllipse(0, 0, this.Width, this.Height);
            this.Region = new Region(ellipsePath);
        }

        // Applies clipping to ensure the drawn content fits within the circular region.
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
    #endregion
    /* --- --- ---  --- --- --- */
}
