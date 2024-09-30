
/* 

    www.mbnq.pl 2024 
    https://mbnq.pl/
    mbnq00 on gmail

*/

using System;
using System.Windows.Forms;
using System.Drawing;
using System.Threading.Tasks;


namespace RED.mbnq
{
    // for saveLoad logics
    public partial class GlassHudOverlay : Form
    {
        private Rectangle glassCaptureArea;
        private Timer glassRefreshTimer;
        private bool isMoving = false;
        private bool isMoveEnabled = false;
        public bool isCircle = false;
        private Point lastMousePos;
        private glassControls glassInfoDisplay;

        private DateTime lastFrameTime = DateTime.MinValue;
        public double currentFps = 0.0;

        private float offsetX = 0f;             // 0.31f
        private float offsetY = 0f;             // 0.18f
        private float zoomFactor = 1.0f;
        private float opacityFactor = 1.0f;

        private bool isBorderVisible = true;

        /* --- --- ---  --- --- --- */
        public GlassHudOverlay(Rectangle mbGlassElement, Rectangle selectedArea)
        {
            this.glassCaptureArea = mbGlassElement;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(mbGlassElement.Right, mbGlassElement.Top);
            this.Size = mbGlassElement.Size;
            this.Opacity = 1.0;
            this.DoubleBuffered = true;
            this.ShowInTaskbar = false;
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);

            ApplyCircularRegion();

            this.glassInfoDisplay = new glassControls(this, selectedArea);

            this.FormClosed += (sender, e) => { if (mbglassCPInstance != null) mbglassCPInstance.Close(); };
            this.MouseClick += OverlayForm_MouseClick;

            glassRefreshTimer = new Timer();
            glassRefreshTimer.Interval = Program.mbFrameDelay;
            glassRefreshTimer.Tick += (s, e) => this.Invalidate();
            glassRefreshTimer.Start();
            EnableFormMovement();
        }

        /* --- --- ---  --- --- --- */

