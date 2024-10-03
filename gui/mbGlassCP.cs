
/* 

    www.mbnq.pl 2024 
    https://mbnq.pl/
    mbnq00 on gmail

*/

using MaterialSkin.Controls;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using static mbFnc;

namespace RED.mbnq
{
    public partial class mbGlassCP : MaterialForm
    {
        private GlassHudOverlay overlay;
        private Timer debugInfoTimer;
        private Action playSND = Sounds.PlayClickSoundOnce;
        private PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        public mbGlassCP(GlassHudOverlay overlay)
        {
            var materialSkinManager = MaterialSkin.MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);

            this.StartPosition = FormStartPosition.CenterParent;

            InitializeComponent();

            this.overlay = overlay;

            this.FormClosing += (sender, e) =>
            {
                if (!Program.mbGlobalExitInProgress)
                {
                    Sounds.PlayClickSoundOnce();
                    overlay.ToggleGlassMenu();
                    e.Cancel = true; 
                } 
                else
                {
                    Sounds.PlayClickSoundOnce();
                    overlay.ToggleGlassMenu();
                }
            };

            InitializeCPApplyValues();
            AddCPEventHandlers();
            InitializeCPInfoTimer();
        }
        private void InitializeCPApplyValues()
        {
            gcpRRSlider.Value = overlay.glassRefreshRate;
            gcpOffX.Value = (overlay.glassOffsetXValue + 100) / 2;
            gcpOffY.Value = (overlay.glassOffsetYValue + 100) / 2;
            gcpZoom.Value = overlay.glassZoomValue;
            gcpAlpha.Value = overlay.glassOpacityValue;
            gcpBindButton.Text = overlay.glassIsBind ? "Bind" : "Unbind";
            gcpReshapeButton.Text = overlay.glassIsCircle ? "Square" : "Circle";
            gcpBorderButton.Text = overlay.glassIsBorderVisible ? "Unframe" : "Frame";
        }
        private void AddCPEventHandlers()
        {
            gcpRRSlider.onValueChanged += (s, e) => {
                if (gcpRRSlider.Value < 1) gcpRRSlider.Value = 1;
                overlay.glassRefreshRate = gcpRRSlider.Value;
            };
            gcpOffX.onValueChanged += (s, e) => {
                overlay.glassOffsetXValue = (2 * gcpOffX.Value - 100);
            };
            gcpOffY.onValueChanged += (s, e) => {
                overlay.glassOffsetYValue = (2 * gcpOffY.Value - 100);
            };
            gcpZoom.onValueChanged += (s, e) => {
                if (gcpZoom.Value < 1) gcpZoom.Value = 1;
                overlay.glassZoomValue = gcpZoom.Value;
            };
            gcpAlpha.onValueChanged += (s, e) => {
                if (gcpAlpha.Value < 1) gcpAlpha.Value = 1;
                overlay.glassOpacityValue = gcpAlpha.Value;
            };
        }
        private void InitializeCPInfoTimer()
        {
            debugInfoTimer = new Timer { Interval = 500 }; // ms
            debugInfoTimer.Tick += (s, e) => UpdateCPInfo();
            debugInfoTimer.Start();
        }
        private async void UpdateCPInfo()
        {
            if (overlay == null)
                return;

            float cpuUsage = await mbGetCpuUsageAsync();
            Rectangle adjustedRegion = overlay.GetAdjustedCaptureArea();
            DateTime mbDateTime = DateTime.Now;

            string[] debugLines = {
                $"RED.PRO - Glass Info - v.{Program.mbVersion} - {mbDateTime}",
                $"Displaying region: Top-Left({adjustedRegion.X}, {adjustedRegion.Y}) Size({adjustedRegion.Width}x{adjustedRegion.Height})",
                $"FPS: {overlay.currentFps:F2} Frame Time: {overlay.GlassFrameTime:F4}s CPU Usage: {cpuUsage:F2}%"
            };

            materialMultiLineTextBox1.Text = string.Join(Environment.NewLine, debugLines);
        }

        // just in case, shouldn't be needed since we have e.Cancel = true in this.FormClosing 
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (debugInfoTimer != null)
            {
                debugInfoTimer.Stop();
                debugInfoTimer.Dispose();
                debugInfoTimer = null;
            }
        }
        private async void gcpLoadSettingsButton_Click(object sender, EventArgs e)
        {
            await SaveLoad.mbLoadGlassSettingsNew(overlay);
            InitializeCPApplyValues();
            this.Refresh();
            playSND();
            gcpLoadSettingsButton.Text = "Loaded!";
            gcpLoadSettingsButton.Enabled = false;
            await Task.Delay(2000);
            gcpLoadSettingsButton.Text = "Load";
            gcpLoadSettingsButton.Enabled = true;
        }
        private async void gcpSaveSettingsButton_Click(object sender, EventArgs e)
        {
            SaveLoad.mbSaveGlassSettings(overlay);
            playSND();
            gcpSaveSettingsButton.Text = "Saved!";
            gcpSaveSettingsButton.Enabled = false;
            await Task.Delay(2000);
            gcpSaveSettingsButton.Text = "Save";
            gcpSaveSettingsButton.Enabled = true;
        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            overlay.Close();
        }
        private void gcpReshapeButton_Click(object sender, EventArgs e)
        {
            overlay.ToggleShape();
            gcpReshapeButton.Text = overlay.glassIsCircle ? "Square" : "Circle";
            playSND();
        }

        private void gcpBorderButton_Click(object sender, EventArgs e)
        {
            overlay.ToggleFrameVisibility();
            gcpBorderButton.Text = overlay.glassIsBorderVisible ? "Unframe" : "Frame";
            playSND();
        }

        private void gcpBindButton_Click(object sender, EventArgs e)
        {
            overlay.ToggleMoveOption();
            gcpBindButton.Text = overlay.glassIsBind ? "Bind" : "Unbind";
            playSND();
        }

        private async void gcpNewAreaButton_Click(object sender, EventArgs e)
        {
            overlay.glassOffsetXValue = 0;
            overlay.glassOffsetYValue = 0;
            playSND();
            await GlassHudOverlay.RestartWithNewAreaAsync();
        }
    }
}
