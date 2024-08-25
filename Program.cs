/* 
    www.mbnq.pl 2024 
    mbnq00 on gmail 
*/

using System;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;

namespace RED.mbnq
{
    static class Program
    {
        public static MainDisplay mainDisplay;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Initialize MainDisplay
            mainDisplay = new MainDisplay();

            // Initialize ControlPanel
            ControlPanel controlPanel = new ControlPanel
            {
                MainDisplay = mainDisplay,
            };

            ZoomMode.InitializeZoomMode();

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

            controlPanel.FormClosing += (sender, e) => {
                if (controlPanel.AutoSaveOnExitChecked)
                {
                    SaveLoad.SaveSettings(controlPanel, false);
                }
            };

            Application.Run(controlPanel);
        }
    }
}
