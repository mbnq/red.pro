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
using System.Drawing;
using System.Threading.Tasks;

namespace RED.mbnq
{
    public static class SaveLoad
    {
        #region init
        private static readonly string settingsDirectory = ControlPanel.mbUserFilesPath;
        private static bool mbIsDebugOn = ControlPanel.mbIsDebugOn;
        public static void EnsureDirectoryExists()
        {
            if (!Directory.Exists(settingsDirectory))
            {
                Debug.WriteLineIf(mbIsDebugOn, "mbnq: Creating folder for user files...");
                Directory.CreateDirectory(settingsDirectory);
            }
        }
        #endregion

        #region INIIO
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

        #endregion

        #region SaveLoad
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
            SaveLoad.INIFile.INIsave("settings.ini", "Crosshair", "ColorRValue", controlPanel.mbColorRSlider.Value);
            SaveLoad.INIFile.INIsave("settings.ini", "Crosshair", "ColorGValue", controlPanel.mbColorGSlider.Value);
            SaveLoad.INIFile.INIsave("settings.ini", "Crosshair", "ColorBValue", controlPanel.mbColorBSlider.Value);
            SaveLoad.INIFile.INIsave("settings.ini", "Crosshair", "SizeValue", controlPanel.mbSizeSlider.Value);
            SaveLoad.INIFile.INIsave("settings.ini", "Crosshair", "TransparencyValue", controlPanel.mbTransparencySlider.Value);
            SaveLoad.INIFile.INIsave("settings.ini", "Crosshair", "OffsetXValue", controlPanel.mbOffsetXSlider.Value);
            SaveLoad.INIFile.INIsave("settings.ini", "Crosshair", "OffsetYValue", controlPanel.mbOffsetYSlider.Value);
            SaveLoad.INIFile.INIsave("settings.ini", "Crosshair", "mbHideCrosshair", controlPanel.mbHideCrosshairCheckBox.Checked);
            if (controlPanel.mbCrosshairOverlay != null) SaveLoad.INIFile.INIsave("settings.ini", "Crosshair", "PositionX", controlPanel.mbCrosshairOverlay.Left);
            if (controlPanel.mbCrosshairOverlay != null) SaveLoad.INIFile.INIsave("settings.ini", "Crosshair", "PositionY", controlPanel.mbCrosshairOverlay.Top);

            // zoomMode aka sniperMode
            SaveLoad.INIFile.INIsave("settings.ini", "ZoomMode", "ZoomLevel", controlPanel.mbZoomLevelSlider.Value);
            SaveLoad.INIFile.INIsave("settings.ini", "ZoomMode", "zoomScopeSize", controlPanel.mbZoomScopeSizeSlider.Value);
            SaveLoad.INIFile.INIsave("settings.ini", "ZoomMode", "zoomTInterval", controlPanel.mbZoomTIntervalSlider.Value);
            SaveLoad.INIFile.INIsave("settings.ini", "ZoomMode", "zoomRefreshInterval", controlPanel.mbZoomRefreshIntervalSlider.Value);
            SaveLoad.INIFile.INIsave("settings.ini", "ZoomMode", "mbEnableZoomMode", controlPanel.mbEnableZoomModeCheckBox.Checked);

            // glass settings, would be nice if it looked like this
            // SaveLoad.INIFile.INIsave("settings.ini", "Glass", "ZoomLevel", glassControls.isGlassMenuEnabled);

            // other
            // SaveLoad.INIFile.INIsave("settings.ini", "Debug", "Time", $"{DateTime.Now.TimeOfDay}");
            // SaveLoad.INIFile.INIsave("settings.ini", "Debug", "Date", $"{DateTime.Now.Day}.{DateTime.Now.Month}.{DateTime.Now.Year}");

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

