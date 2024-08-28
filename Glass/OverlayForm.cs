using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;

namespace RED.mbnq
{
    public partial class OverlayForm : Form
    {
        private Rectangle captureArea;
        private System.Windows.Forms.Timer updateTimer;
        private bool isMoving = false;
        private bool isMoveEnabled = false;
        private Point lastMousePos;
        private DebugThings debugInfoDisplay;

        // Offset fields as modifiers
        private float offsetX = 0f; // 0.31f
        private float offsetY = 0f; // 0.18f

        // Zoom factor
        private float zoomFactor = 1.0f;

        // Opacity factor
        private float opacityFactor = 1.0f;

        // TrackBar controls for adjusting offsets and zoom
        private TrackBar? offsetXSlider;
        private TrackBar? offsetYSlider;
        private TrackBar? zoomSlider;
        private TrackBar? opacitySlider;

        // Labels to display offset and zoom values
        private Label? offsetXLabel;
        private Label? offsetYLabel;
        private Label? zoomLabel;
        private Label? opacityLabel;

        private bool isBorderVisible = true;
        private void ToggleFrameVisibility()
        {
            isBorderVisible = !isBorderVisible;
            this.Invalidate(); // Request the form to be repainted with the new border setting
        }
        public Rectangle GetAdjustedCaptureArea()
        {
            int adjustedX = captureArea.Location.X + (int)(captureArea.Width * offsetX);
            int adjustedY = captureArea.Location.Y + (int)(captureArea.Height * offsetY);
            int adjustedWidth = (int)(captureArea.Width * zoomFactor);
            int adjustedHeight = (int)(captureArea.Height * zoomFactor);

            return new Rectangle(adjustedX, adjustedY, adjustedWidth, adjustedHeight);
        }
        public OverlayForm(Rectangle mbDisplay, Rectangle selectedArea)
        {
            this.captureArea = mbDisplay;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(mbDisplay.Right, mbDisplay.Top);
            this.Size = mbDisplay.Size;
            this.BackColor = Color.Black;
            this.Opacity = 1.0;
            this.DoubleBuffered = true;
            this.debugInfoDisplay = new DebugThings(this, selectedArea); // Updated constructor call

            InitializeTrackBars();

            this.MouseClick += OverlayForm_MouseClick;

            updateTimer = new System.Windows.Forms.Timer();
            updateTimer.Interval = Program.mbFrameDelay;
            updateTimer.Tick += (s, e) => this.Invalidate();
            updateTimer.Start();
            EnableFormMovement();
        }
        public Rectangle CaptureArea => captureArea;

        public void UpdateCaptureArea(Rectangle newCaptureArea)
        {
            this.captureArea = newCaptureArea;
            this.Location = new Point(newCaptureArea.Right + 20, newCaptureArea.Top);
            this.Size = newCaptureArea.Size;
            debugInfoDisplay.UpdateSelectedRegion(newCaptureArea); // Update the debug information
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            int adjustedX = captureArea.Location.X + (int)(captureArea.Width * offsetX);
            int adjustedY = captureArea.Location.Y + (int)(captureArea.Height * offsetY);
            Point adjustedLocation = new Point(adjustedX, adjustedY);
            Rectangle adjustedCaptureArea = new Rectangle(adjustedLocation, captureArea.Size);

            // Invert the zoom factor by subtracting it from a fixed value (e.g., 2.0f)
            float invertedZoomFactor = 2.0f - zoomFactor;

            // Apply the inverted zoom factor to the size of the capture area
            int zoomedWidth = (int)(adjustedCaptureArea.Width * invertedZoomFactor);
            int zoomedHeight = (int)(adjustedCaptureArea.Height * invertedZoomFactor);

            // Create a bitmap to hold the captured screen area at the zoomed size
            using (Bitmap bitmap = new Bitmap(zoomedWidth, zoomedHeight))
            {
                using (Graphics bitmapGraphics = Graphics.FromImage(bitmap))
                {
                    // Capture the screen into the bitmap
                    bitmapGraphics.CopyFromScreen(adjustedCaptureArea.Location, Point.Empty, adjustedCaptureArea.Size);

                    // Draw the scaled bitmap onto the form
                    g.DrawImage(bitmap, new Rectangle(0, 0, this.Width, this.Height));
                }
            }

            // Set opacity and double-buffering
            this.Opacity = opacityFactor;
            this.DoubleBuffered = true;

            // Draw debug information
            debugInfoDisplay.DrawDebugInfo(g);

            // Draw a border around the control
            if (isBorderVisible)
            {
                using (Pen borderPen = new Pen(Color.Gray, 4))
                {
                    g.DrawRectangle(borderPen, 0, 0, this.Width + 1, this.Height + 1);
                }
            }
        }
    }
}
