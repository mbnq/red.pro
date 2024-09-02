using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RED.mbnq
{
    public class mbnqConsole : Form
    {
        private PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        private List<string> displayTexts = new List<string>();
        // console
        private string lastDebugMessage = string.Empty;
        private int initialWidth;
        private int initialHeight;
        // private CancellationTokenSource pingCancellationTokenSource;
        private System.Windows.Forms.Timer pingTimer;
        private System.Windows.Forms.Timer ipTimer;
        private System.Windows.Forms.Timer generalDisplayTimer;
        private string currentPingAddress = "8.8.8.8";

        // Fields to track mouse movements
        private bool isDragging = false;
        private Point startPoint = new Point(0, 0);

        // Field to track how often TXTHUD_Paint is being drawn
        private int paintCounter = 0;

        // Field to track the last draw time
        private DateTime lastDrawTime = DateTime.MinValue;

        // Draw limit, refresh overlay if this amount of seconds passed
        private double throttlePaintTime = 1.00f;



        public mbnqConsole()
        {
            InitializeComponent();
            InitializeTimers();
            InitializeMouseEvents();
            CaptureDebugMessages();
            AdjustSize();
        }

        #region Initialization Methods

        private void InitializeComponent()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(10, 10); // Slight offset from top-left corner
            this.Size = new Size(300, 200); // Adjust size as needed
            this.TopMost = true;
            this.BackColor = Color.Black; // Set background color
            this.Opacity = 0.8; // Set transparency
            this.ShowInTaskbar = false;
            this.DoubleBuffered = true;
            this.Paint += TXTHUD_Paint;

            initialWidth = this.Size.Width;
            initialHeight = this.Size.Height;

            // Initialize Display Texts with placeholders
            displayTexts.Add("Ping: -- ms");
            displayTexts.Add("IP: Fetching...");
            displayTexts.Add("Console Draw Count: 0"); // Initialize the third line for the draw count
            displayTexts.Add("CPU: -- %"); // Placeholder for CPU usage
            displayTexts.Add("Debug: --"); // Placeholder for CPU usage

            AdjustSize();
        }

        private void InitializeTimers()
        {
            // Initialize Ping Timer
            pingTimer = new System.Windows.Forms.Timer();
            pingTimer.Interval = 1000; // 1000 1 second
            pingTimer.Tick += async (s, e) => await UpdatePingAsync();
            pingTimer.Start();

            // Initialize IP Timer
            ipTimer = new System.Windows.Forms.Timer();
            ipTimer.Interval = 10000; // 20 seconds
            ipTimer.Tick += async (s, e) => await UpdateIpAddressAsync();
            ipTimer.Start();

            // Initialize General Display Timer
            generalDisplayTimer = new System.Windows.Forms.Timer();
            generalDisplayTimer.Interval = 1000; // 1 second
            generalDisplayTimer.Tick += (s, e) => UpdateGeneralDisplay();
            generalDisplayTimer.Start();
        }

        private void InitializeMouseEvents()
        {
            this.MouseDown += new MouseEventHandler(TXTHUD_MouseDown);
            this.MouseMove += new MouseEventHandler(TXTHUD_MouseMove);
            this.MouseUp += new MouseEventHandler(TXTHUD_MouseUp);
        }

        #endregion

        #region Mouse Events

        private void TXTHUD_MouseDown(object sender, MouseEventArgs e)
        {
            isDragging = true;
            startPoint = new Point(e.X, e.Y);
        }

        private void TXTHUD_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point p = PointToScreen(e.Location);
                Location = new Point(p.X - startPoint.X, p.Y - startPoint.Y);
            }
        }

        private void TXTHUD_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
        }

        #endregion

        #region Update Methods

        private async Task UpdatePingAsync()
        {
            string pingResult = await GetPingResultAsync(currentPingAddress);
            UpdatePingText($"Ping {currentPingAddress}: {pingResult} ms");
        }

        private async Task UpdateIpAddressAsync()
        {
            string ipAddress = await GetIpAddressAsync();
            UpdateIpText($"IP: {ipAddress}");
        }

        #endregion

        #region Text Update Methods

        private void UpdatePingText(string newText)
        {
            displayTexts[0] = newText;
            this.ThrottlePaint(); // Redraw with the updated text
        }

        private void UpdateIpText(string newText)
        {
            displayTexts[1] = newText;
            this.ThrottlePaint(); // Redraw with the updated text
        }

        private void UpdateDrawCountText()
        {
            displayTexts[2] = $"Console Draw Count: {paintCounter}";
            this.ThrottlePaint(); // Redraw with the updated draw count
        }

        private void UpdateCpuUsageText()
        {
            displayTexts[3] = $"CPU: {GetCpuUsage()}";
            ThrottlePaint(); // Ensure the HUD is redrawn, respecting the throttle
        }

        private void UpdateGeneralDisplay()
        {
            UpdateCpuUsageText();
            ThrottlePaint(); // Ensure the HUD is redrawn, respecting the throttle
        }

        #endregion

        #region Data Retrieval Methods

        // Capturing and displaying debug messages
        private void CaptureDebugMessages()
        {
            Debug.Listeners.Clear();
            Debug.Listeners.Add(new DebugListener(this));
        }

        private void UpdateDebugMessage(string message)
        {
            lastDebugMessage = message;
            UpdateDebugText($"Debug: {message}");
        }

        private void UpdateDebugText(string newText)
        {
            if (displayTexts.Count >= 5)
            {
                displayTexts[4] = newText;
            }
            else
            {
                displayTexts.Add(newText);
            }
            AdjustSize(); // Adjust size based on the new content
            this.ThrottlePaint(); // Redraw with the updated text
        }

        private class DebugListener : TraceListener
        {
            private readonly mbnqConsole parentForm;

            public DebugListener(mbnqConsole parent)
            {
                parentForm = parent;
            }

            public override void Write(string message)
            {
                parentForm.UpdateDebugMessage(message);
            }

            public override void WriteLine(string message)
            {
                parentForm.UpdateDebugMessage(message);
            }
        }

        // cpu usage
        private string GetCpuUsage()
        {
            return $"{cpuCounter.NextValue():F1} % CPU";
        }
        private async Task<string> GetPingResultAsync(string address)
        {
            try
            {
                using (Process p = new Process())
                {
                    p.StartInfo.FileName = "ping";
                    p.StartInfo.Arguments = $"{address} -n 1";
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.CreateNoWindow = true;
                    p.Start();

                    string output = await p.StandardOutput.ReadToEndAsync();
                    p.WaitForExit();

                    var pingTimeLine = output.Split('\n').FirstOrDefault(line => line.Contains("time="));
                    if (!string.IsNullOrEmpty(pingTimeLine))
                    {
                        int timeIndex = pingTimeLine.IndexOf("time=") + 5;
                        int msIndex = pingTimeLine.IndexOf("ms", timeIndex);
                        string timeValue = pingTimeLine.Substring(timeIndex, msIndex - timeIndex).Trim();
                        return timeValue;
                    }
                    else
                    {
                        return "Timeout";
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLineIf(ControlPanel.mIsDebugOn, $"mbnq: {ex.Message}");
                return "Error";
            }
        }

        private async Task<string> GetIpAddressAsync()
        {
            string ipAddress = null;

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    ipAddress = await client.GetStringAsync("https://api.seeip.org/");
                    return ipAddress.Trim();
                }
                catch (HttpRequestException ex)
                {
                    Debug.WriteLine($"Failed to fetch IP from https://api.seeip.org/: {ex.Message}");
                }

                if (string.IsNullOrEmpty(ipAddress))
                {
                    try
                    {
                        ipAddress = await client.GetStringAsync("https://api.my-ip.io/v2/ip.txt");
                        return ipAddress.Trim();
                    }
                    catch (HttpRequestException ex)
                    {
                        Debug.WriteLine($"Failed to fetch IP from https://api.my-ip.io/v2/ip.txt: {ex.Message}");
                    }
                }
                
                if (string.IsNullOrEmpty(ipAddress))
                {
                    try
                    {
                        ipAddress = await client.GetStringAsync("https://wtfismyip.com/text/");
                        return ipAddress.Trim();
                    }
                    catch (HttpRequestException ex)
                    {
                        Debug.WriteLine($"Failed to fetch IP from https://wtfismyip.com/text/: {ex.Message}");
                    }
                }


                if (string.IsNullOrEmpty(ipAddress))
                {
                    try
                    {
                        // Second attempt: https://mbnq.pl/myip/
                        ipAddress = await client.GetStringAsync("https://mbnq.pl/myip/");
                        return ipAddress.Trim();
                    }
                    catch (HttpRequestException ex)
                    {
                        // Log the exception if needed
                        Debug.WriteLine($"Failed to fetch IP from https://mbnq.pl/myip/: {ex.Message}");
                    }
                } 
            }
            // If all attempts fail, return "Unavailable"
            return "Unavailable";
        }


        #endregion

        #region Throttle Paint Method

        private void ThrottlePaint()
        {
            // Check if at least 1 second has passed since the last draw
            if ((DateTime.Now - lastDrawTime).TotalSeconds >= throttlePaintTime)
            {
                this.Invalidate(); // Redraw with the updated text
                lastDrawTime = DateTime.Now; // Update the last draw time
            }
        }

        #endregion

        #region UI Methods
        private void AdjustSize()
        {
            using (Graphics g = this.CreateGraphics())
            {
                using (Font font = new Font("Consolas", 10, FontStyle.Regular))
                {
                    // Calculate the required height based on the number of lines and their height
                    int lineHeight = (int)g.MeasureString("Test", font).Height;
                    int requiredHeight = lineHeight * displayTexts.Count + 20; // Adding padding

                    // Ensure the width does not shrink below the initial width
                    int requiredWidth = Math.Max(initialWidth, displayTexts.Select(text => (int)g.MeasureString(text, font).Width).Max() + 20);

                    // Adjust the form size: increase the height if the required height is greater than the current height
                    this.Size = new Size(requiredWidth, Math.Max(initialHeight, requiredHeight));
                }
            }
        }

        private void TXTHUD_Paint(object sender, PaintEventArgs e)
        {
            // Increment the draw counter
            paintCounter++;

            // Update the third line with the current draw count
            UpdateDrawCountText();

            Graphics g = e.Graphics;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            using (Brush brush = new SolidBrush(Color.White))
            using (Font font = new Font("Consolas", 10, FontStyle.Regular))
            {
                float yPosition = 10f;
                foreach (var text in displayTexts)
                {
                    g.DrawString(text, font, brush, new PointF(10, yPosition));
                    yPosition += 25f; // Adjust line spacing as needed
                }
            }
        }

        public void ToggleOverlay(string pingAddress = "8.8.8.8")
        {
            currentPingAddress = pingAddress;

            if (this.Visible)
            {
                this.Hide();
                StopTimers();
            }
            else
            {
                this.Show();
                AdjustSize(); // Adjust size when overlay is shown
                StartTimers();
            }
        }

        #endregion

        #region Timer Control Methods

        private void StartTimers()
        {
            pingTimer?.Start();
            ipTimer?.Start();
        }

        private void StopTimers()
        {
            pingTimer?.Stop();
            ipTimer?.Stop();
        }

        #endregion

        #region Cleanup

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            StopTimers();
            pingTimer.Dispose();
            ipTimer.Dispose();
            base.OnFormClosing(e);
        }

        #endregion
    }
}
