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

            // Create and show the main display overlay
            mainDisplay = new MainDisplay();
            mainDisplay.Show();

            // Create the sniper mode overlay, but don't show it yet
            sniperModeDisplay = new SniperModeDisplay();
            sniperModeDisplay.Visible = false;  // Set visibility to false at startup

            // Start the ControlPanel
            ControlPanel controlPanel = new ControlPanel();
            controlPanel.MainDisplay = mainDisplay;
            controlPanel.SniperModeDisplay = sniperModeDisplay;

            Application.Run(controlPanel);
        }
    }
}