            controlPanel.mbColorRSlider.Value = SaveLoad.INIFile.INIread("settings.ini", "Crosshair", "ColorRValue", 255);
            controlPanel.mbColorGSlider.Value = SaveLoad.INIFile.INIread("settings.ini", "Crosshair", "ColorGValue", 0);
            controlPanel.mbColorBSlider.Value = SaveLoad.INIFile.INIread("settings.ini", "Crosshair", "ColorBValue", 0);
            controlPanel.mbSizeSlider.Value = SaveLoad.INIFile.INIread("settings.ini", "Crosshair", "SizeValue", 12);
            controlPanel.mbTransparencySlider.Value = SaveLoad.INIFile.INIread("settings.ini", "Crosshair", "TransparencyValue", 64);
            controlPanel.mbOffsetXSlider.Value = SaveLoad.INIFile.INIread("settings.ini", "Crosshair", "OffsetXValue", 1000);
            controlPanel.mbOffsetYSlider.Value = SaveLoad.INIFile.INIread("settings.ini", "Crosshair", "OffsetYValue", 1000);
            controlPanel.mbHideCrosshairCheckBox.Checked = SaveLoad.INIFile.INIread("settings.ini", "Crosshair", "mbHideCrosshair", false);

            if (controlPanel.mbCrosshairOverlay != null)
            {
                int posX = SaveLoad.INIFile.INIread("settings.ini", "Crosshair", "PositionX", controlPanel.mbCrosshairOverlay.Left);
                int posY = SaveLoad.INIFile.INIread("settings.ini", "Crosshair", "PositionY", controlPanel.mbCrosshairOverlay.Top);
                controlPanel.mbCrosshairOverlay.Left = posX;
                controlPanel.mbCrosshairOverlay.Top = posY;
            }

            controlPanel.mbZoomTIntervalSlider.Value = SaveLoad.INIFile.INIread("settings.ini", "ZoomMode", "zoomTInterval", controlPanel.mbZoomTIntervalSlider.Value);
            controlPanel.mbZoomRefreshIntervalSlider.Value = SaveLoad.INIFile.INIread("settings.ini", "ZoomMode", "zoomRefreshInterval", controlPanel.mbZoomRefreshIntervalSlider.Value);
            controlPanel.mbEnableZoomModeCheckBox.Checked = SaveLoad.INIFile.INIread("settings.ini", "ZoomMode", "mbEnableZoomMode", false);
            controlPanel.mbZoomLevelSlider.Value = SaveLoad.INIFile.INIread("settings.ini", "ZoomMode", "ZoomLevel", controlPanel.mbZoomLevelSlider.Value);
            controlPanel.mbZoomScopeSizeSlider.Value = SaveLoad.INIFile.INIread("settings.ini", "ZoomMode", "zoomScopeSize", controlPanel.mbZoomScopeSizeSlider.Value);

            controlPanel.UpdateAllUI();
            controlPanel.mbSettingsLoaded = 1;
            Debug.WriteLineIf(mbIsDebugOn, "mbnq: Settings Loaded.");
            if (!silent) Sounds.PlayClickSoundOnce();
        }

        #endregion

        #region SaveLoadGlass
        public static void mbSaveGlassSettings(GlassHudOverlay glassOverlay)
        {
            SaveLoad.INIFile.INIsave("settings.ini", "Glass", "glassSaveExist", true);

            // Glass settings
            SaveLoad.INIFile.INIsave("settings.ini", "Glass", "glassOffsetXValue", glassOverlay.glassOffsetXValue);
            SaveLoad.INIFile.INIsave("settings.ini", "Glass", "glassOffsetYValue", glassOverlay.glassOffsetYValue);
            SaveLoad.INIFile.INIsave("settings.ini", "Glass", "glassZoomValue", glassOverlay.glassZoomValue);
            SaveLoad.INIFile.INIsave("settings.ini", "Glass", "glassOpacityValue", glassOverlay.glassOpacityValue);
            SaveLoad.INIFile.INIsave("settings.ini", "Glass", "glassRefreshRate", glassOverlay.glassRefreshRate);
            SaveLoad.INIFile.INIsave("settings.ini", "Glass", "glassIsBorderVisible", glassOverlay.glassIsBorderVisible);
            SaveLoad.INIFile.INIsave("settings.ini", "Glass", "glassIsCircle", glassOverlay.glassIsCircle);
            SaveLoad.INIFile.INIsave("settings.ini", "Glass", "glassIsBind", glassOverlay.glassIsBind);
            SaveLoad.INIFile.INIsave("settings.ini", "Glass", "glassIsMenuEnabled", glassOverlay.glassIsMenuEnabled);

            // Save each property of the capture area
            SaveLoad.INIFile.INIsave("settings.ini", "Glass", "CaptureAreaX", glassOverlay.glassCaptureAreaValue.X);
            SaveLoad.INIFile.INIsave("settings.ini", "Glass", "CaptureAreaY", glassOverlay.glassCaptureAreaValue.Y);
            SaveLoad.INIFile.INIsave("settings.ini", "Glass", "CaptureAreaWidth", glassOverlay.glassCaptureAreaValue.Width);
            SaveLoad.INIFile.INIsave("settings.ini", "Glass", "CaptureAreaHeight", glassOverlay.glassCaptureAreaValue.Height);
            SaveLoad.INIFile.INIsave("settings.ini", "Glass", "AbsolutePosX", glassOverlay.glassAbsolutePos.X);
            SaveLoad.INIFile.INIsave("settings.ini", "Glass", "AbsolutePosY", glassOverlay.glassAbsolutePos.Y);

        }

