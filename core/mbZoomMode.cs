
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
using System.Threading.Tasks;
using static mbFnc;
// using System.Security.Cryptography;

namespace RED.mbnq
{
    public class ZoomMode
    {
        /* --- --- ---  --- --- --- */
        #region init
        private static Timer holdTimer;
        private static System.Timers.Timer zoomUpdateTimer;
        private static mbZoomForm zoomForm;
        private static bool isZooming = false;                                                              // init only
        private static ControlPanel controlPanel;
        private static Bitmap zoomBitmap;

        public static int zoomScopeSizeInternalDefault = (mGetPrimaryScreenCenter2().Y);
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

            zoomUpdateTimer = new System.Timers.Timer(zoomRefreshIntervalInternal);
            zoomUpdateTimer.Elapsed += ZoomUpdateTimer_Tick;
            zoomUpdateTimer.AutoReset = true;

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
            Point screenCenter = mGetPrimaryScreenCenter2();
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
        private static void ZoomUpdateTimer_TickOld(object sender, EventArgs e)
        {
            if (zoomForm != null)
            {
                zoomForm.Invalidate();      // Forces zoomForm to repaint
            }
        }

        private static void ZoomUpdateTimer_Tick(object sender, EventArgs e)
        {
            // offload the zoom update process to a background thread
            Task.Run(() =>
            {
                CaptureScreenToBitmap();

                // back to the UI thread to update the zoomForm
                zoomForm?.Invoke(new Action(() =>
                {
                    if (zoomForm != null)zoomForm.Invalidate();
                }));
            });
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
        private static void ZoomForm_PaintOld(object sender, PaintEventArgs e)
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
        private static void ZoomForm_Paint(object sender, PaintEventArgs e)
        {
            if (controlPanel == null || controlPanel.mbCrosshairOverlay == null || zoomBitmap == null) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.HighSpeed;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = PixelOffsetMode.None;

            // Simply draw the preprocessed bitmap (no heavy processing here)
            g.DrawImage(zoomBitmap, new Rectangle(0, 0, zoomScopeSizeInternal, zoomScopeSizeInternal));

            // Draw crosshair lines and border (this should be fast)
            int centerX = zoomForm.Width / 2;
            int centerY = zoomForm.Height / 2;

            using (Pen crosshairPen = new Pen(Color.Black, 2))
            {
                g.DrawLine(crosshairPen, centerX, 0, centerX, zoomForm.Height);
                g.DrawLine(crosshairPen, 0, centerY, zoomForm.Width, centerY);
            }

            using (Pen borderPen = new Pen(Color.Black, 2))
            {
                g.DrawEllipse(borderPen, 0, 0, zoomForm.Width, zoomForm.Height);
            }
        }

        // Captures the screen area into the bitmap using BitBlt for performance
        private static void CaptureScreenToBitmapOld()
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
        private static void CaptureScreenToBitmap()
        {
            int captureSize = zoomScopeSizeInternal / zoomMultiplier;
            if (captureSize <= 0) captureSize = 1;

            IntPtr hdcSrc = IntPtr.Zero;
            IntPtr hdcDest = IntPtr.Zero;

            using (Graphics gDest = Graphics.FromImage(zoomBitmap))
            {
                try
                {
                    hdcDest = gDest.GetHdc();
                    hdcSrc = GetDC(IntPtr.Zero);

                    // screen capture
                    BitBlt(hdcDest, 0, 0, captureSize, captureSize, hdcSrc, centeredX, centeredY, SRCCOPY);
                }
                finally
                {
                    if (hdcDest != IntPtr.Zero)
                    {
                        // release device context for destination graphics
                        gDest.ReleaseHdc(hdcDest);
                    }

                    if (hdcSrc != IntPtr.Zero)
                    {
                        // release source device context
                        ReleaseDC(IntPtr.Zero, hdcSrc);
                    }
                }
            }
        }

        // displays the zoom overlay
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
                // update the size if the form already exists
                zoomForm.Size = new Size(zoomScopeSizeInternal, zoomScopeSizeInternal);
                zoomForm.ApplyCircularRegion(); // reapply circular region if needed
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
            if (!controlPanel.mbHideCrosshairCheckBox.Checked)
            {
                controlPanel.mbHideCrosshair = hideCrosshair;
                controlPanel.UpdateMainCrosshair();
            }
        }

        #endregion
        /* --- --- ---  --- --- --- */

    }

    /* --- --- ---  --- --- --- */
    #region mbZoomForm
    // custom form for displaying zoom overlay
    public class mbZoomForm : Form
    {
        private GraphicsPath ellipsePath;
        public mbZoomForm()
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            this.UpdateStyles();
            this.ApplyCircularRegion();
        }

        // applies a circular region to the form to create a circular window.
        public void ApplyCircularRegion()
        {
            ellipsePath = new GraphicsPath();
            ellipsePath.AddEllipse(0, 0, this.Width, this.Height);
            this.Region = new Region(ellipsePath);
        }

        // applies clipping to ensure the drawn content fits within the circular region.
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
