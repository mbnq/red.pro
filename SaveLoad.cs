using System.IO;
using System.Text;
using System.Windows.Forms;
using MaterialSkin;             // let's leave it here for now, just in case
using MaterialSkin.Controls;

namespace RED.mbnq
{
    public static class SaveLoad
    {
        private static string settingsFilePath = "RED.settings.sav";
        public static void SaveSettings(ControlPanel controlPanel, bool showMessage = true)
        {
            var sb = new StringBuilder();

            sb.AppendLine(";Do not edit if you don't know what you're doing, please.");
            sb.AppendLine("[MainDisplay]");
            sb.AppendLine($"Red={controlPanel.ColorRValue}");
            sb.AppendLine($"Green={controlPanel.ColorGValue}");
            sb.AppendLine($"Blue={controlPanel.ColorBValue}");
            sb.AppendLine($"Size={controlPanel.SizeValue}");
            sb.AppendLine($"Transparency={controlPanel.TransparencyValue}");
            sb.AppendLine($"OffsetX={controlPanel.OffsetXValue}");
            sb.AppendLine($"OffsetY={controlPanel.OffsetYValue}");
            sb.AppendLine($"TimerInterval={controlPanel.TimerIntervalValue}");
            sb.AppendLine($"AutoSaveOnExit={controlPanel.AutoSaveOnExitChecked}"); // Save AutoSaveOnExit state

            // Save overlay absolute position
            if (controlPanel.MainDisplay != null)
            {
                sb.AppendLine($"PositionX={controlPanel.MainDisplay.Left}");
                sb.AppendLine($"PositionY={controlPanel.MainDisplay.Top}");
            }

            File.WriteAllText(settingsFilePath, sb.ToString());

            if (showMessage)
            {
                MaterialMessageBox.Show("Settings saved.", "Save Settings", MessageBoxButtons.OK, MessageBoxIcon.None);
            }
        }
        public static void LoadSettings(ControlPanel controlPanel, bool showMessage = true)
        {
            if (!File.Exists(settingsFilePath))
            {
                MaterialMessageBox.Show("Settings file not found.", "Load Settings", MessageBoxButtons.OK, MessageBoxIcon.None);
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
                else if (line.StartsWith("AutoSaveOnExit="))
                    controlPanel.AutoSaveOnExitChecked = bool.Parse(line.Substring("AutoSaveOnExit=".Length)); // Load AutoSaveOnExit state
                else if (line.StartsWith("PositionX=") && controlPanel.MainDisplay != null)
                    controlPanel.MainDisplay.Left = int.Parse(line.Substring("PositionX=".Length));
                else if (line.StartsWith("PositionY=") && controlPanel.MainDisplay != null)
                    controlPanel.MainDisplay.Top = int.Parse(line.Substring("PositionY=".Length));
            }

            controlPanel.UpdateMainDisplay();

            if (showMessage)
            {
                MaterialMessageBox.Show("Settings loaded.", "Load Settings", MessageBoxButtons.OK, MessageBoxIcon.None);
                controlPanel.Activate();
            }
        }
        public static void EnsureSettingsFileExists(ControlPanel controlPanel)
        {
            bool fileCreated = false;

            if (!File.Exists(settingsFilePath))
            {
                // Create default settings
                var sb = new StringBuilder();

                sb.AppendLine(";Do not edit if you don't know what you're doing, please.");
                sb.AppendLine("[MainDisplay]");
                sb.AppendLine("Red=255");
                sb.AppendLine("Green=0");
                sb.AppendLine("Blue=0");
                sb.AppendLine("Size=4");
                sb.AppendLine("Transparency=64");
                sb.AppendLine("OffsetX=1000");
                sb.AppendLine("OffsetY=1000");
                sb.AppendLine("TimerInterval=1000");
                sb.AppendLine("AutoSaveOnExit=True");  // Default to true

                File.WriteAllText(settingsFilePath, sb.ToString());
                fileCreated = true;
            }

            LoadSettings(controlPanel, false);

            if (fileCreated)
            {
                controlPanel.UpdateMainDisplay();
            }
        }
    }
}
