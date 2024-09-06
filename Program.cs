using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RED.mbnq
{
    static class Program
    {
        public static mbnqCrosshair mainCrosshair;
        public static int mbFrameDelay = 16;     // in ms, for glass hud, default 60fps
        public static float mbVersion = 0.060f;

        [DllImport("user32.dll")]
        static extern bool SetProcessDPIAware();

        [STAThread]
        static void Main()
        {
            SetProcessDPIAware();   // this is co crucial to deal with windows DPI desktop scaling...

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Initialize MainDisplay
            mainCrosshair = new mbnqCrosshair();

            // Initialize ControlPanel
            ControlPanel controlPanel = new ControlPanel
            {
                mbnqCrosshairOverlay = mainCrosshair,
            };

            ZoomMode.InitializeZoomMode(controlPanel);
            GlobalMouseHook.SetHook();

            // Load settings and update display
            SaveLoad.LoadSettings(controlPanel, false);

            // Check for a custom overlay file
            var customFilePath = Path.Combine(SaveLoad.SettingsDirectory, "RED.custom.png");
            if (File.Exists(customFilePath))
            {
                mainCrosshair.SetCustomPNG();
            }

            // Update the main display after settings have loaded
            controlPanel.updateMainCrosshair();

            controlPanel.FormClosing += (sender, e) =>
            {
                if (controlPanel.AutoSaveOnExitChecked)
                {
                    SaveLoad.SaveSettings(controlPanel, false);
                }
            };

            Application.Run(controlPanel); // This will run the main display and overlay together
        }
    }
}
