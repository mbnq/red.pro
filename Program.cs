﻿using System;
using System.IO;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RED.mbnq
{
    static class Program
    {
        public static mbnqCrosshair mainDisplay;
        public static int mbFrameDelay = 8;     // in ms, for glass hud
        public static float mbVersion = 0.043f;

        [DllImport("user32.dll")]
        static extern bool SetProcessDPIAware();

        [STAThread]
        static void Main()
        {
            SetProcessDPIAware();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Initialize MainDisplay
            mainDisplay = new mbnqCrosshair();

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
    }
}
