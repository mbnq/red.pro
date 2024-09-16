
/* 

    www.mbnq.pl 2024 
    https://mbnq.pl/
    mbnq00 on gmail

*/

using System;
using System.Media;
using System.Windows.Forms;

namespace RED.mbnq.core
{
    public partial class mbSplashScreen : Form
    {
        public mbSplashScreen()
        {
            InitializeSplashScreen();

            if (Sounds.IsSoundEnabled) { 
                var splashSound = new SoundPlayer(Properties.Resources.mbSplash);
                splashSound.Load();
                splashSound.Play();
            }
        }
    }
}
