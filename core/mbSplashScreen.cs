using System;
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
            Sounds.PlayClickSoundOnce();
        }

        private async void StartCloseTimerAsync()
        {
            await Task.Delay(3000); // Wait for 3 seconds
            this.Close();
        }
    }
}
