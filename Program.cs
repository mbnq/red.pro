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

            mainDisplay = new MainDisplay();
            sniperModeDisplay = new SniperModeDisplay();
            sniperModeDisplay.Visible = false;

            ControlPanel controlPanel = new ControlPanel
            {
                MainDisplay = mainDisplay,
                SniperModeDisplay = sniperModeDisplay
            };

            SaveLoad.LoadSettings(controlPanel, false);
            controlPanel.UpdateMainDisplay();

            controlPanel.FormClosing += (sender, e) =>
            {
                if (controlPanel.AutoSaveOnExitChecked)
                {
                    // Save settings on exit without showing the message
                    SaveLoad.SaveSettings(controlPanel, false);
                }
            };

            Application.Run(controlPanel);
        }
    }
}