        private void ToggleFrameVisibility()
        {
            isBorderVisible = !isBorderVisible;
            this.Invalidate();
        }
        private void ToggleShape()
        {
            isCircle = !isCircle;
            ApplyCircularRegion();
            this.Invalidate();
        }
        public Rectangle GetAdjustedCaptureArea()
        {
            int newWidth = (int)(glassCaptureArea.Width * zoomFactor);
            int newHeight = (int)(glassCaptureArea.Height * zoomFactor);

            // calculate the offset to keep the zoom centered
            int offsetXCentered = (glassCaptureArea.Width - newWidth) / 2;
            int offsetYCentered = (glassCaptureArea.Height - newHeight) / 2;

            // calculate the new top-left position based on centered zoom
            int adjustedX = glassCaptureArea.X + offsetXCentered + (int)(glassCaptureArea.Width * offsetX);
            int adjustedY = glassCaptureArea.Y + offsetYCentered + (int)(glassCaptureArea.Height * offsetY);

            return new Rectangle(adjustedX, adjustedY, newWidth, newHeight);
        }
        private void ApplyCircularRegion()
        {
            if (isCircle)
            {
                // circular region based on the form's size
                using (System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    path.AddEllipse(0, 0, this.Width, this.Height);
                    this.Region = new Region(path);
                }
            }
            else
            {
                // rectangular region based on the form's size
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
            // reapply the circular region whenever the form is resized
            ApplyCircularRegion();
        }
        public Rectangle CaptureArea => glassCaptureArea;

        public async Task UpdateCaptureArea(Rectangle newCaptureArea)
        {
            this.glassCaptureArea = newCaptureArea;
            this.Location = new Point(newCaptureArea.Right + 20, newCaptureArea.Top);
            this.Size = newCaptureArea.Size;

            var bitmap = await Task.Run(() =>
            {
                // create a bitmap with the size of the new capture area
                Bitmap generatedBitmap = new Bitmap(newCaptureArea.Width, newCaptureArea.Height);
                using (Graphics bitmapGraphics = Graphics.FromImage(generatedBitmap))
                {
                    bitmapGraphics.CopyFromScreen(newCaptureArea.Location, Point.Empty, newCaptureArea.Size);
                }
                return generatedBitmap;
            });

            // update the UI on the main thread
            this.Invoke((Action)(() =>
            {
                using (Graphics g = this.CreateGraphics())
                {
                    g.DrawImage(bitmap, new Rectangle(0, 0, this.Width, this.Height));
                }

                this.Invalidate();
            }));

            await Task.Run(() =>
            {
                glassInfoDisplay.UpdateSelectedRegion(newCaptureArea);
            });

            this.Invoke((Action)(() => this.Invalidate()));
        }


        /* --- --- ---  --- --- --- */
        public double GlassFrameTime { get; private set; }
        protected override void OnPaint(PaintEventArgs e)
        {
            DateTime currentFrameTime = DateTime.Now;

            if (lastFrameTime != DateTime.MinValue)
            {
                // time difference between frames in seconds
                double timeDelta = (currentFrameTime - lastFrameTime).TotalSeconds;

                GlassFrameTime = timeDelta;
                currentFps = 1.0 / timeDelta;
            }

            lastFrameTime = currentFrameTime;

            BufferedGraphicsContext context = BufferedGraphicsManager.Current;
            using (BufferedGraphics bufferedGraphics = context.Allocate(e.Graphics, e.ClipRectangle))
            {
                Graphics g = bufferedGraphics.Graphics;

                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;      // Bilinear
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.None;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;

                Rectangle adjustedCaptureArea = GetAdjustedCaptureArea();

                using (Bitmap bitmap = new Bitmap(adjustedCaptureArea.Width, adjustedCaptureArea.Height))
                {
                    using (Graphics bitmapGraphics = Graphics.FromImage(bitmap))
                    {

                        bitmapGraphics.CopyFromScreen(adjustedCaptureArea.Location, Point.Empty, adjustedCaptureArea.Size);

                        Rectangle destRect = new Rectangle(0, 0, this.Width, this.Height);

                        if (isCircle)
                        {
                            // draw
                            using (System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath())
                            {
                                path.AddEllipse(destRect);
                                g.SetClip(path);
                                g.DrawImage(bitmap, destRect);
                            }
                        }
                        else
                        {
                            g.DrawImage(bitmap, destRect);
                        }
                    } 
                }
                // draw debug information if enabled
                // glassInfoDisplay.DrawDebugInfo(g);
                this.Opacity = opacityFactor;

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

                bufferedGraphics.Render();
            }
        }

        /* --- --- ---  --- --- --- */

        public static GlassHudOverlay displayOverlay;
        public static async Task RestartWithNewAreaAsync()
        {
            if (displayOverlay != null)
            {
                await Task.Run(() =>
                {
                    displayOverlay.Invoke((MethodInvoker)(() => displayOverlay.Hide()));
                });

                Rectangle newArea = await Task.Run(() => selector.SelectCaptureArea());

                await Task.Run(() =>
                {
                    displayOverlay.Invoke((MethodInvoker)(() => _=displayOverlay.UpdateCaptureArea(newArea)));
                    displayOverlay.Invoke((MethodInvoker)(() => displayOverlay.Show()));
                });
            }
        }
        public static async Task ReloadWithNewAreaAsync()
        {
            if (displayOverlay != null)
            {
                await Task.Run(() =>
                {
                    displayOverlay.Invoke((MethodInvoker)(() => displayOverlay.Hide()));
                });

                Rectangle newArea = displayOverlay.glassCaptureAreaValue;

                await Task.Run(() =>
                {
                    displayOverlay.Invoke((MethodInvoker)(() => _=displayOverlay.UpdateCaptureArea(newArea)));
                    displayOverlay.Invoke((MethodInvoker)(() => displayOverlay.Show()));
                });
            }
        }
    }
}
