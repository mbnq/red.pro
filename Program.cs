using System;
using System.IO;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RED.mbnq
{
    static class Program
    {
        public static MainDisplay mainDisplay;
        public static GlassHudOverlay? displayOverlay;
        public static int mbFrameDelay = 8; // in ms
        public static float mbVersion = 0.02f;

        [DllImport("user32.dll")]
        static extern bool SetProcessDPIAware();

        [STAThread]
        static void Main()
        {
            SetProcessDPIAware();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Initialize MainDisplay
            mainDisplay = new MainDisplay();

            // Initialize ControlPanel
            ControlPanel controlPanel = new ControlPanel
            {
                MainDisplay = mainDisplay,
            };

            ZoomMode.InitializeZoomMode(controlPanel);
            GlobalMouseHook.SetHook();

            // Load settings and update display
            SaveLoad.LoadSettings(controlPanel, false);

            // Check for a custom overlay file
            var customFilePath = Path.Combine(SaveLoad.SettingsDirectory, "RED.custom.png");
            if (File.Exists(customFilePath))
            {
                mainDisplay.SetCustomOverlay();
            }

            // Update the main display after settings have loaded
            controlPanel.UpdateMainDisplay();

            controlPanel.FormClosing += (sender, e) =>
            {
                if (controlPanel.AutoSaveOnExitChecked)
                {
                    SaveLoad.SaveSettings(controlPanel, false);
                }
            };

            Application.Run(controlPanel); // This will run the main display and overlay together
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
                    return Rectangle.Empty; // Unreachable, but necessary for compilation
                }
            }
        }

        public static void RestartWithNewArea()
        {
            if (displayOverlay != null)
            {
                displayOverlay.Hide(); // Hide current overlay

                Rectangle newArea = SelectCaptureArea();

                displayOverlay.UpdateCaptureArea(newArea); // Update the area instead of recreating the form
                displayOverlay.Show(); // Show the updated overlay
            }
        }
    }
}
