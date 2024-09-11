
/* 

    www.mbnq.pl 2024 
    https://mbnq.pl/
    mbnq00 on gmail

*/

using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace RED.mbnq
{
    public class selector : Form
    {
        public Rectangle SelectedArea { get; private set; }

        private Point startPoint;
        private Point endPoint;
        private bool selectingInProgress;
        private Pen selectionPen;
        private Image backgroundScreenshot;

        public selector()
        {
            // Capture the entire screen
            backgroundScreenshot = MakeScreenshotGrey((Bitmap)mbMakeScreenShot());

            this.FormBorderStyle = FormBorderStyle.None;
            this.Opacity = 1.0;
            this.DoubleBuffered = true;
            this.Cursor = Cursors.Cross;
            this.TopMost = true;
            // this.AllowTransparency = true;

            // Span the selector form across all screens
            this.Bounds = Screen.PrimaryScreen.Bounds;

            // Semi-transparent pen for selection rectangle
            selectionPen = new Pen(Color.Blue, 2)
            {
                DashStyle = System.Drawing.Drawing2D.DashStyle.Dash,
                Color = Color.Gray
            };

            this.MouseDown += ScreenAreaSelector_MouseDown;
            this.MouseMove += ScreenAreaSelector_MouseMove;
            this.MouseUp += ScreenAreaSelector_MouseUp;
            this.KeyDown += ScreenAreaSelector_KeyDown;
        }
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            backgroundScreenshot?.Dispose();
            base.OnFormClosed(e);
        }
        private Image mbMakeScreenShot()
        {
            Rectangle primaryScreenBounds = Screen.PrimaryScreen.Bounds;

            Bitmap screenshot = new Bitmap(primaryScreenBounds.Width, primaryScreenBounds.Height);

            using (Graphics g = Graphics.FromImage(screenshot))
            {
                g.CopyFromScreen(primaryScreenBounds.Location, Point.Empty, primaryScreenBounds.Size);
            }

            return screenshot;
        }
        private Bitmap MakeScreenshotGrey(Bitmap original)
        {
            Bitmap greyImage = new Bitmap(original.Width, original.Height);

            using (Graphics g = Graphics.FromImage(greyImage))
            {
                // Create a color matrix to transform the image to grayscale
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
                var attributes = new System.Drawing.Imaging.ImageAttributes();
                attributes.SetColorMatrix(colorMatrix);

                // Draw the original image with the grayscale color matrix applied
                g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height), 0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);
            }

            return greyImage;
        }
        private void ScreenAreaSelector_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                selectingInProgress = true;
                startPoint = e.Location;
                endPoint = e.Location;
            }
        }
        private void ScreenAreaSelector_MouseMove(object sender, MouseEventArgs e)
        {
            if (selectingInProgress)
            {
                endPoint = e.Location;
                this.Invalidate(); // Redraw the selection rectangle
            }
        }
        private void ScreenAreaSelector_MouseUp(object sender, MouseEventArgs e)
        {
            if (selectingInProgress)
            {
                selectingInProgress = false;

                // Normalize the rectangle
                Rectangle selectionRect = GetRectangle(startPoint, endPoint);
                Point screenPoint = this.PointToScreen(selectionRect.Location);

                // Set the SelectedArea with screen coordinates
                SelectedArea = new Rectangle(screenPoint, selectionRect.Size);

                if (SelectedArea.Width >= 10 && SelectedArea.Height >= 10)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    // Optionally, re-enter selection mode if the selection is too small
                    selectingInProgress = true;
                    startPoint = endPoint;
                }
            }
        }
        private void ScreenAreaSelector_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }
        private Rectangle GetRectangle(Point p1, Point p2)
        {
            int x = Math.Min(p1.X, p2.X);
            int y = Math.Min(p1.Y, p2.Y);
            int width = Math.Abs(p2.X - p1.X);
            int height = Math.Abs(p2.Y - p1.Y);
            return new Rectangle(x, y, width, height);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Draw the captured screen as the background
            e.Graphics.DrawImage(backgroundScreenshot, Point.Empty);

            if (selectingInProgress)
            {
                Rectangle selectionRect = GetRectangle(startPoint, endPoint);

                // Draw the semi-transparent selection rectangle
                using (SolidBrush selectionBrush = new SolidBrush(Color.FromArgb(128, Color.Blue)))
                {
                    e.Graphics.FillRectangle(selectionBrush, selectionRect);
                }

                // Draw the dashed border
                e.Graphics.DrawRectangle(selectionPen, selectionRect);
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                selectionPen?.Dispose();
                backgroundScreenshot?.Dispose();
            }
            base.Dispose(disposing);
        }
        public static Rectangle SelectCaptureArea()
        {
            using (var selector = new selector())
            {
                if (selector.ShowDialog() == DialogResult.OK)
                {
                    return selector.SelectedArea;
                }
                else
                {
                    Environment.Exit(0);
                    return Rectangle.Empty; // will never get here but we need it for compilation
                }
            }
        }
    }
}
