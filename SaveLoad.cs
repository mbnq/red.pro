/* 
    www.mbnq.pl 2024 
    mbnq00 on gmail

    File io goes here
*/

using MaterialSkin.Controls;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Windows.Media;

namespace RED.mbnq
{
    public static class SaveLoad
    {
        private static readonly string settingsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "mbnqplSoft");
        private static readonly string settingsFilePath = Path.Combine(settingsDirectory, "RED.settings.sav");
        public static string SettingsDirectory => settingsDirectory;
        private static void EnsureDirectoryExists()
        {
            if (!Directory.Exists(settingsDirectory))
            {
                Debug.WriteLineIf(ControlPanel.mIsDebugOn, "mbnq: Creating folder for userfiles...");
                Directory.CreateDirectory(settingsDirectory);
            }
        }

        /* --- --- --- saving --- --- --- */
        public static async Task SaveSettings(ControlPanel controlPanel, bool showMessage = true)
        {
            var sb = new StringBuilder();

            sb.AppendLine(";Do not edit if you don't know what you're doing, please.");
            sb.AppendLine("[MainDisplay]");
            sb.AppendLine($"Red={controlPanel.ColorRValue}");
            sb.AppendLine($"Green={controlPanel.ColorGValue}");
            sb.AppendLine($"Blue={controlPanel.ColorBValue}");
            sb.AppendLine($"Size={controlPanel.SizeValue}");
            sb.AppendLine($"Transparency={controlPanel.TransparencyValue}");
            sb.AppendLine($"ZoomLevel={controlPanel.zoomLevel.Value}");
            sb.AppendLine($"OffsetX={controlPanel.OffsetXValue}");
            sb.AppendLine($"OffsetY={controlPanel.OffsetYValue}");
            sb.AppendLine($"TimerInterval={controlPanel.TimerIntervalValue}");
            sb.AppendLine($"AutoSaveOnExit={controlPanel.AutoSaveOnExitChecked}");
            sb.AppendLine($"SoundEnabled={Sounds.IsSoundEnabled}");
            sb.AppendLine($"ZoomEnabled={ZoomMode.IsZoomModeEnabled}");

            // Save overlay absolute position
            if (controlPanel.MainDisplay != null)
            {
                sb.AppendLine($"PositionX={controlPanel.MainDisplay.Left}");
                sb.AppendLine($"PositionY={controlPanel.MainDisplay.Top}");
            }

            // File.WriteAllText(settingsFilePath, sb.ToString());
            await Task.Run(() => File.WriteAllText(settingsFilePath, sb.ToString()));

            if (showMessage)
            {
                Sounds.PlayClickSoundOnce();
                MaterialMessageBox.Show("Settings saved.", "Save Settings", MessageBoxButtons.OK, MessageBoxIcon.None);
            }
            Debug.WriteLineIf(ControlPanel.mIsDebugOn, "mbnq: Settings saved.");
        }

        /* --- --- --- loading --- --- --- */
        public static void LoadSettings(ControlPanel controlPanel, bool showMessage = true)
        {
            if (!File.Exists(settingsFilePath))
            {
                Sounds.PlayClickSoundOnce();
                MaterialMessageBox.Show("Settings file not found.", "Load Settings", MessageBoxButtons.OK, MessageBoxIcon.None);
                Debug.WriteLineIf(ControlPanel.mIsDebugOn, "mbnq: Settings file not found.");
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
                else if (line.StartsWith("ZoomLevel="))
                    controlPanel.zoomLevel.Value = int.Parse(line.Substring("ZoomLevel=".Length));
                else if (line.StartsWith("OffsetX="))
                    controlPanel.OffsetXValue = int.Parse(line.Substring("OffsetX=".Length));
                else if (line.StartsWith("OffsetY="))
                    controlPanel.OffsetYValue = int.Parse(line.Substring("OffsetY=".Length));
                else if (line.StartsWith("TimerInterval="))
                    controlPanel.TimerIntervalValue = int.Parse(line.Substring("TimerInterval=".Length));
                else if (line.StartsWith("AutoSaveOnExit="))
                    controlPanel.AutoSaveOnExitChecked = bool.Parse(line.Substring("AutoSaveOnExit=".Length));
                else if (line.StartsWith("PositionX=") && controlPanel.MainDisplay != null)
                    controlPanel.MainDisplay.Left = int.Parse(line.Substring("PositionX=".Length));
                else if (line.StartsWith("PositionY=") && controlPanel.MainDisplay != null)
                    controlPanel.MainDisplay.Top = int.Parse(line.Substring("PositionY=".Length));
                else if (line.StartsWith("SoundEnabled="))
                    Sounds.IsSoundEnabled = bool.Parse(line.Substring("SoundEnabled=".Length));
                else if (line.StartsWith("ZoomEnabled="))
                {
                    bool.TryParse(line.Substring("ZoomEnabled=".Length), out bool zoomEnabled);
                    if (zoomEnabled != ZoomMode.IsZoomModeEnabled)
                    {
                        ZoomMode.ToggleZoomMode(); // This will correctly set the value
                    }
                }
            }

            controlPanel.UpdateMainDisplay();

            if (showMessage)
            {
                Sounds.PlayClickSoundOnce();
                MaterialMessageBox.Show("Settings loaded.", "Load Settings", MessageBoxButtons.OK, MessageBoxIcon.None);
            }

            controlPanel.mSettingsLoaded = 1;
            Debug.WriteLineIf(ControlPanel.mIsDebugOn, "mbnq: Settings Loaded.");
        }

        /* --- --- --- Check if savefile exists --- --- --- */
        public static void EnsureSettingsFileExists(ControlPanel controlPanel)
        {
            bool fileCreated = false;

            EnsureDirectoryExists();

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
                sb.AppendLine("ZoomLevel=3");
                sb.AppendLine("OffsetX=1000");
                sb.AppendLine("OffsetY=1000");
                sb.AppendLine("TimerInterval=1000");
                sb.AppendLine("AutoSaveOnExit=True");
                sb.AppendLine("SoundEnabled=True");
                sb.AppendLine("ZoomEnabled=False");

                File.WriteAllText(settingsFilePath, sb.ToString());
                fileCreated = true;

                Debug.WriteLineIf(ControlPanel.mIsDebugOn, "mbnq: Settings save file created.");
            }
            else
            {
                Debug.WriteLineIf(ControlPanel.mIsDebugOn, "mbnq: Settings save file found.");
            }

            // LoadSettings(controlPanel, false);

            if (fileCreated)
            {
                controlPanel.UpdateMainDisplay();
            }

            controlPanel.mSettingsLoaded = 1;
        }
    }
}
