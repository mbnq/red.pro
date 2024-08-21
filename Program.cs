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

            // Create and show the overlays
            mainDisplay = new MainDisplay();
            mainDisplay.Show();  // Ensure this line is present

            sniperModeDisplay = new SniperModeDisplay();
            sniperModeDisplay.Show();  // Ensure this line is present

            // Start the ControlPanel
            ControlPanel controlPanel = new ControlPanel();
            controlPanel.MainDisplay = mainDisplay;
            controlPanel.SniperModeDisplay = sniperModeDisplay;

            Application.Run(controlPanel);
        }
    }
}
