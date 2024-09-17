
/* 

    www.mbnq.pl 2024 
    https://mbnq.pl/
    mbnq00 on gmail

*/

using System.Drawing;
using System.Windows.Forms;
using System.Security.Cryptography;
using System;
using System.IO;
public static class mbFnc
{
    public static void mbFillCircle(this Graphics g, Brush brush, float x, float y, float radius)
    {
        g.FillEllipse(brush, x - radius, y - radius, radius * 2, radius * 2);
    }
    public struct PointCoordinates
    {
        public int X { get; }
        public int Y { get; }

        public PointCoordinates(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    // these two below needs to be uniffied
    // Public static method to get the center point of the primary screen
    public static PointCoordinates mGetPrimaryScreenCenter()
    {

        Screen primaryScreen = Screen.PrimaryScreen;

        // excludes taskbar
        Rectangle workingArea = primaryScreen.Bounds;

        // Calculate the center point
        int centerX = workingArea.Left + workingArea.Width / 2;
        int centerY = workingArea.Top + workingArea.Height / 2;

        return new PointCoordinates(centerX, centerY);
    }

    // Public static method to get the center point of the primary screen II

    public static Point mGetPrimaryScreenCenter2()
    {
        Screen primaryScreen = Screen.PrimaryScreen;

        // Excludes taskbar
        Rectangle workingArea = primaryScreen.Bounds;

        // Calculate the center point
        int centerX = workingArea.Left + workingArea.Width / 2;
        int centerY = workingArea.Top + workingArea.Height / 2;

        return new Point(centerX, centerY);
    }

    // calculate file hash
    public static string CalculateFileHash(string filePath)
    {
        using (var sha256 = SHA256.Create())
        {
            using (var stream = File.OpenRead(filePath))
            {
                byte[] hash = sha256.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }

    // capture overlay and copy to clipboard
    public static Bitmap CaptureOverlayContent(Form overlayForm, Rectangle captureRect)
    {
        Bitmap bitmap = new Bitmap(overlayForm.Width, overlayForm.Height);
        using (Graphics g = Graphics.FromImage(bitmap))
        {
            g.CopyFromScreen(captureRect.Location, Point.Empty, captureRect.Size);
        }
        return bitmap;
    }

    // Copy overlay content to clipboard
    public static void CopyOverlayToClipboard(Form overlayForm, Rectangle captureRect)
    {
        using (Bitmap bitmap = CaptureOverlayContent(overlayForm, captureRect))
        {
            Clipboard.SetImage(bitmap);
        }
    }
    // ---------------------------------------
    // currentFps = mbFnc.CalculateFps(ref lastFrameTime);
    public static double CalculateFps(ref DateTime lastFrameTime)
    {
        DateTime currentFrameTime = DateTime.Now;

        if (lastFrameTime != DateTime.MinValue)
        {
            double timeDelta = (currentFrameTime - lastFrameTime).TotalSeconds;
            lastFrameTime = currentFrameTime;
            return 1.0 / timeDelta;
        }

        lastFrameTime = currentFrameTime;
        return 0.0;
    }
    // ---------------------------------------
    public static void mbCopyLabelToClipboard(object sender, EventArgs e)
    {

        Label clickedLabel = sender as Label;

        if (clickedLabel != null)
        {
            // Copy the text of the clicked label to the clipboard
            Clipboard.SetText(clickedLabel.Text);

            // Optional: Show a message box to confirm
            MessageBox.Show($"Copied: {clickedLabel.Text} to clipboard!");
        }
    }
}

