
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
        private static bool isZooming = false;                                                              // init only
        private static ControlPanel controlPanel;
        private static Bitmap zoomBitmap;

        public static int zoomScopeSizeInternalDefault = (mbFnc.mGetPrimaryScreenCenter2().Y);
        public static int zoomScopeSizeInternal = zoomScopeSizeInternalDefault;
        public static int zoomMultiplier = 1;                                                               // init only
        public static int startInterval = 1000;                                                             // init only
        public static int zoomRefreshIntervalInternal = Program.mbFrameDelay;                               // init only

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

            int captureSize = (zoomScopeSizeInternal / zoomMultiplier);
            if (captureSize <= 0) captureSize = 1;

            zoomBitmap = new Bitmap(captureSize, captureSize);

            UpdateCenteredCoordinates();
        }
        #endregion
        /* --- --- ---  --- --- --- */

        /* --- --- ---  --- --- --- */
        #region Update fncs

        public static void UpdateZoomMultiplier(int newZoomMultiplier)
        {
            zoomMultiplier = newZoomMultiplier;

            if (zoomBitmap != null)
            {
                zoomBitmap.Dispose();
            }

            if (zoomMultiplier <= 1) zoomMultiplier = 1;

            int captureSize = zoomScopeSizeInternal / zoomMultiplier;
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
            if (InputStartInterval <= 1)
            {
                InputStartInterval = 1;
            }

            startInterval = InputStartInterval;
        }
        public static void UpdateScopeSize(int InputzoomScopeSize)
        {
            if (InputzoomScopeSize <= 1)
            {
                InputzoomScopeSize = 1;
            }

            zoomScopeSizeInternal = zoomScopeSizeInternalDefault + (InputzoomScopeSize * 5);

            // Update the zoom overlay size if it's already displayed
            if (zoomForm != null)
            {
                zoomForm.Size = new Size(zoomScopeSizeInternal, zoomScopeSizeInternal);
                zoomForm.ApplyCircularRegion();         // Reapply circular region if needed
                UpdateCenteredCoordinates();            // Recalculate centered coordinates based on new size
            }

            // Recreate the zoom bitmap with the new size
            if (zoomBitmap != null)
            {
                zoomBitmap.Dispose();
            }

            int captureSize = zoomScopeSizeInternal / zoomMultiplier;
            captureSize = captureSize > 0 ? captureSize : 1;

            zoomBitmap = new Bitmap(captureSize, captureSize);
        }
        public static void UpdateRefreshInterval(int InputTimeInverval)
        {
            if ( (InputTimeInverval < 1 || InputTimeInverval > 100) || (zoomRefreshIntervalInternal < 1 || zoomRefreshIntervalInternal > 100) ) 
            { 
                zoomRefreshIntervalInternal = Program.mbFrameDelay;  
            }

            zoomRefreshIntervalInternal = InputTimeInverval;
        }
        private static void UpdateCenteredCoordinates()
        {
            Point screenCenter = mbFnc.mGetPrimaryScreenCenter2();
            int captureSize = zoomScopeSizeInternal / zoomMultiplier;
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
                zoomForm.Invalidate();      // Forces zoomForm to repaint
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
        private static void ZoomForm_Paint(object sender, PaintEventArgs e)
        {
            if (controlPanel == null || controlPanel.mbCrosshairOverlay == null) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.None;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = PixelOffsetMode.None;


            CaptureScreenToBitmap();
            zoomForm.ApplyClipping(g);              // Draw the zoomed image scaled to fill the zoomForm

            g.DrawImage(zoomBitmap, new Rectangle(0, 0, zoomScopeSizeInternal, zoomScopeSizeInternal));

            // Draw crosshair lines
            int centerX = zoomForm.Width / 2;
            int centerY = zoomForm.Height / 2;

            using (Pen crosshairPen = new Pen(Color.Black, 2))
            {

                g.DrawLine(crosshairPen, centerX, 0, centerX, zoomForm.Height);
                g.DrawLine(crosshairPen, 0, centerY, zoomForm.Width, centerY);
            }

            // Draw border
            using (Pen borderPen = new Pen(Color.Black, 2))
            {
                g.DrawEllipse(borderPen, 0, 0, zoomForm.Width, zoomForm.Height);
            }
        }

        // Captures the screen area into the bitmap using BitBlt for performance
        private static void CaptureScreenToBitmap()
        {
            int captureSize = zoomScopeSizeInternal / zoomMultiplier;
            if (captureSize <= 0) captureSize = 1;

            using (Graphics gDest = Graphics.FromImage(zoomBitmap))
            {
                IntPtr hdcDest = gDest.GetHdc();
                IntPtr hdcSrc = GetDC(IntPtr.Zero);

                BitBlt(hdcDest, 0, 0, captureSize, captureSize, hdcSrc, centeredX, centeredY, SRCCOPY);

                gDest.ReleaseHdc(hdcDest);
                ReleaseDC(IntPtr.Zero, hdcSrc);
            }
        }

        // Displays the zoom overlay.
        public static void ShowZoomOverlay()
        {
            if (zoomForm == null)
            {
                zoomForm = new mbZoomForm
                {
                    FormBorderStyle = FormBorderStyle.None,
                    Size = new Size(zoomScopeSizeInternal, zoomScopeSizeInternal),
                    StartPosition = FormStartPosition.Manual,
                    Location = new Point(0, 0),
                    TopMost = true,
                    ShowInTaskbar = false,
                    TransparencyKey = Color.Magenta,
                    BackColor = Color.Black
                };

                zoomForm.Paint += ZoomForm_Paint;
            }
            else
            {
                // Update the size if the form already exists
                zoomForm.Size = new Size(zoomScopeSizeInternal, zoomScopeSizeInternal);
                zoomForm.ApplyCircularRegion(); // Reapply circular region if needed
            }

            Rectangle screenBounds = Screen.PrimaryScreen.Bounds;
            zoomForm.Left = (screenBounds.Width - zoomForm.Width);
            zoomForm.Top = (screenBounds.Height - zoomForm.Height);

            zoomForm.Show();
            isZooming = true;
            mTempHideCrosshair(true);
            zoomUpdateTimer.Interval = zoomRefreshIntervalInternal;
            zoomUpdateTimer.Start();
        }

        public static void HideZoomOverlay()
        {
            if (zoomForm != null && isZooming)
            {
                zoomUpdateTimer.Stop();
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
