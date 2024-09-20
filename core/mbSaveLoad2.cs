/* 
    
    www.mbnq.pl 2024 
    https://mbnq.pl/
    mbnq00 on gmail

    Example usage: INIFile
                                        filename       section    keyname def

            SaveLoad2.INIFile.INIsave("settings.ini", "Settings", "Test", 100);
            volume = INIFile.INIread("settings.ini", "Settings", "Test", 0);
            MessageBox.Show($"Loaded variable value:{volume}", "Test", MessageBoxButtons.OK, MessageBoxIcon.None);


    Example usage: mbSaveSettings
                                      instance

               SaveLoad2.mbSaveSettings(this);


    Example usage: mbLoadSettings
                                      instance

                SaveLoad2.mbLoadSettings(this);

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
        private static readonly string settingsDirectory = ControlPanel.mbUserFilesPath;
        private static bool mbIsDebugOn = ControlPanel.mIsDebugOn;
        private static void EnsureDirectoryExists2()
        {
            if (!Directory.Exists(settingsDirectory))
            {
                Debug.WriteLineIf(mbIsDebugOn, "mbnq: Creating folder for user files...");
                Directory.CreateDirectory(settingsDirectory);
            }
        }
        public static class INIFile
        {
            public static T INIread<T>(string fileName, string section, string key, T defaultValue)
            {
                EnsureDirectoryExists2();

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
                    Debug.WriteLineIf(mbIsDebugOn, $"mbnq: Error parsing INI value: {ex.Message}");
                    return defaultValue;
                }
            }
            public static void INIsave<T>(string fileName, string section, string key, T value)
            {
                EnsureDirectoryExists2();

                string filePath = Path.Combine(settingsDirectory, fileName);
                string valueToSave = value.ToString();
                bool success = WritePrivateProfileString(section, key, valueToSave, filePath);

                if (!success && mbIsDebugOn)
                {
                    Debug.WriteLine($"mbnq: Failed to write to INI file: {filePath}");
                }
            }

            [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
            private static extern bool WritePrivateProfileString(string section, string key, string value, string filePath);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
            private static extern int GetPrivateProfileString(string section, string key, string defaultValue, StringBuilder result, int size, string filePath);
        }
        public static void mbSaveSettings(ControlPanel controlPanel, bool onExit = false)
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

            if (!onExit)
            {
                Sounds.PlayClickSoundOnce();
                controlPanel.UpdateAllUI();
                Debug.WriteLineIf(mbIsDebugOn, "mbnq: Settings saved.");
            }
        }
        public static void mbLoadSettings(ControlPanel controlPanel)
        {
            controlPanel.AutoSaveOnExitChecked = SaveLoad2.INIFile.INIread("settings.ini", "General", "AutoSaveOnExit", true);
            controlPanel.mbDebugonChecked = SaveLoad2.INIFile.INIread("settings.ini", "General", "mbDebugon", false);
            controlPanel.mbAOnTopChecked = SaveLoad2.INIFile.INIread("settings.ini", "General", "mbAOnTop", false);
            controlPanel.mbDisableSoundChecked = SaveLoad2.INIFile.INIread("settings.ini", "General", "mbDisableSound", false);
            controlPanel.mbDarkModeCheckBoxChecked = SaveLoad2.INIFile.INIread("settings.ini", "General", "mbEnableDarkMode", true);
            controlPanel.mbSplashCheckBoxChecked = SaveLoad2.INIFile.INIread("settings.ini", "General", "mbEnableSplashScreen", true);
            controlPanel.mbAntiCapsCheckBoxChecked = SaveLoad2.INIFile.INIread("settings.ini", "General", "mbEnableAntiCapsLock", true);
            controlPanel.mbEnableFlirChecked = SaveLoad2.INIFile.INIread("settings.ini", "General", "mbEnableFlirMode", false);

            controlPanel.ColorRValue = SaveLoad2.INIFile.INIread("settings.ini", "Crosshair", "ColorRValue", 255);
            controlPanel.ColorGValue = SaveLoad2.INIFile.INIread("settings.ini", "Crosshair", "ColorGValue", 0);
            controlPanel.ColorBValue = SaveLoad2.INIFile.INIread("settings.ini", "Crosshair", "ColorBValue", 0);
            controlPanel.SizeValue = SaveLoad2.INIFile.INIread("settings.ini", "Crosshair", "SizeValue", 12);
            controlPanel.TransparencyValue = SaveLoad2.INIFile.INIread("settings.ini", "Crosshair", "TransparencyValue", 64);
            controlPanel.OffsetXValue = SaveLoad2.INIFile.INIread("settings.ini", "Crosshair", "OffsetXValue", 1000);
            controlPanel.OffsetYValue = SaveLoad2.INIFile.INIread("settings.ini", "Crosshair", "OffsetYValue", 1000);
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
            controlPanel.mSettingsLoaded = 1;
            Debug.WriteLineIf(mbIsDebugOn, "mbnq: Settings Loaded.");
        }

    }
}
