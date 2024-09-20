﻿/* 
    
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
    public static class SaveLoad
    {
        private static readonly string settingsDirectory = ControlPanel.mbUserFilesPath;
        private static bool mbIsDebugOn = ControlPanel.mIsDebugOn;
        public static void EnsureDirectoryExists()
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
                EnsureDirectoryExists();

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
                EnsureDirectoryExists();

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
        public static void mbSaveSettings(ControlPanel controlPanel, bool onExit = false, bool silent = true)
        {
            // general
            SaveLoad.INIFile.INIsave("settings.ini", "General", "AutoSaveOnExit", controlPanel.mbAutoSaveCheckbox.Checked);
            SaveLoad.INIFile.INIsave("settings.ini", "General", "mbDebugon", controlPanel.mbDebugonCheckbox.Checked);
            SaveLoad.INIFile.INIsave("settings.ini", "General", "mbAOnTop", controlPanel.mbAOnTopCheckBox.Checked);
            SaveLoad.INIFile.INIsave("settings.ini", "General", "mbDisableSound", controlPanel.mbDisableSoundCheckBox.Checked);
            SaveLoad.INIFile.INIsave("settings.ini", "General", "mbEnableDarkMode", controlPanel.mbDarkModeCheckBox.Checked);
            SaveLoad.INIFile.INIsave("settings.ini", "General", "mbEnableSplashScreen", controlPanel.mbSplashCheckBox.Checked);
            SaveLoad.INIFile.INIsave("settings.ini", "General", "mbEnableAntiCapsLock", controlPanel.mbAntiCapsCheckBox.Checked);
            SaveLoad.INIFile.INIsave("settings.ini", "General", "mbEnableFlirMode", controlPanel.mbEnableFlirCheckBox.Checked);
            SaveLoad.INIFile.INIsave("settings.ini", "General", "PositionX", controlPanel.Left);
            SaveLoad.INIFile.INIsave("settings.ini", "General", "PositionY", controlPanel.Top);

            // crosshair
            SaveLoad.INIFile.INIsave("settings.ini", "Crosshair", "ColorRValue", controlPanel.colorR.Value);
            SaveLoad.INIFile.INIsave("settings.ini", "Crosshair", "ColorGValue", controlPanel.colorG.Value);
            SaveLoad.INIFile.INIsave("settings.ini", "Crosshair", "ColorBValue", controlPanel.colorB.Value);
            SaveLoad.INIFile.INIsave("settings.ini", "Crosshair", "SizeValue", controlPanel.size.Value);
            SaveLoad.INIFile.INIsave("settings.ini", "Crosshair", "TransparencyValue", controlPanel.transparency.Value);
            SaveLoad.INIFile.INIsave("settings.ini", "Crosshair", "OffsetXValue", controlPanel.offsetX.Value);
            SaveLoad.INIFile.INIsave("settings.ini", "Crosshair", "OffsetYValue", controlPanel.offsetY.Value);
            SaveLoad.INIFile.INIsave("settings.ini", "Crosshair", "mbHideCrosshair", controlPanel.mbHideCrosshairCheckBox.Checked);
            if (controlPanel.mbCrosshairOverlay != null) SaveLoad.INIFile.INIsave("settings.ini", "Crosshair", "PositionX", controlPanel.mbCrosshairOverlay.Left);
            if (controlPanel.mbCrosshairOverlay != null) SaveLoad.INIFile.INIsave("settings.ini", "Crosshair", "PositionY", controlPanel.mbCrosshairOverlay.Top);

            // zoomMode aka sniperMode
            SaveLoad.INIFile.INIsave("settings.ini", "ZoomMode", "ZoomLevel", controlPanel.zoomLevel.Value);
            SaveLoad.INIFile.INIsave("settings.ini", "ZoomMode", "zoomScopeSize", controlPanel.zoomScopeSize.Value);
            SaveLoad.INIFile.INIsave("settings.ini", "ZoomMode", "zoomTInterval", controlPanel.zoomTInterval.Value);
            SaveLoad.INIFile.INIsave("settings.ini", "ZoomMode", "zoomRefreshInterval", controlPanel.zoomRefreshInterval.Value);
            SaveLoad.INIFile.INIsave("settings.ini", "ZoomMode", "mbEnableZoomMode", controlPanel.mbEnableZoomModeCheckBox.Checked);

            // other
            SaveLoad.INIFile.INIsave("settings.ini", "Debug", "Time", $"{DateTime.Now.TimeOfDay}");
            SaveLoad.INIFile.INIsave("settings.ini", "Debug", "Date", $"{DateTime.Now.Day}.{DateTime.Now.Month}.{DateTime.Now.Year}");

            if (!onExit)
            {
                if (!silent) Sounds.PlayClickSoundOnce();
                controlPanel.UpdateAllUI();
                Debug.WriteLineIf(mbIsDebugOn, "mbnq: Settings saved.");
            }
        }
        public static void mbLoadSettings(ControlPanel controlPanel, bool silent = true)
        {
            controlPanel.mbAutoSaveCheckbox.Checked = SaveLoad.INIFile.INIread("settings.ini", "General", "AutoSaveOnExit", true);
            controlPanel.mbDebugonCheckbox.Checked = SaveLoad.INIFile.INIread("settings.ini", "General", "mbDebugon", false);
            controlPanel.mbAOnTopCheckBox.Checked = SaveLoad.INIFile.INIread("settings.ini", "General", "mbAOnTop", false);
            controlPanel.mbDisableSoundCheckBox.Checked = SaveLoad.INIFile.INIread("settings.ini", "General", "mbDisableSound", false);
            controlPanel.mbDarkModeCheckBox.Checked = SaveLoad.INIFile.INIread("settings.ini", "General", "mbEnableDarkMode", true);
            controlPanel.mbSplashCheckBox.Checked = SaveLoad.INIFile.INIread("settings.ini", "General", "mbEnableSplashScreen", true);
            controlPanel.mbAntiCapsCheckBox.Checked = SaveLoad.INIFile.INIread("settings.ini", "General", "mbEnableAntiCapsLock", true);
            controlPanel.mbEnableFlirCheckBox.Checked = SaveLoad.INIFile.INIread("settings.ini", "General", "mbEnableFlirMode", false);

            controlPanel.colorR.Value = SaveLoad.INIFile.INIread("settings.ini", "Crosshair", "ColorRValue", 255);
            controlPanel.colorG.Value = SaveLoad.INIFile.INIread("settings.ini", "Crosshair", "ColorGValue", 0);
            controlPanel.colorB.Value = SaveLoad.INIFile.INIread("settings.ini", "Crosshair", "ColorBValue", 0);
            controlPanel.size.Value = SaveLoad.INIFile.INIread("settings.ini", "Crosshair", "SizeValue", 12);
            controlPanel.transparency.Value = SaveLoad.INIFile.INIread("settings.ini", "Crosshair", "TransparencyValue", 64);
            controlPanel.offsetX.Value = SaveLoad.INIFile.INIread("settings.ini", "Crosshair", "OffsetXValue", 1000);
            controlPanel.offsetY.Value = SaveLoad.INIFile.INIread("settings.ini", "Crosshair", "OffsetYValue", 1000);
            controlPanel.mbHideCrosshairCheckBox.Checked = SaveLoad.INIFile.INIread("settings.ini", "Crosshair", "mbHideCrosshair", false);

            if (controlPanel.mbCrosshairOverlay != null)
            {
                int posX = SaveLoad.INIFile.INIread("settings.ini", "Crosshair", "PositionX", controlPanel.mbCrosshairOverlay.Left);
                int posY = SaveLoad.INIFile.INIread("settings.ini", "Crosshair", "PositionY", controlPanel.mbCrosshairOverlay.Top);
                controlPanel.mbCrosshairOverlay.Left = posX;
                controlPanel.mbCrosshairOverlay.Top = posY;
            }

            controlPanel.zoomTInterval.Value = SaveLoad.INIFile.INIread("settings.ini", "ZoomMode", "zoomTInterval", controlPanel.zoomTInterval.Value);
            controlPanel.zoomRefreshInterval.Value = SaveLoad.INIFile.INIread("settings.ini", "ZoomMode", "zoomRefreshInterval", controlPanel.zoomRefreshInterval.Value);
            controlPanel.mbEnableZoomModeCheckBox.Checked = SaveLoad.INIFile.INIread("settings.ini", "ZoomMode", "mbEnableZoomMode", false);
            controlPanel.zoomLevel.Value = SaveLoad.INIFile.INIread("settings.ini", "ZoomMode", "ZoomLevel", controlPanel.zoomLevel.Value);
            controlPanel.zoomScopeSize.Value = SaveLoad.INIFile.INIread("settings.ini", "ZoomMode", "zoomScopeSize", controlPanel.zoomScopeSize.Value);

            controlPanel.UpdateAllUI();
            controlPanel.mSettingsLoaded = 1;
            Debug.WriteLineIf(mbIsDebugOn, "mbnq: Settings Loaded.");
            if (!silent) Sounds.PlayClickSoundOnce();
        }
    }
}