        public static async Task mbLoadGlassSettings(GlassHudOverlay glassOverlay)
        {
            // the loading sequence order is critical!
            glassOverlay.glassIsBorderVisible = SaveLoad.INIFile.INIread("settings.ini", "Glass", "glassIsBorderVisible", true);
            glassOverlay.glassIsCircle = SaveLoad.INIFile.INIread("settings.ini", "Glass", "glassIsCircle", false);
            glassOverlay.glassIsBind = SaveLoad.INIFile.INIread("settings.ini", "Glass", "glassIsBind", false);

            // Load each property of the capture area
            int x = SaveLoad.INIFile.INIread("settings.ini", "Glass", "CaptureAreaX", 0);
            int y = SaveLoad.INIFile.INIread("settings.ini", "Glass", "CaptureAreaY", 0);
            int width = SaveLoad.INIFile.INIread("settings.ini", "Glass", "CaptureAreaWidth", 100);
            int height = SaveLoad.INIFile.INIread("settings.ini", "Glass", "CaptureAreaHeight", 100);

            // Reconstruct the capture area Rectangle
            glassOverlay.glassCaptureAreaValue = new Rectangle(x, y, width, height);
            glassOverlay.glassCaptureAreaValue = glassOverlay.GetAdjustedCaptureArea();

            int posX = SaveLoad.INIFile.INIread("settings.ini", "Glass", "AbsolutePosX", 0);
            int posY = SaveLoad.INIFile.INIread("settings.ini", "Glass", "AbsolutePosY", 0);

            await GlassHudOverlay.ReloadWithNewAreaAsync();
            glassOverlay.glassAbsolutePos = new Point(posX, posY);

            glassOverlay.InitializeTrackBars();

            // trackbars
            glassOverlay.glassOffsetXValue = SaveLoad.INIFile.INIread("settings.ini", "Glass", "glassOffsetXValue", 0);
            glassOverlay.glassOffsetYValue = SaveLoad.INIFile.INIread("settings.ini", "Glass", "glassOffsetYValue", 0);
            glassOverlay.glassZoomValue = SaveLoad.INIFile.INIread("settings.ini", "Glass", "glassZoomValue", 100);
            glassOverlay.glassOpacityValue = SaveLoad.INIFile.INIread("settings.ini", "Glass", "glassOpacityValue", 100);
            glassOverlay.glassRefreshRate = SaveLoad.INIFile.INIread("settings.ini", "Glass", "glassRefreshRate", Program.mbFrameDelay);
            glassOverlay.glassIsMenuEnabled = SaveLoad.INIFile.INIread("settings.ini", "Glass", "glassIsMenuEnabled", false);

            glassOverlay.UpdateOffsets();
            glassOverlay.UpdateZoom();
            glassOverlay.UpdateOpacity();
            glassOverlay.UpdateRefreshRate();
            glassOverlay.UpdateTrackBarLabels();
            glassOverlay.UpdateGlassMenu();
        }
        #endregion
    }
}
