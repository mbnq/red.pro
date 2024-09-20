/* 
    
    www.mbnq.pl 2024 
    https://mbnq.pl/
    mbnq00 on gmail

*/

using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static RED.mbnq.SaveLoad2;

namespace RED.mbnq
{
    public static class SaveLoad2
    {
        private static readonly string settingsDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "mbnqplSoft"
        );
        private static readonly string settingsFilePath = Path.Combine(settingsDirectory, "RED.settings.ini");
        private static string SettingsDirectory => settingsDirectory;
        private static bool IsDebugOn = ControlPanel.mIsDebugOn;

        // Ensures that the settings directory exists. Creates it if it doesn't.
        public static void EnsureDirectoryExists()
        {
            if (!Directory.Exists(settingsDirectory))
            {
                Debug.WriteLineIf(IsDebugOn, "mbnq: Creating folder for user files...");
                Directory.CreateDirectory(settingsDirectory);
            }
        }

        // Handles reading from and writing to INI files.
        public static class INIFile
        {
            // Reads a value from the INI file.
            public static T INIread<T>(string fileName, string section, string key, T defaultValue)
            {
                // Combine the file name with the settings directory
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

            // Writes a value to the INI file.
            public static void INIsave<T>(string fileName, string section, string key, T value)
            {
                // Combine the file name with the settings directory
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
        public class SettingsManager
        {
             /*
              
                Usage:
                          SaveLoad2.SettingsManager settingsManager = new SaveLoad2.SettingsManager();
                          settingsManager.LoadSettings();
             */
            
            public void SaveSettings()
            {
                EnsureDirectoryExists();

                INIFile.INIsave("controls.ini", "Settings", "CheckboxState", true); // Boolean
                INIFile.INIsave("settings.ini", "Settings", "Volume", 75); // Integer
                INIFile.INIsave("settings.ini", "Settings", "Theme", "Dark"); // String
            }
            public void LoadSettings()
            {
                EnsureDirectoryExists();

                bool checkboxState = INIFile.INIread("controls.ini", "Settings", "CheckboxState", false);
                int volume = INIFile.INIread("settings.ini", "Settings", "Volume", 50);
                string theme = INIFile.INIread("settings.ini", "Settings", "Theme", "Light");


                Console.WriteLine($"Checkbox State: {checkboxState}");
                Console.WriteLine($"Volume: {volume}");
                Console.WriteLine($"Theme: {theme}");
            }
        }
    }
}
