/* www.mbnq.pl 2024 */

using System;
using System.Media;
using System.Windows.Forms;

namespace RED.mbnq
{
    public static class Sounds
    {
        private static SoundPlayer clickSoundPlayer;
        private static bool isPlayingSound = false;

        static Sounds()
        {
            LoadClickSound();
        }

        private static void LoadClickSound()
        {
            try
            {
                clickSoundPlayer = new SoundPlayer("mbnqClick.wav");
                clickSoundPlayer.Load(); // Load the sound file into memory
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load sound: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.None);
            }
        }

        public static void PlayClickSound()
        {
            if (isPlayingSound) return;

            try
            {
                isPlayingSound = true;
                clickSoundPlayer.PlaySync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to play sound: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.None);
            }
            finally
            {
                isPlayingSound = false;
            }
        }
    }
}
