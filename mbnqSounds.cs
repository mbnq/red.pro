﻿/* 
    www.mbnq.pl 2024 
    mbnq00 on gmail

    Sound logic goes here
*/

using System;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace RED.mbnq
{
    public static class Sounds
    {
        private static SoundPlayer clickSoundPlayer;
        private static bool isPlayingSound = false;
        public static bool IsSoundEnabled { get; set; } = true;

        static Sounds()
        {
            LoadClickSound();
        }

        private static void LoadClickSound()
        {
            try
            {
                clickSoundPlayer = new SoundPlayer(Properties.Resources.mbnqClick);
                clickSoundPlayer.Load();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load sound: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLineIf(ControlPanel.mIsDebugOn, $"mbnq: Failed to load sound: {ex.Message}");
            }
        }
        public static void PlayClickSound()
        {
            if (IsSoundEnabled)
            {
                Task.Run(() => PlaySoundInternal());
                // Debug.WriteLineIf(ControlPanel.mIsDebugOn, $"mbnq: Playing Click sound.");
            }
        }
        public static void PlayClickSoundOnce()
        {
            if (IsSoundEnabled)
            {
                Task.Run(() => clickSoundPlayer.Play());
                // Debug.WriteLineIf(ControlPanel.mIsDebugOn, $"mbnq: Playing Click sound once.");
            }
        }
        private static void PlaySoundInternal()
        {
            if (isPlayingSound) return;

            try
            {
                isPlayingSound = true;
                clickSoundPlayer.PlaySync(); // PlaySync ensures that the sound plays synchronously and waits until the sound is done
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to play sound: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLineIf(ControlPanel.mIsDebugOn, $"Failed to play sound: {ex.Message}");
            }
            finally
            {
                isPlayingSound = false; // Reset the flag once the sound is done playing
            }
        }
    }
}
