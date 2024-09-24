
/* 

    www.mbnq.pl 2024 
    https://mbnq.pl/
    mbnq00 on gmail

*/

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;

namespace RED.mbnq
{
    static class Program
    {
        #region variables and constants
        public static mbCrosshair mainCrosshair;
        public const int mbFrameDelay = 16;     // in ms, for glass hud, default 60fps 
        public const string mbVersion = "0.1.1.1";
        #endregion

        #region DPI
        [DllImport("user32.dll")]
        static extern bool SetProcessDPIAware();

        [DllImport("user32.dll")]
        private static extern bool SetProcessDpiAwarenessContext(IntPtr dpiFlag);
        private static readonly IntPtr DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2 = new IntPtr(-4);
        private static readonly IntPtr DPI_AWARENESS_CONTEXT_SYSTEM_AWARE = new IntPtr(-2);
        #endregion

        [STAThread]
        static void Main()
        {
            SetProcessDPIAware();               // this is co crucial to deal with windows DPI desktop scaling...
            SetDpiAwareness();                  // this is an alternative

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            #region init sequence

            // Crosshair init
            mainCrosshair = new mbCrosshair();

            // ControlPanel init
            ControlPanel controlPanel = new ControlPanel { mbCrosshairOverlay = mainCrosshair };

            // SniperMode init
            ZoomMode.InitializeZoomMode(controlPanel);
            GlobalMouseHook.SetHook();

            // Load settings
            SaveLoad.mbLoadSettings(controlPanel);

            // Check for a custom crosshair overlay file
            var customFilePath = Path.Combine(ControlPanel.mbUserFilesPath, "RED.custom.png");
            if (File.Exists(customFilePath)) { mainCrosshair.SetCustomPNG(); }

            // Update all after loading settings
            controlPanel.UpdateAllUI();

            // autoSave on exist eventHandler
            controlPanel.FormClosing += (sender, e) =>
            {
                if (controlPanel.mbAutoSaveCheckbox.Checked) { SaveLoad.mbSaveSettings(controlPanel); }
            };

            // hiding controlPanel before showing splashScreen
            controlPanel.Visible = false;
            controlPanel.Size = new Size(0, 0);
            Application.Run(controlPanel);

            #endregion

            #region DpiAwereness helper fnc
            static void SetDpiAwareness()
            {
                try
                {
                    // Try to use the latest DPI awareness method available in Windows 10
                    if (Environment.OSVersion.Version.Major >= 10)
                    {
                        if (SetProcessDpiAwarenessContext(DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2))
                        {
                            Console.WriteLine("DPI Awareness set to Per-Monitor Aware (V2).");
                            return;
                        }
                    }

                    // If running on older versions, fallback to the older DPI aware method
                    if (!SetProcessDPIAware())
                    {
                        throw new Exception("Failed to set DPI Awareness.");
                    }

                    Console.WriteLine("DPI Awareness set to System DPI Aware.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"DPI Awareness could not be set: {ex.Message}");
                }
            }
            #endregion
        }
    }
}
