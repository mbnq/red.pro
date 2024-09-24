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
using System.Collections.Generic;
using RED.mbnq.Properties;

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

        #region IO
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
                try
                {
                    WritePrivateProfileString(section, key, valueToSave, filePath);

                }
                catch (Exception ex)
                {
                    Debug.WriteLineIf(ControlPanel.mbIsDebugOn, $"mbnq: writing to file {filePath} failed {ex.Message}");
                }
            }

            [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
            private static extern bool WritePrivateProfileString(string section, string key, string value, string filePath);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
            private static extern int GetPrivateProfileString(string section, string key, string defaultValue, StringBuilder result, int size, string filePath);
        }

        #endregion

        #region SaveLoad

        #region Save
        public static void mbSaveSettings(ControlPanel controlPanel, bool onExit = false, bool silent = true)
        {
            // General settings
            var generalSettings = new Dictionary<string, object>
            {
                { "AutoSaveOnExit", controlPanel.mbAutoSaveCheckbox.Checked },
                { "mbDebugon", controlPanel.mbDebugonCheckbox.Checked },
                { "mbAOnTop", controlPanel.mbAOnTopCheckBox.Checked },
                { "mbDisableSound", controlPanel.mbDisableSoundCheckBox.Checked },
                { "mbEnableDarkMode", controlPanel.mbDarkModeCheckBox.Checked },
                { "mbEnableSplashScreen", controlPanel.mbSplashCheckBox.Checked },
                { "mbEnableAntiCapsLock", controlPanel.mbAntiCapsCheckBox.Checked },
                { "mbEnableFlirMode", controlPanel.mbEnableFlirCheckBox.Checked },
                { "PositionX", controlPanel.Left },
                { "PositionY", controlPanel.Top }
            };
            mbSaveSettingsBatch("General", generalSettings);

            // Crosshair settings
            var crosshairSettings = new Dictionary<string, object>
            {
                { "ColorRValue", controlPanel.mbColorRSlider.Value },
                { "ColorGValue", controlPanel.mbColorGSlider.Value },
                { "ColorBValue", controlPanel.mbColorBSlider.Value },
                { "SizeValue", controlPanel.mbSizeSlider.Value },
                { "TransparencyValue", controlPanel.mbTransparencySlider.Value },
                { "OffsetXValue", controlPanel.mbOffsetXSlider.Value },
                { "OffsetYValue", controlPanel.mbOffsetYSlider.Value },
                { "mbHideCrosshair", controlPanel.mbHideCrosshairCheckBox.Checked }
            };

            if (controlPanel.mbCrosshairOverlay != null)
            {
                crosshairSettings.Add("PositionX", controlPanel.mbCrosshairOverlay.Left);
                crosshairSettings.Add("PositionY", controlPanel.mbCrosshairOverlay.Top);
            }
            mbSaveSettingsBatch("Crosshair", crosshairSettings);

            // ZoomMode (sniper mode) settings
            var zoomModeSettings = new Dictionary<string, object>
            {
                { "ZoomLevel", controlPanel.mbZoomLevelSlider.Value },
                { "zoomScopeSize", controlPanel.mbZoomScopeSizeSlider.Value },
                { "zoomTInterval", controlPanel.mbZoomTIntervalSlider.Value },
                { "zoomRefreshInterval", controlPanel.mbZoomRefreshIntervalSlider.Value },
                { "mbEnableZoomMode", controlPanel.mbEnableZoomModeCheckBox.Checked }
            };
            mbSaveSettingsBatch("ZoomMode", zoomModeSettings);

            // Network settings
            var networkSettings = new Dictionary<string, object>
            {
                { "mbIPpingTestTarget", ControlPanel.mbIPpingTestTarget },
                { "mbIPdicoveryProvider", ControlPanel.mbIPdicoveryProvider },
                { "mbIPdicoveryProvider2", ControlPanel.mbIPdicoveryProvider2 },
                { "mbIPdicoveryProvider3", ControlPanel.mbIPdicoveryProvider3 },
                { "mbIPdicoveryProvider4", ControlPanel.mbIPdicoveryProvider4 }
            };
            mbSaveSettingsBatch("Network", networkSettings);

            // Perform actions onExit and silent settings
            if (!onExit)
            {
                if (!silent) Sounds.PlayClickSoundOnce();
                controlPanel.UpdateAllUI();
                Debug.WriteLineIf(mbIsDebugOn, "mbnq: Settings saved.");
            }

            // ---
            static void mbSaveSettingsBatch(string section, Dictionary<string, object> settings)
            {
                foreach (var setting in settings)
                {
                    try
                    {
                        SaveLoad.INIFile.INIsave("settings.ini", section, setting.Key, setting.Value);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLineIf(ControlPanel.mbIsDebugOn, $"mbnq: saving {section}{setting.Key}{setting.Value} failed {ex.Message}");
                    }
                }
            }
        }
        #endregion

        #region Load
        public static void mbLoadSettings(ControlPanel controlPanel, bool silent = true)
        {
            // General settings
            var generalSettings = new Dictionary<string, Action<object>>
            {
                { "AutoSaveOnExit", val => controlPanel.mbAutoSaveCheckbox.Checked = Convert.ToBoolean(val) },
                { "mbDebugon", val => controlPanel.mbDebugonCheckbox.Checked = Convert.ToBoolean(val) },
                { "mbAOnTop", val => controlPanel.mbAOnTopCheckBox.Checked = Convert.ToBoolean(val) },
                { "mbDisableSound", val => controlPanel.mbDisableSoundCheckBox.Checked = Convert.ToBoolean(val) },
                { "mbEnableDarkMode", val => controlPanel.mbDarkModeCheckBox.Checked = Convert.ToBoolean(val) },
                { "mbEnableSplashScreen", val => controlPanel.mbSplashCheckBox.Checked = Convert.ToBoolean(val) },
                { "mbEnableAntiCapsLock", val => controlPanel.mbAntiCapsCheckBox.Checked = Convert.ToBoolean(val) },
                { "mbEnableFlirMode", val => controlPanel.mbEnableFlirCheckBox.Checked = Convert.ToBoolean(val) }
            };
            mbLoadSettingsBatch("General", generalSettings, true);

            // Crosshair settings
            var crosshairSettings = new Dictionary<string, Action<object>>
            {
                { "ColorRValue", val => controlPanel.mbColorRSlider.Value = Convert.ToInt32(val) },
                { "ColorGValue", val => controlPanel.mbColorGSlider.Value = Convert.ToInt32(val) },
                { "ColorBValue", val => controlPanel.mbColorBSlider.Value = Convert.ToInt32(val) },
                { "SizeValue", val => controlPanel.mbSizeSlider.Value = Convert.ToInt32(val) },
                { "TransparencyValue", val => controlPanel.mbTransparencySlider.Value = Convert.ToInt32(val) },
                { "OffsetXValue", val => controlPanel.mbOffsetXSlider.Value = Convert.ToInt32(val) },
                { "OffsetYValue", val => controlPanel.mbOffsetYSlider.Value = Convert.ToInt32(val) },
                { "mbHideCrosshair", val => controlPanel.mbHideCrosshairCheckBox.Checked = Convert.ToBoolean(val) }
            };
            mbLoadSettingsBatch("Crosshair", crosshairSettings);

            if (controlPanel.mbCrosshairOverlay != null)
            {
                controlPanel.mbCrosshairOverlay.Left = SaveLoad.INIFile.INIread("settings.ini", "Crosshair", "PositionX", controlPanel.mbCrosshairOverlay.Left);
                controlPanel.mbCrosshairOverlay.Top = SaveLoad.INIFile.INIread("settings.ini", "Crosshair", "PositionY", controlPanel.mbCrosshairOverlay.Top);
            }

            // ZoomMode (sniper mode) settings
            var zoomModeSettings = new Dictionary<string, Action<object>>
            {
                { "ZoomLevel", val => controlPanel.mbZoomLevelSlider.Value = Convert.ToInt32(val) },
                { "zoomScopeSize", val => controlPanel.mbZoomScopeSizeSlider.Value = Convert.ToInt32(val) },
                { "zoomTInterval", val => controlPanel.mbZoomTIntervalSlider.Value = Convert.ToInt32(val) },
                { "zoomRefreshInterval", val => controlPanel.mbZoomRefreshIntervalSlider.Value = Convert.ToInt32(val) },
                { "mbEnableZoomMode", val => controlPanel.mbEnableZoomModeCheckBox.Checked = Convert.ToBoolean(val) }
            };
            mbLoadSettingsBatch("ZoomMode", zoomModeSettings);

            // Network settings
            ControlPanel.mbIPpingTestTarget = SaveLoad.INIFile.INIread("settings.ini", "Network", "mbIPpingTestTarget", "8.8.8.8");
            ControlPanel.mbIPdicoveryProvider = SaveLoad.INIFile.INIread("settings.ini", "Network", "mbIPdicoveryProvider", "https://mbnq.pl/myip/");
            ControlPanel.mbIPdicoveryProvider2 = SaveLoad.INIFile.INIread("settings.ini", "Network", "mbIPdicoveryProvider2", "https://api.seeip.org/");
            ControlPanel.mbIPdicoveryProvider3 = SaveLoad.INIFile.INIread("settings.ini", "Network", "mbIPdicoveryProvider3", "https://api.my-ip.io/v2/ip.txt");
            ControlPanel.mbIPdicoveryProvider4 = SaveLoad.INIFile.INIread("settings.ini", "Network", "mbIPdicoveryProvider4", "https://wtfismyip.com/text/");

            controlPanel.UpdateAllUI();
            controlPanel.mbSettingsLoaded = 1;
            Debug.WriteLineIf(mbIsDebugOn, "mbnq: Settings Loaded.");
            if (!silent) Sounds.PlayClickSoundOnce();

            // ---
            static void mbLoadSettingsBatch(string section, Dictionary<string, Action<object>> settings, object defaultValue = null)
            {
                foreach (var setting in settings)
                {
                    try
                    {
                        var value = SaveLoad.INIFile.INIread("settings.ini", section, setting.Key, defaultValue ?? default);
                        setting.Value(value);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLineIf(ControlPanel.mbIsDebugOn, $"mbnq: loading {section}{setting.Key}{setting.Value} failed {ex.Message}");
                    }
                }
            }
        }
        #endregion

        #endregion

        #region SaveLoadGlass
        public static void mbSaveGlassSettings(GlassHudOverlay glassOverlay)
        {
            var glassSettings = new Dictionary<string, object>
            {
                { "glassSaveExist", true },
                { "glassOffsetXValue", glassOverlay.glassOffsetXValue },
                { "glassOffsetYValue", glassOverlay.glassOffsetYValue },
                { "glassZoomValue", glassOverlay.glassZoomValue },
                { "glassOpacityValue", glassOverlay.glassOpacityValue },
                { "glassRefreshRate", glassOverlay.glassRefreshRate },
                { "glassIsBorderVisible", glassOverlay.glassIsBorderVisible },
                { "glassIsCircle", glassOverlay.glassIsCircle },
                { "glassIsBind", glassOverlay.glassIsBind },
                { "glassIsMenuEnabled", glassOverlay.glassIsMenuEnabled },
                { "CaptureAreaX", glassOverlay.glassCaptureAreaValue.X },
                { "CaptureAreaY", glassOverlay.glassCaptureAreaValue.Y },
                { "CaptureAreaWidth", glassOverlay.glassCaptureAreaValue.Width },
                { "CaptureAreaHeight", glassOverlay.glassCaptureAreaValue.Height },
                { "AbsolutePosX", glassOverlay.glassAbsolutePos.X },
                { "AbsolutePosY", glassOverlay.glassAbsolutePos.Y }
            };

            mbSaveSettingsBatch("Glass", glassSettings);

            static void mbSaveSettingsBatch(string section, Dictionary<string, object> settings)
            {
                foreach (var setting in settings)
                {
                    SaveLoad.INIFile.INIsave("settings.ini", section, setting.Key, setting.Value);
                }
            }
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
