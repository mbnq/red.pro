using System;
using System.Drawing;
using System.Windows.Forms;

namespace RED.mbnq
{
    public static class ZoomMode
    {
        private static Timer holdTimer;
        private static Timer zoomUpdateTimer;
        private static CustomZoomForm zoomForm;
        private static bool isZooming = false;
        private static ControlPanel controlPanel;
        private static Bitmap zoomBitmap;
        private static int zoomSizeSet = 128;   // Define the zoom area to capture, smaller size for more zoom
        public static int zoomMultiplier = 1;
        public static bool IsZoomModeEnabled { get; private set; } = false;
        public static void ToggleZoomMode()
        {
            IsZoomModeEnabled = !IsZoomModeEnabled;
        }

        public static void UpdateZoomMultiplier(int newZoomMultiplier)
        {
            zoomMultiplier = newZoomMultiplier;

            if (zoomForm != null)
            {
                zoomForm.Size = new Size(zoomSizeSet * zoomMultiplier, zoomSizeSet * zoomMultiplier);
                zoomForm.Invalidate(); // Force the form to repaint with the new size
            }
        }

        public static void InitializeZoomMode(ControlPanel panel)
        {
            controlPanel = panel;

            holdTimer = new Timer
            {
                Interval = 500 // before showing zoom in ms
            };
            holdTimer.Tick += HoldTimer_Tick;

            // Timer for continuous updates to the zoom display
            zoomUpdateTimer = new Timer
            {
                Interval = 16
            };
            zoomUpdateTimer.Tick += ZoomUpdateTimer_Tick;

            // Initialize the bitmap to be reused
            zoomBitmap = new Bitmap(zoomSizeSet, zoomSizeSet);
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
                zoomForm.Invalidate(); // Forces the zoomForm to repaint, which triggers ZoomForm_Paint
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

        // camera
        private static void ZoomForm_Paint(object sender, PaintEventArgs e)
        {
            if (controlPanel == null || controlPanel.MainDisplay == null) return;

            // Create a BufferedGraphicsContext
            BufferedGraphicsContext context = BufferedGraphicsManager.Current;

            // Allocate a BufferedGraphics object for the current form
            using (BufferedGraphics bufferedGraphics = context.Allocate(e.Graphics, e.ClipRectangle))
            {
                // Use the graphics object from the bufferedGraphics
                Graphics g = bufferedGraphics.Graphics;

                // Your existing drawing code, but now using 'g' instead of 'e.Graphics'
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.None;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;

                // Your drawing operations
                int centeredX = mbFnc.mGetPrimaryScreenCenter().X - (zoomSizeSet / 2);
                int centeredY = mbFnc.mGetPrimaryScreenCenter().Y - (zoomSizeSet / 2);

                using (Graphics captureGraphics = Graphics.FromImage(zoomBitmap))
                {
                    captureGraphics.CopyFromScreen(new Point(centeredX, centeredY),
                                                   Point.Empty,
                                                   new Size(zoomSizeSet * zoomMultiplier, zoomSizeSet * zoomMultiplier));
                }

                Rectangle destRect = new Rectangle(0, 0, zoomForm.Width, zoomForm.Height);
                using (System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    path.AddEllipse(destRect);
                    g.SetClip(path);
                    g.DrawImage(zoomBitmap, destRect);
                }

                using (Pen borderPen = new Pen(Color.Black, 2))
                {
                    g.DrawEllipse(borderPen, destRect);
                }

                // Render the buffered content to the screen
                bufferedGraphics.Render(); // This call is crucial to actually display the buffered content on the screen
            }
        }

        // tv

        public static void ShowZoomOverlay()
        {
            if (zoomForm == null)
            {
                zoomForm = new CustomZoomForm
                {
                    FormBorderStyle = FormBorderStyle.None,
                    Size = new Size((zoomSizeSet * zoomMultiplier), (zoomSizeSet * zoomMultiplier)),
                    StartPosition = FormStartPosition.Manual, // Set the position manually
                    Location = new Point(0, 0),
                    TopMost = true,
                    ShowInTaskbar = false,
                    TransparencyKey = Color.Magenta,
                    BackColor = Color.Black
                };

                zoomForm.Paint += ZoomForm_Paint;
            }

            // Delta Force Style
            // Position the zoomForm in the bottom-right corner
            Rectangle screenBounds = Screen.PrimaryScreen.Bounds;
            zoomForm.Left = screenBounds.Width - zoomForm.Width - 10;
            zoomForm.Top = screenBounds.Height - zoomForm.Height - 10;

            zoomForm.Show();
            isZooming = true;
            zoomUpdateTimer.Start(); // Start the update timer for real-time zoom
        }
        public static void HideZoomOverlay()
        {
            if (zoomForm != null && isZooming)
            {
                zoomForm.Hide();
                isZooming = false;
                zoomUpdateTimer.Stop(); // Stop the update timer when zooming ends
            }
        }
    }

    // Define CustomZoomForm class here
    public class CustomZoomForm : Form
    {
        public CustomZoomForm()
        {
            // Enable double buffering to reduce flicker
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint |
                          ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();

            // Apply a circular region to the form
            this.ApplyCircularRegion();
        }

        private void ApplyCircularRegion()
        {
            // Create a circular region based on the form's size
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(0, 0, this.Width, this.Height);
            this.Region = new Region(path);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            // Reapply the circular region whenever the form is resized
            ApplyCircularRegion();
        }
    }
}
