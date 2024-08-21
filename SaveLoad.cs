using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace RED.mbnq
{
    public static class SaveLoad
    {
        private static string settingsFilePath = "RED.mbnq.settings.ini";
        public static void EnsureSettingsFileExists(ControlPanel controlPanel)
        {
            if (!File.Exists(settingsFilePath))
            {
                // Create default settings
                var sb = new StringBuilder();

                sb.AppendLine("[MainDisplay]");
                sb.AppendLine("Red=255");
                sb.AppendLine("Green=0");
                sb.AppendLine("Blue=0");
                sb.AppendLine("Size=3");
                sb.AppendLine("Transparency=64");
                sb.AppendLine("OffsetX=0");
                sb.AppendLine("OffsetY=0");
                sb.AppendLine("TimerInterval=100");
                sb.AppendLine("LockMainDisplay=False");
                sb.AppendLine("SniperMode=False");

                File.WriteAllText(settingsFilePath, sb.ToString());

                // Adjust controls to default settings
                controlPanel.ColorRValue = 255;
                controlPanel.ColorGValue = 0;
                controlPanel.ColorBValue = 0;
                controlPanel.SizeValue = 3;
                controlPanel.TransparencyValue = 64;
                controlPanel.OffsetXValue = 0;
                controlPanel.OffsetYValue = 0;
                controlPanel.TimerIntervalValue = 100;
                controlPanel.LockMainDisplayChecked = false;
                controlPanel.SniperModeChecked = false;
            }
            else
            {
                // Load existing settings
                LoadSettings(controlPanel);
            }

            // Ensure MainDisplay is updated with the loaded or default settings
            controlPanel.UpdateMainDisplay();
        }
        public static void SaveSettings(ControlPanel controlPanel)
        {
            var sb = new StringBuilder();

            sb.AppendLine("[MainDisplay]");
            sb.AppendLine($"Red={controlPanel.ColorRValue}");
            sb.AppendLine($"Green={controlPanel.ColorGValue}");
            sb.AppendLine($"Blue={controlPanel.ColorBValue}");
            sb.AppendLine($"Size={controlPanel.SizeValue}");
            sb.AppendLine($"Transparency={controlPanel.TransparencyValue}");
            sb.AppendLine($"OffsetX={controlPanel.OffsetXValue}");
            sb.AppendLine($"OffsetY={controlPanel.OffsetYValue}");
            sb.AppendLine($"TimerInterval={controlPanel.TimerIntervalValue}");
            sb.AppendLine($"LockMainDisplay={controlPanel.LockMainDisplayChecked}");
            sb.AppendLine($"SniperMode={controlPanel.SniperModeChecked}");

            File.WriteAllText(settingsFilePath, sb.ToString());
            MessageBox.Show("Settings saved successfully.", "Save Settings", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void LoadSettings(ControlPanel controlPanel)
        {
            if (!File.Exists(settingsFilePath))
            {
                MessageBox.Show("Settings file not found.", "Load Settings", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var lines = File.ReadAllLines(settingsFilePath);
            foreach (var line in lines)
            {
                if (line.StartsWith("Red="))
                    controlPanel.ColorRValue = int.Parse(line.Substring("Red=".Length));
                else if (line.StartsWith("Green="))
                    controlPanel.ColorGValue = int.Parse(line.Substring("Green=".Length));
                else if (line.StartsWith("Blue="))
                    controlPanel.ColorBValue = int.Parse(line.Substring("Blue=".Length));
                else if (line.StartsWith("Size="))
                    controlPanel.SizeValue = int.Parse(line.Substring("Size=".Length));
                else if (line.StartsWith("Transparency="))
                    controlPanel.TransparencyValue = int.Parse(line.Substring("Transparency=".Length));
                else if (line.StartsWith("OffsetX="))
                    controlPanel.OffsetXValue = int.Parse(line.Substring("OffsetX=".Length));
                else if (line.StartsWith("OffsetY="))
                    controlPanel.OffsetYValue = int.Parse(line.Substring("OffsetY=".Length));
                else if (line.StartsWith("TimerInterval="))
                    controlPanel.TimerIntervalValue = int.Parse(line.Substring("TimerInterval=".Length));
                else if (line.StartsWith("LockMainDisplay="))
                    controlPanel.LockMainDisplayChecked = bool.Parse(line.Substring("LockMainDisplay=".Length));
                else if (line.StartsWith("SniperMode="))
                    controlPanel.SniperModeChecked = bool.Parse(line.Substring("SniperMode=".Length));
            }

            controlPanel.UpdateMainDisplay();
            MessageBox.Show("Settings loaded successfully.", "Load Settings", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
