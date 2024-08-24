/* 
    www.mbnq.pl 2024 
    mbnq00 on gmail
*/

using System;
using System.Windows.Forms;

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

            mainDisplay = new MainDisplay();

            ControlPanel controlPanel = new ControlPanel
            {
                MainDisplay = mainDisplay,
            };

            SaveLoad.LoadSettings(controlPanel, false);
            controlPanel.UpdateMainDisplay();

            controlPanel.FormClosing += (sender, e) =>
            {
                if (controlPanel.AutoSaveOnExitChecked)
                {
                    SaveLoad.SaveSettings(controlPanel, false);
                }
            };

            Application.Run(controlPanel);
        }
    }
}
