/* 
    
    www.mbnq.pl 2024 
    https://mbnq.pl/
    mbnq00 on gmail

    Example usage:
                                        filename       section    keyname def

            SaveLoad2.INIFile.INIsave("settings.ini", "Settings", "Test", 100);
            volume = INIFile.INIread("settings.ini", "Settings", "Test", 0);
            MessageBox.Show($"Loaded variable value:{volume}", "Test", MessageBoxButtons.OK, MessageBoxIcon.None);
*/

using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace RED.mbnq
{
    public static class SaveLoad2
    {
        private static readonly string settingsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "mbnqplSoft");
        private static string SettingsDirectory => settingsDirectory;
        private static bool IsDebugOn = ControlPanel.mIsDebugOn;
        private static void EnsureDirectoryExists()
        {
            if (!Directory.Exists(settingsDirectory))
            {
                Debug.WriteLineIf(IsDebugOn, "mbnq: Creating folder for user files...");
                Directory.CreateDirectory(settingsDirectory);
            }
        }
        public static class INIFile
        {
            public static T INIread<T>(string fileName, string section, string key, T defaultValue)
            {
                string filePath = Path.Combine(settingsDirectory, fileName);

                StringBuilder result = new StringBuilder(255);
                GetPrivateProfileString(section, key, "", result, result.Capacity, filePath);

                string value = result.ToString();

                if (string.IsNullOrWhiteSpace(value))
                {
                    return defaultValue;
                }

                try
                {
                    if (typeof(T) == typeof(int))
                    {
                        return (T)(object)int.Parse(value);
                    }
                    else if (typeof(T) == typeof(bool))
                    {
                        return (T)(object)bool.Parse(value);
                    }
                    else if (typeof(T) == typeof(double))
                    {
                        return (T)(object)double.Parse(value);
                    }
                    else
                    {
                        // For types like string
                        return (T)(object)value;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLineIf(IsDebugOn, $"Error parsing INI value: {ex.Message}");
                    return defaultValue;
                }
            }
            public static void INIsave<T>(string fileName, string section, string key, T value)
            {
                string filePath = Path.Combine(settingsDirectory, fileName);
                string valueToSave = value.ToString();
                bool success = WritePrivateProfileString(section, key, valueToSave, filePath);

                if (!success && IsDebugOn)
                {
                    Debug.WriteLine($"Failed to write to INI file: {filePath}");
                }
            }

            [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
            private static extern bool WritePrivateProfileString(string section, string key, string value, string filePath);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
            private static extern int GetPrivateProfileString(string section, string key, string defaultValue, StringBuilder result, int size, string filePath);
        }

        // Usage: SaveLoad2.mbSaveSettings2(this);
        public static void mbSaveSettings2(ControlPanel controlPanel)
        {
            // general
            SaveLoad2.INIFile.INIsave("settings.ini", "General", "AutoSaveOnExit", controlPanel.AutoSaveOnExitChecked);
            SaveLoad2.INIFile.INIsave("settings.ini", "General", "mbDebugon", controlPanel.mbDebugonChecked);
            SaveLoad2.INIFile.INIsave("settings.ini", "General", "mbAOnTop", controlPanel.mbAOnTopChecked);
            SaveLoad2.INIFile.INIsave("settings.ini", "General", "mbDisableSound", controlPanel.mbDisableSoundChecked);
            SaveLoad2.INIFile.INIsave("settings.ini", "General", "mbEnableDarkMode", controlPanel.mbDarkModeCheckBoxChecked);
            SaveLoad2.INIFile.INIsave("settings.ini", "General", "mbEnableSplashScreen", controlPanel.mbSplashCheckBoxChecked);
            SaveLoad2.INIFile.INIsave("settings.ini", "General", "mbEnableAntiCapsLock", controlPanel.mbAntiCapsCheckBoxChecked);
            SaveLoad2.INIFile.INIsave("settings.ini", "General", "mbEnableFlirMode", controlPanel.mbEnableFlirChecked);

            // crosshair
            SaveLoad2.INIFile.INIsave("settings.ini", "Crosshair", "ColorRValue", controlPanel.ColorRValue);
            SaveLoad2.INIFile.INIsave("settings.ini", "Crosshair", "ColorGValue", controlPanel.ColorGValue);
            SaveLoad2.INIFile.INIsave("settings.ini", "Crosshair", "ColorBValue", controlPanel.ColorBValue);
            SaveLoad2.INIFile.INIsave("settings.ini", "Crosshair", "SizeValue", controlPanel.SizeValue);
            SaveLoad2.INIFile.INIsave("settings.ini", "Crosshair", "TransparencyValue", controlPanel.TransparencyValue);
            SaveLoad2.INIFile.INIsave("settings.ini", "Crosshair", "OffsetXValue", controlPanel.OffsetXValue);
            SaveLoad2.INIFile.INIsave("settings.ini", "Crosshair", "OffsetYValue", controlPanel.OffsetYValue);
            SaveLoad2.INIFile.INIsave("settings.ini", "Crosshair", "mbHideCrosshair", controlPanel.mbHideCrosshairChecked);
            if (controlPanel.mbCrosshairOverlay != null) SaveLoad2.INIFile.INIsave("settings.ini", "General", "PositionX", controlPanel.mbCrosshairOverlay.Left);
            if (controlPanel.mbCrosshairOverlay != null) SaveLoad2.INIFile.INIsave("settings.ini", "General", "PositionY", controlPanel.mbCrosshairOverlay.Top);

            // zoomMode aka sniperMode
            SaveLoad2.INIFile.INIsave("settings.ini", "ZoomMode", "ZoomLevel", controlPanel.zoomLevel.Value);
            SaveLoad2.INIFile.INIsave("settings.ini", "ZoomMode", "mbEnableZoomMode", controlPanel.mbEnableZoomModeChecked);
        }

        // Usage: volume = INIFile.INIread("settings.ini", "Settings", "Test", 0);
        // Usage: SaveLoad2.mbSaveSettings2(this);
        // Usage: SaveLoad2.LoadSettings2(this);
        public static void LoadSettings2(ControlPanel controlPanel)
        {
            controlPanel.AutoSaveOnExitChecked = SaveLoad2.INIFile.INIread("settings.ini", "General", "AutoSaveOnExit", false);
            controlPanel.mbDebugonChecked = SaveLoad2.INIFile.INIread("settings.ini", "General", "mbDebugon", false);
            controlPanel.mbAOnTopChecked = SaveLoad2.INIFile.INIread("settings.ini", "General", "mbAOnTop", false);
            controlPanel.mbDisableSoundChecked = SaveLoad2.INIFile.INIread("settings.ini", "General", "mbDisableSound", false);
            controlPanel.mbDarkModeCheckBoxChecked = SaveLoad2.INIFile.INIread("settings.ini", "General", "mbEnableDarkMode", false);
            controlPanel.mbSplashCheckBoxChecked = SaveLoad2.INIFile.INIread("settings.ini", "General", "mbEnableSplashScreen", false);
            controlPanel.mbAntiCapsCheckBoxChecked = SaveLoad2.INIFile.INIread("settings.ini", "General", "mbEnableAntiCapsLock", false);
            controlPanel.mbEnableFlirChecked = SaveLoad2.INIFile.INIread("settings.ini", "General", "mbEnableFlirMode", false);

            controlPanel.ColorRValue = SaveLoad2.INIFile.INIread("settings.ini", "Crosshair", "ColorRValue", 255); // Assuming default red
            controlPanel.ColorGValue = SaveLoad2.INIFile.INIread("settings.ini", "Crosshair", "ColorGValue", 255); // Assuming default green
            controlPanel.ColorBValue = SaveLoad2.INIFile.INIread("settings.ini", "Crosshair", "ColorBValue", 255); // Assuming default blue
            controlPanel.SizeValue = SaveLoad2.INIFile.INIread("settings.ini", "Crosshair", "SizeValue", 5); // Default size
            controlPanel.TransparencyValue = SaveLoad2.INIFile.INIread("settings.ini", "Crosshair", "TransparencyValue", 100); // Default transparency
            controlPanel.OffsetXValue = SaveLoad2.INIFile.INIread("settings.ini", "Crosshair", "OffsetXValue", 0); // Default X offset
            controlPanel.OffsetYValue = SaveLoad2.INIFile.INIread("settings.ini", "Crosshair", "OffsetYValue", 0); // Default Y offset
            controlPanel.mbHideCrosshairChecked = SaveLoad2.INIFile.INIread("settings.ini", "Crosshair", "mbHideCrosshair", false);

            if (controlPanel.mbCrosshairOverlay != null)
            {
                int posX = SaveLoad2.INIFile.INIread("settings.ini", "General", "PositionX", controlPanel.mbCrosshairOverlay.Left);
                int posY = SaveLoad2.INIFile.INIread("settings.ini", "General", "PositionY", controlPanel.mbCrosshairOverlay.Top);
                controlPanel.mbCrosshairOverlay.Left = posX;
                controlPanel.mbCrosshairOverlay.Top = posY;
            }

            controlPanel.zoomLevel.Value = SaveLoad2.INIFile.INIread("settings.ini", "ZoomMode", "ZoomLevel", controlPanel.zoomLevel.Value);
            controlPanel.mbEnableZoomModeChecked = SaveLoad2.INIFile.INIread("settings.ini", "ZoomMode", "mbEnableZoomMode", false);

            controlPanel.UpdateAllUI();
        }

    }
}
