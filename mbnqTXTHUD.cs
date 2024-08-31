using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RED.mbnq
{
    public class mbnqTXTHUD : Form
    {
        private string displayText = "";
        private Timer updateTimer;

        public mbnqTXTHUD()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(0, 0); // Top-left corner
            this.Size = new Size(400, 50); // Adjust size as needed
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

        public void DisplayText(string text)
        {
            displayText = text;
            this.Invalidate(); // Redraw with the new text
        }

        private void TXTHUD_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            using (Brush brush = new SolidBrush(Color.White))
            {
                g.DrawString(displayText, this.Font, brush, new PointF(10, 10));
            }
        }

        public async void DisplayPingResult(string address)
        {
            string pingResult = await GetPingResultAsync(address);
            DisplayText(pingResult);
        }

        private Task<string> GetPingResultAsync(string address)
        {
            return Task.Run(() =>
            {
                using (Process p = new Process())
                {
                    p.StartInfo.FileName = "ping";
                    p.StartInfo.Arguments = address;
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.CreateNoWindow = true;
                    p.Start();
                    string output = p.StandardOutput.ReadToEnd();
                    p.WaitForExit();
                    return output;
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
