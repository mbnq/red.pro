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
            sniperModeDisplay = new SniperModeDisplay();

            // Start the ControlPanel
            ControlPanel controlPanel = new ControlPanel();
            controlPanel.MainDisplay = mainDisplay;
            controlPanel.SniperModeDisplay = sniperModeDisplay;

            Application.Run(controlPanel);
        }
    }
}
