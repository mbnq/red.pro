using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;

namespace RED.mbnq
{
    public class mbnqTXTHUD : Form
    {
        private List<string> displayTexts = new List<string>();
        private CancellationTokenSource cancellationTokenSource;
        private System.Windows.Forms.Timer updateTimer;  // Explicitly use the Windows Forms Timer

        public mbnqTXTHUD()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(0, 0); // Top-left corner
            this.Size = new Size(400, 50); // Adjust size as needed
            this.TopMost = true;
            this.BackColor = Color.Black; // Set background color
            this.Opacity = 0.65; // Set transparency
            this.ShowInTaskbar = false;
            this.DoubleBuffered = true;
            this.Paint += TXTHUD_Paint;

            updateTimer = new System.Windows.Forms.Timer();
            updateTimer.Interval = 1000;
            updateTimer.Tick += async (s, e) => await UpdateHUDAsync();
            updateTimer.Start();
        }

        private async Task UpdateHUDAsync()
        {
            await UpdateIpAddressAsync();
            this.Invalidate(); // Redraw the HUD
        }

        private async Task UpdateIpAddressAsync()
        {
            string ipAddress = await GetIpAddressAsync();
            UpdateIpText($"IP: {ipAddress}");
        }

        private void UpdateIpText(string newText)
        {
            if (displayTexts.Count > 1)
            {
                displayTexts[1] = newText; // Assume second line is the IP text
            }
            else
            {
                AddText(newText);
            }
        }

        public void AddText(string text)
        {
            displayTexts.Add(text);
            this.Invalidate(); // Redraw with the new text
        }

        public void ClearTexts()
        {
            displayTexts.Clear();
            this.Invalidate(); // Redraw after clearing
        }

        private void TXTHUD_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            using (Brush brush = new SolidBrush(Color.White))
            {
                float yPosition = 10f;
                foreach (var text in displayTexts)
                {
                    g.DrawString(text, this.Font, brush, new PointF(10, yPosition));
                    yPosition += 20f; // Adjust line spacing as needed
                }
            }
        }

        public void StartPingLoop(string address)
        {
            cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;

            Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    string pingResult = await GetPingResultAsync(address);
                    UpdatePingText($"Ping {address}: {pingResult} ms");
                    await Task.Delay(1000); // Update every second
                }
            }, token);
        }

        private void UpdatePingText(string newText)
        {
            if (displayTexts.Count > 0)
            {
                displayTexts[0] = newText; // Assume first line is the ping text
            }
            else
            {
                AddText(newText);
            }
            this.Invalidate(); // Redraw with the updated text
        }

        private Task<string> GetPingResultAsync(string address)
        {
            return Task.Run(() =>
            {
                using (Process p = new Process())
                {
                    p.StartInfo.FileName = "ping";
                    p.StartInfo.Arguments = address + " -n 1"; // Ping once
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.CreateNoWindow = true;
                    p.Start();
                    string output = p.StandardOutput.ReadToEnd();
                    p.WaitForExit();

                    // Extract the ping time from the output
                    var pingTimeLine = output.Split('\n').FirstOrDefault(line => line.Contains("time="));
                    if (pingTimeLine != null)
                    {
                        var pingTimeStart = pingTimeLine.IndexOf("time=") + 5;
                        var pingTimeEnd = pingTimeLine.IndexOf("ms", pingTimeStart);
                        return pingTimeLine.Substring(pingTimeStart, pingTimeEnd - pingTimeStart).Trim();
                    }

                    return "N/A";
                }
            });
        }

        public void StopPingLoop()
        {
            cancellationTokenSource?.Cancel();
        }

        public async Task<string> GetIpAddressAsync()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string ipAddress = await client.GetStringAsync("https://mbnq.pl/myip/");
                    return ipAddress.Trim();
                }
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            StopPingLoop();
            updateTimer.Stop();
            updateTimer.Dispose();
            base.OnFormClosing(e);
        }
    }
}
