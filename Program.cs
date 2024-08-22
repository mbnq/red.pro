using System;
using System.Windows.Forms;

namespace RED.mbnq
{
    static class Program
    {
        public static MainDisplay mainDisplay;
        public static SniperModeDisplay sniperModeDisplay;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Initialize MainDisplay and SniperModeDisplay
            mainDisplay = new MainDisplay();
            sniperModeDisplay = new SniperModeDisplay();
            sniperModeDisplay.Visible = false;  // Initially hide sniper mode

            // Create ControlPanel and pass MainDisplay and SniperModeDisplay
            ControlPanel controlPanel = new ControlPanel
            {
                MainDisplay = mainDisplay,
                SniperModeDisplay = sniperModeDisplay
            };

            // Load the settings and apply them immediately
            SaveLoad.LoadSettings(controlPanel, false);

            // Apply the settings to the MainDisplay
            controlPanel.UpdateMainDisplay();

            Application.Run(controlPanel);
        }
    }

}
