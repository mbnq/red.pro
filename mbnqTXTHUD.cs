using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RED.mbnq
{
    public class mbnqTXTHUD : Form
    {
        private List<string> displayTexts = new List<string>();
        private Timer updateTimer;

        public mbnqTXTHUD()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(0, 0); // Top-left corner
            this.Size = new Size(400, 100); // Adjust size as needed
            this.TopMost = true;
            this.BackColor = Color.Black; // Set background color
            this.Opacity = 0.75; // Set transparency
            this.ShowInTaskbar = false;
            this.DoubleBuffered = true;
            this.Paint += TXTHUD_Paint;

            updateTimer = new Timer();
            updateTimer.Interval = 1000; // Update every second
            updateTimer.Tick += (s, e) => this.Invalidate();
            updateTimer.Start();
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

        public async void DisplayPingResult(string address)
        {
            string pingResult = await GetPingResultAsync(address);
            AddText($"Ping {address}: {pingResult} ms");
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

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            updateTimer.Stop();
            updateTimer.Dispose();
            base.OnFormClosing(e);
        }
    }
}
