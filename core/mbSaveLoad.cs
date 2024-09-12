/* 

    www.mbnq.pl 2024 
    https://mbnq.pl/
    mbnq00 on gmail

*/

using MaterialSkin.Controls;
using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Security.Cryptography;

namespace RED.mbnq
{
    public static class SaveLoad
    {
        private static readonly string settingsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "mbnqplSoft");
        private static readonly string settingsFilePath = Path.Combine(settingsDirectory, "RED.settings.sav");
        public static string SettingsDirectory => settingsDirectory;

        private static readonly byte[] key = Convert.FromBase64String("69hyLVzQGTHpS28ZR4TDLw==");      // it's public here now, just to prevents user from messing with sav
        private static readonly byte[] iv = new byte[16];                                               // 16 bytes IV for AES

        private static void EnsureDirectoryExists()
        {
            if (!Directory.Exists(settingsDirectory))
            {
                Debug.WriteLineIf(ControlPanel.mIsDebugOn, "mbnq: Creating folder for userfiles...");
                Directory.CreateDirectory(settingsDirectory);
            }
        }

        /* --- --- --- encrypt / decrypt --- --- --- */
        private static byte[] EncryptString(string plainText, byte[] key, byte[] iv)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Padding = PaddingMode.PKCS7;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(plainText);
                        }
                        byte[] encryptedData = ms.ToArray();
                        Debug.WriteLineIf(ControlPanel.mIsDebugOn, "mbnq: Encrypted data length: " + encryptedData.Length);
                        return encryptedData;
                    }
                }
            }
        }

        private static string DecryptString(byte[] cipherText, byte[] key, byte[] iv)
        {
            Debug.WriteLineIf(ControlPanel.mIsDebugOn, "mbnq: Ciphertext length before decryption: " + cipherText.Length);
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Padding = PaddingMode.PKCS7;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream(cipherText))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            try
                            {
                                string result = sr.ReadToEnd();
                                Debug.WriteLineIf(ControlPanel.mIsDebugOn, "mbnq: Decrypted data length: " + result.Length);
                                return result;
                            }
                            catch (CryptographicException ex)
                            {
                                Debug.WriteLineIf(ControlPanel.mIsDebugOn, $"Decryption failed: {ex.Message}");
                                Debug.WriteLineIf(ControlPanel.mIsDebugOn, $"Removing: {settingsFilePath}");
                                File.Delete(settingsFilePath);
                                Debug.WriteLineIf(ControlPanel.mIsDebugOn, $"Restarting...");
                                Application.Restart();
                                return null;
                            }
                        }
                    }
                }
            }
        }

        /* --- --- --- helper method for parsing settings --- --- --- */
        private static void ParseSetting(string line, ControlPanel controlPanel)
        {
            var keyValue = line.Split('=');
            if (keyValue.Length == 2)
            {
                string key = keyValue[0];
                string value = keyValue[1];

                switch (key)
                {
                    case "Red": controlPanel.ColorRValue = int.Parse(value); break;
                    case "Green": controlPanel.ColorGValue = int.Parse(value); break;
                    case "Blue": controlPanel.ColorBValue = int.Parse(value); break;
                    case "Size": controlPanel.SizeValue = int.Parse(value); break;
                    case "Transparency": controlPanel.TransparencyValue = int.Parse(value); break;
                    case "ZoomLevel": controlPanel.zoomLevel.Value = int.Parse(value); break;
                    case "OffsetX": controlPanel.OffsetXValue = int.Parse(value); break;
                    case "OffsetY": controlPanel.OffsetYValue = int.Parse(value); break;
                    case "AutoSaveOnExit": controlPanel.AutoSaveOnExitChecked = bool.Parse(value); break;
                    case "mbDebugon": controlPanel.mbDebugonChecked = bool.Parse(value); break;
                    case "mbAOnTop": controlPanel.mbAOnTopChecked = bool.Parse(value); break;
                    case "mbHideCrosshair": controlPanel.mbHideCrosshairChecked = bool.Parse(value); break;
                    case "mbDisableSound": controlPanel.mbDisableSoundChecked = bool.Parse(value); break;
                    case "mbEnableZoomMode": controlPanel.mbEnableZoomModeChecked = bool.Parse(value); break;
                    case "mbEnableFlirMode": controlPanel.mbEnableFlirChecked = bool.Parse(value); break;
                    case "mbEnableDarkMode": controlPanel.mbDarkModeCheckBoxChecked = bool.Parse(value); break;
                    case "PositionX": if (controlPanel.mbCrosshairOverlay != null) controlPanel.mbCrosshairOverlay.Left = int.Parse(value); break;
                    case "PositionY": if (controlPanel.mbCrosshairOverlay != null) controlPanel.mbCrosshairOverlay.Top = int.Parse(value); break;
                }
            }
        }

        /* --- --- --- saving settings --- --- --- */
        public static void SaveSettings(ControlPanel controlPanel, bool showMessage = true)
        {
            var sb = new StringBuilder();

            controlPanel.mbProgressBar0.Visible = ControlPanel.mPBIsOn;
            controlPanel.mbProgressBar0.Value = 0;

            sb.AppendLine(";Do not edit if you don't know what you're doing, please.");
            sb.AppendLine("[REDDOT]");
            sb.AppendLine($"Red={controlPanel.ColorRValue}");
            sb.AppendLine($"Green={controlPanel.ColorGValue}");
            sb.AppendLine($"Blue={controlPanel.ColorBValue}");
            sb.AppendLine($"Size={controlPanel.SizeValue}");
            sb.AppendLine($"Transparency={controlPanel.TransparencyValue}");
            sb.AppendLine($"ZoomLevel={controlPanel.zoomLevel.Value}");
            sb.AppendLine($"OffsetX={controlPanel.OffsetXValue}");
            sb.AppendLine($"OffsetY={controlPanel.OffsetYValue}");
            sb.AppendLine($"AutoSaveOnExit={controlPanel.AutoSaveOnExitChecked}");
            sb.AppendLine($"mbDebugon={controlPanel.mbDebugonChecked}");
            sb.AppendLine($"mbAOnTop={controlPanel.mbAOnTopChecked}");
            sb.AppendLine($"mbHideCrosshair={controlPanel.mbHideCrosshairChecked}");
            sb.AppendLine($"mbDisableSound={controlPanel.mbDisableSoundChecked}");
            sb.AppendLine($"mbEnableZoomMode={controlPanel.mbEnableZoomModeChecked}");
            sb.AppendLine($"mbEnableFlirMode={controlPanel.mbEnableFlirChecked}");
            sb.AppendLine($"mbEnableDarkMode={controlPanel.mbDarkModeCheckBoxChecked}");

            if (controlPanel.mbCrosshairOverlay != null)
            {
                sb.AppendLine($"PositionX={controlPanel.mbCrosshairOverlay.Left}");
                sb.AppendLine($"PositionY={controlPanel.mbCrosshairOverlay.Top}");
            }

            controlPanel.mbProgressBar0.Value = 50;

            byte[] encryptedData = EncryptString(sb.ToString(), key, iv);
            File.WriteAllBytes(settingsFilePath, encryptedData);

            if (showMessage)
            {
                Sounds.PlayClickSoundOnce();
                MaterialMessageBox.Show("Settings saved.", "Save Settings", MessageBoxButtons.OK, MessageBoxIcon.None);
            }
            Debug.WriteLineIf(ControlPanel.mIsDebugOn, "mbnq: Settings saved.");

            controlPanel.mbProgressBar0.Value = 100;
            controlPanel.mbProgressBar0.Visible = false;
            controlPanel.updateMainCrosshair();
        }

        /* --- --- --- loading settings --- --- --- */
        public static void LoadSettings(ControlPanel controlPanel, bool showMessage = true)
        {
            controlPanel.mbProgressBar0.Visible = ControlPanel.mPBIsOn;
            controlPanel.mbProgressBar0.Value = 0;

            if (!File.Exists(settingsFilePath))
            {
                Sounds.PlayClickSoundOnce();
                MaterialMessageBox.Show("Settings file not found.", "Load Settings", MessageBoxButtons.OK, MessageBoxIcon.None);
                return;
            }

            byte[] encryptedData = File.ReadAllBytes(settingsFilePath);
            string decryptedData = DecryptString(encryptedData, key, iv);

            controlPanel.mbProgressBar0.Value = 30;

            var lines = decryptedData.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (var line in lines)
            {
                ParseSetting(line, controlPanel);
            }

            controlPanel.mbProgressBar0.Value = 60;

            if (showMessage)
            {
                Sounds.PlayClickSoundOnce();
                MaterialMessageBox.Show("Settings loaded.", "Load Settings", MessageBoxButtons.OK, MessageBoxIcon.None);
            }

            Debug.WriteLineIf(ControlPanel.mIsDebugOn, "mbnq: Settings Loaded.");
            controlPanel.mbProgressBar0.Value = 100;
            controlPanel.updateMainCrosshair();
            controlPanel.mSettingsLoaded = 1;
            controlPanel.mbProgressBar0.Visible = false;
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
                sb.AppendLine("AutoSaveOnExit=True");
                sb.AppendLine("mbDebugonChecked=False");
                sb.AppendLine("mbAOnTop=False");
                sb.AppendLine("mbHideCrosshair=False");                
                sb.AppendLine("mbDisableSound=False");
                sb.AppendLine("mbEnableZoomMode=False");
                sb.AppendLine("mbEnableFlirMode=False");
                sb.AppendLine("mbEnableDarkMode=True");

                byte[] encryptedData = EncryptString(sb.ToString(), key, iv);
                File.WriteAllBytes(settingsFilePath, encryptedData);
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
                controlPanel.updateMainCrosshair();
            }
        }
    }
}
