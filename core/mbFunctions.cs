
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
using System.Diagnostics;
using MaterialSkin.Controls;
using RED.mbnq;
using System.Threading.Tasks;

public static class mbFnc
{
    // ---------------------------------------
    // for circle drawing, not used yet
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

    // ---------------------------------------
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

    // ---------------------------------------
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

    // ---------------------------------------
    // Copy overlay content to clipboard
    public static void CopyOverlayToClipboard(Form overlayForm, Rectangle captureRect)
    {
        try
        {
            using (Bitmap bitmap = CaptureOverlayContent(overlayForm, captureRect))
            {
                Clipboard.SetImage(bitmap);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLineIf(ControlPanel.mbIsDebugOn, $"mbnq: function failed {ex.Message}");
        }
    }

    // ---------------------------------------
    // Copy label text to clipboard, for eventhandler usage only

    private static ToolTip mbToolTip = new ToolTip();
    public static void mbCopyLabelToClipboard(object sender, EventArgs e)
    {
        try
        {
            Label clickedLabel = sender as Label;

            if (clickedLabel != null)
            {
                Sounds.PlayClickSoundOnce();

                Clipboard.SetText(clickedLabel.Text);
                var mousePosition = Control.MousePosition;
                Point labelLocation = clickedLabel.PointToClient(mousePosition);

                mbToolTip.Show($"Copied: {clickedLabel.Text} to clipboard!", clickedLabel, labelLocation.X, labelLocation.Y, 2000);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLineIf(ControlPanel.mbIsDebugOn, $"mbnq: function failed {ex.Message}");
        }
    }

    // ---------------------------------------
    // Simple spacer for GUI usage
    public static void mbSpacer2(Control.ControlCollection parentControls, byte desiredHight, string desiredText)
    {
        parentControls.Add(new Label
        {
            Text = desiredText,
            Size = new Size(parentControls.Owner.Width, desiredHight),
            TextAlign = ContentAlignment.MiddleLeft
        });

    }

    // ---------------------------------------
    // make rectangle from 2 points
    public static Rectangle mbGetRectangle(Point p1, Point p2)
    {
        int x = Math.Min(p1.X, p2.X);
        int y = Math.Min(p1.Y, p2.Y);
        int width = Math.Abs(p2.X - p1.X);
        int height = Math.Abs(p2.Y - p1.Y);
        return new Rectangle(x, y, width, height);
    }

    // ---------------------------------------
    // Public static async method to get CPU usage asynchronously
    private static Lazy<PerformanceCounter> cpuCounter = new Lazy<PerformanceCounter>(() => new PerformanceCounter("Processor", "% Processor Time", "_Total"));
    public static async Task<float> mbGetCpuUsageAsync()
    {
        // on init call NextValue and wait before obtaining the value again
        _ = cpuCounter.Value.NextValue();
        await Task.Delay(200);
        return cpuCounter.Value.NextValue();
    }

    // ------------------------------------------
    // simple message box with copy to clipboard
    public partial class mbMessageBox : MaterialForm
    {
        public mbMessageBox(string message, string mBoxTitle)
        {

            // ------------------------------------------
            MaterialTextBox2 txtMessage = new MaterialTextBox2
            {
                Text = message,
                ReadOnly = true,
                // AutoSize = true,
                Dock = DockStyle.Bottom
            };

            // ------------------------------------------
            MaterialButton btnOK = new MaterialButton
            {
                Text = "Close",
                AutoSize = true,
                Dock = DockStyle.Bottom
            };
            btnOK.Click += (s, e) =>
            {
                Sounds.PlayClickSoundOnce();
                this.Close();
            };

            // ------------------------------------------
            MaterialButton btnCopy = new MaterialButton
            {
                Text = "Copy to Clipboard",
                AutoSize = true,
                Dock = DockStyle.Bottom
            };

            btnCopy.Click += (s, e) =>
            {
                Clipboard.SetText(message); // Copy the content to the clipboard
                Sounds.PlayClickSoundOnce();
                // MessageBox.Show("Content copied to clipboard.", "Copied", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Debug.WriteLineIf(ControlPanel.mbIsDebugOn, $"mbnq: Content copied to clipboard.");
            };

            // ------------------------------------------
            this.Text = mBoxTitle;
            this.Size = new Size(200, 200);
            // this.AutoSize = true;
            // this.AutoSizeMode = AutoSizeMode.GrowOnly;
            this.TopMost = true;
            this.Padding = new Padding(4, 4, 4, 4);
            this.Margin = new Padding(10, 10, 10, 10);
            this.BackColor = Color.FromArgb(50, 50, 50);
            this.ForeColor = Color.FromArgb(50, 50, 50);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            // this.Location = ;

            this.Controls.Add(txtMessage);
            this.Controls.Add(btnCopy);
            this.Controls.Add(btnOK);
            this.Activate();    // doesn't really needed
        }
    }

    // ------------------------------------------
    // customized progressBar
    public class mbProgressBar : MaterialProgressBar
    {
        private int _value;
        public new int Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnValueChanged(EventArgs.Empty);
                }
                base.Value = _value;
            }
        }

        public event EventHandler ValueChanged;
        protected virtual void OnValueChanged(EventArgs e)
        {
            ValueChanged?.Invoke(this, e);
        }
    }
}

