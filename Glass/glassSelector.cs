
/* 

    www.mbnq.pl 2024 
    https://mbnq.pl/
    mbnq00 on gmail

*/

// using MaterialSkin.Controls;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RED.mbnq
{
    public class selector : Form // : MaterialForm
    {
        public Rectangle SelectedArea { get; private set; }

        private Point startPoint;
        private Point endPoint;
        private bool selectingInProgress;
        private Pen selectionPen;
        private Image backgroundScreenshot;
        public selector()
        {
            backgroundScreenshot = MakeScreenshotGrey((Bitmap)mbMakeScreenShot());

            this.FormBorderStyle = FormBorderStyle.None;
            this.Opacity = 1.0;
            this.DoubleBuffered = true;
            this.Cursor = Cursors.Cross;
            this.TopMost = true;
            // this.AllowTransparency = true;

            this.Bounds = Screen.PrimaryScreen.Bounds;

            // pen for selection rectangle
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
                System.Drawing.Imaging.ColorMatrix colorMatrix = new System.Drawing.Imaging.ColorMatrix(
                    new float[][]
                    {
                        new float[] { 0.3f, 0.3f, 0.3f, 0, 0 },
                        new float[] { 0.59f, 0.59f, 0.59f, 0, 0 },
                        new float[] { 0.11f, 0.11f, 0.11f, 0, 0 },
                        new float[] { 0, 0, 0, 1, 0 },
                        new float[] { 0, 0, 0, 0, 1 }
                    });


                var attributes = new System.Drawing.Imaging.ImageAttributes();
                attributes.SetColorMatrix(colorMatrix);
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
                this.Invalidate();              // redraw the selection rectangle
            }
        }
        private void ScreenAreaSelector_MouseUp(object sender, MouseEventArgs e)
        {
            if (selectingInProgress)
            {
                selectingInProgress = false;

                Rectangle selectionRect = mbFnc.mbGetRectangle(startPoint, endPoint);
                Point screenPoint = this.PointToScreen(selectionRect.Location);

                SelectedArea = new Rectangle(screenPoint, selectionRect.Size);

                if (SelectedArea.Width >= 10 && SelectedArea.Height >= 10)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    // re-enter selection mode if the selection is too small
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

        // this is needed when selecting area
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // draw the captured screen as the background
            e.Graphics.DrawImage(backgroundScreenshot, Point.Empty);

            if (selectingInProgress)
            {
                Rectangle selectionRect = mbFnc.mbGetRectangle(startPoint, endPoint);

                // semi-transparent selection rectangle
                using (SolidBrush selectionBrush = new SolidBrush(Color.FromArgb(128, Color.Blue)))
                {
                    e.Graphics.FillRectangle(selectionBrush, selectionRect);
                }

                // dashed border
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
                    return Rectangle.Empty;
                }
            }
        }
    }
}
