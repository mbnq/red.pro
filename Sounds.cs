/* www.mbnq.pl 2024 */

using System;
using System.Media;
using System.Threading.Tasks;
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
                // Load the sound from resources
                clickSoundPlayer = new SoundPlayer(Properties.Resources.mbnqClick);
                clickSoundPlayer.Load(); // Load the sound file into memory
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load sound: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public static void PlayClickSound()
        {
            // Run the sound playing logic on a separate thread
            Task.Run(() => PlaySoundInternal());
        }
        public static void PlayClickSoundOnce()
        {
            clickSoundPlayer.Play();
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
            }
            finally
            {
                isPlayingSound = false; // Reset the flag once the sound is done playing
            }
        }
    }
}
