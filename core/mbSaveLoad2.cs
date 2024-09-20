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
    }
}
