/* 
    www.mbnq.pl 2024 
    mbnq00 on gmail

    File io goes here, encryption is just for learning purposes
*/

using MaterialSkin.Controls;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Linq;
using MaterialSkin;
// using System.Windows.Media;

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

        /* --- --- --- progress bar --- --- --- */



    /* --- --- --- encrypt / decrypt --- --- --- */
    private static readonly byte[] key = Convert.FromBase64String("69hyLVzQGTHpS28ZR4TDLw==");  // chill, it's here just for testing and learning purposes
        private static readonly byte[] iv = new byte[16]; // 16 bytes IV for AES

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

                                // Return null or another appropriate value after calling restart
                                return null;
                            }
                        }
                    }
                }
            }
        }

        /* --- --- --- saving --- --- --- */
        public static void SaveSettings(ControlPanel controlPanel, bool showMessage = true)
        {
            var sb = new StringBuilder();

            controlPanel.mbProgressBar0.Visible = ControlPanel.mPBIsOn;
            controlPanel.mbProgressBar0.Value = 0;
            controlPanel.updateMainCrosshair();

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
            sb.AppendLine($"AutoSaveOnExit={controlPanel.AutoSaveOnExitChecked}");
            sb.AppendLine($"mbDebugon={controlPanel.mbDebugonChecked}");
            sb.AppendLine($"mbAOnTop={controlPanel.mbAOnTopChecked}");
            sb.AppendLine($"SoundEnabled={Sounds.IsSoundEnabled}");
            sb.AppendLine($"ZoomEnabled={ZoomMode.IsZoomModeEnabled}");

            controlPanel.mbProgressBar0.Value = 30;
            controlPanel.updateMainCrosshair();

            // Save overlay absolute position
            if (controlPanel.mbnqCrosshairOverlay != null)
            {
                sb.AppendLine($"PositionX={controlPanel.mbnqCrosshairOverlay.Left}");
                sb.AppendLine($"PositionY={controlPanel.mbnqCrosshairOverlay.Top}");
            }

            controlPanel.mbProgressBar0.Value = 50;
            controlPanel.updateMainCrosshair();

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

        /* --- --- --- loading --- --- --- */
        public static void LoadSettings(ControlPanel controlPanel, bool showMessage = true)
        {
            controlPanel.mbProgressBar0.Visible = ControlPanel.mPBIsOn;
            controlPanel.mbProgressBar0.Value = 0;

            if (!File.Exists(settingsFilePath))
            {
                Sounds.PlayClickSoundOnce();
                MaterialMessageBox.Show("Settings file not found.", "Load Settings", MessageBoxButtons.OK, MessageBoxIcon.None);
                Debug.WriteLineIf(ControlPanel.mIsDebugOn, "mbnq: Settings file not found.");
                return;
            }

            byte[] encryptedData = File.ReadAllBytes(settingsFilePath);
            string decryptedData = DecryptString(encryptedData, key, iv);

            controlPanel.mbProgressBar0.Value = 30;

            var lines = decryptedData.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
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

                else if (line.StartsWith("AutoSaveOnExit="))
                    controlPanel.AutoSaveOnExitChecked = bool.Parse(line.Substring("AutoSaveOnExit=".Length));
                else if (line.StartsWith("mbDebugon="))
                    controlPanel.mbDebugonChecked = bool.Parse(line.Substring("mbDebugon=".Length));
                else if (line.StartsWith("mbAOnTop="))
                    controlPanel.mbAOnTopChecked = bool.Parse(line.Substring("mbAOnTop=".Length));


                else if (line.StartsWith("PositionX=") && controlPanel.mbnqCrosshairOverlay != null)
                    controlPanel.mbnqCrosshairOverlay.Left = int.Parse(line.Substring("PositionX=".Length));
                else if (line.StartsWith("PositionY=") && controlPanel.mbnqCrosshairOverlay != null)
                    controlPanel.mbnqCrosshairOverlay.Top = int.Parse(line.Substring("PositionY=".Length));
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

            controlPanel.mbProgressBar0.Value = 60;

            controlPanel.updateMainCrosshair();

            if (showMessage)
            {
                Sounds.PlayClickSoundOnce();
                MaterialMessageBox.Show("Settings loaded.", "Load Settings", MessageBoxButtons.OK, MessageBoxIcon.None);
            }

            Debug.WriteLineIf(ControlPanel.mIsDebugOn, "mbnq: Settings Loaded.");
            controlPanel.mbProgressBar0.Value = 100;
            controlPanel.mbProgressBar0.Visible = false;
            controlPanel.updateMainCrosshair();
            controlPanel.mSettingsLoaded = 1;
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
                sb.AppendLine("SoundEnabled=True");
                sb.AppendLine("ZoomEnabled=False");

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
