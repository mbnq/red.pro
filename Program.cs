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

            // Create and show the ControlPanel
            ControlPanel controlPanel = new ControlPanel
            {
                MainDisplay = mainDisplay,
                SniperModeDisplay = sniperModeDisplay
            };

            // Center the MainDisplay immediately after creating it
            controlPanel.CenterMainDisplay();

            // Ensure MainDisplay is updated after ControlPanel is created
            controlPanel.UpdateMainDisplay();

            Application.Run(controlPanel);
        }
    }
}
