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
    public class mbnqTXTHUD : Form
    {
        private List<string> displayTexts = new List<string>();
        private CancellationTokenSource pingCancellationTokenSource;
        private System.Windows.Forms.Timer pingTimer;
        private System.Windows.Forms.Timer ipTimer;
        private string currentPingAddress = "8.8.8.8";

        public mbnqTXTHUD()
        {
            InitializeComponent();
            InitializeTimers();
        }

        #region Initialization Methods

        private void InitializeComponent()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(10, 10); // Slight offset from top-left corner
            this.Size = new Size(300, 60); // Adjust size as needed
            this.TopMost = true;
            this.BackColor = Color.Black; // Set background color
            this.Opacity = 0.8; // Set transparency
            this.ShowInTaskbar = false;
            this.DoubleBuffered = true;
            this.Paint += TXTHUD_Paint;
        }

        private void InitializeTimers()
        {
            // Initialize Ping Timer
            pingTimer = new System.Windows.Forms.Timer();
            pingTimer.Interval = 1000; // 1 second
            pingTimer.Tick += async (s, e) => await UpdatePingAsync();
            pingTimer.Start();

            // Initialize IP Timer
            ipTimer = new System.Windows.Forms.Timer();
            ipTimer.Interval = 20000; // 20 seconds
            ipTimer.Tick += async (s, e) => await UpdateIpAddressAsync();
            ipTimer.Start();

            // Initialize Display Texts with placeholders
            displayTexts.Add("Ping: -- ms");
            displayTexts.Add("IP: Checking...");
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
            this.Invalidate(); // Redraw with the updated text
        }

        private void UpdateIpText(string newText)
        {
            displayTexts[1] = newText;
            this.Invalidate(); // Redraw with the updated text
        }

        #endregion

        #region Data Retrieval Methods

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
                return "Error";
            }
        }

        private async Task<string> GetIpAddressAsync()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string ipAddress = await client.GetStringAsync("https://mbnq.pl/myip/");
                    return ipAddress.Trim();
                }
            }
            catch (Exception)
            {
                return "Unavailable";
            }
        }

        #endregion

        #region UI Methods

        private void TXTHUD_Paint(object sender, PaintEventArgs e)
        {
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
