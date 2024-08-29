using System;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RED.mbnq
{
    public partial class GlassHudOverlay : Form
    {
        private Rectangle captureArea;
        private System.Windows.Forms.Timer updateTimer;
        private bool isMoving = false;
        private bool isMoveEnabled = false;
        public bool isCircle = false;
        private Point lastMousePos;
        private GlassMenu debugInfoDisplay;

        private DateTime lastFrameTime = DateTime.MinValue; // Initialize to MinValue
        public double currentFps = 0.0;

        // Offset fields as modifiers
        private float offsetX = 0f; // 0.31f
        private float offsetY = 0f; // 0.18f

        // Zoom factor
        private float zoomFactor = 1.0f;

        // Opacity factor
        private float opacityFactor = 1.0f;

        // TrackBar controls for adjusting offsets and zoom
        private TrackBar offsetXSlider;
        private TrackBar offsetYSlider;
        private TrackBar zoomSlider;
        private TrackBar opacitySlider;
        private TrackBar refreshRateSlider;

        // Labels to display offset and zoom values
        private Label offsetXLabel;
        private Label offsetYLabel;
        private Label zoomLabel;
        private Label opacityLabel;
        private Label refreshRateLabel;

        private bool isBorderVisible = true;
        
        public GlassHudOverlay(Rectangle mbDisplay, Rectangle selectedArea)
        {
            this.captureArea = mbDisplay;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(mbDisplay.Right, mbDisplay.Top);
            this.Size = mbDisplay.Size;
            this.Opacity = 1.0;
            this.DoubleBuffered = true;
            this.ShowInTaskbar = false;

            // Apply a circular region to the form
            ApplyCircularRegion();

            this.debugInfoDisplay = new GlassMenu(this, selectedArea); // Updated constructor call

            InitializeTrackBars();

            this.MouseClick += OverlayForm_MouseClick;

            updateTimer = new System.Windows.Forms.Timer();
            updateTimer.Interval = Program.mbFrameDelay;
            updateTimer.Tick += (s, e) => this.Invalidate();
            updateTimer.Start();
            EnableFormMovement();
        }
        private void ToggleFrameVisibility()
        {
            isBorderVisible = !isBorderVisible;
            this.Invalidate(); // Request the form to be repainted with the new border setting
        }
        private void ToggleShape()
        {
            isCircle = !isCircle; // Toggle the shape flag
            ApplyCircularRegion(); // Reapply the shape
            this.Invalidate(); // Trigger a repaint to update the shape
        }

        public Rectangle GetAdjustedCaptureArea()
        {
            int newWidth = (int)(captureArea.Width * zoomFactor);
            int newHeight = (int)(captureArea.Height * zoomFactor);

            // Calculate the offset to keep the zoom centered
            int offsetXCentered = (captureArea.Width - newWidth) / 2;
            int offsetYCentered = (captureArea.Height - newHeight) / 2;

            // Calculate the new top-left position based on centered zoom
            int adjustedX = captureArea.X + offsetXCentered + (int)(captureArea.Width * offsetX);
            int adjustedY = captureArea.Y + offsetYCentered + (int)(captureArea.Height * offsetY);

            return new Rectangle(adjustedX, adjustedY, newWidth, newHeight);
        }

        private void ApplyCircularRegion()
        {
            if (isCircle)
            {
                // Create a circular region based on the form's size
                using (System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    path.AddEllipse(0, 0, this.Width, this.Height);
                    this.Region = new Region(path);
                }
            }
            else
            {
                // Create a rectangular region based on the form's size
                using (System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    path.AddRectangle(new Rectangle(0, 0, this.Width, this.Height));
                    this.Region = new Region(path);
                }
            }
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            // Reapply the circular region whenever the form is resized
            ApplyCircularRegion();
        }

        public Rectangle CaptureArea => captureArea;

        public async void UpdateCaptureArea(Rectangle newCaptureArea)
        {
            // Update UI-related properties on the main thread
            this.captureArea = newCaptureArea;
            this.Location = new Point(newCaptureArea.Right + 20, newCaptureArea.Top);
            this.Size = newCaptureArea.Size;

            // Offload the potentially time-consuming debug update to a background thread
            await Task.Run(() =>
            {
                debugInfoDisplay.UpdateSelectedRegion(newCaptureArea);
            });

            // Invalidate the form on the main thread to trigger a repaint
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            DateTime currentFrameTime = DateTime.Now;

            if (lastFrameTime != DateTime.MinValue)
            {
                // Calculate time difference between frames in seconds
                double timeDelta = (currentFrameTime - lastFrameTime).TotalSeconds;

                // Calculate FPS as the reciprocal of the time taken per frame
                currentFps = 1.0 / timeDelta;
            }

            // Update lastFrameTime for the next frame
            lastFrameTime = currentFrameTime;

            Graphics g = e.Graphics;

            // Adjust the capture area based on offsets and zoom
            Rectangle adjustedCaptureArea = GetAdjustedCaptureArea();

            // Create a bitmap to hold the captured screen area at the zoomed size
            using (Bitmap bitmap = new Bitmap(adjustedCaptureArea.Width, adjustedCaptureArea.Height))
            {
                using (Graphics bitmapGraphics = Graphics.FromImage(bitmap))
                {
                    // Capture the screen into the bitmap
                    bitmapGraphics.CopyFromScreen(adjustedCaptureArea.Location, Point.Empty, adjustedCaptureArea.Size);

                    Rectangle destRect = new Rectangle(0, 0, this.Width, this.Height);

                    if (isCircle)
                    {
                        // Draw the captured bitmap, clipped to the circular region
                        using (System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath())
                        {
                            path.AddEllipse(destRect);
                            e.Graphics.SetClip(path);
                            e.Graphics.DrawImage(bitmap, destRect);
                        }
                    }
                    else
                    {
                        // Draw the scaled bitmap onto the form
                        g.DrawImage(bitmap, destRect);
                    }
                }
            }

            // Set opacity and double-buffering
            this.Opacity = opacityFactor;
            this.DoubleBuffered = true;

            // Draw debug information if enabled
            debugInfoDisplay.DrawDebugInfo(g);

            // Draw a border around the control if enabled
            if (isBorderVisible)
            {
                using (Pen borderPen = new Pen(Color.Gray, 4))
                {
                    if (isCircle)
                    {
                        g.DrawEllipse(borderPen, 0, 0, this.Width - 1, this.Height - 1);
                    }
                    else
                    {
                        g.DrawRectangle(borderPen, 0, 0, this.Width - 1, this.Height - 1);
                    }
                }
            }
        }

        public static GlassHudOverlay displayOverlay;
        public static void RestartWithNewArea()
        {
            if (displayOverlay != null)
            {
                displayOverlay.Hide(); // Hide current overlay

                Rectangle newArea = selector.SelectCaptureArea();

                displayOverlay.UpdateCaptureArea(newArea); // Update the area instead of recreating the form
                displayOverlay.Show(); // Show the updated overlay
            }
        }
    }
}
