using System;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RED.mbnq.core
{
    public partial class mbSplashScreen : Form
    {
        public mbSplashScreen()
        {
            InitializeSplashScreen();
            StartCloseTimerAsync();

            var splashSound = new SoundPlayer(Properties.Resources.mbSplash);
            splashSound.Load();
            splashSound.Play();
        }
        private async void StartCloseTimerAsync()
        {
            await Task.Delay(4000); // Wait for 3 seconds
            this.Close();
        }
    }
}
