/* 
    www.mbnq.pl 2024 
    mbnq00 on gmail 
*/

using System;
using System.Windows.Forms;
using System.IO;
using System.Drawing;

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

            var customFilePath = Path.Combine(SaveLoad.SettingsDirectory, "RED.custom.png");
            if (File.Exists(customFilePath))
            {
                using (var img = Image.FromFile(customFilePath))
                {
                    if (img.Width > 128 || img.Height > 128)
                    {
                        MessageBox.Show("The custom overlay is invalid and will be removed.", "Invalid Custom Overlay", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        File.Delete(customFilePath);
                    }
                }
            }

            mainDisplay = new MainDisplay();

            ControlPanel controlPanel = new ControlPanel
            {
                MainDisplay = mainDisplay,
            };

            SaveLoad.LoadSettings(controlPanel, false);
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
